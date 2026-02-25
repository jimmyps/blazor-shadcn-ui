// Enhanced Masked Input - Industry standard approach
// Based on patterns from react-input-mask, IMask.js, and Cleave.js
// Works WITH the browser's native input handling, not against it

const instances = new Map();

/**
 * Initialize a masked input field
 * @param {string} elementId - The ID of the input element
 * @param {string} mask - The mask pattern
 * @param {string} maskChar - The placeholder character for mask positions
 * @param {object} dotNetHelper - .NET object reference for callbacks
 * @param {string} updateOn - When to update: 'input' or 'change'
 */
export function initializeMaskedInput(elementId, mask, maskChar, dotNetHelper, updateOn = 'change') {
    const element = document.getElementById(elementId);
    if (!element) return;

    // Clean up existing instance if any
    disposeMaskedInput(elementId);

    const maskDefs = {
        '0': /\d/,              // Digit (0-9)
        '9': /[\d\s]/,          // Digit or space
        'A': /[A-Za-z]/,        // Letter
        'a': /[A-Za-z\s]/,      // Letter or space
        '*': /[A-Za-z0-9]/      // Alphanumeric
    };

    // Parse mask into positions array
    const positions = [];
    for (let i = 0; i < mask.length; i++) {
        const c = mask[i];
        const pattern = maskDefs[c];
        positions.push({
            index: i,
            isEditable: !!pattern,
            pattern: pattern,
            literal: pattern ? null : c
        });
    }

    // State
    let lastValue = element.value;
    let lastCursorPos = 0;
    let lastRawValue = ''; // Track last notified raw value
    const updateOnInput = updateOn.toLowerCase() === 'input';
    const updateOnChange = updateOn.toLowerCase() === 'change';

    // ============ UTILITY FUNCTIONS ============

    // Extract raw value (only editable characters, no literals, no mask chars)
    function getRawValue(masked) {
        let raw = '';
        for (let i = 0; i < positions.length && i < masked.length; i++) {
            const pos = positions[i];
            const char = masked[i];
            if (pos.isEditable && char !== maskChar && pos.pattern.test(char)) {
                raw += char;
            }
        }
        return raw;
    }

    // Apply mask to raw value
    function applyMask(raw) {
        let result = '';
        let rawIdx = 0;

        for (let i = 0; i < positions.length; i++) {
            const pos = positions[i];
            
            if (!pos.isEditable) {
                // Literal character
                result += pos.literal;
            } else if (rawIdx < raw.length) {
                // We have a character to place
                const char = raw[rawIdx];
                if (pos.pattern.test(char)) {
                    result += char;
                    rawIdx++;
                } else {
                    // Invalid char, skip it and try next
                    rawIdx++;
                    i--; // Retry this position
                }
            } else {
                // No more raw characters, fill with mask char
                result += maskChar;
            }
        }

        return result;
    }

    // Get editable position count up to index
    function getEditableCountBefore(idx, value) {
        let count = 0;
        for (let i = 0; i < idx && i < positions.length; i++) {
            if (positions[i].isEditable && value[i] !== maskChar) {
                count++;
            }
        }
        return count;
    }

    // Get cursor position for given raw position
    function getCursorPosForRawPos(rawPos) {
        let count = 0;
        for (let i = 0; i < positions.length; i++) {
            if (positions[i].isEditable) {
                if (count === rawPos) return i;
                count++;
            }
        }
        return positions.length;
    }

    // Find first empty editable position
    function getFirstEmptyPos(value) {
        for (let i = 0; i < positions.length; i++) {
            if (positions[i].isEditable && (!value[i] || value[i] === maskChar)) {
                return i;
            }
        }
        return positions.length;
    }

    // Find nearest editable position (forward)
    function getNearestEditablePos(pos, forward = true) {
        if (forward) {
            for (let i = pos; i < positions.length; i++) {
                if (positions[i].isEditable) return i;
            }
            return positions.length;
        } else {
            for (let i = Math.min(pos, positions.length - 1); i >= 0; i--) {
                if (positions[i].isEditable) return i;
            }
            return 0;
        }
    }

    // ============ EVENT HANDLERS ============

    function handleKeyDown(e) {
        lastCursorPos = element.selectionStart;
        
        // Handle special keys
        if (e.key === 'Backspace') {
            e.preventDefault();
            handleBackspace();
        } else if (e.key === 'Delete') {
            e.preventDefault();
            handleDelete();
        } else if (e.key === 'ArrowLeft' || e.key === 'ArrowRight') {
            // Let browser handle, but we'll fix position on keyup
        }
    }

    function handleBackspace() {
        const start = element.selectionStart;
        const end = element.selectionEnd;
        const value = element.value;

        if (start !== end) {
            // Selection - clear selected range
            const raw = getRawValue(value);
            const rawStart = getEditableCountBefore(start, value);
            const rawEnd = getEditableCountBefore(end, value);
            const newRaw = raw.slice(0, rawStart) + raw.slice(rawEnd);
            
            updateValue(newRaw, rawStart);
        } else {
            // No selection - delete char before cursor
            let targetPos = start - 1;
            
            // Skip backwards over literals to find editable position
            while (targetPos >= 0 && !positions[targetPos]?.isEditable) {
                targetPos--;
            }

            if (targetPos >= 0) {
                const raw = getRawValue(value);
                const rawPos = getEditableCountBefore(targetPos + 1, value);
                const newRaw = raw.slice(0, rawPos - 1) + raw.slice(rawPos);
                
                updateValue(newRaw, rawPos - 1);
            }
        }
    }

    function handleDelete() {
        const start = element.selectionStart;
        const end = element.selectionEnd;
        const value = element.value;

        if (start !== end) {
            // Selection - clear selected range
            const raw = getRawValue(value);
            const rawStart = getEditableCountBefore(start, value);
            const rawEnd = getEditableCountBefore(end, value);
            const newRaw = raw.slice(0, rawStart) + raw.slice(rawEnd);
            
            updateValue(newRaw, rawStart);
        } else {
            // No selection - delete char at cursor
            let targetPos = start;
            
            // Skip forward over literals to find editable position
            while (targetPos < positions.length && !positions[targetPos]?.isEditable) {
                targetPos++;
            }

            if (targetPos < positions.length) {
                const raw = getRawValue(value);
                const rawPos = getEditableCountBefore(targetPos + 1, value);
                const newRaw = raw.slice(0, rawPos - 1) + raw.slice(rawPos);
                
                updateValue(newRaw, rawPos - 1);
            }
        }
    }

    function handleInput(e) {
        const newValue = element.value;
        const cursorPos = element.selectionStart;

        // Extract all valid characters from new input
        let allChars = '';
        for (let i = 0; i < newValue.length; i++) {
            const char = newValue[i];
            // Check if this character is valid for ANY editable position
            for (const pos of positions) {
                if (pos.isEditable && pos.pattern.test(char)) {
                    allChars += char;
                    break;
                }
            }
        }

        // Calculate raw cursor position
        // How many editable chars were before cursor in old value
        const oldRawPos = getEditableCountBefore(lastCursorPos, lastValue);
        
        // Estimate new raw position based on what was typed
        const charsTyped = newValue.length - lastValue.length;
        const newRawPos = Math.max(0, oldRawPos + Math.max(0, charsTyped));

        updateValue(allChars, newRawPos);
    }

    function handlePaste(e) {
        e.preventDefault();
        
        const pastedText = (e.clipboardData || window.clipboardData)?.getData('text') || '';
        const cursorPos = element.selectionStart;
        
        // Get current raw value
        const currentRaw = getRawValue(element.value);
        const rawCursorPos = getEditableCountBefore(cursorPos, element.value);
        
        // Filter pasted text to only valid characters
        let validChars = '';
        for (const char of pastedText) {
            for (const pos of positions) {
                if (pos.isEditable && pos.pattern.test(char)) {
                    validChars += char;
                    break;
                }
            }
        }

        // Insert at cursor position
        const newRaw = currentRaw.slice(0, rawCursorPos) + validChars + currentRaw.slice(rawCursorPos);
        
        updateValue(newRaw, rawCursorPos + validChars.length);
    }

    function handleFocus(e) {
        // Delay to let browser set initial cursor
        setTimeout(() => {
            const value = element.value;
            if (!value) return;
            
            const firstEmpty = getFirstEmptyPos(value);
            setCursor(firstEmpty);
        }, 0);
    }

    function handleClick(e) {
        // Ensure cursor is at an editable position
        setTimeout(() => {
            const pos = element.selectionStart;
            if (pos < positions.length && !positions[pos]?.isEditable) {
                const nearestPos = getNearestEditablePos(pos, true);
                setCursor(nearestPos);
            }
        }, 0);
    }

    // ============ CORE UPDATE FUNCTION ============

    function updateValue(rawValue, rawCursorPos, notifyBlazor = true) {
        // Limit raw value to max editable positions
        const maxRaw = positions.filter(p => p.isEditable).length;
        rawValue = rawValue.slice(0, maxRaw);
        
        // Apply mask
        const masked = applyMask(rawValue);
        
        // Update element
        element.value = masked;
        lastValue = masked;

        // Calculate and set cursor position
        const cursorPos = getCursorPosForRawPos(Math.min(rawCursorPos, rawValue.length));
        setCursor(cursorPos);

        // Notify Blazor based on UpdateOn setting
        if (notifyBlazor && rawValue !== lastRawValue) {
            if (updateOnInput) {
                // Update immediately on every change
                lastRawValue = rawValue;
                dotNetHelper.invokeMethodAsync('OnValueChanged', rawValue);
            }
            // For updateOnChange, don't update lastRawValue here
            // It will be updated in handleBlur when we actually notify Blazor
        }
    }

    function setCursor(pos) {
        // Ensure position is at an editable slot or end
        let finalPos = pos;
        if (finalPos < positions.length && !positions[finalPos]?.isEditable) {
            finalPos = getNearestEditablePos(finalPos, true);
        }
        finalPos = Math.min(finalPos, element.value.length);
        
        element.setSelectionRange(finalPos, finalPos);
        lastCursorPos = finalPos;
    }

    function handleBlur(e) {
        // If UpdateOn is Change, notify Blazor on blur
        if (updateOnChange) {
            const currentRaw = getRawValue(element.value);
            if (currentRaw !== lastRawValue) {
                lastRawValue = currentRaw;
                dotNetHelper.invokeMethodAsync('OnValueChanged', currentRaw);
            }
        }
    }

    // ============ ATTACH LISTENERS ============

    element.addEventListener('keydown', handleKeyDown);
    element.addEventListener('input', handleInput);
    element.addEventListener('paste', handlePaste);
    element.addEventListener('focus', handleFocus);
    element.addEventListener('click', handleClick);
    element.addEventListener('blur', handleBlur);

    // Store for cleanup
    instances.set(elementId, {
        element,
        handlers: { handleKeyDown, handleInput, handlePaste, handleFocus, handleClick, handleBlur }
    });

    // Initialize with current value
    if (element.value) {
        const raw = getRawValue(element.value);
        lastRawValue = raw; // Initialize lastRawValue
        element.value = applyMask(raw);
        lastValue = element.value;
    }
}

export function disposeMaskedInput(elementId) {
    const instance = instances.get(elementId);
    if (instance) {
        const { element, handlers } = instance;
        element.removeEventListener('keydown', handlers.handleKeyDown);
        element.removeEventListener('input', handlers.handleInput);
        element.removeEventListener('paste', handlers.handlePaste);
        element.removeEventListener('focus', handlers.handleFocus);
        element.removeEventListener('click', handlers.handleClick);
        element.removeEventListener('blur', handlers.handleBlur);
        instances.delete(elementId);
    }
}

// Legacy compatibility
export function getCursorPosition(elementId) {
    const el = document.getElementById(elementId);
    return el?.selectionStart || 0;
}

export function setCursorPosition(elementId, position) {
    const el = document.getElementById(elementId);
    if (el) {
        const pos = Math.min(Math.max(0, position), el.value.length);
        el.setSelectionRange(pos, pos);
    }
}
