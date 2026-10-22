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
    const definitions = {'0': /[0-9]/, '9': /[0-9 ]/, 'A': /[A-Za-z]/, 'a': /[A-Za-z ]/, '*': /[A-Za-z0-9]/};
    const mask = (value, format, caret) => {
        const pattern = format.mask ?? '', raw = [];
        let index = 0, acceptedBefore = 0;
        // Consume editable characters and matching literals in order. Invalid
        // pasted characters are skipped without moving to the next mask slot.
        for (let source = 0; source < value.length && index < pattern.length; source++) {
            const char = value[source];
            while (index < pattern.length && !definitions[pattern[index]]) {
                if (char === pattern[index]) { index++; break; }
                index++;
            }
            if (definitions[pattern[index]]?.test(char) && char !== format.maskChar) {
                raw.push(char); index++; if (source < caret) acceptedBefore++;
            }
        }
        if (!raw.length) return {value:'',caret:0,count:0};
        let result = '', position = 0, nextCaret = 0;
        for (const char of pattern) {
            if (definitions[char]) {
                if (position < raw.length) { result += raw[position++]; if (position <= acceptedBefore) nextCaret = result.length; }
                else if (format.showMask) result += format.maskChar ?? '_';
                else break;
            } else { result += char; if (position === acceptedBefore) nextCaret = result.length; }
        }
        return {value:result,caret:nextCaret,count:raw.length};
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
            const count = mask(element.value,format,element.value.length).count;
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
        if (format.kind === 'mask') {
            const result = mask(element.value,format,element.selectionStart ?? element.value.length);
            if (element.value !== result.value) { element.value = result.value; element.setSelectionRange(result.caret,result.caret); }
        }
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
    const beforeInput = event => {
        const format = getFormat();
        if (format?.kind !== 'mask' || element.readOnly || element.disabled || composing || !['deleteContentBackward','deleteContentForward'].includes(event.inputType)) return;
        const start = element.selectionStart, end = element.selectionEnd;
        if (start !== end || start === null) return;
        let position = event.inputType === 'deleteContentBackward' ? start-1 : start;
        const direction = event.inputType === 'deleteContentBackward' ? -1 : 1;
        while (position >= 0 && position < (format.mask?.length ?? 0) && !definitions[format.mask[position]]) position += direction;
        if (position >= 0 && position < element.value.length) {
            // WebKit computes the native deletion range before beforeinput. A
            // selection change here does not retarget that deletion across a
            // literal, so apply this edit explicitly and emit one input event.
            event.preventDefault();
            element.setRangeText('',position,position+1,'start');
            element.dispatchEvent(new InputEvent('input',{bubbles:true,inputType:event.inputType}));
        }
    };
    const focus = () => {
        const format = getFormat();
        if (format?.kind === 'currency' && !element.readOnly && !element.disabled) {
            const text = numberText(element.value,format);
            element.value = format.decimal === '.' ? text : text.replace('.',format.decimal);
        }
    };
    const startComposition = () => composing = true;
    const endComposition = event => {composing = false; normalize(event);};
    element.addEventListener('keydown',numberKey);
    element.addEventListener('input',normalize,true);
    element.addEventListener('beforeinput',beforeInput,true);
    element.addEventListener('focus',focus);
    element.addEventListener('compositionstart',startComposition,true);
    element.addEventListener('compositionend',endComposition,true);
    validate();
    return { reconcile: validate, isEmpty(value) { const format = getFormat(); return format?.kind === 'mask' ? mask(value,format,value.length).count === 0 : value.length === 0; }, dispose() {
        if (markdown) disposeListContinuation(element);
        element.removeEventListener('keydown',numberKey);
        element.removeEventListener('input',normalize,true); element.removeEventListener('beforeinput',beforeInput,true);
        element.removeEventListener('focus',focus); element.removeEventListener('compositionstart',startComposition,true); element.removeEventListener('compositionend',endComposition,true);
    }};
}
