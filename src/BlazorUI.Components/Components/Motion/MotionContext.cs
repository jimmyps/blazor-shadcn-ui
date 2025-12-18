namespace BlazorUI.Components.Motion;

/// <summary>
/// Context for staggered animations.
/// Provides stagger delay information to child Motion components.
/// </summary>
public class MotionContext
{
    private int _nextChildIndex = 0;

    /// <summary>
    /// Delay to apply between child animations in seconds.
    /// </summary>
    public double StaggerDelay { get; set; }

    /// <summary>
    /// Index of the current child in the stagger sequence.
    /// This property is set by child components as they initialize.
    /// </summary>
    public int ChildIndex { get; set; }

    /// <summary>
    /// Gets the next child index and increments the counter.
    /// Used internally by child Motion components.
    /// </summary>
    internal int GetNextChildIndex()
    {
        return _nextChildIndex++;
    }
}
