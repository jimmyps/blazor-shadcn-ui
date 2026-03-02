namespace BlazorUI.Components.Motion;

/// <summary>
/// Interface for motion preset components.
/// Presets provide predefined keyframes and options for common animations.
/// </summary>
public interface IMotionPreset
{
    /// <summary>
    /// Gets the keyframes for this preset.
    /// </summary>
    List<MotionKeyframe> GetKeyframes();

    /// <summary>
    /// Gets the animation options for this preset.
    /// </summary>
    MotionOptions? GetOptions();

    /// <summary>
    /// Gets the spring physics options for this preset (if applicable).
    /// </summary>
    SpringOptions? GetSpringOptions();
}
