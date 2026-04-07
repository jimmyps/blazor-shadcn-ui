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
 * Parses a CSS time value ("260ms", "0.26s", "260") to milliseconds.
 * @param {string} value
 * @returns {number}
 */
function parseDurationMs(value) {
    const num = parseFloat(value);
    if (isNaN(num)) return 0;
    // "s" suffix but NOT "ms" suffix → seconds
    const trimmed = value.trim();
    return (trimmed.endsWith('s') && !trimmed.endsWith('ms')) ? num * 1000 : num;
}

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
    // Add scroll offset so the absolute position is correct inside scrollable containers
    const scrollLeft = container.scrollLeft || 0;
    const scrollTop  = container.scrollTop  || 0;
    const left   = `${ar.left - cr.left + scrollLeft}px`;
    const width  = `${ar.width}px`;

    let top, height;
    if (fixedHeight) {
        // Pin to the bottom of the active element at a fixed height.
        // Measure the height in px via layout (supports any CSS unit: px, rem, em…)
        const prevHeight = indicator.style.height;
        indicator.style.height = fixedHeight;
        const pixelHeight = indicator.getBoundingClientRect().height || parseFloat(fixedHeight) || 0;
        indicator.style.height = prevHeight;
        height = fixedHeight;
        top    = `${ar.bottom - pixelHeight - cr.top + scrollTop}px`;
    } else {
        top    = `${ar.top - cr.top + scrollTop}px`;
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
 * Returns false if any ancestor of el (up to but not including container)
 * has data-state="closed", meaning the element is inside a collapsed section.
 * @param {HTMLElement} el
 * @param {HTMLElement} container
 * @returns {boolean}
 */
function isAncestorOpen(el, container) {
    let node = el.parentElement;
    while (node && node !== container) {
        if (node.dataset.state === 'closed') return false;
        node = node.parentElement;
    }
    return true;
}

/**
 * Resolves the active element and repositions the indicator, or hides it.
 */
function positionIndicator(indicator, container, selector, instant, transition, fixedHeight) {
    const activeEl = container.querySelector(selector);
    if (!activeEl || !isAncestorOpen(activeEl, container)) {
        indicator.style.opacity = '0';
        return;
    }
    applyPosition(indicator, container, activeEl, instant, transition, fixedHeight);
}

/**
 * Initialises the indicator: positions it immediately, then sets up a
 * MutationObserver to re-position whenever a relevant attribute changes.
 *
 * @param {HTMLElement} indicator   - The indicator div rendered by SelectionIndicator.razor
 * @param {string}  selector        - CSS selector for the active item
 * @param {boolean} hoverEnabled    - When true, the indicator also follows mouse hover and keyboard focus
 * @param {string|null} hoverTarget - Optional CSS selector used to resolve the hover target.
 *                                    When set, uses e.target.closest(hoverTarget) instead of
 *                                    walking up to the direct child of the container.
 *                                    Useful for nested menus where items exist at varying depths.
 */
export function init(indicator, selector, hoverEnabled, hoverTarget) {
    const container = indicator.parentElement;
    if (!container) return;

    // Ensure the container creates a positioning context for the absolute indicator
    const pos = window.getComputedStyle(container).position;
    if (pos === 'static') container.style.position = 'relative';

    // Read animation config from CSS custom properties (defaults set on the component element)
    const cs        = getComputedStyle(indicator);
    const durRaw    = cs.getPropertyValue('--si-duration').trim();  // e.g. "260ms" or "0.26s"
    const duration  = parseDurationMs(durRaw) || 260;
    const easing    = cs.getPropertyValue('--si-easing').trim()     || 'cubic-bezier(0.34,1.56,0.64,1)';
    const fixedHeight = cs.getPropertyValue('--si-height').trim();  // px only, e.g. "2px"

    const transition = [
        `left ${duration}ms ${easing}`,
        `top ${duration}ms ${easing}`,
        `width ${duration}ms ${easing}`,
        `height ${duration}ms ${easing}`,
        `opacity ${Math.round(duration * 0.58)}ms ease`,
    ].join(', ');

    // Snap to correct position immediately (no animation on first render)
    positionIndicator(indicator, container, selector, true, transition, fixedHeight);

    // If there was no initial active element, applyPosition(instant: true) was never called,
    // so indicator.style.transition was never initialized. Ensure it's set now so that
    // the first hover or selection change animates correctly instead of jumping instantly.
    if (!indicator.style.transition) {
        requestAnimationFrame(() => { indicator.style.transition = transition; });
    }

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
            let el;
            if (hoverTarget) {
                // Find the closest ancestor (or self) matching hoverTarget within the container
                el = e.target.closest(hoverTarget);
                if (!el || el === container || !container.contains(el)) return;
            } else {
                // Default: walk up from event target to find a direct child of the container
                el = e.target;
                while (el && el.parentElement !== container) el = el.parentElement;
            }
            if (!el || el === indicator) return;
            indicator.dataset.siHover = '';
            // Snap on first hover if indicator has no position yet (no active selector match),
            // otherwise animate. Mirrors the behaviour of containers with an initial active item.
            const snap = indicator.style.opacity !== '1';
            applyPosition(indicator, container, el, snap, transition, fixedHeight);
        };

        const snapBack = () => {
            delete indicator.dataset.siHover;
            positionIndicator(indicator, container, selector, false, transition, fixedHeight);
        };

        const onMouseLeave = snapBack;

        // ── Keyboard focus tracking (shares same highlight state as hover) ──────
        const onFocusIn = (e) => {
            let el;
            if (hoverTarget) {
                el = e.target.closest(hoverTarget);
                if (!el || el === container || !container.contains(el)) return;
            } else {
                el = e.target;
                while (el && el.parentElement !== container) el = el.parentElement;
            }
            if (!el || el === indicator) return;
            indicator.dataset.siHover = '';
            const snap = indicator.style.opacity !== '1';
            applyPosition(indicator, container, el, snap, transition, fixedHeight);
        };

        const onFocusOut = (e) => {
            // Only snap back when focus leaves the container entirely
            if (container.contains(e.relatedTarget)) return;
            snapBack();
        };

        container.addEventListener('mouseover', onMouseOver);
        container.addEventListener('mouseleave', onMouseLeave);
        container.addEventListener('focusin', onFocusIn);
        container.addEventListener('focusout', onFocusOut);
        hoverHandlers.push(
            () => container.removeEventListener('mouseover', onMouseOver),
            () => container.removeEventListener('mouseleave', onMouseLeave),
            () => container.removeEventListener('focusin', onFocusIn),
            () => container.removeEventListener('focusout', onFocusOut),
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
