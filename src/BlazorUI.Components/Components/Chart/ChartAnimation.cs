namespace BlazorUI.Components.Chart;

/// <summary>
/// Animation configuration for charts.
/// </summary>
/// <remarks>
/// Configures how charts animate on initial render and when data updates.
/// Automatically respects user's prefers-reduced-motion setting for accessibility.
/// </remarks>
public class ChartAnimation
{
    /// <summary>
    /// Gets or sets whether animations are enabled.
    /// </summary>
    public bool Enabled { get; set; } = true;
    
    /// <summary>
    /// Gets or sets the animation duration in milliseconds.
    /// </summary>
    public int Duration { get; set; } = 750;
    
    /// <summary>
    /// Gets or sets the animation easing function.
    /// </summary>
    public AnimationEasing Easing { get; set; } = AnimationEasing.EaseInOutQuart;
    
    /// <summary>
    /// Gets or sets the delay before animation starts in milliseconds.
    /// </summary>
    public int Delay { get; set; } = 0;
    
    /// <summary>
    /// Gets or sets the animation type for initial render.
    /// </summary>
    public AnimationType Type { get; set; } = AnimationType.Default;
}
