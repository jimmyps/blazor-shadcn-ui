// Dialog keyboard event handling
// Handles Escape key without C# roundtrip for performance

const dialogKeyHandlers = new Map();

/**
 * Initialize keyboard event handling for a dialog
 * @param {string} elementId - The dialog content element ID
 * @param {boolean} closeOnEscape - Whether Escape should close the dialog
 * @param {object} dotNetRef - DotNetObjectReference for callbacks
 * @returns {string} Handler ID for cleanup
 */
export function initializeKeyboardHandler(elementId, closeOnEscape, dotNetRef) {
    const element = document.getElementById(elementId);
    if (!element) {
        console.warn(`Dialog element with id '${elementId}' not found`);
        return null;
    }

    // Clean up any existing handler
    disposeKeyboardHandler(elementId);

    const keydownHandler = (e) => {
        // Only handle Escape key - all other keys pass through without C# roundtrip
        if (e.key === 'Escape') {
            e.preventDefault();
            e.stopPropagation();

            // Call back to C# only when Escape is pressed
            if (dotNetRef) {
                dotNetRef.invokeMethodAsync('HandleEscapeKey');
            }
        }
        // All other keys: do nothing, avoid C# interop overhead
    };

    // Attach with capture: true to intercept before bubbling
    element.addEventListener('keydown', keydownHandler, { capture: true });

    dialogKeyHandlers.set(elementId, { keydownHandler });
    return elementId;
}

/**
 * Dispose keyboard event handling for a dialog
 * @param {string} elementId - The dialog content element ID
 */
export function disposeKeyboardHandler(elementId) {
    const state = dialogKeyHandlers.get(elementId);
    if (!state) return;

    const element = document.getElementById(elementId);
    if (element && state.keydownHandler) {
        element.removeEventListener('keydown', state.keydownHandler, { capture: true });
    }

    dialogKeyHandlers.delete(elementId);
}
