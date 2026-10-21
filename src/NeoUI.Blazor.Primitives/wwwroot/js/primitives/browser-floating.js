import { computePosition, applyPosition, autoUpdate } from './positioning.js';

// A controller owns visibility, focus and ARIA, never Razor's item/content subtree.
const controllers = new Map();
let removalObserver;

function observeRemoval(doc) {
    if (removalObserver) return;
    // One structural observer for all layers. Option/config updates are observed only
    // inside the affected layer, avoiding a full select refresh on unrelated page renders.
    removalObserver = new MutationObserver(() => {
        for (const controller of controllers.values()) if (!controller.connected) controller.dispose();
    });
    removalObserver.observe(doc.body, { childList: true, subtree: true });
}
const itemSelector = '[role="menuitem"], [role="menuitemcheckbox"], [role="menuitemradio"], [role="option"]';
const disabled = element => !element || element.matches(':disabled, [aria-disabled="true"], [data-disabled="true"]');

/** Creates a persistent controller. The injectable positioning adapter supports browser regression tests. */
export function createController(floating, initialOptions, initialPosition, bridge = null,
    positioning = { computePosition, applyPosition, autoUpdate }) {
    const doc = floating.ownerDocument;
    let options = initialOptions;
    let position = initialPosition;
    let content = doc.getElementById(options.contentId);
    if (!content) throw new Error(`Browser floating content '${options.contentId}' is missing.`);
    let opened = false;
    let visible = false;
    let disposed = false;
    let generation = 0;
    let cleanup = null;
    let focused = null;
    let selectedId = options.selectedItemId ?? null;
    let acknowledgedId = selectedId;
    let selectionSequence = 0;
    let selectionEpoch = 0;
    let commits = Promise.resolve();
    let notifications = Promise.resolve();
    let typeahead = '';
    let typeaheadTimer;
    let hoverTimer;
    const trigger = () => doc.getElementById(options.triggerId);
    const parent = () => controllers.get(trigger()?.closest('[data-neo-browser-content]')?.id);
    const belongsTo = other => {
        for (let p = other; p; p = p.parent()) if (p === controller) return true;
        return false;
    };
    const items = () => [...content.querySelectorAll(itemSelector)]
        .filter(item => item.closest('[data-neo-browser-content]') === content && !disabled(item) && !item.hidden);
    const emit = (name, detail) => trigger()?.dispatchEvent(new CustomEvent(`neoui:${name}`, { bubbles: true, detail }));
    const notify = (name, open) => {
        emit(name === 'open' ? 'openchange' : name, { open });
        const subscribed = name === 'open' ? options.notifyOpen :
            name === 'escape' ? options.notifyEscape : name === 'outside' ? options.notifyOutside : options.notifyReady;
        if (bridge && subscribed) {
            notifications = notifications.then(() => disposed ? undefined : bridge.invokeMethodAsync('NotifyAsync', name, open))
                .catch(error => emit('error', { operation: name, message: String(error) }));
        }
    };
    const setVisible = show => {
        visible = show;
        floating.style.setProperty('visibility', visible ? 'visible' : 'hidden', 'important');
        floating.style.setProperty('opacity', visible ? '1' : '0', 'important');
        floating.style.setProperty('pointer-events', visible ? 'auto' : 'none', 'important');
        floating.inert = !visible;
        content.setAttribute('data-state', visible ? 'open' : 'closed');
        content.setAttribute('aria-hidden', String(!visible));
        trigger()?.setAttribute('aria-expanded', String(visible));
        trigger()?.setAttribute('data-state', visible ? 'open' : 'closed');
    };
    const focus = item => {
        focused = item ?? null;
        for (const option of items()) {
            if (option === focused) option.setAttribute('data-focused', 'true');
            else option.removeAttribute('data-focused');
        }
        if (options.kind === 'select') {
            if (focused?.id) content.setAttribute('aria-activedescendant', focused.id);
            else content.removeAttribute('aria-activedescendant');
            content.focus({ preventScroll: true });
        } else focused?.focus({ preventScroll: true });
        focused?.scrollIntoView({ block: 'nearest' });
    };
    const applySelection = id => {
        selectedId = id ?? null;
        for (const option of content.querySelectorAll('[role="option"]')) {
            const selected = option.id === selectedId;
            option.setAttribute('aria-selected', String(selected));
            option.setAttribute('data-state', selected ? 'checked' : 'unchecked');
            for (const indicator of option.querySelectorAll('[data-neo-selection-indicator]'))
                indicator.style.display = selected ? '' : 'none';
        }
        const value = options.valueId && doc.getElementById(options.valueId);
        const display = value?.querySelector('[data-neo-value-display]');
        const fallback = value?.querySelector('[data-neo-value-fallback]');
        const option = selectedId && doc.getElementById(selectedId);
        if (display) {
            // This span is empty in Razor's render tree and is exclusively JS-owned.
            const text = option ? (option.dataset.neoText ?? option.textContent.trim()) :
                (value.dataset.neoPlaceholder ?? fallback?.textContent ?? '');
            if (display.textContent !== text) display.textContent = text;
            if (fallback) fallback.style.display = 'none';
        }
    };
    const closeChildren = report => {
        for (const child of controllers.values())
            if (child !== controller && belongsTo(child)) child.close(false, report);
    };
    const close = (restoreFocus = true, report = true) => {
        if (!opened) return;
        opened = false;
        generation++;
        clearTimeout(hoverTimer);
        closeChildren(report);
        cleanup?.apply();
        cleanup = null;
        setVisible(false);
        focused = null;
        content.removeAttribute('aria-activedescendant');
        if (restoreFocus && !disabled(trigger())) trigger()?.focus({ preventScroll: true });
        if (report) notify('open', false);
    };
    const open = async (initialFocus = 'first', report = true) => {
        if (disposed || disabled(trigger())) return;
        if (opened) return;
        // Opening a sibling closes its branch. Ancestor overlays remain open.
        for (const other of controllers.values()) {
            if (other !== controller && other.isOpen && !belongsTo(other) && !other.belongsTo(controller))
                other.close(false);
        }
        opened = true;
        const version = ++generation;
        // ARIA reflects intent immediately; visibility waits only for browser positioning.
        trigger()?.setAttribute('aria-expanded', 'true');
        if (report) notify('open', true);
        try {
            const result = await positioning.computePosition(trigger(), floating, position);
            if (disposed || !opened || version !== generation) return;
            positioning.applyPosition(floating, result, false);
            const handle = await positioning.autoUpdate(trigger(), floating, position);
            if (disposed || !opened || version !== generation) { handle?.apply(); return; }
            cleanup = handle;
            setVisible(true);
            if (options.kind === 'popover') {
                (content.querySelector('[autofocus], button:not(:disabled), a[href], input:not(:disabled), select:not(:disabled), textarea:not(:disabled), [tabindex="0"]') ?? content)
                    .focus({ preventScroll: true });
            } else {
                const enabled = items();
                focus(initialFocus === 'last' ? enabled.at(-1) :
                    options.kind === 'select' ? enabled.find(item => item.id === selectedId) ?? enabled[0] : enabled[0]);
                if (!enabled.length) content.focus({ preventScroll: true });
            }
            notify('ready', true);
        } catch (error) {
            if (version === generation) close(true);
            emit('error', { operation: 'position', message: String(error) });
        }
    };
    const select = item => {
        if (disabled(item) || disabled(trigger())) return;
        const sequence = ++selectionSequence;
        const epoch = selectionEpoch;
        applySelection(item.id);
        close();
        emit('valuechange', { itemId: item.id, sequence });
        if (!bridge) return;
        // Serialize application callbacks while letting later local selections render immediately.
        commits = commits.then(async () => {
            if (disposed || epoch !== selectionEpoch) return;
            const acceptedId = await bridge.invokeMethodAsync('CommitAsync', item.id);
            if (disposed || epoch !== selectionEpoch) return;
            acknowledgedId = acceptedId;
            if (sequence === selectionSequence) applySelection(acceptedId);
        }).catch(error => {
            if (!disposed && sequence === selectionSequence && epoch === selectionEpoch) applySelection(acknowledgedId);
            emit('error', { operation: 'selection', message: String(error) });
        });
    };
    const stop = event => { event.preventDefault(); event.stopImmediatePropagation(); };
    const click = event => {
        const target = event.target;
        if (trigger()?.contains(target) && options.kind !== 'submenu') {
            stop(event);
            if (!disabled(trigger())) opened ? close() : void open();
            return;
        }
        if (!opened || target.closest('[data-neo-browser-content]') !== content) return;
        const item = target.closest(itemSelector);
        if (!item) return;
        if (disabled(item)) { stop(event); return; }
        const submenu = [...controllers.values()].find(other => other.options.kind === 'submenu' && other.trigger() === item);
        if (submenu) { stop(event); void submenu.open(); return; }
        if (options.kind === 'select') { stop(event); select(item); return; }
        // Keep action clicks/native links intact, but dismiss before their server handler runs.
        if (item.dataset.neoCloseOnSelect !== 'false') {
            let root = controller;
            while (root.parent()?.options.kind === 'menu' || root.parent()?.options.kind === 'submenu') root = root.parent();
            root.close(true);
        }
    };
    const pointerDown = event => {
        if (!opened || !options.closeOnClickOutside) return;
        const inside = [...controllers.values()].some(other => belongsTo(other) &&
            (other.content.contains(event.target) || other.trigger()?.contains(event.target)));
        if (!inside) { close(false); notify('outside', false); }
    };
    const keydown = event => {
        const onTrigger = trigger()?.contains(event.target);
        const inContent = event.target.closest('[data-neo-browser-content]') === content;
        if (onTrigger && options.kind !== 'submenu' && ['Enter', ' ', 'ArrowDown', 'ArrowUp'].includes(event.key)) {
            stop(event);
            if (opened && ['Enter', ' '].includes(event.key)) close();
            else void open(event.key === 'ArrowUp' ? 'last' : 'first');
            return;
        }
        if (!opened || !inContent) return;
        if (event.key === 'Escape' && options.closeOnEscape) {
            stop(event); close(); notify('escape', false); return;
        }
        if (event.key === 'Tab') { close(true); return; }
        if (options.kind === 'popover' || event.target.matches('input, textarea, select, [contenteditable="true"]')) return;
        const enabled = items();
        const current = options.kind === 'select' ? focused : doc.activeElement;
        const index = enabled.indexOf(current);
        let next;
        switch (event.key === ' ' && typeahead ? 'TypeaheadSpace' : event.key) {
            case 'ArrowDown': next = options.loop ? enabled[(index + 1) % enabled.length] : enabled[Math.min(index + 1, enabled.length - 1)]; break;
            case 'ArrowUp': next = options.loop ? enabled[(index <= 0 ? enabled.length : index) - 1] : enabled[Math.max(index - 1, 0)]; break;
            case 'Home': next = enabled[0]; break;
            case 'End': next = enabled.at(-1); break;
            case 'Enter': case ' ':
                stop(event);
                if (current) options.kind === 'select' ? select(current) : current.click();
                return;
            default:
                if (event.key === (options.rtl ? 'ArrowLeft' : 'ArrowRight')) {
                    const sub = [...controllers.values()].find(other => other.options.kind === 'submenu' && other.trigger() === current);
                    if (sub) { stop(event); void sub.open(); }
                    return;
                }
                if (event.key === (options.rtl ? 'ArrowRight' : 'ArrowLeft') && options.kind === 'submenu') {
                    stop(event); close(); return;
                }
                if (event.key.length !== 1 || event.ctrlKey || event.metaKey || event.altKey) return;
                clearTimeout(typeaheadTimer);
                typeahead += event.key.toLocaleLowerCase();
                typeaheadTimer = setTimeout(() => typeahead = '', 700);
                const query = [...typeahead].every(char => char === typeahead[0]) ? typeahead[0] : typeahead;
                next = [...enabled.slice(index + 1), ...enabled.slice(0, index + 1)]
                    .find(item => (item.dataset.neoText ?? item.textContent.trim()).toLocaleLowerCase().startsWith(query));
        }
        stop(event);
        if (next) focus(next);
    };
    const pointerOver = event => {
        const item = event.target.closest(itemSelector);
        if (!opened || !item || item.closest('[data-neo-browser-content]') !== content || disabled(item)) return;
        if (item.contains(event.relatedTarget)) return;
        focus(item);
        clearTimeout(hoverTimer);
        hoverTimer = setTimeout(() => {
            if (!opened || disposed) return;
            for (const child of controllers.values()) if (child.parent() === controller) {
                if (child.trigger() === item) void child.open(); else child.close(false);
            }
        }, 200);
    };
    const pointerOut = event => {
        const item = event.target.closest(itemSelector);
        if (item && !item.contains(event.relatedTarget)) clearTimeout(hoverTimer);
    };
    const listeners = { click, pointerdown: pointerDown, keydown, pointerover: pointerOver, pointerout: pointerOut };
    const observer = new MutationObserver(() => {
        if (options.kind === 'select') applySelection(selectedId);
    });
    const triggerObserver = new MutationObserver(() => {
        if (disabled(trigger()) && opened) close(false);
    });
    let observedTrigger;
    const watchTrigger = () => {
        if (observedTrigger === trigger()) return;
        triggerObserver.disconnect();
        observedTrigger = trigger();
        if (observedTrigger) triggerObserver.observe(observedTrigger, { attributes: true,
            attributeFilter: ['disabled', 'aria-disabled', 'data-disabled'] });
    };
    const controller = {
        get options() { return options; }, get content() { return content; },
        get connected() { return floating.isConnected && trigger()?.isConnected; },
        get isOpen() { return opened; }, trigger, parent, belongsTo, open, close,
        update(nextOptions, nextPosition) {
            const openCommand = options.openVersion !== nextOptions.openVersion;
            const valueCommand = options.valueVersion !== nextOptions.valueVersion;
            const positionChanged = JSON.stringify(position) !== JSON.stringify(nextPosition);
            options = nextOptions; position = nextPosition;
            content = doc.getElementById(options.contentId) ?? content;
            content.setAttribute('data-neo-browser-content', '');
            watchTrigger();
            if (valueCommand) {
                selectionEpoch++;
                acknowledgedId = options.selectedItemId ?? null;
                applySelection(acknowledgedId);
            } else if (options.kind === 'select') applySelection(selectedId);
            if (disabled(trigger())) close(false);
            else if (openCommand) options.open ? void open('first', false) : close(true, false);
            else {
                setVisible(visible);
                if (opened && positionChanged) {
                    const version = ++generation;
                    cleanup?.apply(); cleanup = null;
                    void (async () => {
                        const result = await positioning.computePosition(trigger(), floating, position);
                        if (disposed || !opened || version !== generation) return;
                        positioning.applyPosition(floating, result, false);
                        const handle = await positioning.autoUpdate(trigger(), floating, position);
                        if (disposed || !opened || version !== generation) { handle?.apply(); return; }
                        cleanup = handle;
                        setVisible(true);
                    })().catch(error => emit('error', { operation: 'position', message: String(error) }));
                }
            }
        },
        dispose() {
            if (disposed) return;
            close(false, false);
            disposed = true;
            generation++;
            clearTimeout(hoverTimer); clearTimeout(typeaheadTimer);
            observer.disconnect();
            triggerObserver.disconnect();
            for (const [name, listener] of Object.entries(listeners)) doc.removeEventListener(name, listener, true);
            controllers.delete(options.contentId);
            if (!controllers.size) { removalObserver?.disconnect(); removalObserver = null; }
        }
    };
    controllers.get(options.contentId)?.dispose();
    controllers.set(options.contentId, controller);
    content.setAttribute('data-neo-browser-content', '');
    for (const [name, listener] of Object.entries(listeners)) doc.addEventListener(name, listener, true);
    // Observe structure/config only; JS-owned presentation attributes don't feed back into this observer.
    observer.observe(floating, { childList: true, subtree: true, attributes: true,
        attributeFilter: ['disabled', 'aria-disabled', 'data-disabled', 'data-neo-text'] });
    watchTrigger();
    observeRemoval(doc);
    setVisible(false);
    if (options.kind === 'select') applySelection(selectedId);
    if (options.open) void open('first', false);
    return controller;
}

/** Blazor interop entry point. Disposing also unregisters the controller's DOM listeners. */
export function initialize(floating, options, position, bridge) {
    return createController(floating, options, position, bridge);
}
