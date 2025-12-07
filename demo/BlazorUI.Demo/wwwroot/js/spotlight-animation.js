// Spotlight command palette height animation
let resizeObserver = null;
let mutationObserver = null;
let containerElement = null;
let maxListHeight = null;

/**
 * Sets up smooth height animation for the command palette
 * Observes the content list and animates height changes
 */
export function setupHeightAnimation(container) {
    if (!container) return;
    
    // Clean up any existing observers first
    cleanup();
    
    containerElement = container;
    
    // Find the command list element (ComboboxContent renders as div with role="listbox")
    const listElement = container.querySelector('[role="listbox"]');
    if (!listElement) return;
    
    // Set initial height
    updateHeight(container, listElement);
    
    // Observe size changes to the list content
    resizeObserver = new ResizeObserver(() => {
        updateHeight(container, listElement);
    });
    
    // Observe DOM mutations (items added/removed)
    mutationObserver = new MutationObserver(() => {
        updateHeight(container, listElement);
    });
    
    // Start observing the list element
    resizeObserver.observe(listElement);
    mutationObserver.observe(listElement, {
        childList: true,
        subtree: true,
        attributes: true,
        attributeFilter: ['style', 'class', 'hidden']
    });
}

/**
 * Updates the container height based on content
 */
function updateHeight(container, listElement) {
    if (!container || !listElement) return;
    
    // Get the actual content height
    const listHeight = listElement.scrollHeight;
    
    // Get the input element height (looking for role="combobox" or the wrapper div)
    const inputWrapper = container.querySelector('.flex.items-center.border-b');
    const inputHeight = inputWrapper ? inputWrapper.offsetHeight : 0;
    
  // Calculate total height (input + list content, capped at max-height)
    if (maxListHeight === null)
      maxListHeight = listElement.offsetHeight;

    const actualListHeight = Math.min(listHeight, maxListHeight);
    const totalHeight = inputHeight + actualListHeight;
    
    // Apply the height with smooth transition
    // The CSS transition will handle the animation
    container.style.height = `${totalHeight}px`;
}

/**
 * Cleans up observers
 */
export function cleanup() {
    if (resizeObserver) {
        resizeObserver.disconnect();
        resizeObserver = null;
    }
    
    if (mutationObserver) {
        mutationObserver.disconnect();
        mutationObserver = null;
    }
    
    containerElement = null;
}

/**
 * Handles dialog closing animation
 * Adds closing class, waits for animation to complete, then calls the callback
 * @param {HTMLElement} dialogElement - The dialog element
 * @param {Function} onComplete - Callback to invoke after animation completes
 */
export function handleDialogClose(dialogElement, onComplete) {
    if (!dialogElement) {
        onComplete?.();
        return;
    }

    // Add closing class to trigger exit animations
    dialogElement.classList.add('dialog--closing');
    dialogElement.classList.remove('dialog--open');

    // Wait for animation to complete
    const animationDuration = parseFloat(
        getComputedStyle(document.documentElement)
            .getPropertyValue('--dialog-panel-duration') || '200'
    );

    setTimeout(() => {
        dialogElement.classList.remove('dialog--closing');
        onComplete?.();
    }, animationDuration);
}

/**
 * Adds the open class to trigger enter animations
 * @param {HTMLElement} dialogElement - The dialog element
 */
export function handleDialogOpen(dialogElement) {
    if (!dialogElement) return;

    // Add open class to trigger enter animations
    dialogElement.classList.add('dialog--open');
    dialogElement.classList.remove('dialog--closing');
}

