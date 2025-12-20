/**
 * Motion One JavaScript Interop Module
 * Provides Blazor integration for Motion One animation library
 * 
 * Motion One provides: animate(), timeline(), stagger(), scroll(), inView()
 * 
 * Usage:
 * 1. Include Motion One in your app (recommended):
 *    <script src="https://cdn.jsdelivr.net/npm/motion@latest/dist/motion.js"></script>
 * 2. Or let this module auto-load from CDN as ESM module
 */

// Import Motion One functions from CDN or use global if available
let motionLib = null;
let loadingPromise = null; // Track in-flight loading

async function loadMotion() {
    // If already loaded, return immediately
    if (motionLib) return motionLib;
    
    // If currently loading, wait for that promise
    if (loadingPromise) {
        return loadingPromise;
    }
    
    // Check if Motion One is already loaded globally (via script tag)
    if (typeof window.Motion !== 'undefined' && window.Motion) {
        motionLib = window.Motion;
        return motionLib;
    }
    
    // Start loading from CDN and store the promise
    loadingPromise = (async () => {
        try {
            const lib = await import('https://cdn.jsdelivr.net/npm/motion@latest/+esm');
            motionLib = lib;
            return lib;
        } catch (error) {
            console.error('Motion: Failed to load library', error);
            throw new Error('Motion One library could not be loaded. Include it via script tag or check network connection.');
        } finally {
            loadingPromise = null;
        }
    })();
    
    return loadingPromise;
}

// Registry to store active animation instances
const animationRegistry = new Map();
let animationIdCounter = 0;

// Registry for IntersectionObserver instances
const observerRegistry = new Map();
let observerIdCounter = 0;

/**
 * Helper to check if a value is not null or undefined
 */
function isDefined(value) {
    return value !== null && value !== undefined;
}

/**
 * Convert MotionKeyframe C# object to Motion One keyframe format
 * Motion One expects: animate(element, { opacity: [0, 1], scale: [0.8, 1] }, options)
 * Or: animate(element, [{ opacity: 0, scale: 0.8 }, { opacity: 1, scale: 1 }], options)
 */
function convertKeyframe(keyframe) {
    const converted = {};
    
    // Numeric properties
    const numericProps = ['opacity', 'scale', 'rotate', 'scaleX', 'scaleY', 'rotateX', 'rotateY', 'skewX', 'skewY'];
    numericProps.forEach(prop => {
        if (isDefined(keyframe[prop])) {
            converted[prop] = keyframe[prop];
        }
    });
    
    // Transform properties that accept both string and numeric values
    const transformProps = ['x', 'y', 'z'];
    transformProps.forEach(prop => {
        if (isDefined(keyframe[prop])) {
            converted[prop] = keyframe[prop];
        }
    });
    
    // String properties
    const stringProps = ['filter', 'backgroundColor', 'color', 'borderRadius', 'width', 'height'];
    stringProps.forEach(prop => {
        if (isDefined(keyframe[prop])) {
            converted[prop] = keyframe[prop];
        }
    });
    
    return converted;
}

/**
 * Convert array of keyframes to Motion One's preferred object format
 * Input: [{ opacity: 0, scale: 0.8 }, { opacity: 1, scale: 1 }]
 * Output: { opacity: [0, 1], scale: [0.8, 1] }
 */
function convertKeyframesToObjectFormat(keyframes) {
    if (!Array.isArray(keyframes) || keyframes.length === 0) {
        return {};
    }
    
    // If only one keyframe, return it as-is (Motion One will animate from current state)
    if (keyframes.length === 1) {
        return keyframes[0];
    }
    
    // Convert multiple keyframes to object format with arrays
    const result = {};
    const properties = new Set();
    
    // Collect all unique properties
    keyframes.forEach(kf => {
        Object.keys(kf).forEach(prop => properties.add(prop));
    });
    
    // Build arrays for each property
    properties.forEach(prop => {
        result[prop] = keyframes.map(kf => kf[prop]);
    });
    
    return result;
}

/**
 * Convert MotionOptions C# object to Motion One options format
 */
function convertOptions(options) {
    if (!options) return {};
    
    const converted = {};
    
    if (isDefined(options.duration)) {
        converted.duration = options.duration;
    }
    if (isDefined(options.delay)) {
        converted.delay = options.delay;
    }
    
    // Handle easing: Motion One accepts CSS-style cubic-bezier strings
    // Check for customEasing FIRST before checking the enum
    if (options.customEasing && Array.isArray(options.customEasing) && options.customEasing.length === 4) {
        // Convert array to CSS cubic-bezier string
        converted.ease = options.customEasing;
        
    } else if (isDefined(options.easing)) {
        // Map enum to Motion One's named easings
        const easingMap = {
            0: 'linear',
            1: 'easeIn',
            2: 'easeOut',
            3: 'easeInOut',
            4: 'easeOut' // Default fallback
        };
        converted.ease = easingMap[options.easing] || 'easeOut';
    }
    
    if (isDefined(options.repeat)) {
        converted.repeat = options.repeat === -1 ? Infinity : options.repeat;
    }
    if (options.direction) {
        converted.direction = options.direction;
    }
    
    return converted;
}

/**
 * Convert SpringOptions C# object to Motion One spring format
 */
function convertSpringOptions(springOptions) {
    if (!springOptions) return null;
    
    const spring = {};
    
    if (isDefined(springOptions.stiffness)) {
        spring.stiffness = springOptions.stiffness;
    }
    if (isDefined(springOptions.damping)) {
        spring.damping = springOptions.damping;
    }
    if (isDefined(springOptions.mass)) {
        spring.mass = springOptions.mass;
    }
    if (isDefined(springOptions.velocity)) {
        spring.velocity = springOptions.velocity;
    }
    if (isDefined(springOptions.duration)) {
        spring.duration = springOptions.duration;
    }
    
    // Motion One spring format: { type: 'spring', stiffness, damping, mass }
    return Object.keys(spring).length > 0 ? { type: 'spring', ...spring } : null;
}

/**
 * Animate an element with Motion One
 * @param {HTMLElement} element - The element to animate
 * @param {Object|Array} keyframes - Single keyframe or array of keyframes
 * @param {Object} options - Animation options
 * @param {Object} springOptions - Optional spring physics options
 * @returns {number} Animation instance ID for cleanup
 */
export async function motionAnimate(element, keyframes, options, springOptions) {
    if (!element) {
        console.error('Motion: Invalid element provided');
        return -1;
    }

    try {
        const motion = await loadMotion();
        
        if (!motion || !motion.animate) {
            console.error('Motion: animate function not available');
            return -1;
        }
        
        // Convert keyframes
        let convertedKeyframes;
        if (Array.isArray(keyframes)) {
            const converted = keyframes.map(k => convertKeyframe(k));
            convertedKeyframes = convertKeyframesToObjectFormat(converted);
        } else {
            convertedKeyframes = convertKeyframe(keyframes);
        }
        
        // Convert options
        let animationOptions = convertOptions(options);
        
        // Apply spring physics if provided (overrides easing)
        if (springOptions) {
            const spring = convertSpringOptions(springOptions);
            if (spring) {
                delete animationOptions.ease;
                animationOptions = { ...animationOptions, ...spring };
            }
        }
        
        console.log('Motion: Calling motion.animate with', { 
            element, 
            keyframes: convertedKeyframes, 
            options: animationOptions 
        });
        
        // Call Motion One's animate function
        const animation = motion.animate(element, convertedKeyframes, animationOptions);
        
        // Store in registry
        const id = animationIdCounter++;
        animationRegistry.set(id, animation);
        
        return id;
    } catch (error) {
        console.error('Motion: Animation failed', error);
        return -1;
    }
}

/**
 * Set up IntersectionObserver for in-view animations
 * @param {HTMLElement} element - The element to observe
 * @param {Object} inViewOptions - IntersectionObserver options
 * @param {Object} dotNetHelper - DotNet object reference
 * @returns {number} Observer instance ID for cleanup
 */
export function setupInViewObserver(element, inViewOptions, dotNetHelper) {
    if (!element || !dotNetHelper) {
        console.error('Motion: Invalid parameters for IntersectionObserver');
        return -1;
    }

    try {
        const options = {
            threshold: inViewOptions?.threshold ?? 0.1,
            rootMargin: inViewOptions?.rootMargin ?? '0px'
        };

        const observer = new IntersectionObserver((entries) => {
            entries.forEach(entry => {
                if (entry.isIntersecting) {
                    dotNetHelper.invokeMethodAsync('OnIntersecting');
                    
                    if (inViewOptions?.once !== false) {
                        observer.disconnect();
                    }
                }
            });
        }, options);

        observer.observe(element);

        const id = observerIdCounter++;
        observerRegistry.set(id, { observer, element });
        
        return id;
    } catch (error) {
        console.error('Motion: IntersectionObserver setup failed', error);
        return -1;
    }
}

/**
 * Stop and remove an animation
 * @param {number} animationId - The animation ID returned from motionAnimate
 */
export function stopAnimation(animationId) {
    const animation = animationRegistry.get(animationId);
    if (animation) {
        try {
            // Motion One animations have .stop() and .cancel() methods
            if (typeof animation.stop === 'function') {
                animation.stop();
            }
            if (typeof animation.cancel === 'function') {
                animation.cancel();
            }
        } catch (error) {
            console.warn('Motion: Failed to stop animation', animationId, error);
        }
        animationRegistry.delete(animationId);
    }
}

/**
 * Cleanup IntersectionObserver
 * @param {number} observerId - The observer ID returned from setupInViewObserver
 */
export function cleanupInViewObserver(observerId) {
    const entry = observerRegistry.get(observerId);
    if (entry) {
        try {
            entry.observer.disconnect();
        } catch (error) {
            console.warn('Motion: Failed to disconnect observer', observerId, error);
        }
        observerRegistry.delete(observerId);
    }
}

/**
 * Cleanup all animations and observers
 */
export function cleanupAll() {
    // Clean up all animations
    animationRegistry.forEach((animation, id) => {
        try {
            if (typeof animation.stop === 'function') {
                animation.stop();
            }
            if (typeof animation.cancel === 'function') {
                animation.cancel();
            }
        } catch (error) {
            console.warn('Motion: Failed to cleanup animation', id, error);
        }
    });
    animationRegistry.clear();

    // Clean up all observers
    observerRegistry.forEach((entry, id) => {
        try {
            entry.observer.disconnect();
        } catch (error) {
            console.warn('Motion: Failed to cleanup observer', id, error);
        }
    });
    observerRegistry.clear();
}

/**
 * Check if Motion One library is loaded
 * @returns {boolean} True if Motion is available
 */
export async function isMotionAvailable() {
    try {
        await loadMotion();
        return motionLib !== null;
    } catch {
        return false;
    }
}

/**
 * Check if user prefers reduced motion
 * @returns {boolean} True if user prefers reduced motion
 */
export function checkReducedMotion() {
    return window.matchMedia('(prefers-reduced-motion: reduce)').matches;
}
