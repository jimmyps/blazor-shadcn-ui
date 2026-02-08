// Input component JavaScript module
// Manages input event handling, validation tooltips, error states, and UpdateOn mode behavior

// Track elements with active input handling (event listeners, debounce timers, etc.)
const inputState = new Map();

// Track elements with active validation and their update modes
const validationState = new Map();

/**
 * Safely invoke a .NET method with consistent error handling
 * @param {object} dotNetRef - The DotNetObjectReference
 * @param {string} methodName - The method name to invoke
 * @param  {...any} args - Arguments to pass to the method
 * @returns {Promise} - The result of the invocation or undefined on error
 */
async function safeInvoke(dotNetRef, methodName, ...args) {
    if (!dotNetRef) {
        console.warn(`Cannot invoke ${methodName}: dotNetRef is null`);
        return;
    }
    
    try {
        return await dotNetRef.invokeMethodAsync(methodName, ...args);
    } catch (err) {
        // Ignore disposed object errors (normal during component disposal)
        if (err.message?.includes('disposed') || err.message?.includes('released')) {
            return;
        }
        console.error(`Error invoking ${methodName}:`, err);
        throw err;
    }
}

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
    const element = document.getElementById(elementId);
    const state = validationState.get(elementId);
    
    if (element) {
        // Clear validation state from element
        element.setCustomValidity('');
        
        // Remove event listener
        if (state && state.inputHandler) {
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
 * @param {boolean} enableBlurValidation - Whether to call ValidateAndClamp on blur (for UpdateOn=Input + Min/Max)
 */
export function initializeInput(elementId, updateOn = 'change', debounceDelay = 0, dotNetRef = null, enableBlurValidation = false) {
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
        enableBlurValidation: enableBlurValidation,
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
                
                // Set new timer with state check
                state.debounceTimer = setTimeout(() => {
                    // Check if state still exists before invoking
                    const currentState = inputState.get(elementId);
                    if (currentState && currentState.dotNetRef) {
                        // Just invoke - no DOM update during typing (clamping deferred to blur)
                        safeInvoke(currentState.dotNetRef, 'OnInputChanged', value);
                    }
                }, state.debounceDelay);
            } else {
                // No debounce - immediate call
                // No DOM update during typing - would interfere with user input
                safeInvoke(dotNetRef, 'OnInputChanged', value);
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
                // C# will call updateValue() if value gets clamped
                safeInvoke(dotNetRef, 'OnInputChanged', value);
            }
        };
        element.addEventListener('change', state.changeHandler);

        // Unified blur handler with conditional logic
        state.blurHandler = (e) => {
            const value = e.target.value;
            
            // Clear any pending debounce timer
            if (state.debounceTimer) {
                clearTimeout(state.debounceTimer);
                state.debounceTimer = null;
            }
            
            // Handle validation clamping FIRST if enabled (UpdateOn=Input + Min/Max)
            if (state.enableBlurValidation) {
                // C# will call updateValue() if value gets clamped
                safeInvoke(dotNetRef, 'ValidateAndClamp');
            }
            // Final sync for UpdateOn=Input without blur validation
            // Just syncing state - no transformation expected
            else if (state.updateOn === 'input') {
                safeInvoke(dotNetRef, 'OnInputChanged', value);
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
            safeInvoke(dotNetRef, 'HandleNavigationKey', e.key);
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

// Track elements with maxlength enforcement
const maxLengthState = new Map();

/**
 * Enforce maxlength on input elements (works for type="number" which doesn't support maxlength attribute)
 * @param {string} elementId - The input element ID
 * @param {number} maxLength - Maximum number of characters allowed
 */
export function enforceMaxLength(elementId, maxLength) {
    const element = document.getElementById(elementId);
    if (!element) {
        console.warn(`Element with id '${elementId}' not found for maxLength enforcement`);
        return;
    }

    // Clean up any existing handler
    disposeMaxLength(elementId);

    const inputHandler = (e) => {
        const value = e.target.value;
        
        // If value exceeds maxLength, truncate it
        if (value.length > maxLength) {
            // Truncate to maxLength
            e.target.value = value.slice(0, maxLength);
            
            // Create and dispatch a new input event so other handlers see truncated value
            const newEvent = new Event('input', { bubbles: true });
            e.target.dispatchEvent(newEvent);
            
            // Stop the ORIGINAL event from propagating further
            e.stopImmediatePropagation();
        }
    };

    // Use capture phase to run BEFORE other input handlers
    element.addEventListener('input', inputHandler, { capture: true });

    maxLengthState.set(elementId, { inputHandler });
}

/**
 * Dispose maxlength enforcement for an element
 * @param {string} elementId - The input element ID
 */
export function disposeMaxLength(elementId) {
    const state = maxLengthState.get(elementId);
    if (!state) return;

    const element = document.getElementById(elementId);
    if (element && state.inputHandler) {
        element.removeEventListener('input', state.inputHandler, { capture: true });
    }

    maxLengthState.delete(elementId);
}






