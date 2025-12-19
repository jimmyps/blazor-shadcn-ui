/**
 * Motion.dev JavaScript Interop Module
 * Provides Blazor integration for Motion.dev animation library
 */

// Import Motion.dev from CDN (will be loaded via script tag in HTML)
// This assumes motion.dev library is loaded globally as 'Motion'

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
 * Convert MotionKeyframe C# object to Motion.dev keyframe format
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
 * Convert MotionOptions C# object to Motion.dev options format
 */
function convertOptions(options) {
    if (!options) return {};
    
    const converted = {};
    
    if (options.duration !== null && options.duration !== undefined) {
        converted.duration = options.duration;
    }
    if (options.delay !== null && options.delay !== undefined) {
        converted.delay = options.delay;
    }
    if (options.easing !== null && options.easing !== undefined) {
        // Convert enum to easing string
        const easingMap = {
            0: 'linear',
            1: 'ease-in',
            2: 'ease-out',
            3: 'ease-in-out',
            4: options.customEasing ? `cubic-bezier(${options.customEasing.join(',')})` : 'ease'
        };
        converted.easing = easingMap[options.easing] || 'ease';
    }
    if (options.repeat !== null && options.repeat !== undefined) {
        converted.repeat = options.repeat === -1 ? Infinity : options.repeat;
    }
    if (options.repeatReverse !== null && options.repeatReverse !== undefined) {
        converted.repeatType = options.repeatReverse ? 'reverse' : 'loop';
    }
    if (options.repeatDelay !== null && options.repeatDelay !== undefined) {
        converted.repeatDelay = options.repeatDelay;
    }
    if (options.fill) converted.fill = options.fill;
    if (options.direction) converted.direction = options.direction;
    
    return converted;
}

/**
 * Convert SpringOptions C# object to Motion.dev spring format
 */
function convertSpringOptions(springOptions) {
    if (!springOptions) return { type: 'spring' };
    
    const converted = { type: 'spring' };
    
    if (springOptions.mass !== null && springOptions.mass !== undefined) {
        converted.mass = springOptions.mass;
    }
    if (springOptions.stiffness !== null && springOptions.stiffness !== undefined) {
        converted.stiffness = springOptions.stiffness;
    }
    if (springOptions.damping !== null && springOptions.damping !== undefined) {
        converted.damping = springOptions.damping;
    }
    if (springOptions.velocity !== null && springOptions.velocity !== undefined) {
        converted.velocity = springOptions.velocity;
    }
    if (springOptions.bounce !== null && springOptions.bounce !== undefined) {
        converted.bounce = springOptions.bounce;
    }
    if (springOptions.duration !== null && springOptions.duration !== undefined) {
        converted.duration = springOptions.duration;
    }
    
    return converted;
}

/**
 * Animate an element with Motion.dev
 * @param {HTMLElement} element - The element to animate
 * @param {Object|Array} keyframes - Single keyframe or array of keyframes
 * @param {Object} options - Animation options
 * @param {Object} springOptions - Optional spring physics options
 * @returns {number} Animation instance ID for cleanup
 */
export function motionAnimate(element, keyframes, options, springOptions) {
    if (!element) {
        console.error('Motion: Invalid element provided');
        return -1;
    }

    try {
        // Check if Motion library is available (use window.Motion to avoid ReferenceError)
        if (typeof window.Motion === 'undefined') {
            console.error('Motion.dev library not loaded. Include it via script tag: <script src="_content/BlazorUI.Components/js/motion-one.min.js"></script>');
            return -1;
        }

        if (typeof window.Motion.animate !== 'function') {
            console.error('Motion.animate is not a function. Motion library may not be fully loaded.', window.Motion);
            return -1;
        }

        const MotionLib = window.Motion;
        
        // Convert keyframes
        let convertedKeyframes;
        if (Array.isArray(keyframes)) {
            convertedKeyframes = keyframes.map(k => convertKeyframe(k));
        } else {
            convertedKeyframes = convertKeyframe(keyframes);
        }
        
        // Convert options
        let animationOptions = convertOptions(options);
        
        // Apply spring physics if provided
        if (springOptions) {
            animationOptions = { ...animationOptions, ...convertSpringOptions(springOptions) };
        }
        
        // Create animation
        console.log('Motion: Calling Motion.animate with', { element, keyframes: convertedKeyframes, options: animationOptions });
        const animation = MotionLib.animate(element, convertedKeyframes, animationOptions);
        console.log('Motion: Animation created successfully', animation);
        
        // Store in registry
        const id = animationIdCounter++;
        animationRegistry.set(id, animation);
        
        return id;
    } catch (error) {
        console.error('Motion: Animation failed', error);
        console.error('Motion: Error details - element:', element, 'keyframes:', keyframes, 'options:', options);
        return -1;
    }
}

/**
 * Set up IntersectionObserver for in-view animations
 * @param {HTMLElement} element - The element to observe
 * @param {Object} inViewOptions - IntersectionObserver options
 * @param {Function} callback - Callback to invoke via DotNetObjectReference
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
            threshold: inViewOptions?.threshold || 0.1,
            rootMargin: inViewOptions?.rootMargin || '0px'
        };

        const observer = new IntersectionObserver((entries) => {
            entries.forEach(entry => {
                if (entry.isIntersecting) {
                    // Notify Blazor component
                    dotNetHelper.invokeMethodAsync('OnIntersecting');
                    
                    // Disconnect if "once" is true
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
            animation.stop();
            animation.cancel();
        } catch (error) {
            console.warn('Motion: Failed to stop animation', error);
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
            console.warn('Motion: Failed to disconnect observer', error);
        }
        observerRegistry.delete(observerId);
    }
}

/**
 * Cleanup all animations and observers
 */
export function cleanupAll() {
    animationRegistry.forEach(animation => {
        try {
            animation.stop();
            animation.cancel();
        } catch (error) {
            // Ignore cleanup errors
        }
    });
    animationRegistry.clear();

    observerRegistry.forEach(entry => {
        try {
            entry.observer.disconnect();
        } catch (error) {
            // Ignore cleanup errors
        }
    });
    observerRegistry.clear();
}

/**
 * Check if Motion.dev library is loaded
 * @returns {boolean} True if Motion is available
 */
export function isMotionAvailable() {
    return typeof window.Motion !== 'undefined';
}

/**
 * Check if user prefers reduced motion
 * @returns {boolean} True if user prefers reduced motion
 */
export function checkReducedMotion() {
    return window.matchMedia('(prefers-reduced-motion: reduce)').matches;
}
