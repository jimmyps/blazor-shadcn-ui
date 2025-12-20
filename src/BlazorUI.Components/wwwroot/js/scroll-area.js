// ScrollArea JavaScript module for scroll shadow detection and native-like scrollbar

// Threshold for detecting scroll position (in pixels)
const SCROLL_THRESHOLD = 1;

export function initialize(scrollAreaElement, options) {
    if (!scrollAreaElement) {
        console.error('ScrollArea: Element reference is null');
        return null;
    }

    const viewport = scrollAreaElement.querySelector('[data-scroll-viewport]');
    if (!viewport) {
        console.error('ScrollArea: Viewport element not found');
        return null;
    }

    const config = {
        enableShadows: options?.enableShadows ?? true,
        ...options
    };

    // Find scrollbar elements
    const verticalScrollbar = scrollAreaElement.querySelector('[data-scrollbar][data-orientation="vertical"]');
    const horizontalScrollbar = scrollAreaElement.querySelector('[data-scrollbar][data-orientation="horizontal"]');
    const verticalThumb = verticalScrollbar?.querySelector('[data-scrollbar-thumb]');
    const horizontalThumb = horizontalScrollbar?.querySelector('[data-scrollbar-thumb]');

    let rafId = null;
    let isDisposed = false;
    let isDragging = false;
    let dragStartY = 0;
    let dragStartX = 0;
    let dragStartScrollTop = 0;
    let dragStartScrollLeft = 0;

    // Update scrollbar thumb size and position based on scroll state
    function updateScrollbars() {
        if (isDisposed) return;

        const { scrollTop, scrollLeft, scrollHeight, scrollWidth, clientHeight, clientWidth } = viewport;

        // Update vertical scrollbar
        if (verticalThumb && verticalScrollbar) {
            const scrollableHeight = scrollHeight - clientHeight;
            if (scrollableHeight > 0) {
                // Calculate thumb height as a ratio of viewport to content
                const thumbHeightRatio = clientHeight / scrollHeight;
                const thumbHeight = Math.max(thumbHeightRatio * clientHeight, 20); // Minimum 20px
                
                // Calculate thumb position
                const scrollRatio = scrollTop / scrollableHeight;
                const maxThumbOffset = clientHeight - thumbHeight;
                const thumbOffset = scrollRatio * maxThumbOffset;
                
                verticalThumb.style.height = `${thumbHeight}px`;
                verticalThumb.style.transform = `translateY(${thumbOffset}px)`;
            }
        }

        // Update horizontal scrollbar
        if (horizontalThumb && horizontalScrollbar) {
            const scrollableWidth = scrollWidth - clientWidth;
            if (scrollableWidth > 0) {
                // Calculate thumb width as a ratio of viewport to content
                const thumbWidthRatio = clientWidth / scrollWidth;
                const thumbWidth = Math.max(thumbWidthRatio * clientWidth, 20); // Minimum 20px
                
                // Calculate thumb position
                const scrollRatio = scrollLeft / scrollableWidth;
                const maxThumbOffset = clientWidth - thumbWidth;
                const thumbOffset = scrollRatio * maxThumbOffset;
                
                horizontalThumb.style.width = `${thumbWidth}px`;
                horizontalThumb.style.transform = `translateX(${thumbOffset}px)`;
            }
        }
    }

    // Update shadow visibility based on scroll position
    function updateShadows() {
        if (isDisposed || !config.enableShadows) return;

        const { scrollTop, scrollLeft, scrollHeight, scrollWidth, clientHeight, clientWidth } = viewport;

        // Calculate shadow visibility using threshold
        const canScrollTop = scrollTop > SCROLL_THRESHOLD;
        const canScrollBottom = scrollTop < scrollHeight - clientHeight - SCROLL_THRESHOLD;
        const canScrollLeft = scrollLeft > SCROLL_THRESHOLD;
        const canScrollRight = scrollLeft < scrollWidth - clientWidth - SCROLL_THRESHOLD;

        // Update data attributes for CSS-based shadows
        scrollAreaElement.setAttribute('data-scroll-top', canScrollTop ? 'true' : 'false');
        scrollAreaElement.setAttribute('data-scroll-bottom', canScrollBottom ? 'true' : 'false');
        scrollAreaElement.setAttribute('data-scroll-left', canScrollLeft ? 'true' : 'false');
        scrollAreaElement.setAttribute('data-scroll-right', canScrollRight ? 'true' : 'false');
    }

    // Combined update function
    function updateAll() {
        updateScrollbars();
        updateShadows();
    }

    // Throttled scroll handler using requestAnimationFrame
    function handleScroll() {
        if (rafId) return;
        
        rafId = requestAnimationFrame(() => {
            updateAll();
            rafId = null;
        });
    }

    // Attach scroll listener
    viewport.addEventListener('scroll', handleScroll, { passive: true });

    // Initial updates
    updateAll();

    // Observe content size changes (throttled for performance)
    let resizeRafId = null;
    const resizeObserver = new ResizeObserver(() => {
        if (isDisposed) return;
        
        if (resizeRafId) return;
        
        resizeRafId = requestAnimationFrame(() => {
            updateAll();
            resizeRafId = null;
        });
    });
    resizeObserver.observe(viewport);

    // Drag functionality for vertical scrollbar
    let verticalDragHandlers = null;
    if (verticalThumb) {
        const handleVerticalDragMove = (e) => {
            if (!isDragging) return;
            
            const clientY = e.clientY || e.touches?.[0]?.clientY || 0;
            const deltaY = clientY - dragStartY;
            
            const { scrollHeight, clientHeight } = viewport;
            const scrollableHeight = scrollHeight - clientHeight;
            const thumbHeightRatio = clientHeight / scrollHeight;
            const thumbHeight = Math.max(thumbHeightRatio * clientHeight, 20);
            const maxThumbOffset = clientHeight - thumbHeight;
            
            // Convert thumb movement to scroll position
            const scrollDelta = (deltaY / maxThumbOffset) * scrollableHeight;
            viewport.scrollTop = dragStartScrollTop + scrollDelta;
            
            e.preventDefault();
        };

        const handleVerticalDragEnd = () => {
            if (!isDragging) return;
            isDragging = false;
            
            document.removeEventListener('mousemove', handleVerticalDragMove);
            document.removeEventListener('touchmove', handleVerticalDragMove);
            document.removeEventListener('mouseup', handleVerticalDragEnd);
            document.removeEventListener('touchend', handleVerticalDragEnd);
        };

        const handleVerticalDragStart = (e) => {
            isDragging = true;
            dragStartY = e.clientY || e.touches?.[0]?.clientY || 0;
            dragStartScrollTop = viewport.scrollTop;
            
            document.addEventListener('mousemove', handleVerticalDragMove);
            document.addEventListener('touchmove', handleVerticalDragMove, { passive: false });
            document.addEventListener('mouseup', handleVerticalDragEnd);
            document.addEventListener('touchend', handleVerticalDragEnd);
            
            e.preventDefault();
        };

        verticalThumb.addEventListener('mousedown', handleVerticalDragStart);
        verticalThumb.addEventListener('touchstart', handleVerticalDragStart, { passive: false });
        
        // Store handlers for cleanup
        verticalDragHandlers = {
            start: handleVerticalDragStart,
            move: handleVerticalDragMove,
            end: handleVerticalDragEnd
        };
    }

    // Drag functionality for horizontal scrollbar
    let horizontalDragHandlers = null;
    if (horizontalThumb) {
        const handleHorizontalDragMove = (e) => {
            if (!isDragging) return;
            
            const clientX = e.clientX || e.touches?.[0]?.clientX || 0;
            const deltaX = clientX - dragStartX;
            
            const { scrollWidth, clientWidth } = viewport;
            const scrollableWidth = scrollWidth - clientWidth;
            const thumbWidthRatio = clientWidth / scrollWidth;
            const thumbWidth = Math.max(thumbWidthRatio * clientWidth, 20);
            const maxThumbOffset = clientWidth - thumbWidth;
            
            // Convert thumb movement to scroll position
            const scrollDelta = (deltaX / maxThumbOffset) * scrollableWidth;
            viewport.scrollLeft = dragStartScrollLeft + scrollDelta;
            
            e.preventDefault();
        };

        const handleHorizontalDragEnd = () => {
            if (!isDragging) return;
            isDragging = false;
            
            document.removeEventListener('mousemove', handleHorizontalDragMove);
            document.removeEventListener('touchmove', handleHorizontalDragMove);
            document.removeEventListener('mouseup', handleHorizontalDragEnd);
            document.removeEventListener('touchend', handleHorizontalDragEnd);
        };

        const handleHorizontalDragStart = (e) => {
            isDragging = true;
            dragStartX = e.clientX || e.touches?.[0]?.clientX || 0;
            dragStartScrollLeft = viewport.scrollLeft;
            
            document.addEventListener('mousemove', handleHorizontalDragMove);
            document.addEventListener('touchmove', handleHorizontalDragMove, { passive: false });
            document.addEventListener('mouseup', handleHorizontalDragEnd);
            document.addEventListener('touchend', handleHorizontalDragEnd);
            
            e.preventDefault();
        };

        horizontalThumb.addEventListener('mousedown', handleHorizontalDragStart);
        horizontalThumb.addEventListener('touchstart', handleHorizontalDragStart, { passive: false });
        
        // Store handlers for cleanup
        horizontalDragHandlers = {
            start: handleHorizontalDragStart,
            move: handleHorizontalDragMove,
            end: handleHorizontalDragEnd
        };
    }

    // Track click functionality for vertical scrollbar
    if (verticalScrollbar && verticalThumb) {
        const handleVerticalTrackClick = (e) => {
            // Don't handle if clicking on thumb
            if (e.target === verticalThumb || verticalThumb.contains(e.target)) {
                return;
            }
            
            const trackRect = verticalScrollbar.getBoundingClientRect();
            const clickY = e.clientY - trackRect.top;
            
            const { scrollHeight, clientHeight } = viewport;
            const scrollableHeight = scrollHeight - clientHeight;
            const thumbHeightRatio = clientHeight / scrollHeight;
            const thumbHeight = Math.max(thumbHeightRatio * clientHeight, 20);
            
            // Calculate target scroll position
            // Click position minus half thumb height to center thumb on click
            const targetThumbPosition = clickY - (thumbHeight / 2);
            const maxThumbOffset = clientHeight - thumbHeight;
            const scrollRatio = Math.max(0, Math.min(1, targetThumbPosition / maxThumbOffset));
            
            viewport.scrollTop = scrollRatio * scrollableHeight;
        };
        
        verticalScrollbar.addEventListener('mousedown', handleVerticalTrackClick);
    }

    // Track click functionality for horizontal scrollbar
    if (horizontalScrollbar && horizontalThumb) {
        const handleHorizontalTrackClick = (e) => {
            // Don't handle if clicking on thumb
            if (e.target === horizontalThumb || horizontalThumb.contains(e.target)) {
                return;
            }
            
            const trackRect = horizontalScrollbar.getBoundingClientRect();
            const clickX = e.clientX - trackRect.left;
            
            const { scrollWidth, clientWidth } = viewport;
            const scrollableWidth = scrollWidth - clientWidth;
            const thumbWidthRatio = clientWidth / scrollWidth;
            const thumbWidth = Math.max(thumbWidthRatio * clientWidth, 20);
            
            // Calculate target scroll position
            // Click position minus half thumb width to center thumb on click
            const targetThumbPosition = clickX - (thumbWidth / 2);
            const maxThumbOffset = clientWidth - thumbWidth;
            const scrollRatio = Math.max(0, Math.min(1, targetThumbPosition / maxThumbOffset));
            
            viewport.scrollLeft = scrollRatio * scrollableWidth;
        };
        
        horizontalScrollbar.addEventListener('mousedown', handleHorizontalTrackClick);
    }

    // Return instance with cleanup method
    return {
        dispose: () => {
            isDisposed = true;
            
            if (rafId) {
                cancelAnimationFrame(rafId);
                rafId = null;
            }

            if (resizeRafId) {
                cancelAnimationFrame(resizeRafId);
                resizeRafId = null;
            }

            viewport.removeEventListener('scroll', handleScroll);
            resizeObserver.disconnect();
            
            // Clean up vertical drag handlers
            if (verticalDragHandlers && verticalThumb) {
                verticalThumb.removeEventListener('mousedown', verticalDragHandlers.start);
                verticalThumb.removeEventListener('touchstart', verticalDragHandlers.start);
                // Also ensure move/end listeners are removed in case drag is in progress
                document.removeEventListener('mousemove', verticalDragHandlers.move);
                document.removeEventListener('touchmove', verticalDragHandlers.move);
                document.removeEventListener('mouseup', verticalDragHandlers.end);
                document.removeEventListener('touchend', verticalDragHandlers.end);
            }
            
            // Clean up horizontal drag handlers
            if (horizontalDragHandlers && horizontalThumb) {
                horizontalThumb.removeEventListener('mousedown', horizontalDragHandlers.start);
                horizontalThumb.removeEventListener('touchstart', horizontalDragHandlers.start);
                // Also ensure move/end listeners are removed in case drag is in progress
                document.removeEventListener('mousemove', horizontalDragHandlers.move);
                document.removeEventListener('touchmove', horizontalDragHandlers.move);
                document.removeEventListener('mouseup', horizontalDragHandlers.end);
                document.removeEventListener('touchend', horizontalDragHandlers.end);
            }
        },
        
        scrollTo: (options) => {
            viewport.scrollTo(options);
        },
        
        scrollToTop: () => {
            viewport.scrollTo({ top: 0, behavior: 'smooth' });
        },
        
        scrollToBottom: () => {
            viewport.scrollTo({ top: viewport.scrollHeight, behavior: 'smooth' });
        },

        getScrollPosition: () => ({
            scrollTop: viewport.scrollTop,
            scrollLeft: viewport.scrollLeft,
            scrollHeight: viewport.scrollHeight,
            scrollWidth: viewport.scrollWidth,
            clientHeight: viewport.clientHeight,
            clientWidth: viewport.clientWidth
        })
    };
}

export function dispose(instance) {
    if (instance && typeof instance.dispose === 'function') {
        instance.dispose();
    }
}
