namespace BlazorUI.Components.Motion;

/// <summary>
/// Defines when the animation should be triggered.
/// </summary>
public enum MotionTrigger
{
    /// <summary>
    /// Animation triggers when the component first appears.
    /// </summary>
    OnAppear,

    /// <summary>
    /// Animation triggers when the element enters the viewport.
    /// Uses IntersectionObserver to detect visibility.
    /// </summary>
    OnInView,

    /// <summary>
    /// Animation triggers manually via code.
    /// </summary>
    Manual
}
