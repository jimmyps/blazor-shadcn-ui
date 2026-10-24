import { initializeMaskedInput } from './masked-input.js';
import { initializeListContinuation, disposeListContinuation, resetHistory } from './markdown-editor.js';
// Formatting runs before the shared draft controller observes native input events.
// It never sends callbacks or writes a Razor-owned value.
export function attachFormat(element, getFormat) {
    let composing = false, markdownRevision = getFormat()?.revision;
    const markdown = getFormat()?.kind === 'markdown';
    if (markdown) initializeListContinuation(element,null,true);
    const numberText = (value, format) => {
        let result = value.trim();
        for (const symbol of [format.symbol, format.group]) if (symbol) result = result.split(symbol).join('');
        if (format.decimal && format.decimal !== '.') result = result.split(format.decimal).join('.');
        return result.trim();
    };
    let maskEditor, maskKey, currencyFormat = getFormat(), currencyDisplay = element.value, currencyEdited = false;
    const configureMask = () => {
        const format = getFormat();
        if (format?.kind !== 'mask' || !format.mask) { maskEditor?.dispose(); maskEditor = undefined; maskKey = undefined; return; }
        const next = JSON.stringify([format.mask, format.maskChar, format.showMask]);
        if (next === maskKey) return;
        maskEditor?.dispose(); maskKey = next;
        maskEditor = initializeMaskedInput(element.id, format.mask, format.maskChar ?? '_', null, 'change', {
            browserOwned: true, showMask: format.showMask, onFormat: () => validate()
        });
    };
    const validate = () => {
        const format = getFormat(); if (!format) return;
        if (markdown && markdownRevision !== format.revision) { markdownRevision = format.revision; resetHistory(element); }
        let message = '';
        if (format.kind === 'number' || format.kind === 'currency') {
            const value = numberText(element.value,format);
            const valid = format.integer ? /^[+-]?\d+$/.test(value) : /^[+-]?(?:\d+(?:\.\d*)?|\.\d+)(?:[eE][+-]?\d+)?$/.test(value);
            if (value && (!valid || !Number.isFinite(Number(value)))) message = 'Enter a valid number.';
            if (format.kind === 'number') {
                element.setAttribute('role','spinbutton');
                for (const bound of ['min','max']) { if (format[bound] !== null && format[bound] !== undefined && format[bound] !== '') element.setAttribute('aria-value'+bound,format[bound]); else element.removeAttribute('aria-value'+bound); }
                if (value && valid) element.setAttribute('aria-valuenow',value); else element.removeAttribute('aria-valuenow');
            }
        } else if (format.kind === 'mask' && format.required && element.value) {
            const count = maskEditor?.rawValue(element.value).length ?? 0;
            const required = [...(format.mask ?? '')].filter(char => ['0','A','*'].includes(char)).length;
            if (count < required) message = 'Complete this value.';
        }
        // Own only the formatting error. Existing EditContext validation may
        // provide a separate custom validity message.
        if (message || element.dataset.neoFormatError) element.setCustomValidity(message);
        if (message) element.dataset.neoFormatError = message; else delete element.dataset.neoFormatError;
    };
    const normalize = event => {
        const format = getFormat(); if (!format || composing || event?.isComposing) return;
        if (format.kind === 'currency') currencyEdited = true;
        validate();
    };
    const numberKey = event => {
        const format = getFormat();
        if (format?.kind !== 'number' || !['ArrowUp','ArrowDown'].includes(event.key) || element.disabled || element.readOnly || composing) return;
        event.preventDefault();
        const step = Number(format.step) > 0 ? Number(format.step) : 1, current = Number(element.value) || 0;
        let next = current + (event.key === 'ArrowUp' ? step : -step);
        if (format.min !== null && format.min !== undefined && format.min !== '') next = Math.max(Number(format.min), next);
        if (format.max !== null && format.max !== undefined && format.max !== '') next = Math.min(Number(format.max), next);
        element.value = String(Number(next.toPrecision(14))); element.dispatchEvent(new Event('input',{bubbles:true}));
    };
    const exactCurrency = () => getFormat()?.kind === 'currency' && !currencyEdited && currencyFormat?.editValue != null &&
        element.value === currencyDisplay && !element.hasAttribute('data-neo-input-pending') && !element.hasAttribute('data-neo-input-uncertain');
    const prepareCommit = () => { if (exactCurrency()) element.value = currencyFormat.editValue; };
    const focus = () => {
        const format = getFormat();
        if (format?.kind === 'currency' && !element.readOnly && !element.disabled) {
            // Exact editable text comes from the same C# currency rules as the
            // legacy editor. Never derive it from a rounded display string.
            const accepted = exactCurrency();
            const text = accepted && currencyFormat?.editValue != null ? currencyFormat.editValue : numberText(element.value,format);
            element.value = accepted && currencyFormat?.editValue != null || format.decimal === '.' ? text : text.replace('.',format.decimal);
        }
    };
    const startComposition = () => composing = true;
    const endComposition = event => {composing = false; normalize(event);};
    configureMask();
    element.addEventListener('keydown',numberKey);
    element.addEventListener('input',normalize,true);
    element.addEventListener('focus',focus);
    element.addEventListener('compositionstart',startComposition,true);
    element.addEventListener('compositionend',endComposition,true);
    validate();
    return { validate, prepareCommit, reconcile(format) {
        currencyFormat = format ?? getFormat(); currencyDisplay = element.value; currencyEdited = false;
        configureMask(); maskEditor?.reconcile(); validate();
    }, isEmpty(value) { return maskEditor ? !maskEditor.rawValue(value).length : value.length === 0; }, dispose() {
        maskEditor?.dispose();
        if (markdown) disposeListContinuation(element);
        element.removeEventListener('keydown',numberKey);
        element.removeEventListener('input',normalize,true);
        element.removeEventListener('focus',focus); element.removeEventListener('compositionstart',startComposition,true); element.removeEventListener('compositionend',endComposition,true);
    }};
}
