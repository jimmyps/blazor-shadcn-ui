/**
 * Auto-positioned arrow for Popover, Tooltip, and HoverCard components.
 * Intelligently positions the arrow to point at the trigger element's center.
 */

// Map to store observer cleanup functions
const observers = new Map();

/**
 * Calculates the arrow position based on content placement and trigger center.
 * @param {string} contentId - The ID of the popover content element
 * @param {HTMLElement} triggerElement - The trigger element
 * @param {string} arrowId - The ID of the arrow element
 * @param {Object} options - Configuration options
 * @param {number} options.width - Arrow width in pixels
 * @param {number} options.height - Arrow height in pixels
 * @param {number} options.edgePadding - Minimum distance from popover edges
 * @param {string} [options.side] - Preferred side hint from placement
 * @returns {Object} Position information with side and alignOffset
 */
export function calculateArrowPosition(contentId, triggerElement, arrowId, options) {
    const contentElement = document.getElementById(contentId);
    if (!contentElement || !triggerElement) {
        return { side: 'bottom', alignOffset: 0 };
    }

    // Get bounding rectangles
    const contentRect = contentElement.getBoundingClientRect();
    const triggerRect = triggerElement.getBoundingClientRect();

    // Try to get side from options, data attribute, or infer from position
    let side = options.side || contentElement.getAttribute('data-side');
    
    if (!side) {
        // Infer side from relative positions
        const contentCenterY = contentRect.top + (contentRect.height / 2);
        const contentCenterX = contentRect.left + (contentRect.width / 2);
        const triggerCenterY = triggerRect.top + (triggerRect.height / 2);
        const triggerCenterX = triggerRect.left + (triggerRect.width / 2);
        
        const deltaX = Math.abs(contentCenterX - triggerCenterX);
        const deltaY = Math.abs(contentCenterY - triggerCenterY);
        
        if (deltaY > deltaX) {
            side = contentCenterY < triggerCenterY ? 'top' : 'bottom';
        } else {
            side = contentCenterX < triggerCenterX ? 'left' : 'right';
        }
    }

    // Calculate trigger center point
    const triggerCenterX = triggerRect.left + (triggerRect.width / 2);
    const triggerCenterY = triggerRect.top + (triggerRect.height / 2);

    let alignOffset = 0;

    // Calculate alignment offset based on side
    if (side === 'top' || side === 'bottom') {
        // Horizontal sides: align arrow horizontally to trigger center
        alignOffset = triggerCenterX - contentRect.left;

        // Calculate min/max bounds with edge padding
        const minOffset = options.edgePadding + (options.width / 2);
        const maxOffset = contentRect.width - options.edgePadding - (options.width / 2);

        // Handle case where popover is too small for edge padding
        if (maxOffset <= minOffset) {
            alignOffset = contentRect.width / 2; // Center arrow
        } else {
            alignOffset = Math.max(minOffset, Math.min(maxOffset, alignOffset));
        }
    } else {
        // Vertical sides (left/right): align arrow vertically to trigger center
        alignOffset = triggerCenterY - contentRect.top;

        // Calculate min/max bounds with edge padding
        const minOffset = options.edgePadding + (options.width / 2);
        const maxOffset = contentRect.height - options.edgePadding - (options.width / 2);

        // Handle case where popover is too small for edge padding
        if (maxOffset <= minOffset) {
            alignOffset = contentRect.height / 2; // Center arrow
        } else {
            alignOffset = Math.max(minOffset, Math.min(maxOffset, alignOffset));
        }
    }

    return {
        side: side,
        alignOffset: alignOffset
    };
}

/**
 * Sets up observers to automatically update arrow position on window resize and scroll.
 * @param {string} contentId - The ID of the popover content element
 * @param {HTMLElement} triggerElement - The trigger element
 * @param {string} arrowId - The ID of the arrow element
 * @param {Object} dotNetRef - .NET object reference for callbacks
 * @param {Object} options - Configuration options
 */
export function observeArrowPosition(contentId, triggerElement, arrowId, dotNetRef, options) {
    // Clean up any existing observers for this arrow
    cleanup(arrowId);

    // Create resize handler
    const resizeHandler = () => {
        dotNetRef.invokeMethodAsync('UpdatePosition');
    };

    // Create debounced scroll handler
    let scrollTimeout;
    const scrollHandler = () => {
        clearTimeout(scrollTimeout);
        scrollTimeout = setTimeout(() => {
            dotNetRef.invokeMethodAsync('UpdatePosition');
        }, 10);
    };

    // Add event listeners
    window.addEventListener('resize', resizeHandler);
    window.addEventListener('scroll', scrollHandler, true); // Use capture phase for all scroll events

    // Store handlers for cleanup
    observers.set(arrowId, {
        resizeHandler,
        scrollHandler,
        scrollTimeout
    });
}

/**
 * Cleans up event listeners and observers for an arrow.
 * @param {string} arrowId - The ID of the arrow element
 */
export function cleanup(arrowId) {
    const observer = observers.get(arrowId);
    if (observer) {
        // Remove event listeners
        window.removeEventListener('resize', observer.resizeHandler);
        window.removeEventListener('scroll', observer.scrollHandler, true);

        // Clear any pending timeout
        if (observer.scrollTimeout) {
            clearTimeout(observer.scrollTimeout);
        }

        // Remove from map
        observers.delete(arrowId);
    }
}
