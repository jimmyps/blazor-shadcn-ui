/**
 * Element utilities for common DOM operations
 * Provides reusable functions to replace eval() calls
 */

/**
 * Shows an element by setting opacity and pointer-events.
 * Used as fallback when positioning setup fails.
 * @param {string} elementId - The ID of the element to show
 */
export function showElement(elementId) {
    const element = document.getElementById(elementId);
    if (element) {
        element.style.opacity = '1';
        element.style.pointerEvents = 'auto';
    }
}

/**
 * Scrolls an element into view with configurable options.
 * @param {string} elementId - The ID of the element to scroll into view
 * @param {string} block - The block alignment ('nearest', 'start', 'center', 'end')
 * @param {string} behavior - The scroll behavior ('instant', 'smooth', 'auto')
 */
export function scrollIntoView(elementId, block = 'nearest', behavior = 'instant') {
    const element = document.getElementById(elementId);
    if (element) {
        element.scrollIntoView({
            block: block,
            behavior: behavior
        });
    }
}

/**
 * Focuses an element by its ID.
 * @param {string} elementId - The ID of the element to focus
 */
export function focusElement(elementId) {
    const element = document.getElementById(elementId);
    if (element) {
        element.focus();
    }
}

/**
 * Gets the current computed position of an element from its inline styles.
 * Used to sync JS-applied position changes back to C# before re-renders.
 * @param {HTMLElement} element - The element to get position from
 * @returns {Object} Position object with left and top in pixels
 */
export function getElementPosition(element) {
    if (!element) {
        return { left: 0, top: 0 };
    }
    
    // Parse left and top from computed or inline styles
    const computedStyle = window.getComputedStyle(element);
    const left = parseFloat(computedStyle.left) || 0;
    const top = parseFloat(computedStyle.top) || 0;
    
    return { left, top };
}

