using Microsoft.AspNetCore.Components;

namespace BlazorUI.Components.Motion;

/// <summary>
/// Base class for motion preset components.
/// </summary>
public abstract class MotionPresetBase : ComponentBase, IMotionPreset
{
    [CascadingParameter]
    protected Motion? ParentMotion { get; set; }

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
}
