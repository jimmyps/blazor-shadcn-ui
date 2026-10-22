import { closeForElement } from '../../NeoUI.Blazor.Primitives/js/primitives/browser-floating.js';

// The rendered items/selection remain Razor-owned. Only transient active-item state is local.
const instances = new Map();
let removalObserver;
let itemId = 0;
const disabled = item => item.matches(':disabled,[aria-disabled="true"],[data-disabled="true"]');
function observeRoots(doc) {
    if (removalObserver) return;
    const scan = node => {
        if (node.nodeType !== 1) return;
        if (node.matches('[data-neo-client-list]')) initialize(node);
        for (const child of node.querySelectorAll('[data-neo-client-list]')) initialize(child);
    };
    removalObserver = new MutationObserver(records => {
        for (const [element, instance] of instances) if (!element.isConnected) instance.dispose();
        for (const record of records) {
            if (record.type === 'attributes') scan(record.target);
            else for (const node of record.addedNodes) scan(node);
        }
    });
    removalObserver.observe(doc.body, { childList: true, subtree: true, attributes: true, attributeFilter: ['data-neo-client-list'] });
    scan(doc.body);
}
export function initialize(root) {
    observeRoots(root?.ownerDocument ?? document);
    if (!root?.isConnected) return null;
    if (instances.has(root)) return instances.get(root);
    const doc = root.ownerDocument;
    const multiple = root.dataset.neoClientList === 'multiple';
    let active = null, activeId = null, keyboard = false, lastPointer = null, disposed = false, pendingVirtual = null;
    const input = () => root.querySelector('input:not(:disabled)');
    const staleSearch = () => input()?.hasAttribute('data-neo-input-pending');
    const items = () => root.dataset.neoListDisabled === 'true' || staleSearch() ? [] : [...root.querySelectorAll('[role="option"]')].filter(item =>
        item.closest('[data-neo-client-list]') === root && !disabled(item) && !item.hidden &&
        getComputedStyle(item).display !== 'none');
    const paint = () => {
        const available = items();
        if (pendingVirtual) {
            const target = available.find(item => item.closest('[data-neo-virtualized]') === pendingVirtual.group && Number(item.dataset.neoIndex) === pendingVirtual.index);
            if (target) { active = target; activeId = target.id; pendingVirtual = null; }
        }
        if (!available.includes(active) || active?.id !== activeId) { active = null; activeId = null; }
        for (const item of root.querySelectorAll('[role="option"]')) {
            if (!item.id) item.id = `neo-client-item-${++itemId}`;
            const focused = item === active;
            if (item.dataset.focused !== String(focused)) item.dataset.focused = String(focused);
            if (!multiple && item.getAttribute('aria-selected') !== String(focused)) item.setAttribute('aria-selected', String(focused));
        }
        if (active) input()?.setAttribute('aria-activedescendant', active.id);
        else input()?.removeAttribute('aria-activedescendant');
    };
    const focus = (item, scroll = false) => {
        active = item;
        activeId = item?.id ?? null;
        paint();
        if (scroll) active?.scrollIntoView({ block: 'nearest', behavior: 'instant' });
    };
    const optionFor = event => event.target.closest('[role="option"]');
    const pointerOver = event => {
        const item = optionFor(event);
        if (keyboard || !items().includes(item)) return;
        focus(item);
    };
    const pointerMove = event => {
        const point = `${event.clientX},${event.clientY}`;
        if (point === lastPointer) return;
        lastPointer = point;
        keyboard = false;
        pointerOver(event);
    };
    const virtualTarget = (item, delta, edge = null) => {
        const group = pendingVirtual?.group ?? item?.closest('[data-neo-virtualized="true"]');
        if (!group) return false;
        const count = Number(group.dataset.neoItemCount);
        const index = edge === 'first' ? 0 : edge === 'last' ? count - 1 : (pendingVirtual?.index ?? Number(item.dataset.neoIndex)) + delta;
        if (index < 0 || index >= count) return false;
        const mounted = items().find(option => option.closest('[data-neo-virtualized]') === group && Number(option.dataset.neoIndex) === index);
        if (mounted) { focus(mounted, true); return true; }
        // Virtualize owns materialization. Scroll locally to request the next DOM window;
        // never invoke C# merely to update the focused index.
        pendingVirtual = { group, index };
        const viewport = root.querySelector('[role="listbox"]');
        if (viewport) {
            const height = item?.getBoundingClientRect().height || 32;
            const top = group.getBoundingClientRect().top - viewport.getBoundingClientRect().top + viewport.scrollTop;
            viewport.scrollTop = top + index * height;
        }
        return true;
    };
    const stop = event => { event.preventDefault(); event.stopImmediatePropagation(); };
    const keydown = event => {
        if (event.key === 'Escape' && !event.isComposing) {
            if (closeForElement(root)) stop(event);
            return;
        }
        if (event.isComposing || event.ctrlKey || event.metaKey || event.altKey ||
            (event.target !== input() && event.target !== root && !event.target.matches('[role="option"]'))) return;
        if (staleSearch() && ['ArrowDown', 'ArrowUp', 'Home', 'End', 'Enter', ' '].includes(event.key)) { stop(event); return; }
        const available = items();
        const index = available.indexOf(active);
        let next;
        switch (event.key) {
            case 'ArrowDown': if (virtualTarget(active, 1)) { stop(event); keyboard = true; return; } next = available[(index + 1) % available.length]; break;
            case 'ArrowUp': if (virtualTarget(active, -1)) { stop(event); keyboard = true; return; } next = available[index <= 0 ? available.length - 1 : index - 1]; break;
            case 'Home': if (virtualTarget(available[0], 0, 'first')) { stop(event); keyboard = true; return; } next = available[0]; break;
            case 'End': if (virtualTarget(available.at(-1), 0, 'last')) { stop(event); keyboard = true; return; } next = available.at(-1); break;
            case 'Enter':
                stop(event);
                // Only a value/action commit invokes Blazor, via the item's existing click handler.
                const selected = active ?? (!multiple ? available[0] : null);
                selected?.click();
                if (multiple || (!selected && root.dataset.neoCloseOnSelect === 'true')) closeForElement(root);
                return;
            case ' ':
                if (!multiple || !keyboard || !active) return;
                stop(event); active.click(); return;
            case 'Escape':
                if (closeForElement(root)) stop(event);
                return;
            default: return;
        }
        stop(event);
        keyboard = true;
        focus(next ?? null, true);
    };
    const click = event => {
        const item = optionFor(event);
        if (!item) return;
        if (disabled(item) || root.dataset.neoListDisabled === 'true' || staleSearch()) { stop(event); return; }
        if (!items().includes(item)) return;
        focus(item);
        if (multiple) input()?.focus({ preventScroll: true });
        if (!multiple && root.dataset.neoCloseOnSelect === 'true') closeForElement(root);
    };
    const onInput = () => { keyboard = false; pendingVirtual = null; focus(null); };
    const onInputState = () => {
        root.toggleAttribute('data-neo-search-pending', Boolean(staleSearch()));
        root.setAttribute('aria-busy', String(Boolean(staleSearch())));
        paint();
    };
    const onOpenChange = event => {
        const layer = root.closest('[data-neo-browser-content]');
        if (!layer || event.target.getAttribute('aria-controls') !== layer.id || event.detail.open) return;
        keyboard = false; focus(null);
        if (multiple && input()?.value) {
            // Query changes are application work; clearing the visible draft is immediate.
            input().value = '';
            input().dispatchEvent(new Event('input', { bubbles: true }));
        }
    };
    const observer = new MutationObserver(paint);
    observer.observe(root, { childList: true, subtree: true, characterData: true, attributes: true,
        attributeFilter: ['id', 'data-neo-index', 'disabled', 'aria-disabled', 'data-disabled', 'class', 'hidden', 'data-neo-list-disabled', 'data-neo-item-count'] });
    const listeners = { pointerover: pointerOver, pointermove: pointerMove, keydown, click, input: onInput, 'neoui:input-state': onInputState };
    for (const [name, listener] of Object.entries(listeners)) root.addEventListener(name, listener, true);
    doc.addEventListener('neoui:openchange', onOpenChange);
    const controller = {
        dispose() {
            if (disposed) return;
            disposed = true;
            observer.disconnect();
            for (const [name, listener] of Object.entries(listeners)) root.removeEventListener(name, listener, true);
            doc.removeEventListener('neoui:openchange', onOpenChange);
            instances.delete(root);
            if (!instances.size) { removalObserver?.disconnect(); removalObserver = null; }
        }
    };
    root.dataset.neoClientReady = 'true';
    instances.set(root, controller);
    paint();
    return controller;
}

export function initializeById(id) { return initializeInput(document.getElementById(id)); }

export function initializeInput(input) { return initialize(input?.closest('[data-neo-client-list]')); }


export function observeLoadMore(element, bridge) {
    if (!element?.isConnected) return null;
    let pending = false, disposed = false, lastHeight = -1;
    const scroll = async () => {
        if (disposed || pending || element.dataset.neoLoadEnabled !== 'true') return;
        if (element.scrollHeight - element.scrollTop - element.clientHeight > 80) { lastHeight = -1; return; }
        if (lastHeight === element.scrollHeight) return;
        lastHeight = element.scrollHeight;
        pending = true;
        try { await bridge.invokeMethodAsync('LoadBrowserItems'); }
        catch (error) { element.dispatchEvent(new CustomEvent('neoui:error', { bubbles: true, detail: { operation: 'load-more', message: String(error) } })); }
        finally { pending = false; }
    };
    const observer = new MutationObserver(() => { lastHeight = -1; });
    observer.observe(element, { attributes: true, attributeFilter: ['data-neo-load-enabled'] });
    element.addEventListener('scroll', scroll, { passive: true });
    return { dispose() { disposed = true; observer.disconnect(); element.removeEventListener('scroll', scroll); } };
}
