export function numericText(value, format) {
    let result = value.trim();
    for (const token of [format.symbol, format.cultureSymbol, format.group, format.currencyGroup]) {
        if (token) result = result.split(token).join('');
    }
    for (const [token, replacement] of [[format.decimal, '.'], [format.negativeSign, '-'], [format.positiveSign, '+']]) {
        if (token && token !== replacement) result = result.split(token).join(replacement);
    }
    return result.trim();
}

function isNumericDraft(value, format) {
    const text = numericText(value, format);
    if (format.kind === 'currency') {
        const amount = format.integer ? /^\d+$/ : /^(?:\d+(?:\.\d*)?|\.\d*)$/;
        if (/^[+-]?$/.test(text)) return true;
        return amount.test(text.startsWith('+') || text.startsWith('-') ? text.slice(1) : text.replace(/[+-]$/, ''));
    }
    if (format.integer) return /^[+-]?\d*$/.test(text);
    // Keep incomplete decimal and exponent drafts editable. Commit validates the value.
    return /^[+-]?(?:\d*(?:\.\d*)?|(?:\d+(?:\.\d*)?|\.\d+)[eE][+-]?\d*)$/.test(text);
}

export function attachNumericEditing(element, getFormat) {
    let composing = false;
    let snapshot;
    const sync = () => {
        snapshot = { value: element.value, start: element.selectionStart, end: element.selectionEnd, direction: element.selectionDirection };
    };
    const beforeInput = event => {
        if (composing || event.isComposing || event.defaultPrevented) return;
        sync();
        const data = event.data ?? event.dataTransfer?.getData('text/plain');
        const format = getFormat();
        if (!format || data == null || !event.inputType?.startsWith('insert') || snapshot.start == null) return;
        const candidate = snapshot.value.slice(0, snapshot.start) + data + snapshot.value.slice(snapshot.end);
        if (!isNumericDraft(candidate, format)) event.preventDefault();
    };
    const accept = () => {
        const format = getFormat();
        if (!format || isNumericDraft(element.value, format)) {
            sync();
            return true;
        }
        // Some input methods, including IME and autofill, cannot be cancelled in beforeinput.
        element.value = snapshot.value;
        if (snapshot.start != null) element.setSelectionRange(snapshot.start, snapshot.end, snapshot.direction);
        return false;
    };
    const input = event => {
        if (composing || event.isComposing) return;
        // Deleting part of an exponent can temporarily leave an incomplete draft.
        if (event.inputType?.startsWith('delete')) {
            sync();
            return;
        }
        if (!accept()) event.stopImmediatePropagation();
    };
    const compositionStart = () => { sync(); composing = true; };
    const compositionEnd = () => { composing = false; accept(); };
    sync();
    element.addEventListener('beforeinput', beforeInput, true);
    element.addEventListener('input', input, true);
    element.addEventListener('compositionstart', compositionStart, true);
    element.addEventListener('compositionend', compositionEnd, true);
    return {
        sync,
        dispose() {
            element.removeEventListener('beforeinput', beforeInput, true);
            element.removeEventListener('input', input, true);
            element.removeEventListener('compositionstart', compositionStart, true);
            element.removeEventListener('compositionend', compositionEnd, true);
        }
    };
}
