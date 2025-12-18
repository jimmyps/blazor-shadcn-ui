namespace BlazorUI.Components.Motion;

/// <summary>
/// Context for staggered animations.
/// Provides stagger delay information to child Motion components.
/// </summary>
public class MotionContext
{
    /// <summary>
    /// Delay to apply between child animations in seconds.
    /// </summary>
    public double StaggerDelay { get; set; }

    /// <summary>
    /// Index of the current child in the stagger sequence.
    /// </summary>
    public int ChildIndex { get; set; }
}
