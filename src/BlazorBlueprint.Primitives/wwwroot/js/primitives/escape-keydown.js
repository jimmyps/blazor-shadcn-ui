// Stack-based escape key detection for dialog, sheet, and drawer components.
// Uses a single document-level listener with a stack so only the topmost
// overlay handles Escape. When it closes and disposes, the next one becomes
// active — no focus-restoration dependency.

const stack = [];
let listening = false;

function handleKeyDown(e) {
  if (e.key === 'Escape' && stack.length > 0) {
    const top = stack[stack.length - 1];
    top.dotNetRef.invokeMethodAsync('JsOnEscapeKey').catch(() => {});
  }
}

export function initialize(dotNetRef, instanceId) {
  if (!dotNetRef) {
    return;
  }

  stack.push({ dotNetRef, instanceId });

  if (!listening) {
    document.addEventListener('keydown', handleKeyDown);
    listening = true;
  }
}

export function dispose(instanceId) {
  const index = stack.findIndex(s => s.instanceId === instanceId);
  if (index !== -1) {
    stack.splice(index, 1);
  }

  if (stack.length === 0 && listening) {
    document.removeEventListener('keydown', handleKeyDown);
    listening = false;
  }
}
