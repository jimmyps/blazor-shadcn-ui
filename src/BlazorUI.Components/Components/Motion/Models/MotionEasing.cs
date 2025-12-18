namespace BlazorUI.Components.Motion;

/// <summary>
/// Standard easing functions for animations.
/// </summary>
public enum MotionEasing
{
    /// <summary>
    /// Linear easing (no acceleration).
    /// </summary>
    Linear,

    /// <summary>
    /// Ease-in (slow start, fast end).
    /// </summary>
    EaseIn,

    /// <summary>
    /// Ease-out (fast start, slow end).
    /// </summary>
    EaseOut,

    /// <summary>
    /// Ease-in-out (slow start and end, fast middle).
    /// </summary>
    EaseInOut,

    /// <summary>
    /// Custom cubic bezier curve.
    /// </summary>
    Custom
}
