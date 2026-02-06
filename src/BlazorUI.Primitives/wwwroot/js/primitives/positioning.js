// Positioning service using Floating UI
// CDN: https://cdn.jsdelivr.net/npm/@floating-ui/dom@1.5.3/+esm

let floatingUI = null;
let stylesInjected = false;
let floatingZIndex = 60; // to be consistent with ZIndexLevels.Popover

/**
 * Injects required CSS for positioning primitives.
 * This ensures the library works without requiring manual CSS imports.
 */
function injectRequiredStyles() {
    if (stylesInjected) return;

    const css = `
        /* BlazorUI Primitives - Auto-injected positioning styles */
        [data-positioned="false"] {
            position: absolute !important;
            top: -9999px !important;
            left: -9999px !important;
            opacity: 0 !important;
            pointer-events: none !important;
            z-index: ${floatingZIndex};
        }
    `;

    const style = document.createElement('style');
    style.setAttribute('data-blazorui-primitives', 'positioning');
    style.textContent = css;
    document.head.appendChild(style);
    stylesInjected = true;
}

// Store cleanup functions with unique IDs to avoid passing functions through JS interop
const cleanupRegistry = new Map();
let cleanupIdCounter = 0;

/**
 * Lazy loads Floating UI from preloaded global or CDN with fallback
 */
async function loadFloatingUI() {
    if (floatingUI) return floatingUI;

    // Check for preloaded global first (from App.razor script)
    if (window.FloatingUIDOM) {
        floatingUI = window.FloatingUIDOM;
        return floatingUI;
    }

    try {
        // Try CDN first
        floatingUI = await import('https://cdn.jsdelivr.net/npm/@floating-ui/dom@1.5.3/+esm');
        return floatingUI;
    } catch (cdnError) {
        console.warn('Failed to load Floating UI from CDN, trying local fallback:', cdnError);

        try {
            // Try unpkg as alternative CDN
            floatingUI = await import('https://unpkg.com/@floating-ui/dom@1.5.3/+esm');
            return floatingUI;
        } catch (fallbackError) {
            console.error('Failed to load Floating UI from all sources:', fallbackError);
            throw new Error('Floating UI library could not be loaded. Please check network connection or bundle the library locally.');
        }
    }
}

/**
 * Checks if an element is ready for positioning (exists in DOM and is valid).
 * @param {HTMLElement} element - Element to check
 * @returns {boolean} True if element is ready
 */
export function isElementReady(element) {
    return element &&
           element instanceof Element &&
           document.body.contains(element);
}

/**
 * Waits for an element to be ready in the DOM.
 * @param {string} elementId - ID of the element to wait for
 * @param {number} maxWaitMs - Maximum time to wait in milliseconds
 * @param {number} intervalMs - Check interval in milliseconds
 * @returns {Promise<boolean>} True if element found, false otherwise
 */
export async function waitForElement(elementId, maxWaitMs = 100, intervalMs = 10) {
    const startTime = Date.now();

    while (Date.now() - startTime < maxWaitMs) {
        const element = document.getElementById(elementId);
        if (element && document.body.contains(element)) {
            return true;
        }
        await new Promise(resolve => setTimeout(resolve, intervalMs));
    }

    return false;
}

/**
 * Computes the optimal position for a floating element.
 * @param {HTMLElement} reference - Reference element
 * @param {HTMLElement} floating - Floating element to position
 * @param {Object} options - Positioning options
 * @returns {Promise<Object>} Position result with x, y, placement
 */
export async function computePosition(reference, floating, options = {}) {
    // Inject required CSS on first use
    injectRequiredStyles();

    // Validate elements before proceeding
    if (!isElementReady(reference)) {
        throw new Error('Reference element is not ready or not in DOM');
    }
    if (!isElementReady(floating)) {
        throw new Error('Floating element is not ready or not in DOM');
    }

    try {
        const lib = await loadFloatingUI();

    const {
        placement = 'bottom',
        offset: offsetValue = 8,
        flip = true,
        shift = true,
        padding = 8,
        strategy = 'absolute',
        matchReferenceWidth = false
    } = options;

    const middleware = [
        lib.offset(offsetValue)
    ];

    if (flip) {
        middleware.push(lib.flip({ padding }));
    }

    if (shift) {
        // Allow aggressive shifting to maximize use of available space
        middleware.push(lib.shift({ padding }));
    }

    // Add arrow middleware if arrow element provided
    if (options.arrow) {
        middleware.push(lib.arrow({ element: options.arrow }));
    }

    // Add size middleware to match floating element width to reference element
    if (matchReferenceWidth) {
        middleware.push(lib.size({
            apply({ rects, elements }) {
                Object.assign(elements.floating.style, {
                    width: `${rects.reference.width}px`,
                    minWidth: `${rects.reference.width}px`,
                    maxWidth: `${rects.reference.width}px`
                });
            }
        }));
    }

    const result = await lib.computePosition(reference, floating, {
        placement,
        middleware,
        strategy
    });

        return {
            x: result.x,
            y: result.y,
            placement: result.placement,
            transformOrigin: getTransformOrigin(result.placement),
            strategy: result.strategy || strategy
        };
    } catch (error) {
        console.error('Failed to compute position:', error);
        throw error;  // Re-throw for caller to handle
    }
}

/**
 * Applies computed position to a floating element.
 * @param {HTMLElement} floating - The floating element
 * @param {Object} position - Position result from computePosition
 * @param {boolean} makeVisible - Whether to make the element visible after positioning
 * @returns {Object} Final position after all adjustments {x, y, strategy}
 */
export function applyPosition(floating, position, makeVisible = false) {
    // Handle null/undefined cases - return a safe default position object
    if (!floating || !position) {
        console.warn('applyPosition: invalid floating element or position');
        return {
            x: 0,
            y: 0,
            strategy: 'absolute',
            placement: 'bottom',
            transformOrigin: 'center'
        };
    }

// Track the initial position before viewport adjustments
let finalX = position.x;
let finalY = position.y;

// Apply all positioning styles atomically to prevent flash
// Use setProperty with 'important' to ensure these styles take precedence over inline styles from Blazor
const zIndex = position.strategy === 'fixed' ? '9999' : floatingZIndex;
floating.style.setProperty('position', position.strategy || 'absolute', 'important');
floating.style.setProperty('left', `${position.x}px`, 'important');
floating.style.setProperty('top', `${position.y}px`, 'important');
floating.style.setProperty('z-index', zIndex, 'important');
if (position.transformOrigin) {
    floating.style.transformOrigin = position.transformOrigin;
}

    // Set transform-origin on the first child if it exists (for proper animations)
    // Otherwise, set it on the floating element itself
    if (position.transformOrigin) {
        const targetElement = floating.firstElementChild || floating;
        targetElement.style.transformOrigin = position.transformOrigin;
    }

    // Handle viewport boundary constraints AFTER positioning
    // This ensures flip/shift have done their work first
    const rect = floating.getBoundingClientRect();
    const viewportHeight = window.innerHeight;
    const viewportWidth = window.innerWidth;
    const padding = 8;

    // Check if element extends beyond viewport boundaries
    const exceedsBottom = rect.bottom > viewportHeight - padding;
    const exceedsTop = rect.top < padding;
    const exceedsRight = rect.right > viewportWidth - padding;
    const exceedsLeft = rect.left < padding;

    // Only add scrollbars if element genuinely exceeds viewport
    // This is the absolute last resort after all positioning attempts
    if (exceedsBottom || exceedsTop) {
        const maxHeight = viewportHeight - (padding * 2);
        if (rect.height > maxHeight) {
            // Content is larger than viewport - add scrollbar
            floating.style.maxHeight = `${maxHeight}px`;
            floating.style.overflowY = 'auto';
            // Reposition to ensure it's within bounds
            if (exceedsBottom) {
                finalY = padding;
                floating.style.setProperty('top', `${finalY}px`, 'important');
            }
        } else {
            // Content fits in viewport but positioned poorly - just reposition it
            if (exceedsBottom) {
                // Shift up to fit
                const newTop = viewportHeight - rect.height - padding;
                finalY = Math.max(padding, newTop);
                floating.style.setProperty('top', `${finalY}px`, 'important');
            } else if (exceedsTop) {
                // Shift down to fit
                finalY = padding;
                floating.style.setProperty('top', `${finalY}px`, 'important');
            }
        }
    }

    if (exceedsRight || exceedsLeft) {
        const maxWidth = viewportWidth - (padding * 2);
        if (rect.width > maxWidth) {
            // Content is wider than viewport - add scrollbar
            floating.style.maxWidth = `${maxWidth}px`;
            floating.style.overflowX = 'auto';
            // Reposition to ensure it's within bounds
            if (exceedsRight) {
                finalX = padding;
                floating.style.setProperty('left', `${finalX}px`, 'important');
            }
        } else {
            // Content fits in viewport but positioned poorly - just reposition it
            if (exceedsRight) {
                // Shift left to fit
                const newLeft = viewportWidth - rect.width - padding;
                finalX = Math.max(padding, newLeft);
                floating.style.setProperty('left', `${finalX}px`, 'important');
            } else if (exceedsLeft) {
                // Shift right to fit
                finalX = padding;
                floating.style.setProperty('left', `${finalX}px`, 'important');
            }
        }
    }

    // If makeVisible is true, show the element after positioning
    // Set all visibility-related properties to ensure the element is fully visible
    if (makeVisible) {
        // Use requestAnimationFrame to ensure position is applied before visibility
        requestAnimationFrame(() => {
            // Use setProperty with 'important' to override any CSS animations/transitions
            floating.style.setProperty('visibility', 'visible', 'important');
            floating.style.setProperty('opacity', '1', 'important');
            floating.style.setProperty('pointer-events', 'auto', 'important');
            // Also ensure position is not being reset by CSS
            floating.style.setProperty('top', floating.style.top, 'important');
            floating.style.setProperty('left', floating.style.left, 'important');

            // Dispatch event to signal element is now visible and positioned
            floating.dispatchEvent(new CustomEvent('blazorui:visible', { bubbles: true }));
        });
    }

    // Return final position after all adjustments so C# can stay in sync
    return {
        x: finalX,
        y: finalY,
        strategy: position.strategy || 'absolute',
        placement: position.placement,
        transformOrigin: position.transformOrigin
    };
}

/**
 * Sets up auto-update for dynamic positioning.
 * @param {HTMLElement} reference - Reference element
 * @param {HTMLElement} floating - Floating element
 * @param {Object} options - Positioning options (including optional positionSyncCallback)
 * @returns {Object} Disposable object with id and apply() method for cleanup
 */
export async function autoUpdate(reference, floating, options = {}) {
    try {
        const lib = await loadFloatingUI();

    const update = async () => {
        const position = await computePosition(reference, floating, options);
        applyPosition(floating, position);
    };

    // Initial position
    await update();

        // Set up auto-update - this returns a cleanup function
        const cleanupFunc = lib.autoUpdate(reference, floating, update);

        // Store cleanup function in registry with unique ID
        const id = cleanupIdCounter++;
        cleanupRegistry.set(id, cleanupFunc);

        // Return disposable object with ID (no functions in the object)
        return {
            _cleanupId: id,
            apply: function() {
                const cleanup = cleanupRegistry.get(this._cleanupId);
                if (cleanup) {
                    cleanup();
                    cleanupRegistry.delete(this._cleanupId);
                }
            }
        };
    } catch (error) {
        console.error('Failed to set up auto-update:', error);
        throw error;  // Re-throw for caller to handle
    }
}

/**
 * Gets the transform-origin based on placement for animations.
 * @param {string} placement - The placement value
 * @returns {string} CSS transform-origin value
 */
function getTransformOrigin(placement) {
    const placements = {
        'top': 'bottom center',
        'top-start': 'bottom left',
        'top-end': 'bottom right',
        'bottom': 'top center',
        'bottom-start': 'top left',
        'bottom-end': 'top right',
        'left': 'center right',
        'left-start': 'top right',
        'left-end': 'bottom right',
        'right': 'center left',
        'right-start': 'top left',
        'right-end': 'bottom left'
    };

    return placements[placement] || 'center';
}

/**
 * Sets up positioning using element IDs. Waits for elements to appear in DOM before positioning.
 * This decouples C# timing from portal rendering - JS handles all waiting.
 * @param {string} referenceId - ID of the reference (trigger) element
 * @param {string} floatingId - ID of the floating (content) element
 * @param {Object} options - Positioning options (placement, offset, flip, shift, padding, strategy, maxWaitMs)
 * @returns {Promise<Object>} Disposable object with cleanup method, or no-op if elements not found
 */
export async function setupPositioningById(referenceId, floatingId, options = {}) {
    const maxWaitMs = options.maxWaitMs || 200;

    // Wait for both elements to appear in DOM
    const [refReady, floatReady] = await Promise.all([
        waitForElement(referenceId, maxWaitMs),
        waitForElement(floatingId, maxWaitMs)
    ]);

    if (!refReady || !floatReady) {
        console.warn(`setupPositioningById: Elements not found within ${maxWaitMs}ms - ref: ${refReady}, float: ${floatReady}`);
        return {
            _cleanupId: -1,
            apply: function() {}
        };
    }

    const reference = document.getElementById(referenceId);
    const floating = document.getElementById(floatingId);

    // Use existing autoUpdate logic
    return await autoUpdate(reference, floating, options);
}

/**
 * Focuses an element by ID, waiting for it to appear in DOM if necessary.
 * @param {string} elementId - ID of the element to focus
 * @param {number} maxWaitMs - Maximum time to wait in milliseconds
 * @returns {Promise<boolean>} True if focus succeeded, false otherwise
 */
export async function focusById(elementId, maxWaitMs = 200) {
    const ready = await waitForElement(elementId, maxWaitMs);
    if (!ready) {
        return false;
    }

    const element = document.getElementById(elementId);
    if (element) {
        try {
            element.focus();
            return true;
        } catch (e) {
            console.warn(`focusById: Could not focus element ${elementId}:`, e);
            return false;
        }
    }
    return false;
}

/**
 * Positions an element at specific X/Y coordinates with viewport boundary detection.
 * Used for context menus and other components that need to appear at arbitrary coordinates.
 * @param {HTMLElement} floating - The element to position
 * @param {number} x - X coordinate in pixels
 * @param {number} y - Y coordinate in pixels
 * @param {Object} options - Positioning options (padding, makeVisible)
 * @returns {Object} Final position after viewport adjustments {x, y, transformOrigin}
 */
export function applyCoordinatePosition(floating, x, y, options = {}) {
    if (!floating) {
        console.warn('applyCoordinatePosition: floating element is null');
        return { x, y, transformOrigin: 'top left' };
    }

    const {
        padding = 8,
        makeVisible = true
    } = options;

    // Set initial position
    floating.style.position = 'fixed';
    floating.style.left = `${x}px`;
    floating.style.top = `${y}px`;

    // Get element dimensions after it's positioned
    const rect = floating.getBoundingClientRect();
    const vw = window.innerWidth;
    const vh = window.innerHeight;

    let newX = x;
    let newY = y;

    // Adjust if overflowing viewport (with padding)
    if (newX + rect.width > vw - padding) {
        newX = vw - rect.width - padding;
    }
    if (newY + rect.height > vh - padding) {
        newY = vh - rect.height - padding;
    }
    if (newX < padding) {
        newX = padding;
    }
    if (newY < padding) {
        newY = padding;
    }

    // Apply final position
    floating.style.left = `${newX}px`;
    floating.style.top = `${newY}px`;

    // Calculate transform-origin based on which quadrant of the viewport the element is in
    // This makes animations appear to emanate from the correct corner
    const transformOrigin = getTransformOriginFromCoordinates(x, y, vw, vh);
    
    // Set transform-origin on the first child if it exists (for proper animations)
    // Otherwise, set it on the floating element itself
    const targetElement = floating.firstElementChild || floating;
    targetElement.style.transformOrigin = transformOrigin;

    // Make visible if requested
    if (makeVisible) {
        floating.style.opacity = '1';
        floating.style.visibility = 'visible';
        floating.style.pointerEvents = 'auto';
    }

    return { x: newX, y: newY, transformOrigin };
}

/**
 * Gets the transform-origin based on position in viewport for coordinate-based positioning.
 * @param {number} x - X coordinate in pixels
 * @param {number} y - Y coordinate in pixels
 * @param {number} viewportWidth - Viewport width in pixels
 * @param {number} viewportHeight - Viewport height in pixels
 * @returns {string} CSS transform-origin value
 */
function getTransformOriginFromCoordinates(x, y, viewportWidth, viewportHeight) {
    // Determine which quadrant of the viewport we're in
    const isLeft = x < viewportWidth / 2;
    const isTop = y < viewportHeight / 2;

    // Map quadrant to transform-origin
    // Top-left: origin from top-left corner
    // Top-right: origin from top-right corner
    // Bottom-left: origin from bottom-left corner
    // Bottom-right: origin from bottom-right corner
    if (isTop && isLeft) {
        return 'top left';
    } else if (isTop && !isLeft) {
        return 'top right';
    } else if (!isTop && isLeft) {
        return 'bottom left';
    } else {
        return 'bottom right';
    }
}

