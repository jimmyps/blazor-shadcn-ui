import { attachFormat } from './browser-input-formats.js';
// Razor renders a fixed initial value in Browser mode. Only this controller writes
// subsequent DOM values; post-render repair cannot prevent stale render-batch writes.
const controllers = new WeakMap();
const controllerFor = element => controllers.get(element) ?? element?.[Symbol.for('NeoUI.BrowserField')];
const activeScopes = new Map();
const editFormGuards = new WeakMap();
// Resolve from the application's base, not this module's possibly fingerprinted URL.
const { canInvokeServer, setConnectionAvailable } = await import(new URL('_content/NeoUI.Blazor.Primitives/js/primitives/browser-connection.js', document.baseURI).href);
export { setConnectionAvailable };

export function initialize(element, bridge) {
    if (!element || controllers.has(element)) return;
    let config = JSON.parse(element.getAttribute('data-neo-input-config') || '{}');
    // The fixed Razor value is the hydration baseline. A different live value
    // belongs to a user who typed before the module finished loading.
    let pristine = element.value === element.defaultValue;
    const formatting = attachFormat(element, () => config.format);
    let revision = config.revision ?? 0, sequence = 0, draft = element.value;
    let lastSubmitted = config.value ?? '', submittedSequence = 0, acceptedValue = config.value ?? '', composing = false, timer, running = false, disposed = false;
    const queue = [];
    let lastFinalSequence = -1, equivalentSequence = -1;
    let heldAck, generation = 0, observedEmpty = formatting.isEmpty(config.value ?? ''), lockDepth = 0;
    const waiters = new Set();
    const finish = (waiter, status) => { clearTimeout(waiter.timer); waiters.delete(waiter); waiter.resolve({ status, fieldId: element.id }); };
    const settleWaiters = () => {
        for (const waiter of [...waiters]) {
            if (disposed) finish(waiter, 'Disposed');
            else if (waiter.revision !== revision) finish(waiter, 'Reset');
            else if (waiter.sequence !== sequence) finish(waiter, 'Changed');
            else if (element.hasAttribute('data-neo-input-uncertain')) finish(waiter, 'Uncertain');
            else if (element.hasAttribute('data-neo-input-invalid')) finish(waiter, 'Invalid');
            else if (!element.hasAttribute('data-neo-input-pending')) finish(waiter, (acceptedValue === waiter.value || equivalentSequence === waiter.sequence) ? 'Success' : 'ValueAdjusted');
        }
    };
    const emit = () => { settleWaiters(); element.dispatchEvent(new CustomEvent('neoui:input-state', {
        bubbles: true, detail: { value: draft, sequence, revision,
            pending: element.hasAttribute('data-neo-input-pending'),
            uncertain: element.hasAttribute('data-neo-input-uncertain') }
    })); };
    const pending = value => { element.toggleAttribute('data-neo-input-pending', value); emit(); };
    const cancelTimer = () => { clearTimeout(timer); timer = undefined; };
    const write = value => {
        // Avoid touching the value/caret for unchanged acknowledgements.
        if (element.value !== value) {
            const start = element.selectionStart, end = element.selectionEnd, direction = element.selectionDirection;
            element.value = value;
            if (document.activeElement === element && start !== null && end !== null)
                element.setSelectionRange(Math.min(start, value.length), Math.min(end, value.length), direction ?? 'none');
        }
        draft = value;
    };
    const observeDraft = () => {
        if (draft === element.value) return false;
        pristine = false;
        draft = element.value; sequence++;
        if (element.hasAttribute('data-neo-input-invalid')) { element.removeAttribute('data-neo-input-invalid'); element.setCustomValidity(''); formatting.validate(); }
        element.removeAttribute('data-neo-input-uncertain');
        pending(true);
        return true;
    };
    const reconcile = ack => {
        if (disposed || ack.revision !== revision || ack.sequence !== sequence) return;
        if (composing) { heldAck = ack; return; }
        if (ack.ignored) { element.setAttribute('data-neo-input-uncertain', ''); emit(); return; }
        element.toggleAttribute('data-neo-input-invalid', Boolean(ack.invalid));
        if (ack.invalid) element.setCustomValidity('Enter a value supported by this field.');
        equivalentSequence = ack.equivalent ? sequence : -1;
        write(ack.value ?? ''); formatting.reconcile(ack.format); lastSubmitted = acceptedValue = draft; submittedSequence = sequence;
        observedEmpty = formatting.isEmpty(draft);
        element.removeAttribute('data-neo-input-uncertain');
        pending(false);
    };
    async function pump() {
        if (running || disposed) return;
        running = true;
        const currentGeneration = generation;
        try {
            while (queue.length && !disposed && currentGeneration === generation) {
                const edit = queue.shift();
                if (edit.revision !== revision) continue;
                try {
                    if (config.server && !canInvokeServer()) {
                        queue.unshift(edit); break;
                    }
                    const ack = await bridge.invokeMethodAsync(edit.final ? 'OnBrowserInputCommitted' : 'OnBrowserInputChanged', edit.value, edit.sequence, edit.revision);
                    if (currentGeneration !== generation || disposed || edit.revision !== revision || ack.revision !== revision) continue;
                    // A stale acknowledgement must not paint over a newer draft, but it
                    // still records the server's accepted value. Change-mode emptiness
                    // transitions must compare against that value when the draft is cleared.
                    if (!ack.ignored) acceptedValue = ack.value ?? '';
                    if (ack.sequence !== sequence) continue;
                    reconcile(ack);
                } catch {
                    // The server may already have executed the callback. Preserve the draft,
                    // discard queued work and never automatically replay business callbacks.
                    if (currentGeneration === generation && !disposed && edit.revision === revision) {
                        queue.length = 0;
                        element.setAttribute('data-neo-input-uncertain', '');
                        emit();
                    }
                }
            }
        } finally { if (currentGeneration === generation) running = false; }
    }
    const prepareFinal = () => {
        if (!config.readonly) formatting.prepareCommit();
        observeDraft();
        if (config.commitOnBlur && !config.readonly && lastFinalSequence !== sequence) { sequence++; lastFinalSequence = sequence; pending(true); }
    };
    const submit = (flushing = false) => {
        cancelTimer();
        if (disposed || composing || element.disabled || (element.readOnly && !flushing && !lockDepth) || !bridge) return;
        observeDraft();
        if (flushing) prepareFinal();
        if (draft === lastSubmitted && submittedSequence === sequence && !element.hasAttribute('data-neo-input-uncertain')) return;
        if (!(flushing && config.commitOnBlur) && !running && !queue.length && draft === acceptedValue && !element.hasAttribute('data-neo-input-uncertain')) {
            lastSubmitted = draft; submittedSequence = sequence; pending(false); return;
        }
        lastSubmitted = draft; submittedSequence = sequence;
        queue.push({ value: draft, sequence, revision, final: flushing });
        pending(true);
        void pump();
    };
    const schedule = () => {
        cancelTimer();
        if (config.updateOn !== 'input' || composing) return;
        if ((config.debounce ?? 0) > 0) timer = setTimeout(submit, config.debounce);
        else submit();
    };
    const notifyChangeBoundary = () => {
        const empty = formatting.isEmpty(draft), crossed = empty !== observedEmpty;
        if (!composing) observedEmpty = empty;
        if (config.updateOn === 'change' && crossed && !composing) submit();
        else schedule();
    };
    const input = () => { pristine = false; observeDraft(); notifyChangeBoundary(); };
    const compositionStart = () => { pristine = false; composing = true; cancelTimer(); };
    const compositionEnd = () => { composing = false; observeDraft(); if (heldAck) { reconcile(heldAck); heldAck = undefined; } if (document.activeElement !== element) submit(true); else notifyChangeBoundary(); };
    const change = () => { observeDraft(); submit(true); };
    const acceptParameterValue = () => {
        write(config.value ?? ''); formatting.reconcile();
        lastSubmitted = acceptedValue = draft; submittedSequence = sequence;
        observedEmpty = formatting.isEmpty(draft);
    };
    const observer = new MutationObserver(() => {
        const previousValue = config.value ?? '';
        config = JSON.parse(element.getAttribute('data-neo-input-config') || '{}');
        if (lockDepth) element.readOnly = true;
        if (config.revision === revision) {
            // Ordinary binding may initialize a field asynchronously. It can only
            // replace a pristine field, never a pending or previously edited draft.
            if (pristine && (config.value ?? '') !== previousValue) {
                acceptParameterValue(); pending(false);
            }
            return;
        }
        revision = config.revision;
        cancelTimer(); queue.length = 0; heldAck = undefined; generation++; running = false;
        if (element.hasAttribute('data-neo-input-invalid')) element.setCustomValidity('');
        element.removeAttribute('data-neo-input-invalid');
        pristine = true; acceptParameterValue(); lastFinalSequence = -1;
        element.removeAttribute('data-neo-input-uncertain');
        pending(false);
    });
    observer.observe(element, { attributes: true, attributeFilter: ['data-neo-input-config'] });
    const preventLockedEdit = event => { if (lockDepth) event.preventDefault(); };
    element.addEventListener('beforeinput', preventLockedEdit, true);
    element.addEventListener('input', input);
    element.addEventListener('change', change);
    element.addEventListener('blur', change);
    element.addEventListener('compositionstart', compositionStart);
    element.addEventListener('compositionend', compositionEnd);
    // Config may already contain a later Value when initialization was delayed.
    if (pristine) acceptParameterValue();
    element.setAttribute('data-neo-input-ready', 'true');
    if (!pristine && draft !== acceptedValue) { sequence++; pending(true); notifyChangeBoundary(); }
    controllers.set(element, {
        get composing() { return composing; },
        enableNativeValidation() {
            const previous = element.readOnly;
            element.readOnly = config.readonly ?? false;
            return () => { element.readOnly = previous; };
        },
        lock() {
            const original = element.readOnly;
            lockDepth++; element.readOnly = true;
            return () => { if (--lockDepth === 0) element.readOnly = config.readonly ?? original; };
        },
        flush(timeout) {
            if (disposed) return Promise.resolve({ status: 'Disposed', fieldId: element.id });
            if (config.server && !canInvokeServer()) return Promise.resolve({ status: 'Uncertain', fieldId: element.id });
            if (composing) return Promise.resolve({ status: 'Composing', fieldId: element.id });
            if (element.hasAttribute('data-neo-input-uncertain')) return Promise.resolve({ status: 'Uncertain', fieldId: element.id });
            prepareFinal();
            return new Promise(resolve => {
                const waiter = { resolve, sequence, revision, value: draft };
                waiters.add(waiter);
                waiter.timer = setTimeout(() => {
                    finish(waiter, 'TimedOut'); element.setAttribute('data-neo-input-uncertain', ''); emit();
                }, timeout);
                submit(true);
                void pump();
                settleWaiters();
            });
        },
        dispose() {
        disposed = true; formatting.dispose(); generation++; queue.length = 0; cancelTimer(); observer.disconnect();
        element.removeEventListener('beforeinput', preventLockedEdit, true);
        element.removeEventListener('input', input);
        element.removeEventListener('change', change);
        element.removeEventListener('blur', change);
        element.removeEventListener('compositionstart', compositionStart);
        element.removeEventListener('compositionend', compositionEnd);
        element.removeAttribute('data-neo-input-ready'); settleWaiters();
    } });
}

export function dispose(element) {
    controllers.get(element)?.dispose(); controllers.delete(element);
}


// Capture all fields and acquire their locks in one browser task, including fields
// rendered through a portal. Keep the lease until application validation/save completes.
function makeLease(result, releases) {
    let released = false;
    return {
        wait: () => result,
        watch(bridge, requestId, method, server = false) {
            // Do not return this long-running Promise through JS interop: its
            // eventual reply could be sent over a disconnected circuit.
            void result.then(value => {
                if (server && !canInvokeServer()) return;
                return bridge.invokeMethodAsync(method, requestId, value);
            }).catch(() => {});
        },
        release() { if (released) return; released = true; for (const release of releases.reverse()) release(); }
    };
}
function validateNative(elements, forms) {
    const restores = elements.map(element => controllerFor(element)?.enableNativeValidation()).filter(Boolean);
    try {
        for (const element of elements) { const result = controllerFor(element)?.validate?.(); if (result && result.status !== 'Success') { element.querySelector('button,input')?.focus(); return { ...result, nativeValidation: true }; } }
        for (const field of [...forms.flatMap(form => [...form.elements]), ...elements]) {
            if (!field.checkValidity || field.checkValidity()) continue;
            field.reportValidity();
            return { status: 'Invalid', fieldId: field.id || null, nativeValidation: true };
        }
        return { status: 'Success' };
    } finally { for (const restore of restores) restore(); }
}
function begin(elements, timeout, validate = null) {
    const targets = elements.filter(element => !element.disabled && !controllerFor(element)?.disabled);
    for (const element of targets) {
        const controller = controllerFor(element);
        const status = !controller ? 'NotReady' : controller.composing ? 'Composing' : null;
        if (status) return makeLease(Promise.resolve({ status, fieldId: element.id }), []);
    }
    const releases = targets.map(element => controllerFor(element).lock());
    const result = Promise.all(targets.map(element => controllerFor(element).flush(timeout)))
        .then(results => {
            const failure = results.find(result => result.status !== 'Success');
            if (failure?.status === 'Invalid') {
                const field = elements.find(element => element.id === failure.fieldId);
                const restore = controllerFor(field)?.enableNativeValidation?.();
                try { field?.reportValidity?.(); } finally { restore?.(); }
                (field?.matches('input,textarea,select,button') ? field : field?.querySelector('input,button,[tabindex]'))?.focus();
            }
            return failure ?? (validate ? validate() : { status: 'Success' });
        });
    return makeLease(result, releases);
}

// EditForm has no asynchronous pre-validation hook. The thin BrowserEditForm wrapper
// defers native validation and uses this capture guard only to identify the submitter
// and suppress duplicate native events; .NET still owns validation and business handlers.
export function initializeEditForm(root, server = false) {
    if (editFormGuards.has(root)) return editFormGuards.get(root);
    let busy = false, skipNative = false, started = false;
    const forms = () => [...root.querySelectorAll('form[data-neo-edit-form]')]
        .filter(form => form.closest('[data-neo-input-scope-root]') === root);
    const release = () => { busy = false; skipNative = false; started = false; for (const form of forms()) form.setAttribute('aria-busy', 'false'); };
    const unavailable = form => {
        release(); form.dataset.neoFormFlushStatus = 'Uncertain';
        form.dispatchEvent(new CustomEvent('neoui:form-flush-failed', { bubbles: true, detail: { status: 'Uncertain' } }));
    };
    const submit = event => {
        if (!forms().includes(event.target)) return;
        if (busy) { event.preventDefault(); event.stopImmediatePropagation(); return; }
        if (server && !canInvokeServer()) {
            event.preventDefault(); event.stopImmediatePropagation(); unavailable(event.target); return;
        }
        busy = true; started = false;
        delete event.target.dataset.neoFormFlushStatus;
        skipNative = event.submitter?.formNoValidate === true;
        event.target.setAttribute('aria-busy', 'true');
    };
    // Blazor may dispatch submit from document capture synchronously in WebAssembly.
    // Capture at window first so the submitter and duplicate guard are known before .NET runs.
    const eventTarget = root.ownerDocument.defaultView;
    eventTarget.addEventListener('submit', submit, true);
    const availability = () => {
        if (server && busy && !started && !canInvokeServer()) for (const form of forms()) unavailable(form);
    };
    eventTarget.addEventListener('offline', availability);
    root.ownerDocument.addEventListener('neoui:input-connection-state', availability);
    // Rendered wrapper state is the reconnect fallback; scope completion can precede
    // the wrapper's failure handler/finally, so it must not release this guard.
    const observer = new MutationObserver(() => { if (forms().every(form => form.dataset.neoEditFormBusy === 'false')) release(); });
    observer.observe(root, { subtree: true, attributes: true, attributeFilter: ['data-neo-edit-form-busy'] });
    const removal = new MutationObserver(() => { if (!root.isConnected) guard.dispose(); });
    removal.observe(root.ownerDocument.body, { childList: true, subtree: true });
    const guard = {
        acquire() { busy = true; started = true; },
        get skipNative() { return skipNative; },
        release,
        dispose() { release(); observer.disconnect(); removal.disconnect(); eventTarget.removeEventListener('submit', submit, true); eventTarget.removeEventListener('offline', availability); root.ownerDocument.removeEventListener('neoui:input-connection-state', availability); editFormGuards.delete(root); delete root.dataset.neoEditFormReady; }
    };
    editFormGuards.set(root, guard);
    root.dataset.neoEditFormReady = 'true';
    return guard;
}

export function beginScope(root, scopeId, timeout = 30000, checkNative = false) {
    if (!root?.isConnected) return makeLease(Promise.resolve({ status: 'NotReady' }), []);
    const elements = [...root.ownerDocument.querySelectorAll('[data-neo-input-scope]')]
        .filter(element => element.dataset.neoInputScope === scopeId && (element.hasAttribute('data-neo-input-config') || element.hasAttribute('data-neo-browser-field')));
    activeScopes.get(scopeId)?.release();
    const forms = [...root.querySelectorAll('form')].filter(form => form.closest('[data-neo-input-scope-root]') === root);
    const guard = editFormGuards.get(root);
    guard?.acquire();
    const lease = begin(elements, timeout, checkNative && !guard?.skipNative ? () => validateNative(elements, forms) : null), release = lease.release;
    const observer = new MutationObserver(() => {
        if (!root.isConnected || root.dataset.neoInputBusy === 'false') lease.release();
    });
    observer.observe(root, { attributes: true, attributeFilter: ['data-neo-input-busy'] });
    const removal = new MutationObserver(() => { if (!root.isConnected) lease.release(); });
    removal.observe(root.ownerDocument.body, { childList: true, subtree: true });
    lease.release = () => {
        observer.disconnect(); removal.disconnect(); release();
        if (activeScopes.get(scopeId) === lease) activeScopes.delete(scopeId);
    };
    activeScopes.set(scopeId, lease);
    void lease.wait().then(result => { if (result.status !== 'Success') lease.release(); });
    return lease;
}

export function beginInput(id, timeout = 30000) {
    const element = document.getElementById(id);
    const lease = element ? begin([element], timeout) : makeLease(Promise.resolve({ status: 'NotReady', fieldId: id }), []);
    void lease.wait().then(() => lease.release());
    return lease;
}

export async function flushById(id, timeout = 30000) {
    const lease = beginInput(id, timeout);
    try { return await lease.wait(); } finally { lease.release(); }
}
