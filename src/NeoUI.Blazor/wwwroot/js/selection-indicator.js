/**
 * SelectionIndicator interop module.
 * Positions an absolutely-placed indicator element over the active child in a container
 * and animates it with CSS transitions when selection changes.
 *
 * Works with any container that marks its active child via:
 *   data-state="active"    (Tabs, RadioGroup, Select, DropdownMenuRadioItem)
 *   aria-checked="true"    (ToggleGroup single mode)
 *   data-state="checked"   (RadioGroup, Select, DropdownMenuRadioItem)
 *   aria-current="page"    (Pagination)
 *   data-active="true"     (NavigationMenuLink)
 *
 * CSS custom properties (set on the indicator element):
 *   --si-duration   transition duration in ms         (default: 260)
 *   --si-easing     CSS easing function                (default: cubic-bezier(0.34, 1.56, 0.64, 1))
 *   --si-height     fixed height override in any unit  (when set, indicator is pinned to the bottom
 *                   of the active/hovered element — useful for underline variants)
 */

/** @type {WeakMap<Element, { observer: MutationObserver, cleanup: () => void }>} */
const instanceMap = new WeakMap();

/**
 * Measures an element and updates the indicator's inline position.
 * @param {HTMLElement} indicator
 * @param {HTMLElement} container
 * @param {HTMLElement} activeEl   - already-resolved active/hovered element
 * @param {boolean}     instant    - Skip animation (used on first render)
 * @param {string}      transition - Pre-built CSS transition string
 * @param {string|null} fixedHeight - Value of --si-height (e.g. "2px"), or empty string
 */
function applyPosition(indicator, container, activeEl, instant, transition, fixedHeight) {
    const cr = container.getBoundingClientRect();
    const ar = activeEl.getBoundingClientRect();
    const left   = `${ar.left - cr.left}px`;
    const width  = `${ar.width}px`;

    let top, height;
    if (fixedHeight) {
        // Pin to the bottom of the active element with a fixed height (e.g. underline)
        height = fixedHeight;
        top    = `${ar.bottom - parseFloat(fixedHeight) - cr.top}px`;
    } else {
        top    = `${ar.top - cr.top}px`;
        height = `${ar.height}px`;
    }

    if (instant) {
        indicator.style.transition = 'none';
        Object.assign(indicator.style, { left, width, top, height, opacity: '1' });
        requestAnimationFrame(() => { indicator.style.transition = transition; });
    } else {
        Object.assign(indicator.style, { left, width, top, height, opacity: '1' });
    }
}

/**
 * Resolves the active element and repositions the indicator, or hides it.
 */
function positionIndicator(indicator, container, selector, instant, transition, fixedHeight) {
    const activeEl = container.querySelector(selector);
    if (!activeEl) {
        indicator.style.opacity = '0';
        return;
    }
    applyPosition(indicator, container, activeEl, instant, transition, fixedHeight);
}

/**
 * Initialises the indicator: positions it immediately, then sets up a
 * MutationObserver to re-position whenever a relevant attribute changes.
 *
 * @param {HTMLElement} indicator - The indicator div rendered by SelectionIndicator.razor
 * @param {string}  selector     - CSS selector for the active item
 * @param {boolean} hoverEnabled - When true, the indicator also follows mouse hover
 */
export function init(indicator, selector, hoverEnabled) {
    const container = indicator.parentElement;
    if (!container) return;

    // Ensure the container creates a positioning context for the absolute indicator
    const pos = window.getComputedStyle(container).position;
    if (pos === 'static') container.style.position = 'relative';

    // Read animation config from CSS custom properties (defaults set on the component element)
    const cs       = getComputedStyle(indicator);
    const duration  = parseInt(cs.getPropertyValue('--si-duration')) || 260;
    const easing    = cs.getPropertyValue('--si-easing').trim()      || 'cubic-bezier(0.34,1.56,0.64,1)';
    const fixedHeight = cs.getPropertyValue('--si-height').trim();   // e.g. "2px" or ""

    const transition = [
        `left ${duration}ms ${easing}`,
        `top ${duration}ms ${easing}`,
        `width ${duration}ms ${easing}`,
        `height ${duration}ms ${easing}`,
        `opacity ${Math.round(duration * 0.58)}ms ease`,
    ].join(', ');

    // Snap to correct position immediately (no animation on first render)
    positionIndicator(indicator, container, selector, true, transition, fixedHeight);

    // Watch for attribute changes that signal a new active item
    const observer = new MutationObserver(() =>
        positionIndicator(indicator, container, selector, false, transition, fixedHeight));

    observer.observe(container, {
        subtree: true,
        attributeFilter: ['data-state', 'aria-selected', 'aria-checked', 'aria-current', 'data-active'],
    });

    // ── Hover tracking ──────────────────────────────────────────────────────
    const hoverHandlers = [];
    if (hoverEnabled) {
        const onMouseOver = (e) => {
            // Walk up from event target to find a direct child of the container
            let el = e.target;
            while (el && el.parentElement !== container) el = el.parentElement;
            if (!el || el === indicator) return;
            indicator.dataset.siHover = '';
            applyPosition(indicator, container, el, false, transition, fixedHeight);
        };

        const onMouseLeave = () => {
            delete indicator.dataset.siHover;
            positionIndicator(indicator, container, selector, false, transition, fixedHeight);
        };

        container.addEventListener('mouseover', onMouseOver);
        container.addEventListener('mouseleave', onMouseLeave);
        hoverHandlers.push(
            () => container.removeEventListener('mouseover', onMouseOver),
            () => container.removeEventListener('mouseleave', onMouseLeave),
        );
    }

    instanceMap.set(indicator, {
        observer,
        cleanup: () => hoverHandlers.forEach(fn => fn()),
    });
}

/**
 * Disconnects the MutationObserver and removes hover listeners for the given indicator.
 * Called from SelectionIndicator.razor's DisposeAsync.
 *
 * @param {HTMLElement} indicator
 */
export function dispose(indicator) {
    const instance = instanceMap.get(indicator);
    if (!instance) return;
    instance.observer.disconnect();
    instance.cleanup();
    instanceMap.delete(indicator);
}
