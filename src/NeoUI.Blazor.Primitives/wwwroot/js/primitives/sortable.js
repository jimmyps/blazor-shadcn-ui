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
 * Computes the closest-center over-id using:
 *  - The overlay's current centre as the reference point (dnd-kit pattern), NOT the raw pointer.
 *    This makes displacement trigger based on where the ghost item is, accounting for where
 *    the user grabbed it within the item.
 *  - Snapshot rects for ALL items including the active item itself (stable positions, not
 *    affected by CSS transforms). If the active item's own snapshot centre is closest, the
 *    ghost is back over its original slot — returns activeId (overIdx === activeIdx → no reorder).
 *    This fixes the "space won't open when hovering back to original position" issue.
 */
function computeOverId(state) {
    if (!state.snapshotItems) return null;

    // Overlay centre in viewport space
    const overlayLeft = state.currentX - state.offsetX;
    const overlayTop  = state.currentY - state.offsetY;
    const cx = overlayLeft + state.overlayWidth  / 2;
    const cy = overlayTop  + state.overlayHeight / 2;

    let minDist = Infinity;
    let overId  = null;

    for (const snap of state.snapshotItems) {
        // Include the active item — if its snapshot centre is the nearest,
        // the ghost is back over the origin slot and no reorder should occur.
        const snapCx = snap.rect.left + snap.rect.width  / 2;
        const snapCy = snap.rect.top  + snap.rect.height / 2;
        const dist   = Math.hypot(cx - snapCx, cy - snapCy);
        if (dist < minDist) { minDist = dist; overId = snap.id; }
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

    const activeIdx  = state.snapshotItems.findIndex(s => s.id === state.activeId);
    const overId     = computeOverId(state);
    const overIdx    = state.snapshotItems.findIndex(s => s.id === overId);

    if (activeIdx < 0 || overIdx < 0) return;

    // Ghost is back over the original slot — clear all transforms (no reorder preview)
    if (overIdx === activeIdx) {
        for (const snap of state.snapshotItems) {
            snap.el.style.transition = `transform ${TRANSITION_MS}ms ease`;
            snap.el.style.transform  = '';
        }
        return;
    }

    const activeSnap = state.snapshotItems[activeIdx];
    const overSnap   = state.snapshotItems[overIdx];

    for (let i = 0; i < state.snapshotItems.length; i++) {
        const snap = state.snapshotItems[i];

        // ── Active placeholder (dnd-kit preview effect) ────────────────────────
        // Translate the in-place placeholder to the target slot using snapshot
        // positions. This opens the gap at the drop target, not at the source,
        // and eliminates visual overlap with items sliding in to fill the gap.
        if (snap.id === state.activeId) {
            let ptx = 0, pty = 0;
            if (orientation === 'vertical') {
                pty = overSnap.rect.top  - activeSnap.rect.top;
            } else if (orientation === 'horizontal') {
                ptx = overSnap.rect.left - activeSnap.rect.left;
            } else if (orientation === 'grid') {
                // Grid: placeholder moves to the exact snapshot position of the over slot
                ptx = overSnap.rect.left - activeSnap.rect.left;
                pty = overSnap.rect.top  - activeSnap.rect.top;
            }
            snap.el.style.transition = `transform ${TRANSITION_MS}ms ease`;
            snap.el.style.transform  = (ptx !== 0 || pty !== 0) ? `translate(${ptx}px,${pty}px)` : '';
            continue;
        }

        // ── Non-active items: slide to fill the gap left by the placeholder ───
        let tx = 0, ty = 0;

        if (orientation === 'vertical') {
            const activeH = activeSnap.rect.height;
            if (activeIdx < overIdx && i > activeIdx && i <= overIdx) ty = -activeH;
            else if (activeIdx > overIdx && i >= overIdx && i < activeIdx) ty = activeH;
        } else if (orientation === 'horizontal') {
            const activeW = activeSnap.rect.width;
            if (activeIdx < overIdx && i > activeIdx && i <= overIdx) tx = -activeW;
            else if (activeIdx > overIdx && i >= overIdx && i < activeIdx) tx = activeW;
        } else if (orientation === 'grid') {
            // Grid: each item in the displaced range slides to the exact snapshot
            // position of the adjacent slot (accounts for wrapping rows and gaps).
            if (activeIdx < overIdx && i > activeIdx && i <= overIdx) {
                // Moving forward → each item shifts backward one slot
                tx = state.snapshotItems[i - 1].rect.left - snap.rect.left;
                ty = state.snapshotItems[i - 1].rect.top  - snap.rect.top;
            } else if (activeIdx > overIdx && i >= overIdx && i < activeIdx) {
                // Moving backward → each item shifts forward one slot
                tx = state.snapshotItems[i + 1].rect.left - snap.rect.left;
                ty = state.snapshotItems[i + 1].rect.top  - snap.rect.top;
            }
        }
        // 'mixed' orientation: no item transforms — only the overlay moves

        snap.el.style.transition = `transform ${TRANSITION_MS}ms ease`;
        snap.el.style.transform  = (tx !== 0 || ty !== 0) ? `translate(${tx}px,${ty}px)` : '';
    }
}

/**
 * Clones sourceEl into the overlay.
 *  - width/height/transform/transition are reset so the clone
 *    fills the overlay at rest (no inherited placeholder state).
 *  - All visual appearance (bg, border, shadow, etc.) comes from
 *    the clone's own component classes and from the overlay element's
 *    own CSS classes (e.g. shadow-lg from SortableOverlay).
 */
function setupCloneInOverlay(overlay, sourceEl) {
    const clone = sourceEl.cloneNode(true);
    clone.removeAttribute('data-sortable-id');
    clone.removeAttribute('data-dragging');
    clone.style.transform  = '';   // clear any in-flight placeholder transform
    clone.style.transition = '';   // clear any in-flight placeholder transition
    clone.style.opacity    = '';   // clear placeholder opacity (0.4) copied from source
    clone.style.width      = '100%';
    clone.style.height     = '100%';

    // Table rows lose column widths when detached from their parent table.
    // Snapshot each cell's computed width and stamp it on the clone's cells.
    if (sourceEl.tagName === 'TR') {
        const sourceCells = sourceEl.querySelectorAll('td, th');
        const cloneCells  = clone.querySelectorAll('td, th');
        sourceCells.forEach((cell, i) => {
            if (cloneCells[i]) {
                cloneCells[i].style.width = cell.getBoundingClientRect().width + 'px';
            }
        });
    }

    overlay.appendChild(clone);
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
        state.overlayWidth  = activeRect.width;
        state.overlayHeight = activeRect.height;

        overlay.style.width   = activeRect.width  + 'px';
        overlay.style.height  = activeRect.height + 'px';
        overlay.style.left    = activeRect.left   + 'px';
        overlay.style.top     = activeRect.top    + 'px';
        overlay.style.display = 'block';
        overlay.setAttribute('data-state', 'dragging');

        // Clone source element into overlay if no custom child content.
        // Skip if the overlay declares custom ChildContent via data-has-child-content —
        // Blazor owns that DOM and will render it once ActiveId is set.
        if (overlay.childElementCount === 0 && state.activeEl && !overlay.dataset.hasChildContent) {
            setupCloneInOverlay(overlay, state.activeEl);
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
    // Use overlay centre + snapshot positions (same as applyItemTransforms)
    // so the final committed position matches the last visual state.
    state.currentX = x;
    state.currentY = y;
    const overId = computeOverId(state) ?? state.activeId;
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
    // Ensure document listeners are removed if resetDrag is called directly
    // (e.g. from cancelDrag or dispose) without going through onDocPointerUp.
    if (state.detachDocListeners) state.detachDocListeners();

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

/**
 * Pointer sensor following dnd-kit's PointerSensor pattern:
 *  - `pointerdown` stays on the container (to detect drag starts)
 *  - `pointermove`, `pointerup`, `pointercancel` are attached to `document`
 *    when a drag starts and removed when it ends.
 *  This ensures dragging is unconstrained — the ghost and transforms keep
 *  updating even when the pointer travels outside the container or the page.
 */
function buildPointerHandlers(state, orientation) {

    function onDocPointerMove(e) {
        if (state.pendingDrag && !state.isDragging) {
            const dx = e.clientX - state.startX;
            const dy = e.clientY - state.startY;
            if (Math.abs(dx) < DRAG_THRESHOLD && Math.abs(dy) < DRAG_THRESHOLD) return;
            startDrag(state, orientation);
        }
        if (state.isDragging) moveDrag(state, e.clientX, e.clientY, orientation);
    }

    function onDocPointerUp(e) {
        detachDocListeners();
        if (state.isDragging) {
            endDrag(state, e.clientX, e.clientY);
        } else {
            resetDrag(state);
        }
    }

    function onDocPointerCancel() {
        detachDocListeners();
        cancelDrag(state);
    }

    function attachDocListeners() {
        document.addEventListener('pointermove',   onDocPointerMove);
        document.addEventListener('pointerup',     onDocPointerUp);
        document.addEventListener('pointercancel', onDocPointerCancel);
        state.detachDocListeners = detachDocListeners;
    }

    function detachDocListeners() {
        document.removeEventListener('pointermove',   onDocPointerMove);
        document.removeEventListener('pointerup',     onDocPointerUp);
        document.removeEventListener('pointercancel', onDocPointerCancel);
        state.detachDocListeners = null;
    }

    function onPointerDown(e) {
        if (e.button !== 0) return;

        const handle = e.target.closest('[data-sortable-handle]');
        const item   = e.target.closest('[data-sortable-id]');
        if (!item) return;

        const hasHandle = item.querySelector('[data-sortable-handle]');
        if (hasHandle && !handle) return;

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

        // Attach document listeners so drag tracking is unconstrained —
        // move/up/cancel fire even when the pointer leaves the container.
        attachDocListeners();
    }

    return { onPointerDown };
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
            state.isDragging   = true;
            state.pendingDrag  = false;
            // Track which snapshot index the keyboard cursor is currently at
            state.keyboardIndex = state.snapshotItems.findIndex(s => s.id === state.activeId);

            // Show overlay
            const overlay = getOverlay(state.instanceId);
            if (overlay) {
                overlay.style.width   = rect.width  + 'px';
                overlay.style.height  = rect.height + 'px';
                overlay.style.left    = rect.left   + 'px';
                overlay.style.top     = rect.top    + 'px';
                overlay.style.display = 'block';
                overlay.setAttribute('data-state', 'dragging');
                if (overlay.childElementCount === 0 && !overlay.dataset.hasChildContent) {
                    setupCloneInOverlay(overlay, item);
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
            const snap = state.snapshotItems ?? [];

            // Always prevent scrolling for all arrow keys during drag
            if (['ArrowUp', 'ArrowDown', 'ArrowLeft', 'ArrowRight'].includes(e.key)) {
                e.preventDefault();
            }

            let moved = false;

            if (orientation === 'vertical' || orientation === 'mixed') {
                if (e.key === 'ArrowDown') {
                    state.keyboardIndex = Math.min(state.keyboardIndex + 1, snap.length - 1);
                    moved = true;
                } else if (e.key === 'ArrowUp') {
                    state.keyboardIndex = Math.max(state.keyboardIndex - 1, 0);
                    moved = true;
                }
            }
            if (orientation === 'horizontal' || orientation === 'mixed') {
                if (e.key === 'ArrowRight') {
                    state.keyboardIndex = Math.min(state.keyboardIndex + 1, snap.length - 1);
                    moved = true;
                } else if (e.key === 'ArrowLeft') {
                    state.keyboardIndex = Math.max(state.keyboardIndex - 1, 0);
                    moved = true;
                }
            }

            if (moved) {
                const target = snap[state.keyboardIndex];
                state.currentX = target.rect.left + target.rect.width  / 2;
                state.currentY = target.rect.top  + target.rect.height / 2;
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
        startX:        0, startY:      0,
        currentX:      0, currentY:    0,
        offsetX:       0, offsetY:     0,
        overlayWidth:  0, overlayHeight: 0,
        pointerId:     null,
        snapshotItems: null,
        detachDocListeners: null,
        containerEl,
        dotNetRef,
        instanceId,
        handlers:      null,
    };

    const pointer  = buildPointerHandlers(state, orientation);
    const keyboard = buildKeyboardHandlers(state, orientation);

    // Only pointerdown and keydown live on the container permanently.
    // pointermove / pointerup / pointercancel are hoisted to document
    // inside buildPointerHandlers for unconstrained drag tracking.
    containerEl.addEventListener('pointerdown', pointer.onPointerDown);
    containerEl.addEventListener('keydown',     keyboard.onKeyDown);

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
    containerEl.removeEventListener('pointerdown', handlers.onPointerDown);
    containerEl.removeEventListener('keydown',     handlers.onKeyDown);

    instances.delete(instanceId);
}
