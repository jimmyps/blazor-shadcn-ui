/**
 * sortable.js — Headless drag-and-drop sensor for NeoUI SortablePrimitive.
 *
 * Responsibilities:
 *  - Pointer (mouse/touch) and keyboard drag sensors
 *  - Closest-centre collision detection
 *  - Item displacement transforms (translate) for vertical/horizontal lists
 *  - Overlay (ghost) positioning
 *  - Calls back into C# via DotNetObjectReference on drag-start, drag-end, drag-cancel
 *
 * C# calls into this module:
 *  - init(containerEl, dotNetRef, instanceId, orientation)
 *  - dispose(instanceId)
 */

const DRAG_THRESHOLD          = 5;    // px movement before a pending-drag becomes a real drag
const TRANSITION_MS           = 150;  // ms for item displacement CSS transitions
const DRAG_PLACEHOLDER_OPACITY = '0.4'; // opacity of the source item during drag
const DEFAULT_ITEM_HEIGHT     = 60;   // px fallback height used in keyboard drag calculations
const DEFAULT_ITEM_WIDTH      = 120;  // px fallback width used in keyboard drag calculations

const instances = new Map();

// ─── Helpers ──────────────────────────────────────────────────────────────────

/** Returns all sortable item elements inside a container. */
function getItems(container) {
    return Array.from(container.querySelectorAll('[data-sortable-id]'));
}

/** Returns the item element that owns a given sortable-id. */
function getItemById(container, id) {
    return container.querySelector(`[data-sortable-id="${CSS.escape(id)}"]`);
}

/** Returns the overlay element for this sortable instance. */
function getOverlay(instanceId) {
    return document.querySelector(`[data-sortable-overlay-for="${instanceId}"]`);
}

/**
 * Closest-centre collision detection.
 * Returns the id of the item whose centre is nearest to (x, y), excluding the active item.
 */
function closestCenter(container, activeId, x, y) {
    const items = getItems(container);
    let minDist = Infinity;
    let overId  = null;

    for (const item of items) {
        const id = item.getAttribute('data-sortable-id');
        if (id === activeId) continue;
        const rect = item.getBoundingClientRect();
        const cx   = rect.left + rect.width  / 2;
        const cy   = rect.top  + rect.height / 2;
        const dist = Math.hypot(x - cx, y - cy);
        if (dist < minDist) {
            minDist = dist;
            overId  = id;
        }
    }
    return overId;
}

/**
 * Applies displacement CSS transforms to all items based on the current
 * drag position, giving real-time visual feedback.
 *
 * @param {object} state       - Current drag state
 * @param {string} orientation - 'vertical' | 'horizontal' | 'mixed'
 */
function applyItemTransforms(state, orientation) {
    if (!state.isDragging || !state.snapshotItems) return;

    const activeIdx = state.snapshotItems.findIndex(s => s.id === state.activeId);
    const overId    = closestCenter(state.containerEl, state.activeId, state.currentX, state.currentY);
    const overIdx   = state.snapshotItems.findIndex(s => s.id === overId);

    if (activeIdx < 0 || overIdx < 0) return;

    for (let i = 0; i < state.snapshotItems.length; i++) {
        const snap = state.snapshotItems[i];
        if (snap.id === state.activeId) continue;

        let tx = 0, ty = 0;

        if (orientation === 'vertical') {
            const activeH = state.snapshotItems[activeIdx].rect.height;
            if (activeIdx < overIdx && i > activeIdx && i <= overIdx) ty = -activeH;
            else if (activeIdx > overIdx && i >= overIdx && i < activeIdx) ty = activeH;
        } else if (orientation === 'horizontal') {
            const activeW = state.snapshotItems[activeIdx].rect.width;
            if (activeIdx < overIdx && i > activeIdx && i <= overIdx) tx = -activeW;
            else if (activeIdx > overIdx && i >= overIdx && i < activeIdx) tx = activeW;
        }
        // 'mixed' orientation: no item transforms — only the overlay moves

        snap.el.style.transition = `transform ${TRANSITION_MS}ms ease`;
        snap.el.style.transform  = (tx !== 0 || ty !== 0) ? `translate(${tx}px,${ty}px)` : '';
    }
}

/** Resets all item transforms to their natural positions. */
function resetItemTransforms(state) {
    if (!state.snapshotItems) return;
    for (const snap of state.snapshotItems) {
        snap.el.style.transition = '';
        snap.el.style.transform  = '';
    }
}

// ─── Drag lifecycle ──────────────────────────────────────────────────────────

/**
 * Begins a real drag after the DRAG_THRESHOLD has been exceeded.
 * Snapshots item positions, shows the overlay, and calls OnDragStart.
 */
function startDrag(state, orientation) {
    state.isDragging    = true;
    state.pendingDrag   = false;

    // Snapshot item rects before we apply any transforms
    state.snapshotItems = getItems(state.containerEl).map(el => ({
        el,
        id:   el.getAttribute('data-sortable-id'),
        rect: el.getBoundingClientRect(),
    }));

    // Style the dragged item as the "source" placeholder
    if (state.activeEl) {
        state.activeEl.setAttribute('data-dragging', 'true');
        state.activeEl.style.opacity = DRAG_PLACEHOLDER_OPACITY;
    }

    // Position and show overlay
    const overlay = getOverlay(state.instanceId);
    if (overlay) {
        const activeRect = state.activeEl ? state.activeEl.getBoundingClientRect()
                                          : { left: state.startX, top: state.startY, width: 0, height: 0 };

        state.offsetX = state.startX - activeRect.left;
        state.offsetY = state.startY - activeRect.top;

        overlay.style.width   = activeRect.width  + 'px';
        overlay.style.height  = activeRect.height + 'px';
        overlay.style.left    = activeRect.left   + 'px';
        overlay.style.top     = activeRect.top    + 'px';
        overlay.style.display = 'block';
        overlay.setAttribute('data-state', 'dragging');

        // Clone source element into overlay if no custom child content
        if (overlay.childElementCount === 0 && state.activeEl) {
            const clone = state.activeEl.cloneNode(true);
            clone.removeAttribute('data-sortable-id');
            clone.removeAttribute('data-dragging');
            clone.style.opacity   = '1';
            clone.style.transform = '';
            clone.style.transition = '';
            overlay.appendChild(clone);
            state.overlayCloned = true;
        }
    }

    document.body.style.userSelect = 'none';
    document.body.style.cursor     = 'grabbing';

    state.dotNetRef.invokeMethodAsync('OnDragStart', state.activeId).catch(() => {});
}

/**
 * Updates the overlay position and item displacement transforms on pointer/keyboard move.
 */
function moveDrag(state, x, y, orientation) {
    state.currentX = x;
    state.currentY = y;

    const overlay = getOverlay(state.instanceId);
    if (overlay) {
        overlay.style.left = (x - state.offsetX) + 'px';
        overlay.style.top  = (y - state.offsetY)  + 'px';
    }

    applyItemTransforms(state, orientation);
}

/**
 * Ends a drag: calls OnDragEnd(activeId, overId) and resets all visual state.
 */
function endDrag(state, x, y) {
    if (!state.isDragging) { resetDrag(state); return; }
    const overId = closestCenter(state.containerEl, state.activeId, x, y) ?? state.activeId;
    state.dotNetRef.invokeMethodAsync('OnDragEnd', state.activeId, overId).catch(() => {});
    resetDrag(state);
}

/**
 * Cancels a drag: calls OnDragCancel and resets all visual state.
 */
function cancelDrag(state) {
    if (state.isDragging) {
        state.dotNetRef.invokeMethodAsync('OnDragCancel').catch(() => {});
    }
    resetDrag(state);
}

/**
 * Resets all visual state after a drag (whether completed or cancelled).
 */
function resetDrag(state) {
    if (state.activeEl) {
        state.activeEl.removeAttribute('data-dragging');
        state.activeEl.style.opacity = '';
    }

    resetItemTransforms(state);

    const overlay = getOverlay(state.instanceId);
    if (overlay) {
        overlay.style.display = 'none';
        overlay.setAttribute('data-state', 'idle');
        // If content was JS-cloned (no custom ChildContent), remove it so it
        // is rebuilt fresh on the next drag.
        if (state.overlayCloned) {
            while (overlay.firstChild) overlay.removeChild(overlay.firstChild);
        }
    }

    document.body.style.userSelect = '';
    document.body.style.cursor     = '';

    state.isDragging    = false;
    state.pendingDrag   = false;
    state.activeId      = null;
    state.activeEl      = null;
    state.pointerId     = null;
    state.snapshotItems = null;
    state.overlayCloned = false;
}

// ─── Pointer sensor ──────────────────────────────────────────────────────────

function buildPointerHandlers(state, orientation) {
    function onPointerDown(e) {
        if (e.button !== 0) return;

        // Determine drag target: handle-restricted or full-item
        const handle = e.target.closest('[data-sortable-handle]');
        const item   = e.target.closest('[data-sortable-id]');
        if (!item) return;

        // If no handle is present on this item, allow full-item drag when AsHandle
        const hasHandle = item.querySelector('[data-sortable-handle]');
        if (hasHandle && !handle) return; // click was not on the handle

        state.pendingDrag = true;
        state.startX      = e.clientX;
        state.startY      = e.clientY;
        state.currentX    = e.clientX;
        state.currentY    = e.clientY;
        state.activeEl    = item;
        state.activeId    = item.getAttribute('data-sortable-id');
        state.pointerId   = e.pointerId;
        state.offsetX     = 0;
        state.offsetY     = 0;
    }

    function onPointerMove(e) {
        if (!state.pendingDrag && !state.isDragging) return;

        if (state.pendingDrag && !state.isDragging) {
            const dx = e.clientX - state.startX;
            const dy = e.clientY - state.startY;
            if (Math.abs(dx) < DRAG_THRESHOLD && Math.abs(dy) < DRAG_THRESHOLD) return;
            startDrag(state, orientation);
        }

        if (state.isDragging) moveDrag(state, e.clientX, e.clientY, orientation);
    }

    function onPointerUp(e) {
        if (!state.pendingDrag && !state.isDragging) return;
        if (state.isDragging) {
            endDrag(state, e.clientX, e.clientY);
        } else {
            resetDrag(state);
        }
    }

    function onPointerCancel() {
        cancelDrag(state);
    }

    return { onPointerDown, onPointerMove, onPointerUp, onPointerCancel };
}

// ─── Keyboard sensor ─────────────────────────────────────────────────────────

/**
 * Keyboard sensor for accessibility.
 * Space/Enter on an item (or its handle) starts keyboard-drag mode.
 * Arrow keys move the item; Escape cancels; Space/Enter confirms.
 */
function buildKeyboardHandlers(state, orientation) {
    function onKeyDown(e) {
        // Start drag
        if (!state.isDragging && !state.keyboardMode) {
            if (e.key !== ' ' && e.key !== 'Enter') return;
            const handle = e.target.closest('[data-sortable-handle]');
            const item   = e.target.closest('[data-sortable-id]');
            if (!item) return;
            const hasHandle = item.querySelector('[data-sortable-handle]');
            if (hasHandle && !handle) return;

            e.preventDefault();
            state.keyboardMode = true;
            state.activeEl     = item;
            state.activeId     = item.getAttribute('data-sortable-id');

            // Snapshot items
            state.snapshotItems = getItems(state.containerEl).map(el => ({
                el, id: el.getAttribute('data-sortable-id'), rect: el.getBoundingClientRect(),
            }));

            // Use the item's centre as the virtual cursor position
            const rect   = item.getBoundingClientRect();
            state.startX = state.currentX = rect.left + rect.width  / 2;
            state.startY = state.currentY = rect.top  + rect.height / 2;
            state.offsetX = rect.width  / 2;
            state.offsetY = rect.height / 2;
            state.isDragging = true;
            state.pendingDrag = false;

            // Show overlay
            const overlay = getOverlay(state.instanceId);
            if (overlay) {
                overlay.style.width   = rect.width  + 'px';
                overlay.style.height  = rect.height + 'px';
                overlay.style.left    = rect.left   + 'px';
                overlay.style.top     = rect.top    + 'px';
                overlay.style.display = 'block';
                overlay.setAttribute('data-state', 'dragging');
                if (overlay.childElementCount === 0) {
                    const clone = item.cloneNode(true);
                    clone.removeAttribute('data-sortable-id');
                    overlay.appendChild(clone);
                    state.overlayCloned = true;
                }
            }
            state.activeEl.setAttribute('data-dragging', 'true');
            state.activeEl.style.opacity = DRAG_PLACEHOLDER_OPACITY;
            state.dotNetRef.invokeMethodAsync('OnDragStart', state.activeId).catch(() => {});
            return;
        }

        // During keyboard drag
        if (state.keyboardMode && state.isDragging) {
            const snap       = state.snapshotItems ?? [];
            const activeIdx  = snap.findIndex(s => s.id === state.activeId);
            const itemHeight = snap[activeIdx]?.rect.height ?? DEFAULT_ITEM_HEIGHT;
            const itemWidth  = snap[activeIdx]?.rect.width  ?? DEFAULT_ITEM_WIDTH;
            let moved = false;

            if (orientation === 'vertical' || orientation === 'mixed') {
                if (e.key === 'ArrowDown' && activeIdx < snap.length - 1) {
                    state.currentY += itemHeight;
                    moved = true;
                } else if (e.key === 'ArrowUp' && activeIdx > 0) {
                    state.currentY -= itemHeight;
                    moved = true;
                }
            }
            if (orientation === 'horizontal' || orientation === 'mixed') {
                if (e.key === 'ArrowRight' && activeIdx < snap.length - 1) {
                    state.currentX += itemWidth;
                    moved = true;
                } else if (e.key === 'ArrowLeft' && activeIdx > 0) {
                    state.currentX -= itemWidth;
                    moved = true;
                }
            }

            if (moved) {
                e.preventDefault();
                moveDrag(state, state.currentX, state.currentY, orientation);
                return;
            }

            if (e.key === ' ' || e.key === 'Enter') {
                e.preventDefault();
                state.keyboardMode = false;
                endDrag(state, state.currentX, state.currentY);
                return;
            }

            if (e.key === 'Escape') {
                e.preventDefault();
                state.keyboardMode = false;
                cancelDrag(state);
                return;
            }
        }
    }

    return { onKeyDown };
}

// ─── Public API ──────────────────────────────────────────────────────────────

/**
 * Initialises drag-and-drop for a Sortable instance.
 *
 * @param {HTMLElement}           containerEl  - The SortableContentPrimitive element
 * @param {DotNetObjectReference} dotNetRef    - C# callback target
 * @param {string}                instanceId   - Unique instance identifier
 * @param {string}                orientation  - 'vertical' | 'horizontal' | 'mixed'
 */
export function init(containerEl, dotNetRef, instanceId, orientation) {
    if (!containerEl || !dotNetRef) return;

    const state = {
        isDragging:    false,
        pendingDrag:   false,
        keyboardMode:  false,
        activeId:      null,
        activeEl:      null,
        startX:        0, startY:   0,
        currentX:      0, currentY: 0,
        offsetX:       0, offsetY:  0,
        pointerId:     null,
        snapshotItems: null,
        containerEl,
        dotNetRef,
        instanceId,
        handlers:      null,
    };

    const pointer  = buildPointerHandlers(state, orientation);
    const keyboard = buildKeyboardHandlers(state, orientation);

    // Attach pointer listeners to the container so captured events still fire
    containerEl.addEventListener('pointerdown',   pointer.onPointerDown);
    containerEl.addEventListener('pointermove',   pointer.onPointerMove);
    containerEl.addEventListener('pointerup',     pointer.onPointerUp);
    containerEl.addEventListener('pointercancel', pointer.onPointerCancel);
    containerEl.addEventListener('keydown',       keyboard.onKeyDown);

    state.handlers = { ...pointer, ...keyboard };
    state.overlayCloned = false;
    instances.set(instanceId, state);
}

/**
 * Disposes all listeners for a Sortable instance and resets visual state.
 *
 * @param {string} instanceId - The instance identifier passed to init()
 */
export function dispose(instanceId) {
    const state = instances.get(instanceId);
    if (!state) return;

    resetDrag(state);

    const { containerEl, handlers } = state;
    containerEl.removeEventListener('pointerdown',   handlers.onPointerDown);
    containerEl.removeEventListener('pointermove',   handlers.onPointerMove);
    containerEl.removeEventListener('pointerup',     handlers.onPointerUp);
    containerEl.removeEventListener('pointercancel', handlers.onPointerCancel);
    containerEl.removeEventListener('keydown',       handlers.onKeyDown);

    instances.delete(instanceId);
}
