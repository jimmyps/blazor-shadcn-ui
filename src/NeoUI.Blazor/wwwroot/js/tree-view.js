/**
 * TreeView drag-and-drop interop module for NeoUI.
 */

const DRAG_THRESHOLD = 5;
const instances = new Map();

function getTreeItemFromPoint(x, y) {
  const el = document.elementFromPoint(x, y);
  return el ? el.closest('[data-value]') : null;
}

function isDescendantOf(target, source) {
  if (!target || !source) return false;
  let el = target.parentElement;
  while (el) { if (el === source) return true; el = el.parentElement; }
  return false;
}

function getDropPosition(item, y) {
  const rect = item.getBoundingClientRect();
  const third = rect.height / 3;
  if (y < rect.top + third)    return 'before';
  if (y > rect.bottom - third) return 'after';
  return 'inside';
}

function updateIndicator(state, target, position) {
  if (!state.indicator) {
    state.indicator = document.createElement('div');
    state.indicator.style.cssText = 'position:absolute;left:0;right:0;height:2px;background:hsl(var(--primary));pointer-events:none;z-index:9999;border-radius:1px;';
    document.body.appendChild(state.indicator);
  }
  const rect = target.getBoundingClientRect();
  if (position === 'before') {
    state.indicator.style.display = 'block';
    state.indicator.style.top = (rect.top + window.scrollY) + 'px';
  } else if (position === 'after') {
    state.indicator.style.display = 'block';
    state.indicator.style.top = (rect.bottom + window.scrollY) + 'px';
  } else {
    state.indicator.style.display = 'none';
  }
}

function clearIndicator(state) {
  if (state.indicator) state.indicator.style.display = 'none';
}

function startDrag(state) {
  state.isDragging = true;
  if (state.sourceElement) state.sourceElement.style.opacity = '0.5';
  document.body.style.userSelect = 'none';
  document.body.style.cursor = 'grabbing';
  if (state.pointerId !== null) {
    try { state.containerElement.setPointerCapture(state.pointerId); } catch {}
  }
}

/**
 * Initialize drag-and-drop for a tree view instance.
 * @param {HTMLElement} containerElement
 * @param {DotNetObjectReference} dotNetRef
 * @param {string} instanceId
 */
export function initDragDrop(containerElement, dotNetRef, instanceId) {
  if (!containerElement || !dotNetRef) return;

  const state = {
    isDragging: false, pendingDrag: false,
    sourceElement: null, sourceValue: null,
    startX: 0, startY: 0, pointerId: null,
    indicator: null, containerElement,
    handlePointerDown: null, handlePointerMove: null,
    handlePointerUp: null, handlePointerCancel: null
  };

  const handlePointerDown = (e) => {
    if (e.button !== 0) return;
    const treeItem = e.target.closest('[data-value]');
    if (!treeItem) return;
    if (e.target.closest('[data-tree-toggle]') || e.target.closest('[data-tree-checkbox]')) return;
    state.pendingDrag = true;
    state.startX = e.clientX; state.startY = e.clientY;
    state.sourceElement = treeItem;
    state.sourceValue = treeItem.getAttribute('data-value');
    state.pointerId = e.pointerId;
  };

  const handlePointerMove = (e) => {
    if (!state.isDragging && !state.pendingDrag) return;
    if (state.pendingDrag && !state.isDragging) {
      const dx = e.clientX - state.startX, dy = e.clientY - state.startY;
      if (Math.abs(dx) < DRAG_THRESHOLD && Math.abs(dy) < DRAG_THRESHOLD) return;
      startDrag(state);
    }
    const targetItem = getTreeItemFromPoint(e.clientX, e.clientY);
    if (!targetItem || isDescendantOf(targetItem, state.sourceElement)) { clearIndicator(state); return; }
    updateIndicator(state, targetItem, getDropPosition(targetItem, e.clientY));
  };

  const resetState = () => {
    if (state.sourceElement) state.sourceElement.style.opacity = '';
    clearIndicator(state);
    document.body.style.userSelect = '';
    document.body.style.cursor = '';
    if (state.isDragging && state.pointerId !== null) {
      try { containerElement.releasePointerCapture(state.pointerId); } catch {}
    }
    state.isDragging = false; state.pendingDrag = false;
    state.sourceElement = null; state.sourceValue = null; state.pointerId = null;
  };

  const handlePointerUp = (e) => {
    if (!state.isDragging && !state.pendingDrag) return;
    if (!state.isDragging) { resetState(); return; }
    const targetItem = getTreeItemFromPoint(e.clientX, e.clientY);
    if (targetItem && !isDescendantOf(targetItem, state.sourceElement)) {
      const targetValue = targetItem.getAttribute('data-value');
      const position    = getDropPosition(targetItem, e.clientY);
      if (state.sourceValue && targetValue)
        dotNetRef.invokeMethodAsync('JsOnNodeDrop', state.sourceValue, targetValue, position).catch(() => {});
    }
    resetState();
  };

  state.handlePointerDown   = handlePointerDown;
  state.handlePointerMove   = handlePointerMove;
  state.handlePointerUp     = handlePointerUp;
  state.handlePointerCancel = resetState;

  containerElement.addEventListener('pointerdown',   handlePointerDown);
  containerElement.addEventListener('pointermove',   handlePointerMove);
  containerElement.addEventListener('pointerup',     handlePointerUp);
  containerElement.addEventListener('pointercancel', resetState);

  instances.set(instanceId, state);
}

/** Dispose drag-and-drop for a tree view instance. */
export function disposeDragDrop(instanceId) {
  const state = instances.get(instanceId);
  if (!state) return;
  state.containerElement.removeEventListener('pointerdown',   state.handlePointerDown);
  state.containerElement.removeEventListener('pointermove',   state.handlePointerMove);
  state.containerElement.removeEventListener('pointerup',     state.handlePointerUp);
  state.containerElement.removeEventListener('pointercancel', state.handlePointerCancel);
  if (state.indicator && state.indicator.parentElement) state.indicator.parentElement.removeChild(state.indicator);
  instances.delete(instanceId);
}
