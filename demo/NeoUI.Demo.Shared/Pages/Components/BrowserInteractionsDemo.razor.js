// The live activity display is browser-owned, so observing UI events adds no server traffic.
export async function initialize(root) {
    await import('../../js/vendor/floating-ui.core.umd.min.js');
    await import('../../js/vendor/floating-ui.dom.umd.min.js');
    const count = root.querySelector('[data-local-events]');
    const reveal = root.querySelector('[data-reveal-time]');
    const log = root.querySelector('[data-event-log]');
    const empty = root.querySelector('[data-empty-log]');
    const starts = new Map();
    let events = 0;
    let disposed = false;
    count.textContent = '0';
    reveal.textContent = '—';
    const start = event => {
        const trigger = event.target.closest('[aria-controls]');
        if (trigger?.id) starts.set(trigger.id, performance.now());
    };
    const record = event => {
        if (disposed) return;
        if (event.type === 'neoui:ready') {
            const began = starts.get(event.target.id);
            if (began != null) { reveal.textContent = Math.round(performance.now() - began) + ' ms'; starts.delete(event.target.id); }
            return;
        }
        count.textContent = String(++events);
        empty.style.display = 'none';
        const row = document.createElement('li');
        const kind = document.createElement('span');
        const label = document.createElement('span');
        const time = document.createElement('time');
        kind.className = 'font-medium text-foreground';
        kind.textContent = event.type === 'neoui:valuechange' ? 'SELECT' : event.type === 'neoui:error' ? 'ERROR' : event.detail.open ? 'OPEN' : 'CLOSE';
        label.textContent = event.target.getAttribute('aria-haspopup') === 'listbox' ? 'Workspace select' : (event.target.textContent.trim().replace(/\s+/g, ' ').slice(0, 35) || 'Floating layer');
        if (event.type === 'neoui:error') label.textContent = event.detail.message;
        time.textContent = new Date().toLocaleTimeString([], { hour12: false });
        row.append(kind, label, time);
        log.prepend(row);
        while (log.children.length > 6) log.lastElementChild.remove();
    };
    const names = ['neoui:openchange', 'neoui:valuechange', 'neoui:ready', 'neoui:error'];
    for (const name of names) document.addEventListener(name, record);
    document.addEventListener('pointerdown', start, true);
    document.addEventListener('keydown', start, true);
    const observer = new MutationObserver(() => { if (!root.isConnected) dispose(); });
    observer.observe(document.body, { childList: true, subtree: true });
    function dispose() {
        if (disposed) return;
        disposed = true;
        observer.disconnect();
        for (const name of names) document.removeEventListener(name, record);
        document.removeEventListener('pointerdown', start, true);
        document.removeEventListener('keydown', start, true);
    }
    return { dispose };
}
