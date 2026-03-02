namespace BlazorUI.Components.Chart;

/// <summary>
/// Animation easing functions supported by ECharts.
/// </summary>
/// <remarks>
/// These easing functions control the acceleration curve of chart animations.
/// </remarks>
public enum AnimationEasing
{
    /// <summary>Linear animation with constant speed</summary>
    Linear,
    
    /// <summary>Ease in using quadratic function</summary>
    QuadIn,
    
    /// <summary>Ease out using quadratic function</summary>
    QuadOut,
    
    /// <summary>Ease in and out using quadratic function</summary>
    QuadInOut,
    
    /// <summary>Ease in using cubic function</summary>
    CubicIn,
    
    /// <summary>Ease out using cubic function (default)</summary>
    CubicOut,
    
    /// <summary>Ease in and out using cubic function</summary>
    CubicInOut,
    
    /// <summary>Ease in using quartic function</summary>
    QuartIn,
    
    /// <summary>Ease out using quartic function</summary>
    QuartOut,
    
    /// <summary>Ease in and out using quartic function</summary>
    QuartInOut,
    
    /// <summary>Ease in using quintic function</summary>
    QuintIn,
    
    /// <summary>Ease out using quintic function</summary>
    QuintOut,
    
    /// <summary>Ease in and out using quintic function</summary>
    QuintInOut,
    
    /// <summary>Ease in using sinusoidal function</summary>
    SineIn,
    
    /// <summary>Ease out using sinusoidal function</summary>
    SineOut,
    
    /// <summary>Ease in and out using sinusoidal function</summary>
    SineInOut,
    
    /// <summary>Ease in using exponential function</summary>
    ExpoIn,
    
    /// <summary>Ease out using exponential function</summary>
    ExpoOut,
    
    /// <summary>Ease in and out using exponential function</summary>
    ExpoInOut,
    
    /// <summary>Ease in using circular function</summary>
    CircIn,
    
    /// <summary>Ease out using circular function</summary>
    CircOut,
    
    /// <summary>Ease in and out using circular function</summary>
    CircInOut,
    
    /// <summary>Elastic ease in (spring effect)</summary>
    ElasticIn,
    
    /// <summary>Elastic ease out (spring effect)</summary>
    ElasticOut,
    
    /// <summary>Elastic ease in and out (spring effect)</summary>
    ElasticInOut,
    
    /// <summary>Ease in using back function (overshoots)</summary>
    BackIn,
    
    /// <summary>Ease out using back function (overshoots)</summary>
    BackOut,
    
    /// <summary>Ease in and out using back function (overshoots)</summary>
    BackInOut,
    
    /// <summary>Bounce ease in</summary>
    BounceIn,
    
    /// <summary>Bounce ease out</summary>
    BounceOut,
    
    /// <summary>Bounce ease in and out</summary>
    BounceInOut,
    
    // Legacy aliases for backward compatibility
    /// <summary>Legacy alias for QuadIn</summary>
    [Obsolete("Use QuadIn instead")]
    EaseInQuad = QuadIn,
    
    /// <summary>Legacy alias for QuadOut</summary>
    [Obsolete("Use QuadOut instead")]
    EaseOutQuad = QuadOut,
    
    /// <summary>Legacy alias for QuadInOut</summary>
    [Obsolete("Use QuadInOut instead")]
    EaseInOutQuad = QuadInOut,
    
    /// <summary>Legacy alias for CubicIn</summary>
    [Obsolete("Use CubicIn instead")]
    EaseInCubic = CubicIn,
    
    /// <summary>Legacy alias for CubicOut</summary>
    [Obsolete("Use CubicOut instead")]
    EaseOutCubic = CubicOut,
    
    /// <summary>Legacy alias for CubicInOut</summary>
    [Obsolete("Use CubicInOut instead")]
    EaseInOutCubic = CubicInOut,
    
    /// <summary>Legacy alias for QuartIn</summary>
    [Obsolete("Use QuartIn instead")]
    EaseInQuart = QuartIn,
    
    /// <summary>Legacy alias for QuartOut</summary>
    [Obsolete("Use QuartOut instead")]
    EaseOutQuart = QuartOut,
    
    /// <summary>Legacy alias for QuartInOut</summary>
    [Obsolete("Use QuartInOut instead")]
    EaseInOutQuart = QuartInOut,
    
    /// <summary>Legacy alias for QuintIn</summary>
    [Obsolete("Use QuintIn instead")]
    EaseInQuint = QuintIn,
    
    /// <summary>Legacy alias for QuintOut</summary>
    [Obsolete("Use QuintOut instead")]
    EaseOutQuint = QuintOut,
    
    /// <summary>Legacy alias for QuintInOut</summary>
    [Obsolete("Use QuintInOut instead")]
    EaseInOutQuint = QuintInOut,
    
    /// <summary>Legacy alias for ExpoIn</summary>
    [Obsolete("Use ExpoIn instead")]
    EaseInExpo = ExpoIn,
    
    /// <summary>Legacy alias for ExpoOut</summary>
    [Obsolete("Use ExpoOut instead")]
    EaseOutExpo = ExpoOut,
    
    /// <summary>Legacy alias for ExpoInOut</summary>
    [Obsolete("Use ExpoInOut instead")]
    EaseInOutExpo = ExpoInOut,
    
    /// <summary>Legacy alias for BackIn</summary>
    [Obsolete("Use BackIn instead")]
    EaseInBack = BackIn,
    
    /// <summary>Legacy alias for BackOut</summary>
    [Obsolete("Use BackOut instead")]
    EaseOutBack = BackOut,
    
    /// <summary>Legacy alias for BackInOut</summary>
    [Obsolete("Use BackInOut instead")]
    EaseInOutBack = BackInOut,
    
    /// <summary>Legacy alias for ElasticIn</summary>
    [Obsolete("Use ElasticIn instead")]
    EaseInElastic = ElasticIn,
    
    /// <summary>Legacy alias for ElasticOut</summary>
    [Obsolete("Use ElasticOut instead")]
    EaseOutElastic = ElasticOut,
    
    /// <summary>Legacy alias for ElasticInOut</summary>
    [Obsolete("Use ElasticInOut instead")]
    EaseInOutElastic = ElasticInOut,
    
    /// <summary>Legacy alias for BounceIn</summary>
    [Obsolete("Use BounceIn instead")]
    EaseInBounce = BounceIn,
    
    /// <summary>Legacy alias for BounceOut</summary>
    [Obsolete("Use BounceOut instead")]
    EaseOutBounce = BounceOut,
    
    /// <summary>Legacy alias for BounceInOut</summary>
    [Obsolete("Use BounceInOut instead")]
    EaseInOutBounce = BounceInOut
}
