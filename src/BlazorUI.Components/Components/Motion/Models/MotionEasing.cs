namespace BlazorUI.Components.Motion;

/// <summary>
/// Comprehensive easing functions for animations.
/// Based on standard easing equations used in web animations.
/// </summary>
public enum MotionEasing
{
    // ========== Basic Easings ==========
    
    /// <summary>
    /// Linear easing (no acceleration). Cubic-bezier: (0, 0, 1, 1)
    /// </summary>
    Linear,

    // ========== Quadratic Easings ==========
    
    /// <summary>
    /// Quadratic ease-in (slow start). Cubic-bezier: (0.11, 0, 0.5, 0)
    /// </summary>
    QuadraticIn,

    /// <summary>
    /// Quadratic ease-out (slow end). Cubic-bezier: (0.5, 1, 0.89, 1)
    /// </summary>
    QuadraticOut,

    /// <summary>
    /// Quadratic ease-in-out (slow start and end). Cubic-bezier: (0.45, 0, 0.55, 1)
    /// </summary>
    QuadraticInOut,

    // ========== Cubic Easings ==========
    
    /// <summary>
    /// Cubic ease-in (slow start). Cubic-bezier: (0.32, 0, 0.67, 0)
    /// </summary>
    CubicIn,

    /// <summary>
    /// Cubic ease-out (slow end). Cubic-bezier: (0.33, 1, 0.68, 1)
    /// </summary>
    CubicOut,

    /// <summary>
    /// Cubic ease-in-out (slow start and end). Cubic-bezier: (0.65, 0, 0.35, 1)
    /// </summary>
    CubicInOut,

    // ========== Quartic Easings ==========
    
    /// <summary>
    /// Quartic ease-in (slow start). Cubic-bezier: (0.5, 0, 0.75, 0)
    /// </summary>
    QuarticIn,

    /// <summary>
    /// Quartic ease-out (slow end). Cubic-bezier: (0.25, 1, 0.5, 1)
    /// </summary>
    QuarticOut,

    /// <summary>
    /// Quartic ease-in-out (slow start and end). Cubic-bezier: (0.76, 0, 0.24, 1)
    /// </summary>
    QuarticInOut,

    // ========== Quintic Easings ==========
    
    /// <summary>
    /// Quintic ease-in (slow start). Cubic-bezier: (0.64, 0, 0.78, 0)
    /// </summary>
    QuinticIn,

    /// <summary>
    /// Quintic ease-out (slow end). Cubic-bezier: (0.22, 1, 0.36, 1)
    /// </summary>
    QuinticOut,

    /// <summary>
    /// Quintic ease-in-out (slow start and end). Cubic-bezier: (0.83, 0, 0.17, 1)
    /// </summary>
    QuinticInOut,

    // ========== Sinusoidal Easings ==========
    
    /// <summary>
    /// Sinusoidal ease-in (smooth start). Cubic-bezier: (0.12, 0, 0.39, 0)
    /// </summary>
    SinusoidalIn,

    /// <summary>
    /// Sinusoidal ease-out (smooth end). Cubic-bezier: (0.61, 1, 0.88, 1)
    /// </summary>
    SinusoidalOut,

    /// <summary>
    /// Sinusoidal ease-in-out (smooth start and end). Cubic-bezier: (0.37, 0, 0.63, 1)
    /// </summary>
    SinusoidalInOut,

    // ========== Exponential Easings ==========
    
    /// <summary>
    /// Exponential ease-in (very slow start). Cubic-bezier: (0.7, 0, 0.84, 0)
    /// </summary>
    ExponentialIn,

    /// <summary>
    /// Exponential ease-out (very slow end). Cubic-bezier: (0.16, 1, 0.3, 1)
    /// </summary>
    ExponentialOut,

    /// <summary>
    /// Exponential ease-in-out (very slow start and end). Cubic-bezier: (0.87, 0, 0.13, 1)
    /// </summary>
    ExponentialInOut,

    // ========== Circular Easings ==========
    
    /// <summary>
    /// Circular ease-in (smooth acceleration). Cubic-bezier: (0.55, 0, 1, 0.45)
    /// </summary>
    CircularIn,

    /// <summary>
    /// Circular ease-out (smooth deceleration). Cubic-bezier: (0, 0.55, 0.45, 1)
    /// </summary>
    CircularOut,

    /// <summary>
    /// Circular ease-in-out (smooth start and end). Cubic-bezier: (0.85, 0, 0.15, 1)
    /// </summary>
    CircularInOut,

    // ========== Back Easings (Overshoot) ==========
    
    /// <summary>
    /// Back ease-in (anticipation before start). Cubic-bezier: (0.36, 0, 0.66, -0.56)
    /// </summary>
    BackIn,

    /// <summary>
    /// Back ease-out (overshoot at end). Cubic-bezier: (0.34, 1.56, 0.64, 1)
    /// </summary>
    BackOut,

    /// <summary>
    /// Back ease-in-out (anticipation and overshoot). Cubic-bezier: (0.68, -0.6, 0.32, 1.6)
    /// </summary>
    BackInOut,

    // ========== Elastic Easings (Spring-like) ==========
    
    /// <summary>
    /// Elastic ease-in (spring wind-up before start). Note: Best implemented with keyframes.
    /// </summary>
    ElasticIn,

    /// <summary>
    /// Elastic ease-out (bouncy spring at end). Note: Best implemented with keyframes.
    /// </summary>
    ElasticOut,

    /// <summary>
    /// Elastic ease-in-out (spring both ends). Note: Best implemented with keyframes.
    /// </summary>
    ElasticInOut,

    // ========== Bounce Easings ==========
    
    /// <summary>
    /// Bounce ease-in (bouncing ball effect before start). Note: Best implemented with keyframes.
    /// </summary>
    BounceIn,

    /// <summary>
    /// Bounce ease-out (bouncing ball effect at end). Note: Best implemented with keyframes.
    /// </summary>
    BounceOut,

    /// <summary>
    /// Bounce ease-in-out (bouncing both ends). Note: Best implemented with keyframes.
    /// </summary>
    BounceInOut,

    // ========== Legacy Aliases ==========
    
    /// <summary>
    /// Alias for QuadraticIn. Legacy support.
    /// </summary>
    EaseIn,

    /// <summary>
    /// Alias for QuadraticOut. Legacy support.
    /// </summary>
    EaseOut,

    /// <summary>
    /// Alias for QuadraticInOut. Legacy support.
    /// </summary>
    EaseInOut,

    // ========== Custom ==========
    
    /// <summary>
    /// Custom cubic bezier curve defined by CustomEasing parameter.
    /// </summary>
    Custom
}
