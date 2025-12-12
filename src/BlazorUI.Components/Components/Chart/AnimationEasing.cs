namespace BlazorUI.Components.Chart;

/// <summary>
/// Animation easing functions matching Recharts conventions.
/// </summary>
/// <remarks>
/// These easing functions control the acceleration curve of chart animations.
/// Used in conjunction with <see cref="ChartAnimation"/> to create smooth, natural motion.
/// </remarks>
public enum AnimationEasing
{
    /// <summary>Linear animation with constant speed</summary>
    Linear,
    
    /// <summary>Ease in using quadratic function</summary>
    EaseInQuad,
    
    /// <summary>Ease out using quadratic function</summary>
    EaseOutQuad,
    
    /// <summary>Ease in and out using quadratic function</summary>
    EaseInOutQuad,
    
    /// <summary>Ease in using cubic function</summary>
    EaseInCubic,
    
    /// <summary>Ease out using cubic function</summary>
    EaseOutCubic,
    
    /// <summary>Ease in and out using cubic function</summary>
    EaseInOutCubic,
    
    /// <summary>Ease in using quartic function</summary>
    EaseInQuart,
    
    /// <summary>Ease out using quartic function</summary>
    EaseOutQuart,
    
    /// <summary>Ease in and out using quartic function (default)</summary>
    EaseInOutQuart,
    
    /// <summary>Ease in using quintic function</summary>
    EaseInQuint,
    
    /// <summary>Ease out using quintic function</summary>
    EaseOutQuint,
    
    /// <summary>Ease in and out using quintic function</summary>
    EaseInOutQuint,
    
    /// <summary>Ease in using exponential function</summary>
    EaseInExpo,
    
    /// <summary>Ease out using exponential function</summary>
    EaseOutExpo,
    
    /// <summary>Ease in and out using exponential function</summary>
    EaseInOutExpo,
    
    /// <summary>Ease in using back function (overshoots)</summary>
    EaseInBack,
    
    /// <summary>Ease out using back function (overshoots)</summary>
    EaseOutBack,
    
    /// <summary>Ease in and out using back function (overshoots)</summary>
    EaseInOutBack,
    
    /// <summary>Elastic ease in (spring effect)</summary>
    EaseInElastic,
    
    /// <summary>Elastic ease out (spring effect)</summary>
    EaseOutElastic,
    
    /// <summary>Elastic ease in and out (spring effect)</summary>
    EaseInOutElastic,
    
    /// <summary>Bounce ease in</summary>
    EaseInBounce,
    
    /// <summary>Bounce ease out</summary>
    EaseOutBounce,
    
    /// <summary>Bounce ease in and out</summary>
    EaseInOutBounce
}
