// Carousel with animations and touch/drag gestures

const carouselInstances = new Map();

/**
 * Initializes a carousel instance with gesture support.
 * @param {HTMLElement} carouselElement - The carousel container
 * @param {HTMLElement} viewportElement - The viewport element
 * @param {HTMLElement} containerElement - The slides container
 * @param {Object} dotNetRef - DotNet object reference for callbacks
 * @param {Object} options - Carousel options
 * @returns {Object} Carousel instance with control methods
 */
export function initCarousel(carouselElement, viewportElement, containerElement, dotNetRef, options) {
    if (!carouselElement || !viewportElement || !containerElement || !dotNetRef) {
        console.warn('carousel: Required elements or dotNetRef is null');
        return {
            dispose: function() {},
            previous: function() {},
            next: function() {},
            scrollTo: function() {},
            getTotalSlides: function() { return 0; },
            getCurrentIndex: function() { return 0; }
        };
    }

    const {
        orientation = 'horizontal',
        autoPlay = false,
        autoPlayInterval = 3000,
        loop = false,
        slidesPerView = 1,
        gap = 0,
        enableDrag = true
    } = options;

    const isHorizontal = orientation === 'horizontal';
    let currentIndex = 0;
    let totalSlides = 0;
    let isDragging = false;
    let startPos = 0;
    let currentTranslate = 0;
    let prevTranslate = 0;
    let autoPlayTimer = null;
    let animationId = null;

    // Get all slide elements
    function getSlides() {
        return Array.from(containerElement.querySelectorAll('[data-carousel-item]'));
    }

    // Calculate total slides
    function updateSlideCount() {
        totalSlides = getSlides().length;
    }

    // Get slide size
    function getSlideSize() {
        const viewport = viewportElement.getBoundingClientRect();
        return isHorizontal ? viewport.width : viewport.height;
    }

    // Set container transform
    function setContainerPosition(translate) {
        const property = isHorizontal ? 'translateX' : 'translateY';
        containerElement.style.transform = `${property}(${translate}px)`;
    }

    // Animate to position
    function animateToPosition(position, callback) {
        const startPosition = currentTranslate;
        const distance = position - startPosition;
        const duration = 300; // ms
        const startTime = performance.now();

        function animate(currentTime) {
            const elapsed = currentTime - startTime;
            const progress = Math.min(elapsed / duration, 1);
            
            // Easing function (ease-out)
            const eased = 1 - Math.pow(1 - progress, 3);
            const currentPosition = startPosition + (distance * eased);
            
            setContainerPosition(currentPosition);

            if (progress < 1) {
                animationId = requestAnimationFrame(animate);
            } else {
                currentTranslate = position;
                prevTranslate = position;
                if (callback) callback();
            }
        }

        animationId = requestAnimationFrame(animate);
    }

    // Navigate to slide
    function scrollToSlide(index, animate = true) {
        updateSlideCount();
        
        // Handle looping
        if (!loop) {
            index = Math.max(0, Math.min(index, totalSlides - 1));
        } else {
            index = ((index % totalSlides) + totalSlides) % totalSlides;
        }

        if (index === currentIndex && animate) {
            return;
        }

        currentIndex = index;
        const slideSize = getSlideSize();
        const targetPosition = -(currentIndex * (slideSize + gap));

        if (animate) {
            containerElement.style.transition = '';
            animateToPosition(targetPosition, () => {
                notifySlideChange();
            });
        } else {
            setContainerPosition(targetPosition);
            currentTranslate = targetPosition;
            prevTranslate = targetPosition;
            notifySlideChange();
        }
    }

    // Notify Blazor about slide change
    function notifySlideChange() {
        try {
            if (dotNetRef && !dotNetRef._disposed) {
                dotNetRef.invokeMethodAsync('HandleSlideChange', currentIndex);
            }
        } catch (error) {
            console.error('carousel slide change callback error:', error);
        }
    }

    // Auto-play
    function startAutoPlay() {
        if (!autoPlay) return;
        
        stopAutoPlay();
        autoPlayTimer = setInterval(() => {
            next();
        }, autoPlayInterval);
    }

    function stopAutoPlay() {
        if (autoPlayTimer) {
            clearInterval(autoPlayTimer);
            autoPlayTimer = null;
        }
    }

    // Drag handlers
    function handleDragStart(clientX, clientY) {
        if (!enableDrag) return;
        
        isDragging = true;
        startPos = isHorizontal ? clientX : clientY;
        
        if (animationId) {
            cancelAnimationFrame(animationId);
        }
        
        containerElement.style.transition = '';
        stopAutoPlay();
    }

    function handleDragMove(clientX, clientY) {
        if (!isDragging) return;

        const currentPos = isHorizontal ? clientX : clientY;
        const delta = currentPos - startPos;
        currentTranslate = prevTranslate + delta;

        setContainerPosition(currentTranslate);
    }

    function handleDragEnd() {
        if (!isDragging) return;

        isDragging = false;
        const movedBy = currentTranslate - prevTranslate;
        const slideSize = getSlideSize();
        
        // Determine if we should change slide based on drag distance
        const threshold = slideSize * 0.2; // 20% of slide width/height
        
        if (Math.abs(movedBy) > threshold) {
            if (movedBy < 0) {
                next();
            } else {
                previous();
            }
        } else {
            // Snap back to current slide
            scrollToSlide(currentIndex, true);
        }

        if (autoPlay) {
            startAutoPlay();
        }
    }

    // Mouse events
    const handleMouseDown = (e) => {
        if (e.button !== 0) return; // Only left mouse button
        e.preventDefault();
        handleDragStart(e.clientX, e.clientY);
    };

    const handleMouseMove = (e) => {
        if (!isDragging) return;
        e.preventDefault();
        handleDragMove(e.clientX, e.clientY);
    };

    const handleMouseUp = (e) => {
        handleDragEnd();
    };

    const handleMouseLeave = (e) => {
        if (isDragging) {
            handleDragEnd();
        }
    };

    // Touch events
    const handleTouchStart = (e) => {
        if (e.touches.length !== 1) return;
        const touch = e.touches[0];
        handleDragStart(touch.clientX, touch.clientY);
    };

    const handleTouchMove = (e) => {
        if (!isDragging || e.touches.length !== 1) return;
        const touch = e.touches[0];
        handleDragMove(touch.clientX, touch.clientY);
    };

    const handleTouchEnd = (e) => {
        handleDragEnd();
    };

    // Keyboard navigation
    const handleKeyDown = (e) => {
        const key = e.key;
        
        if (isHorizontal) {
            if (key === 'ArrowLeft') {
                e.preventDefault();
                previous();
            } else if (key === 'ArrowRight') {
                e.preventDefault();
                next();
            }
        } else {
            if (key === 'ArrowUp') {
                e.preventDefault();
                previous();
            } else if (key === 'ArrowDown') {
                e.preventDefault();
                next();
            }
        }
    };

    // Attach event listeners
    if (enableDrag) {
        containerElement.addEventListener('mousedown', handleMouseDown);
        containerElement.addEventListener('mousemove', handleMouseMove);
        containerElement.addEventListener('mouseup', handleMouseUp);
        containerElement.addEventListener('mouseleave', handleMouseLeave);
        containerElement.addEventListener('touchstart', handleTouchStart, { passive: true });
        containerElement.addEventListener('touchmove', handleTouchMove, { passive: true });
        containerElement.addEventListener('touchend', handleTouchEnd);
        containerElement.style.cursor = 'grab';
    }

    carouselElement.addEventListener('keydown', handleKeyDown);
    carouselElement.setAttribute('tabindex', '0');

    // Handle window resize
    const handleResize = () => {
        scrollToSlide(currentIndex, false);
    };
    window.addEventListener('resize', handleResize);

    // Initialize
    updateSlideCount();
    scrollToSlide(0, false);
    
    if (autoPlay) {
        startAutoPlay();
    }

    // Public API
    const instance = {
        previous() {
            scrollToSlide(currentIndex - 1, true);
        },

        next() {
            scrollToSlide(currentIndex + 1, true);
        },

        scrollTo(index) {
            scrollToSlide(index, true);
        },

        getTotalSlides() {
            updateSlideCount();
            return totalSlides;
        },

        getCurrentIndex() {
            return currentIndex;
        },

        dispose() {
            stopAutoPlay();
            
            if (animationId) {
                cancelAnimationFrame(animationId);
            }

            containerElement.removeEventListener('mousedown', handleMouseDown);
            containerElement.removeEventListener('mousemove', handleMouseMove);
            containerElement.removeEventListener('mouseup', handleMouseUp);
            containerElement.removeEventListener('mouseleave', handleMouseLeave);
            containerElement.removeEventListener('touchstart', handleTouchStart);
            containerElement.removeEventListener('touchmove', handleTouchMove);
            containerElement.removeEventListener('touchend', handleTouchEnd);
            carouselElement.removeEventListener('keydown', handleKeyDown);
            window.removeEventListener('resize', handleResize);
        }
    };

    const carouselId = carouselElement.getAttribute('data-carousel-id');
    if (carouselId) {
        carouselInstances.set(carouselId, instance);
    }

    return instance;
}

/**
 * Gets a carousel instance by ID.
 * @param {string} carouselId - The carousel ID
 * @returns {Object|null} The carousel instance or null
 */
export function getCarouselInstance(carouselId) {
    return carouselInstances.get(carouselId) || null;
}
