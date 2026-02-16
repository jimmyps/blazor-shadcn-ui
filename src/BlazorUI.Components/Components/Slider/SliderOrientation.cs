namespace BlazorUI.Components.Slider;

/// <summary>
/// Defines the orientation of a Slider component.
/// </summary>
/// <remarks>
/// The slider orientation determines the direction of the track and thumb movement.
/// </remarks>
public enum SliderOrientation
{
    /// <summary>
    /// Horizontal slider orientation (left to right).
    /// </summary>
    /// <remarks>
    /// The track runs horizontally and the thumb moves along the x-axis.
    /// Most common orientation for sliders.
    /// </remarks>
    Horizontal,

    /// <summary>
    /// Vertical slider orientation (bottom to top).
    /// </summary>
    /// <remarks>
    /// The track runs vertically and the thumb moves along the y-axis.
    /// Useful for volume controls or vertical ranges.
    /// </remarks>
    Vertical
}
