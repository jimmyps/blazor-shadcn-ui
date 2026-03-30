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
const groups     = new Map(); // groupName → Set<instanceId>

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

/** Returns the instanceIds of all other instances in the same group. */
function getGroupPeers(state) {
    if (!state.group) return [];
    const group = groups.get(state.group);
    if (!group) return [];
    const peers = [];
    for (const id of group) {
        if (id !== state.instanceId) peers.push(id);
    }
    return peers;
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
 *  - For cross-list groups: also scans peer containers. Sets state.overInstanceId as a side-effect.
 */
function computeOverId(state) {
    if (!state.snapshotItems) return null;

    // Overlay centre in viewport space
    const overlayLeft = state.currentX - state.offsetX;
    const overlayTop  = state.currentY - state.offsetY;
    const cx = overlayLeft + state.overlayWidth  / 2;
    const cy = overlayTop  + state.overlayHeight / 2;

    let minDist        = Infinity;
    let overId         = null;
    let overInstanceId = state.instanceId;

    for (const snap of state.snapshotItems) {
        // Include the active item — if its snapshot centre is the nearest,
        // the ghost is back over the origin slot and no reorder should occur.
        const snapCx = snap.rect.left + snap.rect.width  / 2;
        const snapCy = snap.rect.top  + snap.rect.height / 2;
        const dist   = Math.hypot(cx - snapCx, cy - snapCy);
        if (dist < minDist) { minDist = dist; overId = snap.id; overInstanceId = state.instanceId; }
    }

    // Scan peer containers in the same group
    if (state.groupSnapshots) {
        for (const [instanceId, snaps] of state.groupSnapshots) {
            // Empty container: treat the container rect as a virtual drop target
            const peerState = instances.get(instanceId);
            if (snaps.length === 0 && peerState) {
                const rect = peerState.containerEl.getBoundingClientRect();
                if (cx >= rect.left && cx <= rect.right && cy >= rect.top && cy <= rect.bottom) {
                    // Pointer is inside an empty peer container — use centre distance to container
                    const dist = Math.hypot(cx - (rect.left + rect.width / 2), cy - (rect.top + rect.height / 2));
                    if (dist < minDist) {
                        minDist = dist;
                        overId = '__neo_empty__';
                        overInstanceId = instanceId;
                    }
                }
                continue;
            }
            for (const snap of snaps) {
                const snapCx = snap.rect.left + snap.rect.width  / 2;
                const snapCy = snap.rect.top  + snap.rect.height / 2;
                const dist   = Math.hypot(cx - snapCx, cy - snapCy);
                if (dist < minDist) { minDist = dist; overId = snap.id; overInstanceId = instanceId; }
            }
        }
    }

    state.overInstanceId = overInstanceId;

    // When the overlay centre is below the last item's bottom edge in a peer container
    // (i.e. it's in the padded drop zone), treat the drop as "append at end".
    if (overInstanceId !== state.instanceId && overId !== '__neo_empty__') {
        const peerSnaps = state.groupSnapshots?.get(overInstanceId);
        if (peerSnaps && peerSnaps.length > 0) {
            const lastSnap = peerSnaps[peerSnaps.length - 1];
            if (cy > lastSnap.rect.bottom) {
                overId = '__neo_append__';
            }
        }
    }

    return overId;
}

/**
 * Cross-container displacement: source items close the gap, target items open a gap.
 * Called when the active item is hovering over a different group container.
 */
function applyItemTransformsCross(state, activeIdx, overId) {
    const activeSnap    = state.snapshotItems[activeIdx];
    const overInstanceId = state.overInstanceId;
    const targetSnaps   = state.groupSnapshots?.get(overInstanceId) ?? [];
    const overIdxInTarget = overId === '__neo_empty__'  ? 0
                          : overId === '__neo_append__' ? targetSnaps.length
                          : targetSnaps.findIndex(s => s.id === overId);
    const targetOrientation = instances.get(overInstanceId)?.orientation ?? 'vertical';

    // Source container: items close the gap left by the active placeholder.
    for (let i = 0; i < state.snapshotItems.length; i++) {
        const snap = state.snapshotItems[i];
        if (snap.id === state.activeId) {
            snap.el.style.transition = '';
            snap.el.style.transform  = '';
            continue;
        }
        let tx = 0, ty = 0;
        if (state.orientation === 'vertical' || state.orientation === 'mixed') {
            if (i > activeIdx) ty = -activeSnap.rect.height;
        } else if (state.orientation === 'horizontal') {
            if (i > activeIdx) tx = -activeSnap.rect.width;
        }
        snap.el.style.transition = `transform ${TRANSITION_MS}ms ease`;
        snap.el.style.transform  = (tx !== 0 || ty !== 0) ? `translate(${tx}px,${ty}px)` : '';
    }

    // Target container: open a gap at the drop position.
    for (let i = 0; i < targetSnaps.length; i++) {
        const snap = targetSnaps[i];
        let tx = 0, ty = 0;
        if (overIdxInTarget >= 0 && i >= overIdxInTarget) {
            if (targetOrientation === 'vertical' || targetOrientation === 'mixed') {
                ty = activeSnap.rect.height;
            } else if (targetOrientation === 'horizontal') {
                tx = activeSnap.rect.width;
            }
        }
        snap.el.style.transition = `transform ${TRANSITION_MS}ms ease`;
        snap.el.style.transform  = (tx !== 0 || ty !== 0) ? `translate(${tx}px,${ty}px)` : '';
    }
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
    const overId     = computeOverId(state); // also sets state.overInstanceId
    const isCross    = state.overInstanceId && state.overInstanceId !== state.instanceId;

    if (activeIdx < 0) return;

    if (isCross) {
        applyItemTransformsCross(state, activeIdx, overId);
        return;
    }

    // ── Same-container reorder (existing logic) ───────────────────────────────
    const overIdx = state.snapshotItems.findIndex(s => s.id === overId);
    if (overIdx < 0) return;

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
    // Also copy the row's resolved font-size so children without an explicit
    // font-size class (e.g. a bare SortableItemHandle button) inherit the
    // correct value rather than falling back to the browser default (16px).
    if (sourceEl.tagName === 'TR') {
        clone.style.fontSize = window.getComputedStyle(sourceEl).fontSize;
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

/**
 * Updates `data-state` on peer containers to reflect which one the overlay is hovering over.
 * Only touches the DOM when the hovered container changes, to avoid thrashing on every move.
 */
function updateGroupContainerStates(state) {
    if (!state.groupSnapshots || state.groupSnapshots.size === 0) return;

    const newOverId = (state.overInstanceId && state.overInstanceId !== state.instanceId)
        ? state.overInstanceId
        : null;

    if (state.overContainerInstanceId === newOverId) return; // nothing changed

    for (const peerId of state.groupSnapshots.keys()) {
        const peerState = instances.get(peerId);
        if (!peerState) continue;
        if (peerId === newOverId) {
            peerState.containerEl.setAttribute('data-state', 'over');
        } else {
            peerState.containerEl.removeAttribute('data-state');
        }
    }
    state.overContainerInstanceId = newOverId;
}

/**
 * Finds scrollable elements that are DESCENDANTS of containerEl (e.g. a DataTable's
 * inner scroll wrapper). We do NOT walk up the DOM — that would capture page-level
 * scroll containers and break the layout.
 */
function findScrollableDescendants(containerEl) {
    const result = [];
    const candidates = containerEl.querySelectorAll('*');
    for (const el of candidates) {
        const cs = window.getComputedStyle(el);
        const oy = cs.overflowY;
        const ox = cs.overflowX;
        if (oy === 'auto' || oy === 'scroll' || ox === 'auto' || ox === 'scroll') {
            result.push({
                target:    el,
                overflowY: el.style.overflowY,
                overflowX: el.style.overflowX,
                maxHeight: el.style.maxHeight,
            });
        }
    }
    return result;
}

/** Resets all item transforms to their natural positions (own + peer containers). */
function resetItemTransforms(state) {
    if (!state.snapshotItems) return;
    for (const snap of state.snapshotItems) {
        snap.el.style.transition = '';
        snap.el.style.transform  = '';
    }
    if (state.groupSnapshots) {
        for (const [, snaps] of state.groupSnapshots) {
            for (const snap of snaps) {
                snap.el.style.transition = '';
                snap.el.style.transform  = '';
            }
        }
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

    // Snapshot peer containers so cross-container collision can use stable rects,
    // then expand each peer's bottom padding so the end of the list is an easy drop target,
    // and temporarily remove overflow clipping so all rows are reachable as drop targets.
    state.groupSnapshots = new Map();
    state.peerScrollableAncestors = new Map();
    for (const peerId of getGroupPeers(state)) {
        const peerState = instances.get(peerId);
        if (!peerState) continue;
        state.groupSnapshots.set(peerId, getItems(peerState.containerEl).map(el => ({
            el,
            id:   el.getAttribute('data-sortable-id'),
            rect: el.getBoundingClientRect(),
        })));
        // Expand the peer container so users can easily drop at the end of the list
        const activeHeight = state.activeEl ? state.activeEl.getBoundingClientRect().height : DEFAULT_ITEM_HEIGHT;
        peerState.containerEl.style.paddingBottom = activeHeight + 'px';
        // Remove overflow clipping on scrollable ancestors so all rows are visible/droppable
        const ancestors = findScrollableDescendants(peerState.containerEl);
        ancestors.forEach(a => {
            a.target.style.overflowY = 'visible';
            a.target.style.overflowX = 'visible';
            a.target.style.maxHeight = 'none';
        });
        state.peerScrollableAncestors.set(peerId, ancestors);
    }

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
    updateGroupContainerStates(state);
}

/**
 * Freezes the container's current visual state as a fixed-position clone,
 * then hides the real container. This lets Blazor re-render underneath
 * without any visible flash. Call snapshot.remove() once Blazor has settled.
 */
function freezeSnapshot(container) {
    const rect  = container.getBoundingClientRect();
    const clone = container.cloneNode(true);
    clone.style.cssText = `
        position: fixed;
        top: ${rect.top}px;
        left: ${rect.left}px;
        width: ${rect.width}px;
        height: ${rect.height}px;
        z-index: 9999;
        pointer-events: none;
        margin: 0;
        overflow: hidden;
    `;
    document.body.appendChild(clone);
    container.style.visibility = 'hidden';
    return {
        remove() {
            clone.remove();
            container.style.visibility = '';
        }
    };
}

/**
 * Ends a drag: positions the active item at the overlay's location via
 * transform (no DOM order change), snapshots the correct settled visual state,
 * then resets everything under the snapshot while Blazor re-renders.
 *
 * For cross-list drops: asks the target instance (OnTransferIn) first.
 * If accepted, notifies the source (OnTransferOut). Both containers are
 * snapshotted so their Blazor re-renders are seamless.
 */
function endDrag(state, x, y) {
    if (!state.isDragging) { resetDrag(state); return; }
    state.currentX = x;
    state.currentY = y;
    const overId           = computeOverId(state) ?? state.activeId;
    const activeId         = state.activeId; // save before resetDrag nulls it
    const sourceInstanceId = state.instanceId;
    const targetInstanceId = state.overInstanceId ?? state.instanceId;
    const isCross          = sourceInstanceId !== targetInstanceId;
    const moved            = overId !== activeId || isCross;

    let snapshot = null;
    if (moved && state.activeEl) {
        const overlay = getOverlay(state.instanceId);

        state.activeEl.style.opacity    = '1';
        state.activeEl.removeAttribute('data-dragging');

        // Hide overlay before snapshotting so it doesn't float above the clone.
        if (overlay) overlay.style.display = 'none';

        if (isCross) {
            // Snapshot both containers so both Blazor re-renders are seamless
            const targetState  = instances.get(targetInstanceId);
            const snapSource   = freezeSnapshot(state.containerEl);
            const snapTarget   = targetState ? freezeSnapshot(targetState.containerEl) : null;
            snapshot = { remove() { snapSource.remove(); snapTarget?.remove(); } };
        } else {
            snapshot = freezeSnapshot(state.containerEl);
        }
    }

    // Clean up all drag state under the snapshot (invisible to the user).
    resetDrag(state);

    if (isCross && moved) {
        const targetState = instances.get(targetInstanceId);
        if (!targetState) { snapshot?.remove(); return; }

        // Ask target if it will accept the drop. If yes, notify source to transfer out.
        targetState.dotNetRef.invokeMethodAsync('OnTransferIn', activeId, overId, sourceInstanceId, targetInstanceId)
            .then(accepted => {
                if (accepted) {
                    return state.dotNetRef.invokeMethodAsync('OnTransferOut', activeId, overId, sourceInstanceId, targetInstanceId);
                }
                // Rejected: snapshot.remove() is handled by the final .then/.catch
            })
            .then(() => { snapshot?.remove(); })
            .catch(() => { snapshot?.remove(); });
    } else {
        state.dotNetRef.invokeMethodAsync('OnDragEnd', activeId, overId)
            .then(() => { snapshot?.remove(); })
            .catch(() => { snapshot?.remove(); });
    }
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

    // Restore peer container padding, overflow, and clear hover state applied during drag
    if (state.groupSnapshots) {
        for (const peerId of state.groupSnapshots.keys()) {
            const peerState = instances.get(peerId);
            if (peerState) {
                peerState.containerEl.style.paddingBottom = '';
                peerState.containerEl.removeAttribute('data-state');
            }
        }
    }
    if (state.peerScrollableAncestors) {
        for (const ancestors of state.peerScrollableAncestors.values()) {
            ancestors.forEach(a => {
                a.target.style.overflowY = a.overflowY;
                a.target.style.overflowX = a.overflowX;
                a.target.style.maxHeight = a.maxHeight;
            });
        }
    }

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
    state.groupSnapshots = null;
    state.peerScrollableAncestors = null;
    state.overInstanceId = null;
    state.overContainerInstanceId = null;
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
 * @param {string|null}           group        - Group name for cross-list DnD (optional)
 */
export function init(containerEl, dotNetRef, instanceId, orientation, group) {
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
        groupSnapshots: null,
        peerScrollableAncestors: null,
        overInstanceId: null,
        overContainerInstanceId: null,
        detachDocListeners: null,
        containerEl,
        dotNetRef,
        instanceId,
        orientation,
        group:         group ?? null,
        handlers:      null,
    };

    // Register in group registry so peers can discover this instance
    if (group) {
        if (!groups.has(group)) groups.set(group, new Set());
        groups.get(group).add(instanceId);
    }

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

    // Unregister from group so peers no longer see this instance
    if (state.group) {
        groups.get(state.group)?.delete(instanceId);
    }

    resetDrag(state);

    const { containerEl, handlers } = state;
    containerEl.removeEventListener('pointerdown', handlers.onPointerDown);
    containerEl.removeEventListener('keydown',     handlers.onKeyDown);

    instances.delete(instanceId);
}
