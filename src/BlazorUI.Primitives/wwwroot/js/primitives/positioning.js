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

    // Add viewport constraint middleware to ensure element stays within viewport bounds
    // This runs after flip/shift to handle any remaining viewport overflow
    middleware.push({
      name: 'blazorViewportConstraint',
      fn(state) {
        const { x, y, rects } = state;
        const floatingWidth = rects.floating.width;
        const floatingHeight = rects.floating.height;
        const viewportHeight = window.innerHeight;
        const viewportWidth = window.innerWidth;
        // Use padding from options (default 8px) for viewport boundary calculations
        const viewportPadding = padding;

        let adjustedX = x;
        let adjustedY = y;
        const data = {};

        // Check if element extends beyond viewport boundaries
        const exceedsBottom = y + floatingHeight > viewportHeight - viewportPadding;
        const exceedsTop = y < viewportPadding;
        const exceedsRight = x + floatingWidth > viewportWidth - viewportPadding;
        const exceedsLeft = x < viewportPadding;

        // Handle vertical overflow
        if (exceedsBottom || exceedsTop) {
          const maxHeight = viewportHeight - (viewportPadding * 2);
          if (floatingHeight > maxHeight) {
            // Content is larger than viewport - signal need for scrollbar
            data.needsVerticalScroll = true;
            data.maxHeight = maxHeight;
            // Position at top of viewport with padding
            adjustedY = viewportPadding;
          } else {
            // Content fits in viewport but positioned poorly - reposition it
            if (exceedsBottom) {
              // Shift up to fit
              const newTop = viewportHeight - floatingHeight - viewportPadding;
              adjustedY = Math.max(viewportPadding, newTop);
            } else if (exceedsTop) {
              // Shift down to fit
              adjustedY = viewportPadding;
            }
          }
        }

        // Handle horizontal overflow
        if (exceedsRight || exceedsLeft) {
          const maxWidth = viewportWidth - (viewportPadding * 2);
          if (floatingWidth > maxWidth) {
            // Content is wider than viewport - signal need for scrollbar
            data.needsHorizontalScroll = true;
            data.maxWidth = maxWidth;
            // Position at left edge of viewport with padding
            adjustedX = viewportPadding;
          } else {
            // Content fits in viewport but positioned poorly - reposition it
            if (exceedsRight) {
              // Shift left to fit
              const newLeft = viewportWidth - floatingWidth - viewportPadding;
              adjustedX = Math.max(viewportPadding, newLeft);
            } else if (exceedsLeft) {
              // Shift right to fit
              adjustedX = viewportPadding;
            }
          }
        }

        return {
          x: adjustedX,
          y: adjustedY,
          data
        };
      }
    });

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
      strategy: result.strategy || strategy,
      middlewareData: result.middlewareData
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
 */
export function applyPosition(floating, position, makeVisible = false) {
  if (!floating || !position) return;

  // Apply all positioning styles atomically to prevent flash
  // Use higher z-index for fixed positioning (nested dropdowns) to ensure they appear above parent popovers
  const zIndex = position.strategy === 'fixed' ? '9999' : floatingZIndex;
  Object.assign(floating.style, {
    position: position.strategy || 'absolute',
    left: `${position.x}px`,
    top: `${position.y}px`,
    transformOrigin: position.transformOrigin,
    zIndex: zIndex
  });

  // Set transform-origin on the first child if it exists (for proper animations)
  // Otherwise, set it on the floating element itself
  if (position.transformOrigin) {
    const targetElement = floating.firstElementChild || floating;
    targetElement.style.transformOrigin = position.transformOrigin;
  }

  // Apply scrollbar constraints if needed (set by viewport constraint middleware)
  const viewportData = position.middlewareData?.blazorViewportConstraint;
  if (viewportData) {
    if (viewportData.needsVerticalScroll) {
      floating.style.maxHeight = `${viewportData.maxHeight}px`;
      floating.style.overflowY = 'auto';
    }

    if (viewportData.needsHorizontalScroll) {
      floating.style.maxWidth = `${viewportData.maxWidth}px`;
      floating.style.overflowX = 'auto';
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
}

/**
 * Sets up auto-update for dynamic positioning.
 * @param {HTMLElement} reference - Reference element
 * @param {HTMLElement} floating - Floating element
 * @param {Object} options - Positioning options
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
      apply: function () {
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
 * Shows a floating element with proper visibility and animation support.
 * Uses requestAnimationFrame to ensure smooth transitions.
 * @param {HTMLElement} floating - The floating element to show
 */
export function showFloating(floating) {
  if (!floating) return;

  requestAnimationFrame(() => {
    // Find element with data-state attribute (generic for all floating portals)
    const contentElement = floating.querySelector('[data-state]') || floating;
    if (contentElement.hasAttribute('data-state')) {
      contentElement.setAttribute('data-state', 'open');
    }

    // Then update visibility styles
    // Use setProperty with 'important' to override any CSS animations/transitions
    floating.style.setProperty('visibility', 'visible', 'important');
    floating.style.setProperty('opacity', '1', 'important');
    floating.style.setProperty('pointer-events', 'auto', 'important');

    // Dispatch event to signal element is now visible
    floating.dispatchEvent(new CustomEvent('blazorui:visible', { bubbles: true }));
  });
}

/**
 * Hides a floating element while keeping it in the DOM (for ForceMount).
 * Uses requestAnimationFrame to ensure smooth transitions.
 * @param {HTMLElement} floating - The floating element to hide
 */
export function hideFloating(floating) {
  if (!floating) return;

  requestAnimationFrame(() => {
    // Find element with data-state attribute (generic for all floating portals)
    const contentElement = floating.querySelector('[data-state]') || floating;
    if (contentElement.hasAttribute('data-state')) {
      contentElement.setAttribute('data-state', 'closed');
    }

    // Then update visibility styles
    // Use setProperty with 'important' to override any CSS animations/transitions
    floating.style.setProperty('visibility', 'hidden', 'important');
    floating.style.setProperty('opacity', '0', 'important');
    floating.style.setProperty('pointer-events', 'none', 'important');

    // Dispatch event to signal element is now hidden
    floating.dispatchEvent(new CustomEvent('blazorui:hidden', { bubbles: true }));
  });
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
      apply: function () { }
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

