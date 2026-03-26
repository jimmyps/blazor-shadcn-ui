/**
 * SelectionIndicator interop module.
 * Positions an absolutely-placed indicator element over the active child in a container
 * and spring-animates it with Motion One when selection changes.
 *
 * Works with any container that marks its active child via:
 *   data-state="active"    (Tabs, RadioGroup, Select, DropdownMenuRadioItem)
 *   aria-checked="true"    (ToggleGroup single mode)
 *   data-state="checked"   (RadioGroup, Select, DropdownMenuRadioItem)
 *   aria-current="page"    (Pagination)
 *   data-active="true"     (NavigationMenuLink)
 */

import { animate } from 'https://cdn.jsdelivr.net/npm/motion@latest/+esm';

const SPRING = { type: 'spring', stiffness: 400, damping: 30 };

/** @type {WeakMap<Element, MutationObserver>} */
const observerMap = new WeakMap();

/**
 * Measures the active element and updates the indicator position.
 * @param {HTMLElement} indicator
 * @param {HTMLElement} container
 * @param {string} selector
 * @param {boolean} instant - Skip animation (used on first render to avoid spurious slide)
 */
function positionIndicator(indicator, container, selector, instant) {
    const activeEl = container.querySelector(selector);
    if (!activeEl) {
        indicator.style.opacity = '0';
        return;
    }

    const cr = container.getBoundingClientRect();
    const ar = activeEl.getBoundingClientRect();
    const left = `${ar.left - cr.left}px`;
    const width = `${ar.width}px`;

    if (instant) {
        // Snap directly — no animation on mount
        Object.assign(indicator.style, { left, width, opacity: '1' });
    } else {
        animate(indicator, { left, width }, SPRING);
    }
}

/**
 * Initialises the indicator: positions it immediately, then sets up a
 * MutationObserver to re-position whenever a relevant attribute changes.
 *
 * @param {HTMLElement} indicator - The indicator div rendered by SelectionIndicator.razor
 * @param {string} selector - CSS selector for the active item, e.g. "[data-state=active]"
 */
export function init(indicator, selector) {
    const container = indicator.parentElement;
    if (!container) return;

    // Ensure the container creates a positioning context for the absolute indicator
    const pos = window.getComputedStyle(container).position;
    if (pos === 'static') {
        container.style.position = 'relative';
    }

    // Snap to correct position immediately (no animation on first render)
    positionIndicator(indicator, container, selector, true);

    // Watch for attribute changes that signal a new active item
    const observer = new MutationObserver(() =>
        positionIndicator(indicator, container, selector, false));

    observer.observe(container, {
        subtree: true,
        attributeFilter: ['data-state', 'aria-selected', 'aria-checked', 'aria-current', 'data-active'],
    });

    observerMap.set(indicator, observer);
}

/**
 * Disconnects the MutationObserver for the given indicator element.
 * Called from SelectionIndicator.razor's DisposeAsync.
 *
 * @param {HTMLElement} indicator
 */
export function dispose(indicator) {
    observerMap.get(indicator)?.disconnect();
    observerMap.delete(indicator);
}
