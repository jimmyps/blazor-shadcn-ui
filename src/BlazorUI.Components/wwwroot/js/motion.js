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
let observerId = 0;

/**
 * Convert MotionKeyframe C# object to Motion.dev keyframe format
 */
function convertKeyframe(keyframe) {
    const converted = {};
    
    if (keyframe.opacity !== null && keyframe.opacity !== undefined) {
        converted.opacity = keyframe.opacity;
    }
    if (keyframe.scale !== null && keyframe.scale !== undefined) {
        converted.scale = keyframe.scale;
    }
    if (keyframe.x) converted.x = keyframe.x;
    if (keyframe.y) converted.y = keyframe.y;
    if (keyframe.rotate !== null && keyframe.rotate !== undefined) {
        converted.rotate = keyframe.rotate;
    }
    if (keyframe.scaleX !== null && keyframe.scaleX !== undefined) {
        converted.scaleX = keyframe.scaleX;
    }
    if (keyframe.scaleY !== null && keyframe.scaleY !== undefined) {
        converted.scaleY = keyframe.scaleY;
    }
    if (keyframe.rotateX !== null && keyframe.rotateX !== undefined) {
        converted.rotateX = keyframe.rotateX;
    }
    if (keyframe.rotateY !== null && keyframe.rotateY !== undefined) {
        converted.rotateY = keyframe.rotateY;
    }
    if (keyframe.z) converted.z = keyframe.z;
    if (keyframe.skewX !== null && keyframe.skewX !== undefined) {
        converted.skewX = keyframe.skewX;
    }
    if (keyframe.skewY !== null && keyframe.skewY !== undefined) {
        converted.skewY = keyframe.skewY;
    }
    if (keyframe.filter) converted.filter = keyframe.filter;
    if (keyframe.backgroundColor) converted.backgroundColor = keyframe.backgroundColor;
    if (keyframe.color) converted.color = keyframe.color;
    if (keyframe.borderRadius) converted.borderRadius = keyframe.borderRadius;
    if (keyframe.width) converted.width = keyframe.width;
    if (keyframe.height) converted.height = keyframe.height;
    
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
        // Check if Motion library is available
        if (typeof Motion === 'undefined' && typeof window.Motion === 'undefined') {
            console.error('Motion.dev library not loaded. Include it via CDN in your HTML.');
            return -1;
        }

        const MotionLib = Motion || window.Motion;
        
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
        const animation = MotionLib.animate(element, convertedKeyframes, animationOptions);
        
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

        const id = observerId++;
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
    return typeof Motion !== 'undefined' || typeof window.Motion !== 'undefined';
}
