/**
 * drawer.js — Snap-point support for DrawerContent (Direction=Bottom).
 *
 * Each instance tracks:
 *  - element     : the drawer content element
 *  - dotNetRef   : DotNetObjectReference for Blazor callbacks
 *  - snapPoints  : float[] (0–1, fraction of vh)
 *  - currentSnap : active snap index
 *
 * Exported surface:
 *  init(instanceId, dotNetRef, element, snapPoints)
 *  snapTo(instanceId, index)
 *  dispose(instanceId)
 */

const instances = new Map();

// ─── Public API ────────────────────────────────────────────────────────────

export function init(instanceId, dotNetRef, element, snapPoints) {
    dispose(instanceId);

    if (!snapPoints?.length) return;

    const state = {
        dotNetRef,
        element,
        snapPoints,
        currentSnap: 0,
        isDragging: false,
        startY: 0,
        startHeight: 0,
        _removeHandleListeners: null
    };

    instances.set(instanceId, state);

    // Attach drag handlers to the handle element
    const handle = element.querySelector('[data-drawer-handle]');
    if (handle) {
        const onMouseDown = e => startDrag(state, e.clientY);
        const onTouchStart = e => startDrag(state, e.touches[0].clientY);

        handle.addEventListener('mousedown', onMouseDown);
        handle.addEventListener('touchstart', onTouchStart, { passive: true });

        state._removeHandleListeners = () => {
            handle.removeEventListener('mousedown', onMouseDown);
            handle.removeEventListener('touchstart', onTouchStart);
        };
    }

    // Initial height is already set by Blazor inline style; just record current snap.
    state.currentSnap = 0;
    resetScroll(element);
}

export function snapTo(instanceId, index) {
    const state = instances.get(instanceId);
    if (!state || index < 0 || index >= state.snapPoints.length) return;
    setSnapHeight(state, index, /* instant */ false);
}

export function dispose(instanceId) {
    const state = instances.get(instanceId);
    if (state) {
        state._removeHandleListeners?.();
    }
    instances.delete(instanceId);
}

// ─── Internal helpers ──────────────────────────────────────────────────────

function setSnapHeight(state, index, instant) {
    const fraction = state.snapPoints[index];
    const height = fraction * window.innerHeight;

    if (instant) {
        state.element.style.transition = 'none';
    } else {
        const ease = 'cubic-bezier(0.32, 0.72, 0, 1)';
        state.element.style.transition = `height 0.3s ${ease}, max-height 0.3s ${ease}`;
        setTimeout(() => {
            if (state.element) state.element.style.transition = '';
        }, 350);
    }

    state.element.style.height = `${height}px`;
    state.element.style.maxHeight = `${height}px`;
    state.element.style.overflowY = 'auto';
    state.currentSnap = index;

    // After shrinking, reset scroll so content isn't offset
    if (instant) {
        resetScroll(state.element);
    } else {
        setTimeout(() => resetScroll(state.element), 310);
    }
}

function startDrag(state, startY) {
    if (state.isDragging) return;

    state.isDragging = true;
    state.startY = startY;
    state.startHeight = state.element.getBoundingClientRect().height;
    state.element.style.transition = 'none';
    state.element.style.userSelect = 'none';

    const onMouseMove = e => moveDrag(state, e.clientY);
    const onTouchMove = e => moveDrag(state, e.touches[0].clientY);
    const onMouseUp   = e => endDrag(state, e.clientY, cleanup);
    const onTouchEnd  = e => endDrag(state, e.changedTouches[0].clientY, cleanup);

    function cleanup() {
        document.removeEventListener('mousemove', onMouseMove);
        document.removeEventListener('mouseup', onMouseUp);
        document.removeEventListener('touchmove', onTouchMove);
        document.removeEventListener('touchend', onTouchEnd);
        state.element.style.userSelect = '';
    }

    document.addEventListener('mousemove', onMouseMove);
    document.addEventListener('mouseup', onMouseUp);
    document.addEventListener('touchmove', onTouchMove, { passive: true });
    document.addEventListener('touchend', onTouchEnd);
}

function moveDrag(state, currentY) {
    if (!state.isDragging) return;
    const delta = state.startY - currentY;
    const newHeight = Math.max(50, state.startHeight + delta);
    state.element.style.height = `${newHeight}px`;
    state.element.style.maxHeight = `${newHeight}px`;
}

function endDrag(state, currentY, cleanup) {
    cleanup();
    if (!state.isDragging) return;
    state.isDragging = false;

    const currentFraction = state.element.getBoundingClientRect().height / window.innerHeight;

    // Find nearest snap point
    let nearest = 0;
    let minDiff = Infinity;
    state.snapPoints.forEach((sp, i) => {
        const diff = Math.abs(sp - currentFraction);
        if (diff < minDiff) {
            minDiff = diff;
            nearest = i;
        }
    });

    const prevSnap = state.currentSnap;
    setSnapHeight(state, nearest, /* instant */ false);

    if (nearest !== prevSnap) {
        state.dotNetRef.invokeMethodAsync('OnSnapIndexChangedFromJs', nearest);
    }
}

function resetScroll(element) {
    const scrollable = element.querySelector('.overflow-y-auto') ?? element;
    scrollable.scrollTop = 0;
}
