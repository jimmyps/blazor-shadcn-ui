// ScrollArea JavaScript module for scroll shadow detection and scrollbar enhancements
export function initialize(scrollAreaElement, dotNetRef, options) {
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
        shadowSize: options?.shadowSize ?? 20,
        ...options
    };

    let rafId = null;
    let isDisposed = false;

    // Update shadow visibility based on scroll position
    function updateShadows() {
        if (isDisposed || !config.enableShadows) return;

        const { scrollTop, scrollLeft, scrollHeight, scrollWidth, clientHeight, clientWidth } = viewport;

        // Calculate shadow visibility
        const canScrollTop = scrollTop > 1;
        const canScrollBottom = scrollTop < scrollHeight - clientHeight - 1;
        const canScrollLeft = scrollLeft > 1;
        const canScrollRight = scrollLeft < scrollWidth - clientWidth - 1;

        // Update data attributes for CSS-based shadows
        scrollAreaElement.setAttribute('data-scroll-top', canScrollTop ? 'true' : 'false');
        scrollAreaElement.setAttribute('data-scroll-bottom', canScrollBottom ? 'true' : 'false');
        scrollAreaElement.setAttribute('data-scroll-left', canScrollLeft ? 'true' : 'false');
        scrollAreaElement.setAttribute('data-scroll-right', canScrollRight ? 'true' : 'false');

        // Notify Blazor component if needed
        if (dotNetRef && typeof dotNetRef.invokeMethodAsync === 'function') {
            dotNetRef.invokeMethodAsync('OnScrollPositionChanged', {
                canScrollTop,
                canScrollBottom,
                canScrollLeft,
                canScrollRight,
                scrollTop,
                scrollLeft
            }).catch(err => {
                // Silently ignore if component is disposed
                if (!err.message?.includes('disposed')) {
                    console.warn('ScrollArea: Error invoking OnScrollPositionChanged', err);
                }
            });
        }
    }

    // Throttled scroll handler using requestAnimationFrame
    function handleScroll() {
        if (rafId) return;
        
        rafId = requestAnimationFrame(() => {
            updateShadows();
            rafId = null;
        });
    }

    // Attach scroll listener
    viewport.addEventListener('scroll', handleScroll, { passive: true });

    // Initial shadow update
    updateShadows();

    // Observe content size changes
    const resizeObserver = new ResizeObserver(() => {
        if (!isDisposed) {
            updateShadows();
        }
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
