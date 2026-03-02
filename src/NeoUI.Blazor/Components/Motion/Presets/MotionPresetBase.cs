using Microsoft.AspNetCore.Components;

namespace BlazorUI.Components.Motion;

/// <summary>
/// Base class for motion preset components.
/// </summary>
public abstract class MotionPresetBase : ComponentBase, IMotionPreset
{
    /// <summary>
    /// Gets or sets the parent Motion component.
    /// </summary>
    [CascadingParameter]
    protected Motion? ParentMotion { get; set; }

    /// <summary>
    /// Easing function for the animation.
    /// Default is determined by each preset (commonly EaseOut).
    /// </summary>
    [Parameter]
    public MotionEasing? Easing { get; set; }

    /// <summary>
    /// Custom easing curve (cubic-bezier values).
    /// Only used when Easing is set to Custom.
    /// Example: new[] { 0.4, 0.0, 0.2, 1.0 }
    /// </summary>
    [Parameter]
    public double[]? CustomEasing { get; set; }

    /// <summary>
    /// Initializes the motion preset and registers it with the parent Motion component.
    /// </summary>
    protected override void OnInitialized()
    {
        // Register this preset with the parent Motion component
        if (ParentMotion != null)
        {
            ParentMotion.CollectedPresets.Add(this);
        }
    }

    /// <inheritdoc/>
    public abstract List<MotionKeyframe> GetKeyframes();

    /// <inheritdoc/>
    public virtual MotionOptions? GetOptions() => null;

    /// <inheritdoc/>
    public virtual SpringOptions? GetSpringOptions() => null;

    /// <summary>
    /// Helper method to get the effective easing value.
    /// Uses the parameter if set, otherwise returns the default.
    /// </summary>
    /// <param name="defaultEasing">The default easing to use if none is specified.</param>
    /// <returns>The effective easing value.</returns>
    protected MotionEasing GetEffectiveEasing(MotionEasing defaultEasing)
    {
        return Easing ?? defaultEasing;
    }

    /// <summary>
    /// Helper method to apply easing to motion options.
    /// Automatically maps easing enum values to cubic-bezier curves.
    /// </summary>
    /// <param name="options">The motion options to apply easing to.</param>
    /// <param name="defaultEasing">The default easing to use if none is specified.</param>
    protected void ApplyEasing(MotionOptions options, MotionEasing defaultEasing)
    {
        var effectiveEasing = GetEffectiveEasing(defaultEasing);
        options.Easing = effectiveEasing;

        // If Custom easing and user provided bezier, use it
        if (effectiveEasing == MotionEasing.Custom && CustomEasing != null)
        {
            options.CustomEasing = CustomEasing;
        }
        // Otherwise, auto-map easing to cubic-bezier
        else
        {
            var bezier = effectiveEasing.GetCubicBezier();
            if (bezier != null)
            {
                options.CustomEasing = bezier;
            }
        }
    }
}
