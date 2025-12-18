namespace BlazorUI.Components.Motion;

/// <summary>
/// Configuration options for Motion animations.
/// </summary>
public class MotionOptions
{
    /// <summary>
    /// Duration of the animation in seconds.
    /// </summary>
    public double? Duration { get; set; }

    /// <summary>
    /// Delay before animation starts in seconds.
    /// </summary>
    public double? Delay { get; set; }

    /// <summary>
    /// Easing function for the animation.
    /// </summary>
    public MotionEasing? Easing { get; set; }

    /// <summary>
    /// Custom easing curve (cubic-bezier values).
    /// Only used when Easing is set to Custom.
    /// </summary>
    public double[]? CustomEasing { get; set; }

    /// <summary>
    /// Number of times to repeat the animation.
    /// Use -1 or double.PositiveInfinity for infinite loops.
    /// </summary>
    public double? Repeat { get; set; }

    /// <summary>
    /// Whether the animation should alternate direction on each repeat.
    /// </summary>
    public bool? RepeatReverse { get; set; }

    /// <summary>
    /// Delay between repeats in seconds.
    /// </summary>
    public double? RepeatDelay { get; set; }

    /// <summary>
    /// Fill mode for the animation ("forwards", "backwards", "both", "none").
    /// </summary>
    public string? Fill { get; set; }

    /// <summary>
    /// Direction of the animation ("normal", "reverse", "alternate", "alternate-reverse").
    /// </summary>
    public string? Direction { get; set; }
}
