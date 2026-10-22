// Focus is local for mounted days. Only materializing a different month invokes .NET.
const instances = new WeakMap();
export function initialize(root, bridge) {
    if (!root?.isConnected) return null;
    if (instances.has(root)) return instances.get(root);
    const doc = root.ownerDocument;
    const days = () => [...root.querySelectorAll('[data-neo-calendar-day]')];
    const iso = date => date.toISOString().slice(0, 10);
    const date = value => new Date(value + 'T00:00:00Z');
    let active = root.dataset.neoCalendarFocus;
    let command = active, month = root.dataset.neoCalendarMonth, pending = null, disposed = false;
    const day = value => days().find(item => item.dataset.neoCalendarDay === value && !item.disabled);
    const focused = () => root.contains(doc.activeElement);
    const paint = () => {
        const nextCommand = root.dataset.neoCalendarFocus;
        const nextMonth = root.dataset.neoCalendarMonth;
        if (nextCommand !== command) { command = nextCommand; active = nextCommand; }
        if (nextMonth !== month) {
            month = nextMonth;
            if (!day(active)) active = days().find(item => !item.disabled && item.dataset.neoCalendarDay.startsWith(month))?.dataset.neoCalendarDay;
        }
        if (!day(active)) active = days().find(item => !item.disabled)?.dataset.neoCalendarDay;
        for (const item of days()) {
            const value = String(focused() && item.dataset.neoCalendarDay === active && !item.disabled);
            if (item.dataset.focused !== value) item.dataset.focused = value;
        }
        if (focused() && day(active)) root.setAttribute('aria-activedescendant', day(active).id);
        else root.removeAttribute('aria-activedescendant');
    };
    const requestMonth = async target => {
        pending = target;
        if (requestMonth.running) return;
        requestMonth.running = true;
        try {
            while (pending && !disposed) {
                const requested = pending; pending = null;
                await bridge.invokeMethodAsync('NavigateBrowserMonth', requested);
            }
        } catch (error) {
            root.dispatchEvent(new CustomEvent('neoui:error', { bubbles: true,
                detail: { operation: 'calendar-content', message: String(error) } }));
        } finally { requestMonth.running = false; }
    };
    const move = (target, direction) => {
        let candidate = target;
        for (let count = 0; count < 62; count++) {
            const key = iso(candidate);
            if ((root.dataset.neoMinDate && key < root.dataset.neoMinDate) || (root.dataset.neoMaxDate && key > root.dataset.neoMaxDate)) return;
            const rendered = days().find(item => item.dataset.neoCalendarDay === key);
            if (!rendered) { void requestMonth(key); return; }
            if (!rendered.disabled) { active = key; paint(); return; }
            candidate = new Date(candidate.getTime() + direction * 86400000);
        }
    };
    const keydown = event => {
        if (event.isComposing || event.ctrlKey || event.metaKey || event.altKey ||
            (event.target !== root && !event.target.matches('[data-neo-calendar-day]'))) return;
        if (!active) return;
        let target = date(active), direction = 1;
        switch (event.key) {
            case 'ArrowLeft': direction = -1; target.setUTCDate(target.getUTCDate() - 1); break;
            case 'ArrowRight': target.setUTCDate(target.getUTCDate() + 1); break;
            case 'ArrowUp': direction = -1; target.setUTCDate(target.getUTCDate() - 7); break;
            case 'ArrowDown': target.setUTCDate(target.getUTCDate() + 7); break;
            case 'Home': target = date(month + '-01'); break;
            case 'End': target = new Date(Date.UTC(target.getUTCFullYear(), target.getUTCMonth() + 1, 0)); direction = -1; break;
            case 'PageUp': case 'PageDown': {
                direction = event.key === 'PageUp' ? -1 : 1;
                const dayOfMonth = target.getUTCDate();
                target.setUTCDate(1);
                target.setUTCMonth(target.getUTCMonth() + direction * (event.shiftKey ? 12 : 1));
                target.setUTCDate(Math.min(dayOfMonth, new Date(Date.UTC(target.getUTCFullYear(), target.getUTCMonth() + 1, 0)).getUTCDate()));
                break;
            }
            case 'Enter': case ' ':
                event.preventDefault(); event.stopImmediatePropagation(); day(active)?.click(); return;
            default: return;
        }
        event.preventDefault(); event.stopImmediatePropagation(); move(target, direction);
    };
    const click = event => {
        const item = event.target.closest('[data-neo-calendar-day]');
        if (!item || item.disabled) return;
        active = item.dataset.neoCalendarDay;
        root.focus({ preventScroll: true }); paint();
    };
    const focusout = () => queueMicrotask(paint);
    const listeners = { keydown, click, focusin: paint, focusout };
    for (const [name, listener] of Object.entries(listeners)) root.addEventListener(name, listener, true);
    const observer = new MutationObserver(paint);
    observer.observe(root, { childList: true, subtree: true, attributes: true,
        attributeFilter: ['disabled', 'data-neo-calendar-focus', 'data-neo-calendar-month'] });
    const controller = { dispose() {
        disposed = true; observer.disconnect();
        for (const [name, listener] of Object.entries(listeners)) root.removeEventListener(name, listener, true);
        instances.delete(root);
    } };
    root.dataset.neoCalendarReady = 'true';
    instances.set(root, controller); paint(); return controller;
}
