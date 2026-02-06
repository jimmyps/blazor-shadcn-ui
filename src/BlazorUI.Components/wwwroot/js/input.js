// Input component JavaScript module
// Manages validation tooltips, error states, and UpdateOn mode behavior

// Track elements with active validation and their update modes
const validationState = new Map();

/**
 * Initialize validation tracking for an element
 * @param {string} elementId - The element ID
 * @param {string} updateOn - 'input' or 'change'
 */
export function initializeValidation(elementId, updateOn = 'input') {
    const element = document.getElementById(elementId);
    if (!element) return;

    // Clean up any existing listener
    disposeValidation(elementId);

    const state = {
        updateOn: updateOn.toLowerCase(),
        hasError: false,
        inputHandler: null
    };

    // If updateOn=change, attach input listener to auto-clear tooltip while typing
    if (state.updateOn === 'change') {
        state.inputHandler = () => {
            if (state.hasError) {
                // Clear tooltip on first keystroke, but keep custom validity for styling
                element.setCustomValidity('');
                state.hasError = false;
            }
        };
        element.addEventListener('input', state.inputHandler);
    }

    validationState.set(elementId, state);
}

/**
 * Dispose validation tracking for an element
 * @param {string} elementId - The element ID
 */
export function disposeValidation(elementId) {
    const state = validationState.get(elementId);
    if (state && state.inputHandler) {
        const element = document.getElementById(elementId);
        if (element) {
            element.removeEventListener('input', state.inputHandler);
        }
    }
    validationState.delete(elementId);
}

export function setValidationError(elementId, message) {
    const element = document.getElementById(elementId);
    if (!element) return;

    // Set custom validity for native browser tooltip
    element.setCustomValidity(message);
    
    // Report validity to show the tooltip
    element.reportValidity();
    
    // Focus the element to ensure visibility
    element.focus();

    // Mark that this element has an error
    const state = validationState.get(elementId);
    if (state) {
        state.hasError = true;
    }
}

export function setValidationErrorSilent(elementId, message) {
    const element = document.getElementById(elementId);
    if (!element) return;

    // Set custom validity without showing tooltip or focusing
    element.setCustomValidity(message);

    // Mark that this element has an error
    const state = validationState.get(elementId);
    if (state) {
        state.hasError = true;
    }
}

export function clearValidationError(elementId) {
    const element = document.getElementById(elementId);
    if (!element) return;

    // Clear custom validity
    element.setCustomValidity('');

    // Clear error state
    const state = validationState.get(elementId);
    if (state) {
        state.hasError = false;
    }
}

// Legacy function name for backwards compatibility
export function showValidationError(elementId, message) {
    setValidationError(elementId, message);
}
