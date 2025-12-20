using Microsoft.AspNetCore.Components;

namespace BlazorUI.Components.Motion;

/// <summary>
/// Base class for motion preset components.
/// </summary>
public abstract class MotionPresetBase : ComponentBase, IMotionPreset
{
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
    protected MotionEasing GetEffectiveEasing(MotionEasing defaultEasing)
    {
        return Easing ?? defaultEasing;
    }

    /// <summary>
    /// Helper method to apply easing to motion options.
    /// Automatically maps easing enum values to cubic-bezier curves.
    /// </summary>
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
