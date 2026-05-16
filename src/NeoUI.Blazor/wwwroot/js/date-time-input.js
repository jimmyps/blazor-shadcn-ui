// date-time-input.js — keyboard helpers for DateInput / TimeInput

/**
 * Attach a keydown listener that prevents page scrolling on Arrow keys
 * while still allowing those events to bubble to Blazor.
 * Returns the handler so it can be detached later.
 */
export function initialize(container) {
    if (!container) return;
    container._neoDateTimeHandler = (e) => {
        if (['ArrowUp', 'ArrowDown', 'ArrowLeft', 'ArrowRight'].includes(e.key)) {
            e.preventDefault();
        }
    };
    container.addEventListener('keydown', container._neoDateTimeHandler);
}

export function dispose(container) {
    if (container?._neoDateTimeHandler) {
        container.removeEventListener('keydown', container._neoDateTimeHandler);
        delete container._neoDateTimeHandler;
    }
}

/** Focus the next [role="spinbutton"] sibling within the container. */
export function focusNextSegment(container) {
    if (!container) return;
    const segments = Array.from(container.querySelectorAll('[role="spinbutton"]'));
    const idx = segments.indexOf(document.activeElement);
    if (idx >= 0 && idx < segments.length - 1) {
        segments[idx + 1].focus();
    }
}

/** Focus the previous [role="spinbutton"] sibling within the container. */
export function focusPrevSegment(container) {
    if (!container) return;
    const segments = Array.from(container.querySelectorAll('[role="spinbutton"]'));
    const idx = segments.indexOf(document.activeElement);
    if (idx > 0) {
        segments[idx - 1].focus();
    }
}

/** Focus the first [role="spinbutton"] within the container. */
export function focusFirstSegment(container) {
    if (!container) return;
    const first = container.querySelector('[role="spinbutton"]');
    if (first) first.focus();
}
