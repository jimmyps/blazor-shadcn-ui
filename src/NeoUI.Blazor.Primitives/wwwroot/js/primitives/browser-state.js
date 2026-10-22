import { initialize as initializeField } from './browser-field.js';
// Disclosure/navigation state is local. Rendered parameters issue versioned commands;
// application observations are ordered and never replayed after uncertain delivery.
const instances = new Map();
let removals;
export function initialize(root, bridge) {
    if (!root?.isConnected) return null;
    if (JSON.parse(root.dataset.neoStateHost).field) return initializeField(root,bridge);
    if (instances.has(root)) return instances.get(root);
    const doc = root.ownerDocument;
    let config = JSON.parse(root.dataset.neoStateHost), values = new Set(config.values), version = config.version;
    let disposed = false, sequence = 0, generation = 0, notifications = Promise.resolve();
    const own = element => element.closest('[data-neo-state-host]') === root;
    const panels = () => [...root.querySelectorAll('[data-neo-state-panel]')].filter(own);
    const triggers = () => [...root.querySelectorAll('[aria-controls]')].filter(element => own(element) && panels().some(panel => panel.id === element.getAttribute('aria-controls')));
    const key = trigger => trigger.dataset.neoStateValue ?? doc.getElementById(trigger.getAttribute('aria-controls'))?.dataset.neoStatePanel;
    const disabled = trigger => config.disabled || trigger.matches(':disabled,[data-disabled="true"]');
    const enabled = () => triggers().filter(trigger => !disabled(trigger));
    const paint = () => {
        const tabs = config.kind === 'tabs', available = enabled();
        const roving = available.find(trigger => values.has(key(trigger))) ?? available[0];
        for (const trigger of triggers()) {
            const selected = values.has(key(trigger));
            if (tabs) { trigger.setAttribute('aria-selected', String(selected)); trigger.tabIndex = trigger === roving ? 0 : -1; }
            else trigger.setAttribute('aria-expanded', String(selected));
            trigger.dataset.state = selected ? (tabs ? 'active' : 'open') : (tabs ? 'inactive' : 'closed');
            if (config.disabled) trigger.setAttribute('aria-disabled','true'); else trigger.removeAttribute('aria-disabled');
        }
        for (const panel of panels()) {
            const selected = values.has(panel.dataset.neoStatePanel);
            panel.hidden = !selected; panel.inert = !selected;
            panel.setAttribute('aria-hidden', String(!selected));
            panel.dataset.state = selected ? (tabs ? 'active' : 'open') : (tabs ? 'inactive' : 'closed');
        }
    };
    const notify = () => {
        if (!config.notify || !bridge) return;
        const event = { values: [...values], sequence: ++sequence, version, generation };
        notifications = notifications.then(async () => {
            if (disposed || event.generation !== generation) return;
            try { await bridge.invokeMethodAsync('ChangeAsync',event.values,event.sequence,event.version); }
            catch { if (!disposed && event.generation === generation) root.dataset.neoStateUncertain = 'true'; }
        });
    };
    const activate = trigger => {
        if (!trigger || disabled(trigger)) return;
        const value = key(trigger);
        if (config.kind === 'tabs') { if (values.has(value)) return; values = new Set([value]); }
        else if (values.has(value)) { if (!config.collapsible && !config.multiple) return; values.delete(value); }
        else if (config.multiple) values.add(value);
        else values = new Set([value]);
        paint(); notify();
    };
    const clicked = event => {
        const trigger = event.target.closest('[aria-controls]');
        if (!triggers().includes(trigger)) return;
        event.preventDefault(); activate(trigger);
    };
    const keydown = event => {
        if (event.isComposing || event.ctrlKey || event.metaKey || event.altKey) return;
        const trigger = event.target.closest('[aria-controls]');
        if (!triggers().includes(trigger) || disabled(trigger)) return;
        if (['Enter',' '].includes(event.key) && trigger.tagName !== 'BUTTON') { event.preventDefault(); event.stopImmediatePropagation(); activate(trigger); return; }
        const tabs = config.kind === 'tabs';
        if (config.kind === 'collapsible') return;
        const horizontal = tabs && config.orientation !== 'vertical', rtl = getComputedStyle(root).direction === 'rtl';
        const nextKey = horizontal ? (rtl ? 'ArrowLeft' : 'ArrowRight') : 'ArrowDown';
        const prevKey = horizontal ? (rtl ? 'ArrowRight' : 'ArrowLeft') : 'ArrowUp';
        if (![nextKey,prevKey,'Home','End'].includes(event.key)) return;
        event.preventDefault(); event.stopImmediatePropagation();
        const list = enabled(), index = list.indexOf(trigger);
        const next = event.key === 'Home' ? list[0] : event.key === 'End' ? list.at(-1) : list[(index + (event.key === nextKey ? 1 : -1) + list.length) % list.length];
        next?.focus({preventScroll:true});
        if (tabs && next) { if (config.automatic) activate(next); else for (const item of list) item.tabIndex = item === next ? 0 : -1; }
    };
    const observer = new MutationObserver(() => {
        const next = JSON.parse(root.dataset.neoStateHost);
        if (next.version !== version) { values = new Set(next.values); version = next.version; generation++; delete root.dataset.neoStateUncertain; }
        config = next; paint();
    });
    observer.observe(root,{subtree:true,childList:true,attributes:true,attributeFilter:['data-neo-state-host','disabled','data-disabled']});
    root.addEventListener('click',clicked,true); root.addEventListener('keydown',keydown,true);
    const controller = { focus() { enabled()[0]?.focus(); }, dispose() {
        if (disposed) return; disposed = true; generation++; observer.disconnect();
        root.removeEventListener('click',clicked,true); root.removeEventListener('keydown',keydown,true); instances.delete(root);
        if (!instances.size) { removals?.disconnect(); removals = null; }
    }};
    instances.set(root,controller);
    if (!removals) { removals = new MutationObserver(() => { for (const [element, instance] of instances) if (!element.isConnected) instance.dispose(); }); removals.observe(doc.body,{subtree:true,childList:true}); }
    paint(); root.dataset.neoStateReady = 'true'; return controller;
}
