// Input component JavaScript module
// Manages input event handling, validation tooltips, error states, and UpdateOn mode behavior

// Track elements with active input handling (event listeners, debounce timers, etc.)
const inputState = new Map();

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

/**
 * Alias for setValidationError for backwards compatibility
 * @param {string} elementId - The element ID
 * @param {string} message - The error message
 */
export function showValidationError(elementId, message) {
    setValidationError(elementId, message);
}

/**
 * Initialize input event handling for an element
 * @param {string} elementId - The element ID
 * @param {string} updateOn - 'input' or 'change'
 * @param {number} debounceDelay - Debounce delay in milliseconds (only for 'input' mode)
 * @param {object} dotNetRef - DotNetObjectReference for callbacks
 */
export function initializeInput(elementId, updateOn = 'change', debounceDelay = 0, dotNetRef = null) {
    const element = document.getElementById(elementId);
    if (!element) {
        console.warn(`Input element with id '${elementId}' not found`);
        return;
    }

    // Clean up any existing state
    disposeInput(elementId);

    const state = {
        updateOn: updateOn.toLowerCase(),
        debounceDelay: debounceDelay,
        dotNetRef: dotNetRef,
        debounceTimer: null,
        inputHandler: null,
        changeHandler: null,
        blurHandler: null
    };

    // Attach input event listener ONLY if updateOn='input'
    if (state.updateOn === 'input' && dotNetRef) {
        state.inputHandler = (e) => {
            const value = e.target.value;
            
            // Handle debouncing
            if (state.debounceDelay > 0) {
                // Clear existing timer
                if (state.debounceTimer) {
                    clearTimeout(state.debounceTimer);
                }
                
                // Set new timer
                state.debounceTimer = setTimeout(() => {
                    dotNetRef.invokeMethodAsync('OnInputChanged', value);
                }, state.debounceDelay);
            } else {
                // No debounce - immediate call
                dotNetRef.invokeMethodAsync('OnInputChanged', value);
            }
        };
        element.addEventListener('input', state.inputHandler);
    }

    // Always attach change event listener
    if (dotNetRef) {
        state.changeHandler = (e) => {
            const value = e.target.value;
            
            // Only call C# for change events when updateOn='change'
            if (state.updateOn === 'change') {
                dotNetRef.invokeMethodAsync('OnInputChanged', value);
            }
            // For updateOn='input', change event is just for final sync (handled by blur)
        };
        element.addEventListener('change', state.changeHandler);

        // Blur event for final value sync in all modes
        state.blurHandler = (e) => {
            const value = e.target.value;
            
            // Clear any pending debounce timer
            if (state.debounceTimer) {
                clearTimeout(state.debounceTimer);
                state.debounceTimer = null;
            }
            
            // Final sync on blur (helps with validation state)
            if (state.updateOn === 'input') {
                dotNetRef.invokeMethodAsync('OnInputChanged', value);
            }
        };
        element.addEventListener('blur', state.blurHandler);
    }

    inputState.set(elementId, state);
}

/**
 * Update input value from C# (for programmatic changes)
 * Only updates if value differs to avoid cursor jump
 * @param {string} elementId - The element ID
 * @param {string} value - The new value
 */
export function updateValue(elementId, value) {
    const element = document.getElementById(elementId);
    if (!element) return;

    // Only update if value differs to avoid cursor jump
    if (element.value !== value) {
        element.value = value || '';
    }
}

/**
 * Dispose input event handling for an element
 * Cleans up event listeners and timers
 * @param {string} elementId - The element ID
 */
export function disposeInput(elementId) {
    const state = inputState.get(elementId);
    if (!state) return;

    const element = document.getElementById(elementId);
    
    // Clear debounce timer
    if (state.debounceTimer) {
        clearTimeout(state.debounceTimer);
    }

    // Remove event listeners
    if (element) {
        if (state.inputHandler) {
            element.removeEventListener('input', state.inputHandler);
        }
        if (state.changeHandler) {
            element.removeEventListener('change', state.changeHandler);
        }
        if (state.blurHandler) {
            element.removeEventListener('blur', state.blurHandler);
        }
    }

    inputState.delete(elementId);
}

// Track command input keyboard handlers
const commandInputState = new Map();

/**
 * Initialize keyboard navigation for command palette input
 * Intercepts arrow keys, Home, End, Enter and calls C# for navigation
 * All other keys pass through to Input component without C# overhead
 * @param {string} elementId - The input element ID
 * @param {object} dotNetRef - DotNetObjectReference for callbacks
 * @param {boolean} autoFocus - Whether to automatically focus the input on initialization
 */
export function initializeCommandInput(elementId, dotNetRef, autoFocus = false) {
    const element = document.getElementById(elementId);
    if (!element) {
        console.warn(`Command input element with id '${elementId}' not found`);
        return;
    }

    // Clean up any existing handler
    disposeCommandInput(elementId);

    // Navigation keys that trigger C# callbacks
    const navigationKeys = ['ArrowDown', 'ArrowUp', 'Home', 'End', 'Enter'];

    const keydownHandler = (e) => {
        if (navigationKeys.includes(e.key)) {
            // Prevent default for navigation keys (no scroll, no text cursor move)
            e.preventDefault();
            
            // Call C# only for navigation
            dotNetRef.invokeMethodAsync('HandleNavigationKey', e.key);
        }
        // All other keys: do nothing, let Input component handle normally
    };

    // Attach with capture: true to intercept before Input component sees it
    element.addEventListener('keydown', keydownHandler, { capture: true });

    commandInputState.set(elementId, { keydownHandler });

    // Focus the input if autoFocus is true
    if (autoFocus) {
      // Use requestAnimationFrame to ensure DOM is ready
      setTimeout(() =>
        requestAnimationFrame(() => {
            element.focus();
        }), 10);
    }
}

/**
 * Focus a command input element
 * @param {string} elementId - The input element ID
 */
export function focusCommandInput(elementId) {
    const element = document.getElementById(elementId);
    if (element) {
        element.focus();
    }
}

/**
 * Dispose command input keyboard handling
 * @param {string} elementId - The input element ID
 */
export function disposeCommandInput(elementId) {
    const state = commandInputState.get(elementId);
    if (!state) return;

    const element = document.getElementById(elementId);
    if (element && state.keydownHandler) {
        element.removeEventListener('keydown', state.keydownHandler, { capture: true });
    }

    commandInputState.delete(elementId);
}



