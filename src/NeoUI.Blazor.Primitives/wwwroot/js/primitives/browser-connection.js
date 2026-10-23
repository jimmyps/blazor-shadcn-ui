// Shared public connection signal for text fields, primitive fields and tag search.
export function setConnectionAvailable(available) {
    document.documentElement.dataset.neoConnectionAvailable = String(available);
    document.dispatchEvent(new Event('neoui:input-connection-state'));
}
export function canInvokeServer() {
    const modal = document.getElementById('components-reconnect-modal');
    return navigator.onLine !== false && document.documentElement.dataset.neoConnectionAvailable !== 'false'
        && !modal?.matches('.components-reconnect-show,.components-reconnect-retrying,.components-reconnect-paused,.components-reconnect-failed,.components-reconnect-rejected');
}
document.addEventListener('components-reconnect-state-changed', event => setConnectionAvailable(event.detail?.state === 'hide'), true);
