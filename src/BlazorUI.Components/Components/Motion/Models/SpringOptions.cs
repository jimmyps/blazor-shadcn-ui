namespace BlazorUI.Components.Motion;

/// <summary>
/// Spring physics configuration for natural, physics-based animations.
/// </summary>
public class SpringOptions
{
    /// <summary>
    /// Mass of the object (higher = slower movement).
    /// Default: 1.0
    /// </summary>
    public double Mass { get; set; } = 1.0;

    /// <summary>
    /// Stiffness of the spring (higher = faster, snappier).
    /// Default: 100
    /// </summary>
    public double Stiffness { get; set; } = 100;

    /// <summary>
    /// Damping ratio (higher = less oscillation).
    /// Default: 10
    /// </summary>
    public double Damping { get; set; } = 10;

    /// <summary>
    /// Velocity at the start of the animation.
    /// Default: 0
    /// </summary>
    public double Velocity { get; set; } = 0;

    /// <summary>
    /// Bounce amount (0 = no bounce, 1 = very bouncy).
    /// Alternative to specifying stiffness/damping directly.
    /// </summary>
    public double? Bounce { get; set; }

    /// <summary>
    /// Duration in seconds.
    /// When specified, spring will be constrained to this duration.
    /// </summary>
    public double? Duration { get; set; }
}
