// Razor renders a fixed initial value in Browser mode. Only this controller writes
// subsequent DOM values; post-render repair cannot prevent stale render-batch writes.
const controllers = new WeakMap();

export function initialize(element, bridge) {
    if (!element || controllers.has(element)) return;
    let config = JSON.parse(element.getAttribute('data-neo-input-config') || '{}');
    let revision = config.revision ?? 0, sequence = 0, draft = element.value;
    let lastSubmitted = config.value ?? '', submittedSequence = 0, acceptedValue = config.value ?? '', composing = false, timer, running = false, disposed = false;
    const queue = [];
    let heldAck, generation = 0;
    const emit = () => element.dispatchEvent(new CustomEvent('neoui:input-state', {
        bubbles: true, detail: { value: draft, sequence, revision,
            pending: element.hasAttribute('data-neo-input-pending'),
            uncertain: element.hasAttribute('data-neo-input-uncertain') }
    }));
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
        draft = element.value; sequence++;
        element.removeAttribute('data-neo-input-uncertain');
        pending(true);
        return true;
    };
    const reconcile = ack => {
        if (disposed || ack.revision !== revision || ack.sequence !== sequence) return;
        if (composing) { heldAck = ack; return; }
        if (ack.ignored) { element.setAttribute('data-neo-input-uncertain', ''); emit(); return; }
        write(ack.value ?? ''); lastSubmitted = acceptedValue = draft; submittedSequence = sequence;
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
                    const ack = await bridge.invokeMethodAsync('OnBrowserInputChanged', edit.value, edit.sequence, edit.revision);
                    if (currentGeneration !== generation || disposed || edit.revision !== revision || ack.revision !== revision || ack.sequence !== sequence) continue;
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
    const submit = () => {
        cancelTimer();
        if (disposed || composing || element.disabled || element.readOnly || !bridge) return;
        observeDraft();
        if (draft === lastSubmitted && submittedSequence === sequence && !element.hasAttribute('data-neo-input-uncertain')) return;
        if (!running && !queue.length && draft === acceptedValue && !element.hasAttribute('data-neo-input-uncertain')) {
            lastSubmitted = draft; submittedSequence = sequence; pending(false); return;
        }
        lastSubmitted = draft; submittedSequence = sequence;
        queue.push({ value: draft, sequence, revision });
        pending(true);
        void pump();
    };
    const schedule = () => {
        cancelTimer();
        if (config.updateOn !== 'input' || composing) return;
        if ((config.debounce ?? 0) > 0) timer = setTimeout(submit, config.debounce);
        else submit();
    };
    const input = () => { observeDraft(); schedule(); };
    const compositionStart = () => { composing = true; cancelTimer(); };
    const compositionEnd = () => { composing = false; observeDraft(); if (heldAck) { reconcile(heldAck); heldAck = undefined; } if (document.activeElement !== element) submit(); else schedule(); };
    const change = () => { observeDraft(); submit(); };
    const observer = new MutationObserver(() => {
        config = JSON.parse(element.getAttribute('data-neo-input-config') || '{}');
        if (config.revision === revision) return;
        revision = config.revision;
        cancelTimer(); queue.length = 0; heldAck = undefined; generation++; running = false;
        write(config.value ?? ''); lastSubmitted = acceptedValue = draft; submittedSequence = sequence;
        element.removeAttribute('data-neo-input-uncertain');
        pending(false);
    });
    observer.observe(element, { attributes: true, attributeFilter: ['data-neo-input-config'] });
    element.addEventListener('input', input);
    element.addEventListener('change', change);
    element.addEventListener('blur', change);
    element.addEventListener('compositionstart', compositionStart);
    element.addEventListener('compositionend', compositionEnd);
    element.setAttribute('data-neo-input-ready', 'true');
    if (draft !== acceptedValue) { sequence++; pending(true); schedule(); }
    controllers.set(element, { dispose() {
        disposed = true; generation++; queue.length = 0; cancelTimer(); observer.disconnect();
        element.removeEventListener('input', input);
        element.removeEventListener('change', change);
        element.removeEventListener('blur', change);
        element.removeEventListener('compositionstart', compositionStart);
        element.removeEventListener('compositionend', compositionEnd);
        element.removeAttribute('data-neo-input-ready');
    } });
}

export function dispose(element) {
    controllers.get(element)?.dispose(); controllers.delete(element);
}
