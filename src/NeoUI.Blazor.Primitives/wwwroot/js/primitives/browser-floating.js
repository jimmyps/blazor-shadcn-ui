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
// Non-controller portals (for example TagInput suggestions) retain their anchor's
// logical containment without treating unrelated portaled content as inside.
function logicalTarget(target) {
    const seen = new Set();
    for (let layer; (layer = target?.closest?.('[data-neo-floating-owner]')) && !seen.has(layer);) {
        seen.add(layer);
        const owner = layer.ownerDocument.getElementById(layer.dataset.neoFloatingOwner);
        if (!owner?.isConnected) break;
        target = owner;
    }
    return target;
}
const disabled = element => !element || element.matches(':disabled, [aria-disabled="true"], [data-disabled="true"]');

/** Creates a persistent controller. The injectable positioning adapter supports browser regression tests. */
export function createController(floating, initialOptions, initialPosition, bridge = null,
    positioning = { computePosition, applyPosition, autoUpdate }) {
    const doc = floating.ownerDocument;
    // Initialization can itself have been buffered while disconnected. Prefer the
    // latest rendered snapshot over the interop arguments captured before reconnect.
    const readConfiguration = () => {
        const json = floating.getAttribute('data-neo-browser-config');
        return json ? JSON.parse(json) : null;
    };
    const initialConfiguration = readConfiguration();
    initialOptions = initialConfiguration?.options ?? initialOptions;
    initialPosition = initialConfiguration?.position ?? initialPosition;
    let renderedConfiguration = floating.getAttribute('data-neo-browser-config');
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
    let hoverTimer, hoverOpenTimer, hoverCloseTimer;
    const isHover = () => options.kind === 'tooltip' || options.kind === 'hovercard';
    let contextPoint = { x: options.x ?? 0, y: options.y ?? 0 };
    let backdrop, savedOverflow;
    const positionReference = () => options.kind === 'contextmenu' ? {
        contextElement: trigger(),
        getBoundingClientRect: () => ({ x: contextPoint.x, y: contextPoint.y, left: contextPoint.x, top: contextPoint.y,
            right: contextPoint.x, bottom: contextPoint.y, width: 0, height: 0 })
    } : trigger();
    const positionOptions = () => options.kind === 'contextmenu' ? { ...position, strategy: 'fixed', placement: 'bottom-start', offset: 0, padding: options.viewportPadding } : position;
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
        if (options.kind === 'tooltip') {
            const target = trigger();
            const descriptions = (target?.getAttribute('aria-describedby') ?? '').split(/\s+/).filter(id => id && id !== options.contentId);
            if (visible) descriptions.push(options.contentId);
            if (descriptions.length) target?.setAttribute('aria-describedby', descriptions.join(' '));
            else target?.removeAttribute('aria-describedby');
        } else trigger()?.setAttribute('aria-expanded', String(visible));
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
        clearTimeout(hoverOpenTimer); clearTimeout(hoverCloseTimer);
        if (!opened) return;
        opened = false;
        generation++;
        clearTimeout(hoverTimer);
        closeChildren(report);
        cleanup?.apply();
        cleanup = null;
        setVisible(false);
        backdrop?.remove(); backdrop = null;
        if (savedOverflow !== undefined) { doc.body.style.overflow = savedOverflow; savedOverflow = undefined; }
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
            if (isHover() && !['tooltip', 'hovercard'].includes(other.options.kind)) continue;
            if (other !== controller && other.isOpen && !belongsTo(other) && !other.belongsTo(controller))
                other.close(false);
        }
        opened = true;
        const version = ++generation;
        // ARIA reflects intent immediately; visibility waits only for browser positioning.
        if (options.kind !== 'tooltip') trigger()?.setAttribute('aria-expanded', 'true');
        if (report) notify('open', true);
        try {
            const result = await positioning.computePosition(positionReference(), floating, positionOptions());
            if (disposed || !opened || version !== generation) return;
            positioning.applyPosition(floating, result, false);
            const handle = await positioning.autoUpdate(positionReference(), floating, positionOptions());
            if (disposed || !opened || version !== generation) { handle?.apply(); return; }
            cleanup = handle;
            setVisible(true);
            if (options.kind === 'contextmenu') {
                if (options.lockScroll && savedOverflow === undefined) { savedOverflow = doc.body.style.overflow; doc.body.style.overflow = 'hidden'; }
                if (options.overlayClass && !backdrop) {
                    backdrop = doc.createElement('div'); backdrop.className = options.overlayClass;
                    Object.assign(backdrop.style, { position: 'fixed', inset: '0', zIndex: String(Number(getComputedStyle(floating).zIndex || 50) - 1) });
                    backdrop.setAttribute('data-neo-context-backdrop', ''); doc.body.append(backdrop);
                }
            }
            if (isHover() || initialFocus === 'none') { notify('ready', true); return; }
            if (options.kind === 'popover') {
                (content.querySelector('[data-neo-autofocus]') ?? content.querySelector('[autofocus], button:not(:disabled), a[href], input:not(:disabled), select:not(:disabled), textarea:not(:disabled), [tabindex="0"]') ?? content)
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
    // Each checkbox/radio group has its own queue and reset epoch. Razor owns
    // authoritative metadata; JS owns aria-checked, data-state, and indicator display.
    const menuSelections = new Map();
    const checkItems = () => [...content.querySelectorAll('[data-neo-check-key]')]
        .filter(item => item.closest('[data-neo-browser-content]') === content);
    const paintMenuSelection = (key, state) => {
        for (const item of checkItems().filter(item => item.dataset.neoCheckKey === key)) {
            const checked = item.getAttribute('role') === 'menuitemcheckbox'
                ? state.value === 'true' : state.value === item.dataset.neoCheckValue;
            const aria = String(checked), presentation = checked ? 'checked' : 'unchecked';
            if (item.getAttribute('aria-checked') !== aria) item.setAttribute('aria-checked', aria);
            if (item.dataset.state !== presentation) item.dataset.state = presentation;
            item.toggleAttribute('data-neo-pending', state.pending > 0);
            for (const indicator of item.querySelectorAll('[data-neo-selection-indicator]'))
                indicator.style.display = checked ? '' : 'none';
        }
    };
    const syncMenuSelections = () => {
        const current = checkItems();
        const keys = new Set(current.map(item => item.dataset.neoCheckKey));
        for (const [key, state] of menuSelections) if (!keys.has(key)) {
            state.epoch++;
            menuSelections.delete(key);
        }
        for (const item of current) {
            const key = item.dataset.neoCheckKey, version = item.dataset.neoCheckVersion;
            let state = menuSelections.get(key);
            if (!state) {
                state = { version, value: item.dataset.neoCheckAccepted ?? null,
                    accepted: item.dataset.neoCheckAccepted ?? null, sequence: 0, epoch: 0, pending: 0, queue: Promise.resolve() };
                menuSelections.set(key, state);
            } else if (state.version !== version) {
                state.version = version;
                state.epoch++;
                state.value = state.accepted = item.dataset.neoCheckAccepted ?? null;
                state.pending = 0;
            }
            paintMenuSelection(key, state);
        }
    };
    const selectMenuItem = item => {
        syncMenuSelections();
        const key = item.dataset.neoCheckKey, state = menuSelections.get(key);
        const proposed = item.getAttribute('role') === 'menuitemcheckbox'
            ? String(state.value !== 'true') : item.dataset.neoCheckValue;
        const sequence = ++state.sequence, epoch = state.epoch;
        state.value = proposed;
        state.pending++;
        paintMenuSelection(key, state);
        if (item.dataset.neoCloseOnSelect !== 'false') {
            let root = controller;
            while (['menu', 'contextmenu', 'submenu'].includes(root.parent()?.options.kind)) root = root.parent();
            root.close(true);
        }
        emit('valuechange', { key, value: proposed, sequence, pending: true });
        state.queue = state.queue.then(async () => {
            if (disposed || state.epoch !== epoch) return;
            const accepted = bridge ? await bridge.invokeMethodAsync('CommitMenuAsync', key, proposed) : proposed;
            if (disposed || state.epoch !== epoch) return;
            state.accepted = accepted;
            state.pending--;
            if (sequence === state.sequence) state.value = accepted;
            paintMenuSelection(key, state);
            emit('commit', { key, value: proposed, accepted, sequence, pending: state.pending > 0 });
        }).catch(error => {
            if (!disposed && state.epoch === epoch) {
                state.pending--;
                if (sequence === state.sequence) state.value = state.accepted;
                paintMenuSelection(key, state);
            }
            emit('error', { operation: 'menu-selection', key, message: String(error) });
        });
    };
    const stop = event => { event.preventDefault(); event.stopImmediatePropagation(); };
    const click = event => {
        if (isHover()) { if (options.kind === 'tooltip' && trigger()?.contains(event.target)) close(false); return; }
        const target = event.target;
        if (trigger()?.contains(target) && target.closest('[data-neo-trigger-action]')) return;
        if (trigger()?.contains(target) && options.kind !== 'submenu' && options.kind !== 'contextmenu') {
            stop(event);
            if (!disabled(trigger())) opened ? close() : void open();
            return;
        }
        if (!opened || target.closest('[data-neo-browser-content]') !== content) return;
        if (options.kind === 'popover') {
            const action = target.closest('[data-neo-close]');
            if (action && !disabled(action)) close(true);
            return;
        }
        const item = target.closest(itemSelector);
        if (!item) return;
        if (disabled(item)) { stop(event); return; }
        const submenu = [...controllers.values()].find(other => other.options.kind === 'submenu' && other.trigger() === item);
        if (submenu) { stop(event); void submenu.open(); return; }
        if (options.kind === 'select') { stop(event); select(item); return; }
        if (item.dataset.neoCheckKey) { stop(event); selectMenuItem(item); return; }
        // Keep action clicks/native links intact, but dismiss before their server handler runs.
        if (item.dataset.neoCloseOnSelect !== 'false') {
            let root = controller;
            while (['menu', 'contextmenu', 'submenu'].includes(root.parent()?.options.kind)) root = root.parent();
            root.close(true);
        }
    };
    const pointerDown = event => {
        if (!opened || !options.closeOnClickOutside) return;
        const target = event.target;
        const owner = logicalTarget(target);
        // An ownership marker adds logical containment; it must not discard the
        // physical hit inside a controller's own content (e.g. a marked Browser Select).
        const inside = [...controllers.values()].some(other => belongsTo(other) &&
            (other.content.contains(target) || other.trigger()?.contains(target) ||
             other.content.contains(owner) || other.trigger()?.contains(owner)));
        if (!inside) { close(false); notify('outside', false); }
    };
    const keydown = event => {
        if (event.key !== 'Escape' && trigger()?.contains(event.target) && event.target.closest('[data-neo-trigger-action]')) return;
        if (event.isComposing || event.key === 'Process') return;
        const onTrigger = trigger()?.contains(event.target);
        if (isHover()) {
            // A pointer-revealed card may have focus elsewhere. Give a nested layer
            // the first Escape; its own controller handles that dismissal.
            const nested = [...controllers.values()].some(other => other !== controller && other.isOpen &&
                (content.contains(other.trigger()) || content.contains(other.content)));
            if (event.key === 'Escape' && opened && !nested) { stop(event); close(false); }
            return;
        }
        const inContent = event.target.closest('[data-neo-browser-content]') === content;
        if (onTrigger && options.kind !== 'submenu' && options.kind !== 'contextmenu' && ['Enter', ' ', 'ArrowDown', 'ArrowUp'].includes(event.key)) {
            stop(event);
            if (opened && ['Enter', ' '].includes(event.key)) close();
            else if (opened && options.kind !== 'popover') {
                const enabled = items(); focus(event.key === 'ArrowUp' ? enabled.at(-1) : enabled[0]);
            } else void open(event.key === 'ArrowUp' ? 'last' : 'first');
            return;
        }
        if (onTrigger && options.kind === 'contextmenu' && (event.key === 'ContextMenu' || (event.shiftKey && event.key === 'F10'))) {
            stop(event);
            const rect = trigger().getBoundingClientRect();
            contextPoint = { x: rect.left, y: rect.bottom };
            reportPoint(); void open(); return;
        }
        if (opened && (onTrigger || inContent) && event.key === 'Escape' && options.closeOnEscape) {
            // Let the innermost open controller consume Escape, including when
            // focus remains on a nested trigger inside this controller's content.
            if ([...controllers.values()].some(other => other !== controller && other.isOpen && belongsTo(other) &&
                (other.content.contains(event.target) || other.trigger()?.contains(event.target)))) return;
            stop(event); close(); notify('escape', false); return;
        }
        if (!opened || !inContent) return;
        if (event.key === 'Tab') { if (options.kind !== 'popover') close(true); return; }
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
    const insideHover = target => Boolean(target && (trigger()?.contains(target) || content.contains(target)));
    const revealHover = () => {
        clearTimeout(hoverCloseTimer); clearTimeout(hoverOpenTimer);
        if (!opened) hoverOpenTimer = setTimeout(() => { if (!disposed) void open('none'); }, Math.max(0, options.openDelay ?? 0));
    };
    const dismissHover = () => {
        clearTimeout(hoverOpenTimer); clearTimeout(hoverCloseTimer);
        hoverCloseTimer = setTimeout(() => { if (!disposed && !insideHover(doc.activeElement)) close(false); }, Math.max(0, options.closeDelay ?? 0));
    };
    const focusIn = event => { if (isHover() && insideHover(event.target)) { clearTimeout(hoverCloseTimer); clearTimeout(hoverOpenTimer); void open('none'); } };
    const focusOut = event => { if (isHover() && insideHover(event.target) && !insideHover(event.relatedTarget)) dismissHover(); };
    const pointerOver = event => {
        if (isHover()) {
            if (event.pointerType !== 'touch' && insideHover(event.target) && !insideHover(event.relatedTarget)) revealHover();
            return;
        }
        if (options.kind === 'popover' || event.pointerType === 'touch') return;
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
        if (isHover()) { if (event.pointerType !== 'touch' && insideHover(event.target) && !insideHover(event.relatedTarget)) dismissHover(); return; }
        if (event.pointerType === 'touch') return;
        const item = event.target.closest(itemSelector);
        if (item && !item.contains(event.relatedTarget)) clearTimeout(hoverTimer);
    };
    const reportPoint = () => {
        if (bridge && options.notifyContext) notifications = notifications.then(() => disposed ? undefined : bridge.invokeMethodAsync('NotifyContextAsync', contextPoint.x, contextPoint.y))
            .catch(error => emit('error', { operation: 'context-position', message: String(error) }));
    };
    const contextmenu = event => {
        if (options.kind !== 'contextmenu' || !trigger()?.contains(event.target) || disabled(trigger())) return;
        stop(event);
        contextPoint = { x: event.clientX, y: event.clientY };
        if (opened) close(false, false);
        reportPoint(); void open();
    };
    const listeners = { focusin: focusIn, focusout: focusOut, contextmenu, click, pointerdown: pointerDown, keydown, pointerover: pointerOver, pointerout: pointerOut };
    const observer = new MutationObserver(() => {
        const json = floating.getAttribute('data-neo-browser-config');
        if (json !== renderedConfiguration) {
            renderedConfiguration = json;
            const next = readConfiguration();
            if (next) controller.update(next.options, next.position);
        }
        if (options.kind === 'select') applySelection(selectedId);
        syncMenuSelections();
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
        get isOpen() { return opened; }, trigger, parent, belongsTo, open, close, focus,
        update(nextOptions, nextPosition) {
            const openCommand = options.openVersion !== nextOptions.openVersion;
            const valueCommand = options.valueVersion !== nextOptions.valueVersion;
            const positionChanged = JSON.stringify(position) !== JSON.stringify(nextPosition);
            options = nextOptions; position = nextPosition;
            if (openCommand && options.kind === 'contextmenu') {
                contextPoint = { x: options.x ?? 0, y: options.y ?? 0 };
                if (opened && options.open) close(false, false);
            }
            content = doc.getElementById(options.contentId) ?? content;
            content.setAttribute('data-neo-browser-content', '');
            watchTrigger();
            syncMenuSelections();
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
                        const result = await positioning.computePosition(positionReference(), floating, positionOptions());
                        if (disposed || !opened || version !== generation) return;
                        positioning.applyPosition(floating, result, false);
                        const handle = await positioning.autoUpdate(positionReference(), floating, positionOptions());
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
            clearTimeout(hoverTimer); clearTimeout(hoverOpenTimer); clearTimeout(hoverCloseTimer); clearTimeout(typeaheadTimer);
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
        attributeFilter: ['data-neo-browser-config', 'disabled', 'aria-disabled', 'data-disabled', 'data-neo-text', 'data-neo-check-accepted', 'data-neo-check-version', 'data-neo-check-value', 'aria-checked'] });
    watchTrigger();
    syncMenuSelections();
    observeRemoval(doc);
    setVisible(false);
    if (options.kind === 'select') applySelection(selectedId);
    if (options.open) void open('first', false);
    emit('controllerready', {});
    // A mounted submenu may finish hydrating while the pointer is already on its trigger.
    if (options.kind === 'submenu' && parent()?.isOpen && trigger()?.matches(':hover'))
        hoverTimer = setTimeout(() => { if (!disposed && parent()?.isOpen && trigger()?.matches(':hover')) void open(); }, 200);
    return controller;
}

/** Blazor interop entry point. Disposing also unregisters the controller's DOM listeners. */
export function initialize(floating, options, position, bridge) {
    return createController(floating, options, position, bridge);
}

/** Dismisses the browser-owned floating shell containing a composed control. */
export function closeForElement(element, restoreFocus = true) {
    const layer = element?.closest('[data-neo-browser-content]');
    const controller = controllers.get(layer?.id);
    if (!controller) return false;
    controller.close(restoreFocus);
    return true;
}

// Shared primitive coordinators operate the same controllers without interop round trips.
export function forTrigger(id) {
    return [...controllers.values()].find(controller => controller.options.triggerId === id);
}

/** Tests logical containment when a composed navigation control contains portaled children. */
export function ownsFloatingDescendant(root, target) {
    if (!target) return false;
    const owner = logicalTarget(target);
    if (root.contains(target) || root.contains(owner)) return true;
    return [...controllers.values()].some(controller => {
        if (!controller.isOpen || !(controller.content.contains(target) || controller.content.contains(owner))) return false;
        for (let current = controller; current; current = current.parent())
            if (root.contains(current.trigger())) return true;
        return false;
    });
}
