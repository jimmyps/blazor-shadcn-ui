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
