// RangeSlider JavaScript interop for drag and touch handling

const sliderInstances = new Map();

export function initialize(container, dotNetRef) {
    if (!container) return;

    const instance = {
        container,
        dotNetRef,
        isDragging: false,
        cleanupFunctions: []
    };

    // Mouse events
    const handleMouseMove = (e) => {
        if (instance.isDragging) {
            e.preventDefault();
            dotNetRef.invokeMethodAsync('OnDragMove', e.clientX, e.clientY);
        }
    };

    const handleMouseUp = () => {
        if (instance.isDragging) {
            instance.isDragging = false;
            dotNetRef.invokeMethodAsync('OnDragEnd');
        }
    };

    // Touch events
    const handleTouchMove = (e) => {
        if (instance.isDragging && e.touches.length > 0) {
            e.preventDefault();
            const touch = e.touches[0];
            dotNetRef.invokeMethodAsync('OnDragMove', touch.clientX, touch.clientY);
        }
    };

    const handleTouchEnd = () => {
        if (instance.isDragging) {
            instance.isDragging = false;
            dotNetRef.invokeMethodAsync('OnDragEnd');
        }
    };

    // Track click
    const handleTrackClick = (e) => {
        // Check if click is on track, not on handle
        const target = e.target;
        if (target.hasAttribute('data-handle') || target.closest('[data-handle]')) {
            return;
        }

        dotNetRef.invokeMethodAsync('OnTrackClick', e.clientX, e.clientY);
    };

    // Handle mousedown/touchstart
    const handlePointerDown = (e) => {
        const target = e.target;
        if (target.hasAttribute('data-handle') || target.closest('[data-handle]')) {
            instance.isDragging = true;
        }
    };

    // Attach event listeners
    document.addEventListener('mousemove', handleMouseMove);
    document.addEventListener('mouseup', handleMouseUp);
    document.addEventListener('touchmove', handleTouchMove, { passive: false });
    document.addEventListener('touchend', handleTouchEnd);
    container.addEventListener('mousedown', handlePointerDown);
    container.addEventListener('touchstart', handlePointerDown);
    container.addEventListener('click', handleTrackClick);

    // Store cleanup functions
    instance.cleanupFunctions.push(
        () => document.removeEventListener('mousemove', handleMouseMove),
        () => document.removeEventListener('mouseup', handleMouseUp),
        () => document.removeEventListener('touchmove', handleTouchMove),
        () => document.removeEventListener('touchend', handleTouchEnd),
        () => container.removeEventListener('mousedown', handlePointerDown),
        () => container.removeEventListener('touchstart', handlePointerDown),
        () => container.removeEventListener('click', handleTrackClick)
    );

    sliderInstances.set(container, instance);
}

export function calculateValue(container, clientX, clientY, isVertical, min, max) {
    if (!container) return min;

    const sliderContainer = container.querySelector('[role="group"] > div:last-child');
    if (!sliderContainer) return min;

    const rect = sliderContainer.getBoundingClientRect();
    let percentage;

    if (isVertical) {
        // Vertical: bottom to top
        const relativeY = rect.bottom - clientY;
        percentage = Math.max(0, Math.min(1, relativeY / rect.height));
    } else {
        // Horizontal: left to right (supports RTL)
        const isRtl = getComputedStyle(container).direction === 'rtl';
        const relativeX = isRtl ? rect.right - clientX : clientX - rect.left;
        percentage = Math.max(0, Math.min(1, relativeX / rect.width));
    }

    return min + percentage * (max - min);
}

export function dispose(container) {
    const instance = sliderInstances.get(container);
    if (instance) {
        // Run all cleanup functions
        instance.cleanupFunctions.forEach(fn => fn());
        sliderInstances.delete(container);
    }
}
