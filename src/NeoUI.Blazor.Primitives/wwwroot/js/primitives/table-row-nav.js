/**
 * Table row navigation and click utilities
 * Provides functions for accessible table row interaction
 */

/**
 * Selector matching interactive elements whose clicks should NOT
 * bubble into row-level click / selection handlers.
 */
const INTERACTIVE_SELECTOR =
  'a[href],button,input,select,textarea,label[for],' +
  '[role="button"],[role="checkbox"],[role="switch"],' +
  '[role="menuitem"],[role="option"],[role="tab"]';

/**
 * Attaches a capture-phase click listener on the row that flags clicks
 * originating from interactive child elements.  Instead of calling
 * stopPropagation (which would prevent Blazor's root-level event
 * delegation from seeing the event at all), we set a property on the
 * row element that the C# HandleClick can read via JS interop.
 *
 * Library-owned interactive elements (selection checkbox) already use
 * Blazor's @onclick:stopPropagation="true" to suppress the row handler
 * through Blazor's internal dispatch.  This interceptor handles
 * user-provided interactive content in cell templates.
 *
 * @param {HTMLElement} rowElement - The <tr> element
 * @returns {{ dispose(): void }} Cleanup handle
 */
export function interceptInteractiveClicks(rowElement) {
  if (!rowElement) return { dispose: () => {} };

  const handler = (e) => {
    const interactive = e.target.closest(INTERACTIVE_SELECTOR);
    rowElement._neoInteractiveClick = !!(interactive && rowElement.contains(interactive) && interactive !== rowElement);
  };

  rowElement.addEventListener('click', handler, { capture: true });

  return {
    dispose: () => {
      rowElement.removeEventListener('click', handler, { capture: true });
    }
  };
}

/**
 * Returns true if the last click on this row targeted an interactive
 * child element, then resets the flag.
 *
 * @param {HTMLElement} rowElement - The <tr> element
 * @returns {boolean}
 */
export function consumeInteractiveClickFlag(rowElement) {
  if (!rowElement) return false;
  const flag = rowElement._neoInteractiveClick === true;
  rowElement._neoInteractiveClick = false;
  return flag;
}

/**
 * Prevents Space and Arrow keys from scrolling when a table row is focused.
 *
 * When the keydown originates from an interactive child element (e.g. a
 * Combobox trigger, Popover button, or any element matching
 * INTERACTIVE_SELECTOR), the handler:
 *   1. Skips preventDefault() so the child retains default browser behaviour.
 *   2. Calls stopPropagation() in bubble phase so Blazor's root-level event
 *      delegation never dispatches the event to the row's @onkeydown handler.
 *
 * @param {HTMLElement} element - The row element to attach the handler to
 * @returns {Object} Object with dispose function for cleanup
 */
export function preventSpaceKeyScroll(element) {
    if (!element) return { dispose: () => {} };

    const captureHandler = (e) => {
        element._neoInteractiveKeyDown = isInteractiveTarget(e.target, element);

        if (element._neoInteractiveKeyDown) {
            return; // Let the interactive child handle it normally
        }

        if (e.key === ' ' || e.keyCode === 32 ||
            e.key === 'ArrowUp' || e.keyCode === 38 ||
            e.key === 'ArrowDown' || e.keyCode === 40) {
            e.preventDefault();
        }
    };

    // Bubble phase: stop propagation so Blazor's HandleKeyDown is never
    // invoked for events that originated from interactive children.
    const bubbleHandler = (e) => {
        if (element._neoInteractiveKeyDown) {
            e.stopPropagation();
            element._neoInteractiveKeyDown = false;
        }
    };

    element.addEventListener('keydown', captureHandler, { capture: true });
    element.addEventListener('keydown', bubbleHandler, { capture: false });

    return {
        dispose: () => {
            element.removeEventListener('keydown', captureHandler, { capture: true });
            element.removeEventListener('keydown', bubbleHandler, { capture: false });
        }
    };
}

/**
 * Checks whether the event target is an interactive child of the row.
 * @param {HTMLElement} target - The event target
 * @param {HTMLElement} rowElement - The <tr> row element
 * @returns {boolean}
 */
function isInteractiveTarget(target, rowElement) {
    if (!target || target === rowElement) return false;
    if (!rowElement.contains(target)) return false;
    const interactive = target.closest(INTERACTIVE_SELECTOR);
    return !!(interactive && rowElement.contains(interactive) && interactive !== rowElement);
}

/**
 * Moves focus to the previous focusable row (tabindex="0").
 * @param {HTMLElement} element - The current row element
 */
export function moveFocusToPreviousRow(element) {
    if (!element) return;

    let prevRow = element.previousElementSibling;
    while (prevRow && prevRow.getAttribute('tabindex') !== '0') {
        prevRow = prevRow.previousElementSibling;
    }
    prevRow?.focus();
}

/**
 * Moves focus to the next focusable row (tabindex="0").
 * @param {HTMLElement} element - The current row element
 */
export function moveFocusToNextRow(element) {
    if (!element) return;

    let nextRow = element.nextElementSibling;
    while (nextRow && nextRow.getAttribute('tabindex') !== '0') {
        nextRow = nextRow.nextElementSibling;
    }
    nextRow?.focus();
}
