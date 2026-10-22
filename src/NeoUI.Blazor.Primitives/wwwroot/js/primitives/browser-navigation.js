import { forTrigger, ownsFloatingDescendant } from './browser-floating.js';

const roots = new Map();
let removalObserver;
const disabled = element => !element || element.matches(':disabled, [aria-disabled="true"], [data-disabled="true"]');

// One coordinator per primitive root. Business notifications never gate UI mechanics.
export function initialize(root, bridge) {
    roots.get(root)?.dispose();
    const doc = root.ownerDocument, kind = root.dataset.neoNavigation;
    let config = JSON.parse(root.dataset.neoNavigationConfig), value = config.value ?? '', disposed = false;
    const pillEchoes = new Set();
    let pillAccepted;
    let expanding = false, lastSidebarOpen = root.querySelector('[data-sidebar="pill-nav"]')?.dataset.neoPillSidebarOpen;
    let hoverSwitchedTrigger = null;
    let timer, notifications = Promise.resolve(), reconciling = false;
    const triggers = () => [...root.querySelectorAll(kind === 'pill' ? '[data-neo-pill-item]' : '[aria-haspopup][aria-controls]')]
        .filter(t => t.closest('[data-neo-navigation]') === root && (kind !== 'navigation' || !t.closest('[data-neo-navigation-panel]')) && !disabled(t));
    const key = trigger => kind === 'pill' ? trigger?.dataset.neoPillItem ?? '' : trigger?.closest('[data-neo-menu-value]')?.dataset.neoMenuValue ?? '';
    const panel = trigger => doc.getElementById(trigger?.getAttribute('aria-controls'));
    const emit = next => {
        if (value === next) return;
        value = next;
        root.dispatchEvent(new CustomEvent('neoui:navigationchange', { bubbles: true, detail: { value } }));
        if (config.notify) notifications = notifications.then(() => disposed ? undefined : bridge.invokeMethodAsync('NotifyValueAsync', next))
            .catch(error => root.dispatchEvent(new CustomEvent('neoui:error', { bubbles: true, detail: { operation: 'navigation', message: String(error) } })));
    };
    const paint = () => {
        const all = triggers(), selected = all.find(t => key(t) === value);
        if (kind === 'pill') {
            for (const trigger of all) {
                const active = trigger === selected;
                trigger.dataset.active = String(active); trigger.setAttribute('aria-pressed', String(active));
                for (const [state, classes] of [[true, trigger.dataset.neoPillActiveClass], [false, trigger.dataset.neoPillInactiveClass]]) {
                    for (const token of (classes ?? '').split(/\s+/).filter(Boolean)) {
                        const wanted = state === active || Boolean((active ? trigger.dataset.neoPillActiveClass : trigger.dataset.neoPillInactiveClass)?.split(/\s+/).includes(token));
                        if (trigger.classList.contains(token) !== wanted) trigger.classList.toggle(token, wanted);
                    }
                }
            }
        } else if (kind === 'navigation') {
            for (const trigger of all) {
                const active = trigger === selected;
                trigger.setAttribute('aria-expanded', String(active)); trigger.dataset.state = active ? 'open' : 'closed';
                const content = panel(trigger);
                if (!content) continue;
                content.style.display = active ? '' : 'none'; content.inert = !active;
                content.setAttribute('aria-hidden', String(!active)); content.dataset.state = active ? 'open' : 'closed';
                if (active) content.dataset.activated = 'true';
            }
            for (const viewport of root.querySelectorAll('[data-slot="navigation-menu-viewport"]')) {
                const content = selected && panel(selected);
                viewport.dataset.state = content ? 'open' : 'closed';
                viewport.style.display = content ? '' : 'none';
                if (content) { viewport.style.setProperty('--radix-navigation-menu-viewport-width', `${content.scrollWidth}px`); viewport.style.setProperty('--radix-navigation-menu-viewport-height', `${content.scrollHeight}px`); }
            }
            for (const indicator of root.querySelectorAll('[data-neo-navigation-indicator]')) {
                indicator.style.visibility = selected ? 'visible' : 'hidden'; indicator.dataset.state = selected ? 'visible' : 'hidden';
                if (selected) { indicator.style.width = `${selected.offsetWidth}px`; indicator.style.transform = `translateX(${selected.offsetLeft}px)`; }
            }
        } else {
            const focused = all.includes(doc.activeElement) ? doc.activeElement : selected ?? all[0];
            for (const trigger of all) trigger.tabIndex = trigger === focused ? 0 : -1;
        }
    };
    const set = (next, focus = 'none', report = true) => {
        clearTimeout(timer);
        hoverSwitchedTrigger = null;
        const trigger = triggers().find(t => key(t) === next);
        if (kind === 'menubar') {
            for (const item of triggers()) {
                const controller = forTrigger(item.id);
                if (item === trigger) {
                    const wasOpen = controller?.isOpen;
                    void controller?.open(focus, false);
                    if (wasOpen && focus !== 'none') {
                        const items = [...controller.content.querySelectorAll('[role="menuitem"], [role="menuitemcheckbox"], [role="menuitemradio"]')]
                            .filter(element => !disabled(element) && !element.hidden);
                        controller.focus(focus === 'last' ? items.at(-1) : items[0]);
                    }
                }
                else controller?.close(false, false);
            }
        }
        if (report) emit(trigger ? next : kind === 'menubar' ? '-1' : '');
        else value = trigger ? next : kind === 'menubar' ? '-1' : '';
        paint();
        if (kind === 'navigation' && trigger && focus !== 'none') {
            const links = [...(panel(trigger)?.querySelectorAll('a[href], button:not(:disabled), input:not(:disabled), [tabindex="0"]') ?? [])].filter(t => !disabled(t));
            (focus === 'last' ? links.at(-1) : links[0])?.focus({ preventScroll: true });
        }
    };
    const owns = target => root.contains(target) || ownsFloatingDescendant(root, target) || triggers().some(t => {
        const controller = forTrigger(t.id);
        return controller?.isOpen && (controller.content.contains(target) || [...doc.querySelectorAll('[data-neo-browser-content]')].some(layer => {
            const child = forTrigger(layer.getAttribute('aria-labelledby'));
            return child?.belongsTo(controller) && child.content.contains(target);
        }));
    });
    const from = target => triggers().find(t => t.contains(target));
    const click = event => {
        if (kind === 'pill') {
            const trigger = from(event.target);
            if (trigger) { pillEchoes.add(key(trigger)); set(key(trigger)); }
            // Expanding the desktop sidebar can reveal the already-rendered aside immediately.
            if (event.target.closest('[data-neo-pill-expand]') && matchMedia('(min-width:768px)').matches) {
                const aside = root.closest('[data-neo-sidebar-provider]')?.querySelector('[data-sidebar="sidebar"]');
                if (aside) { expanding = true; aside.dataset.state = 'expanded'; aside.dataset.collapsible = 'none'; const nav = root.querySelector('[data-sidebar="pill-nav"]'); nav.style.visibility = 'hidden'; }
            }
            return;
        }
        if (kind === 'menubar') {
            const trigger = from(event.target);
            if (trigger) {
                event.preventDefault(); event.stopImmediatePropagation();
                const next = value !== key(trigger) || hoverSwitchedTrigger === trigger ? key(trigger) : '-1';
                set(next, 'first');
            }
            return;
        }
        if (kind !== 'navigation') return;
        const trigger = from(event.target);
        if (trigger) { event.preventDefault(); event.stopImmediatePropagation(); set(value === key(trigger) ? '' : key(trigger)); }
        else if (root.contains(event.target) && event.target.closest('a[href]')) {
            // Let native/router link activation finish before making its ancestors inert.
            // Chromium otherwise retargets event.target before the router's bubble listener.
            setTimeout(() => { if (!disposed) set(''); }, 0);
        }
    };
    const over = event => {
        if (kind === 'pill') return;
        const trigger = from(event.target);
        if (trigger && !trigger.contains(event.relatedTarget)) {
            clearTimeout(timer);
            if (kind === 'menubar') { if (value !== '-1' && value !== '' && value !== key(trigger)) { set(key(trigger)); hoverSwitchedTrigger = trigger; } }
            else timer = setTimeout(() => set(key(trigger)), config.delay);
        } else if (kind === 'navigation' && root.contains(event.target)) clearTimeout(timer);
    };
    const out = event => {
        if (kind === 'menubar' && hoverSwitchedTrigger?.contains(event.target) && !hoverSwitchedTrigger.contains(event.relatedTarget))
            hoverSwitchedTrigger = null;
        if (kind === 'navigation' && root.contains(event.target) && !owns(event.relatedTarget)) {
            clearTimeout(timer); timer = setTimeout(() => set(''), config.delay);
        }
    };
    const openchange = event => {
        if (kind !== 'menubar' || !root.contains(event.target) || reconciling) return;
        // A sibling close and open form one local transition, not two binding callbacks.
        queueMicrotask(() => { if (!disposed) { emit(key(triggers().find(t => forTrigger(t.id)?.isOpen)) || '-1'); paint(); } });
    };
    const focusin = event => { if (kind === 'menubar' && from(event.target)) paint(); };
    const keydown = event => {
        if (kind === 'pill' || event.isComposing || event.ctrlKey || event.metaKey || event.altKey || !owns(event.target)) return;
        const onTrigger = from(event.target), all = triggers();
        const current = onTrigger ?? all.find(t => key(t) === value);
        const index = all.indexOf(current);
        const rtl = getComputedStyle(root).direction === 'rtl';
        const nextKey = config.vertical ? 'ArrowDown' : rtl ? 'ArrowLeft' : 'ArrowRight';
        const previousKey = config.vertical ? 'ArrowUp' : rtl ? 'ArrowRight' : 'ArrowLeft';
        if (kind === 'menubar' && !onTrigger) {
            // Submenus retain their own left/right keys; right on a submenu trigger opens it.
            const layer = event.target.closest('[data-neo-browser-content]');
            if (layer && !all.some(t => panel(t) === layer)) return;
            if (event.key === nextKey && event.target.closest('[aria-haspopup="menu"]')) return;
        }
        let next;
        if ([nextKey, previousKey].includes(event.key) && (kind === 'menubar' || onTrigger)) {
            const step = event.key === nextKey ? 1 : -1, candidate = index + step;
            next = config.loop ? all[(candidate + all.length) % all.length] : all[Math.max(0, Math.min(candidate, all.length - 1))];
        } else if (onTrigger && ['Home', 'End'].includes(event.key)) next = event.key === 'Home' ? all[0] : all.at(-1);
        else if (kind === 'navigation' && onTrigger && ['Enter', ' ', 'ArrowDown', 'ArrowUp'].includes(event.key)) {
            event.preventDefault(); event.stopImmediatePropagation(); set(key(current), event.key === 'ArrowUp' ? 'last' : 'first'); return;
        } else if (event.key === 'Escape' && value !== '' && value !== '-1') {
            event.preventDefault(); event.stopImmediatePropagation(); set(kind === 'menubar' ? '-1' : ''); current?.focus(); return;
        } else if (kind === 'navigation' && event.key === 'Tab' && !owns(event.relatedTarget)) {
            // Native tab order is retained; focusout below dismisses only after focus leaves.
            return;
        }
        if (next) {
            event.preventDefault(); event.stopImmediatePropagation(); next.focus({ preventScroll: true });
            if ((value !== '' && value !== '-1') || (!onTrigger && kind === 'menubar')) set(key(next), onTrigger ? 'none' : 'first');
            paint();
        }
    };
    const outside = event => { if (kind === 'navigation' && !owns(event.target)) set(''); };
    // focusout precedes focusin; a microtask can still observe the transient BODY focus.
    const focusout = () => setTimeout(() => { if (!disposed && kind === 'navigation' && !owns(doc.activeElement)) set(''); }, 0);
    const listeners = { click, pointerover: over, pointerout: out, keydown, pointerdown: outside, 'neoui:openchange': openchange, focusin, focusout };
    // Floating controllers also capture document clicks. Coordinate top-level menubar
    // clicks first, independent of parent/child interop initialization order.
    const listenerTarget = name => kind === 'menubar' && name === 'click' ? doc.defaultView : doc;
    for (const [name, listener] of Object.entries(listeners)) listenerTarget(name).addEventListener(name, listener, true);
    const observer = new MutationObserver(() => {
        if (disposed) return;
        const next = JSON.parse(root.dataset.neoNavigationConfig);
        if (kind === 'pill') {
            const nav = root.querySelector('[data-sidebar="pill-nav"]');
            const serverOpen = nav?.dataset.neoPillSidebarOpen;
            if (serverOpen !== lastSidebarOpen) {
                if (serverOpen === 'true') expanding = false;
                else if (!expanding) nav?.style.removeProperty('visibility');
                lastSidebarOpen = serverOpen;
            }
            const accepted = triggers().find(t => t.dataset.neoPillAccepted === 'true');
            if (next.version !== config.version) { value = key(accepted); }
            else if (key(accepted) !== pillAccepted && !pillEchoes.has(key(accepted))) value = key(accepted);
            pillAccepted = key(accepted);
        }
        if (next.version !== config.version) { config = next; reconciling = true; set(kind === 'pill' ? value : next.value ?? '', 'none', false); reconciling = false; }
        else config = next;
        paint();
    });
    observer.observe(root, { childList: true, subtree: true, attributes: true, attributeFilter: ['data-neo-navigation-config', 'data-neo-pill-accepted', 'data-neo-pill-sidebar-open', 'disabled', 'aria-disabled'] });
    // Child portals can finish importing after their root coordinator.
    const ready = event => { if (kind === 'menubar' && root.contains(event.target)) set(value, 'none', false); };
    doc.addEventListener('neoui:controllerready', ready, true);
    root.dataset.neoNavigationReady = 'true';
    if (kind === 'pill') value = pillAccepted = key(triggers().find(t => t.dataset.neoPillAccepted === 'true'));
    if (kind === 'menubar' || kind === 'navigation') set(value, 'none', false); else paint();
    const controller = { dispose() {
        if (disposed) return; disposed = true; clearTimeout(timer); observer.disconnect();
        for (const [name, listener] of Object.entries(listeners)) listenerTarget(name).removeEventListener(name, listener, true);
        doc.removeEventListener('neoui:controllerready', ready, true);
        roots.delete(root);
        if (!roots.size) { removalObserver?.disconnect(); removalObserver = null; }
    } };
    roots.set(root, controller);
    if (!removalObserver) {
        removalObserver = new MutationObserver(() => { for (const [element, instance] of roots) if (!element.isConnected) instance.dispose(); });
        removalObserver.observe(doc.body, { childList: true, subtree: true });
    }
    return controller;
}
