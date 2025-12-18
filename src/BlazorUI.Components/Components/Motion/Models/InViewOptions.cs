namespace BlazorUI.Components.Motion;

/// <summary>
/// Configuration for IntersectionObserver-based animations.
/// </summary>
public class InViewOptions
{
    /// <summary>
    /// Threshold for triggering the animation (0-1).
    /// 0 = as soon as any part is visible.
    /// 1 = entire element must be visible.
    /// Default: 0.1
    /// </summary>
    public double Threshold { get; set; } = 0.1;

    /// <summary>
    /// Root margin for the IntersectionObserver.
    /// Similar to CSS margin (e.g., "0px 0px -100px 0px").
    /// </summary>
    public string? RootMargin { get; set; }

    /// <summary>
    /// Whether the animation should trigger only once.
    /// If false, animation triggers every time element enters viewport.
    /// Default: true
    /// </summary>
    public bool Once { get; set; } = true;

    /// <summary>
    /// Amount to offset the viewport intersection (in pixels or percentage).
    /// Positive values trigger earlier, negative values trigger later.
    /// </summary>
    public string? Offset { get; set; }
}
