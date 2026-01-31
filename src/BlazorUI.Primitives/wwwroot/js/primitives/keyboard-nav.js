/**
 * Keyboard navigation helper for primitives
 * Prevents default scroll behavior for navigation keys
 */

<<<<<<< HEAD
/**
 * Waits for an element to appear in the DOM by ID.
 * @param {string} elementId - ID of the element to wait for
 * @param {number} maxWaitMs - Maximum time to wait in milliseconds
 * @param {number} intervalMs - Check interval in milliseconds
 * @returns {Promise<HTMLElement|null>} The element if found, null otherwise
 */
async function waitForElementById(elementId, maxWaitMs = 200, intervalMs = 10) {
    const startTime = Date.now();

    while (Date.now() - startTime < maxWaitMs) {
        const element = document.getElementById(elementId);
        if (element && document.body.contains(element)) {
            return element;
        }
        await new Promise(resolve => setTimeout(resolve, intervalMs));
    }

    return null;
}

export function setupKeyboardNav(element, dotNetRef) {
    if (!element) return;
=======
export function setupKeyboardNav(element, dotNetRef) {
    if (!element) return { dispose: () => {} };
>>>>>>> pr-89

    const handleKeyDown = (e) => {
        // Prevent default scroll behavior for navigation keys
        if (['ArrowDown', 'ArrowUp', 'Home', 'End', 'PageUp', 'PageDown'].includes(e.key)) {
            e.preventDefault();
        }
    };

    element.addEventListener('keydown', handleKeyDown);

    // Return cleanup function
    return {
        dispose: () => {
            element.removeEventListener('keydown', handleKeyDown);
        }
    };
}

/**
<<<<<<< HEAD
 * Sets up keyboard navigation using element ID. Waits for element to appear in DOM.
 * This decouples C# timing from portal rendering - JS handles all waiting.
 * @param {string} elementId - ID of the element to set up keyboard nav for
 * @param {Object} dotNetRef - DotNet object reference (optional, for future callback support)
 * @param {number} maxWaitMs - Maximum time to wait for element in milliseconds
 * @returns {Promise<Object>} Disposable object with dispose() method, or no-op if element not found
 */
export async function setupKeyboardNavById(elementId, dotNetRef = null, maxWaitMs = 200) {
    const element = await waitForElementById(elementId, maxWaitMs);
    if (!element) {
        console.warn(`setupKeyboardNavById: Element ${elementId} not found within ${maxWaitMs}ms`);
        return {
            dispose: function() {}
        };
    }

    return setupKeyboardNav(element, dotNetRef);
=======
 * Gets menu items in DOM order within a container.
 * Includes menuitem, menuitemcheckbox, and menuitemradio roles.
 * @param {HTMLElement} container - The menu container element
 * @returns {HTMLElement[]} Array of menu item elements in DOM order
 */
function getMenuItemsInDomOrder(container) {
    if (!container) return [];
    return Array.from(container.querySelectorAll('[role="menuitem"], [role="menuitemcheckbox"], [role="menuitemradio"]'));
}

/**
 * Gets enabled (not disabled) menu items in DOM order.
 * @param {HTMLElement} container - The menu container element
 * @returns {HTMLElement[]} Array of enabled menu item elements in DOM order
 */
function getEnabledMenuItems(container) {
    return getMenuItemsInDomOrder(container).filter(item =>
        item.getAttribute('aria-disabled') !== 'true'
    );
}

/**
 * Navigates to the next menu item in DOM order.
 * @param {HTMLElement} container - The menu container element
 * @param {boolean} loop - Whether to loop from last to first
 * @returns {HTMLElement|null} The focused element, or null if none
 */
export function navigateNext(container, loop = true) {
    const items = getEnabledMenuItems(container);
    if (items.length === 0) return null;

    const currentIndex = items.findIndex(item => item === document.activeElement);
    let nextIndex;

    if (currentIndex === -1) {
        nextIndex = 0;
    } else if (currentIndex === items.length - 1) {
        nextIndex = loop ? 0 : currentIndex;
    } else {
        nextIndex = currentIndex + 1;
    }

    items[nextIndex]?.focus();
    return items[nextIndex] || null;
}

/**
 * Navigates to the previous menu item in DOM order.
 * @param {HTMLElement} container - The menu container element
 * @param {boolean} loop - Whether to loop from first to last
 * @returns {HTMLElement|null} The focused element, or null if none
 */
export function navigatePrevious(container, loop = true) {
    const items = getEnabledMenuItems(container);
    if (items.length === 0) return null;

    const currentIndex = items.findIndex(item => item === document.activeElement);
    let prevIndex;

    if (currentIndex === -1) {
        prevIndex = items.length - 1;
    } else if (currentIndex === 0) {
        prevIndex = loop ? items.length - 1 : 0;
    } else {
        prevIndex = currentIndex - 1;
    }

    items[prevIndex]?.focus();
    return items[prevIndex] || null;
}

/**
 * Navigates to the first menu item.
 * @param {HTMLElement} container - The menu container element
 * @returns {HTMLElement|null} The focused element, or null if none
 */
export function navigateFirst(container) {
    const items = getEnabledMenuItems(container);
    if (items.length === 0) return null;

    items[0]?.focus();
    return items[0] || null;
}

/**
 * Navigates to the last menu item.
 * @param {HTMLElement} container - The menu container element
 * @returns {HTMLElement|null} The focused element, or null if none
 */
export function navigateLast(container) {
    const items = getEnabledMenuItems(container);
    if (items.length === 0) return null;

    const lastIndex = items.length - 1;
    items[lastIndex]?.focus();
    return items[lastIndex] || null;
>>>>>>> pr-89
}
