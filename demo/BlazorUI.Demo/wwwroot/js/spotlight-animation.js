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
 * Adds closing class, waits for animation to complete
 * @param {HTMLElement} containerElement - The container element inside the dialog
 */
export function handleDialogClose(containerElement) {
    if (!containerElement) {
        return;
    }

    // Find the dialog panel and overlay elements
    const dialogPanel = containerElement.closest('[role="dialog"]');
    const dialogOverlay = dialogPanel?.parentElement?.querySelector('[data-radix-dialog-overlay]');

    if (dialogPanel) {
        // Add closing class to trigger exit animations
        dialogPanel.classList.add('dialog--closing');
        dialogPanel.classList.remove('dialog--open');
    }

    if (dialogOverlay) {
        dialogOverlay.classList.add('dialog--closing');
        dialogOverlay.classList.remove('dialog--open');
    }
}

/**
 * Adds the open class to trigger enter animations
 * @param {HTMLElement} containerElement - The container element inside the dialog
 */
export function handleDialogOpen(containerElement) {
    if (!containerElement) return;

    // Find the dialog panel and overlay elements
    const dialogPanel = containerElement.closest('[role="dialog"]');
    const dialogOverlay = dialogPanel?.parentElement?.querySelector('[data-radix-dialog-overlay]');

    if (dialogPanel) {
        // Add open class to trigger enter animations
        dialogPanel.classList.add('dialog--open');
        dialogPanel.classList.remove('dialog--closing');
    }

    if (dialogOverlay) {
        dialogOverlay.classList.add('dialog--open');
        dialogOverlay.classList.remove('dialog--closing');
    }
}

