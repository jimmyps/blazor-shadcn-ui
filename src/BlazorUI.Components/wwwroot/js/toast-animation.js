/**
 * Toast animation helper
 * Completely handles toast animation state in JS to avoid Blazor re-render interference.
 * Blazor renders with initial hidden state, JS removes inline styles and manages data-state.
 */

export function initializeToastAnimation(elementId) {
    const element = document.getElementById(elementId);
    if (!element) {
        console.warn(`Toast element ${elementId} not found`);
        return;
    }

    // STEP 1: Set data-state to closed (Blazor already has inline styles for hidden state)
    // Not needed, causing animation flicker
    // element.setAttribute('data-state', 'closed');

    // STEP 2: Use double RAF to ensure Blazor's initial state is painted
    requestAnimationFrame(() => {
        requestAnimationFrame(() => {
            // STEP 3: Remove inline styles and change data-state to trigger CSS animations
            element.style.opacity = '';
            element.style.transform = '';
            element.style.pointerEvents = '';
            element.setAttribute('data-state', 'open');
        });
    });
}

/**
 * Cleanup function to remove any attributes or styles
 */
export function cleanupToastAnimation(elementId) {
    const element = document.getElementById(elementId);
    if (element) {
        element.style.opacity = '';
        element.style.transform = '';
        element.style.pointerEvents = '';
        element.removeAttribute('data-state');
    }
}
