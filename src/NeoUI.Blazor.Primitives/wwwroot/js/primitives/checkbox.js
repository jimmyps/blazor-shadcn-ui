// A checkbox uses Space, unlike its native button host which also activates on
// Enter. Suppress only Enter locally; Space retains exactly one native click.
const handlers = new WeakMap(), disposed = new WeakSet();
export function initialize(element) {
    if (!element?.isConnected || disposed.has(element) || handlers.has(element)) return;
    const keydown = event => { if (event.key === 'Enter') event.preventDefault(); };
    element.addEventListener('keydown', keydown);
    handlers.set(element, keydown);
    element.dataset.neoCheckboxReady = 'true';
}
export function dispose(element) {
    if (!element) return;
    disposed.add(element);
    const handler = handlers.get(element);
    if (handler) element.removeEventListener('keydown', handler);
    handlers.delete(element);
    delete element.dataset.neoCheckboxReady;
}
