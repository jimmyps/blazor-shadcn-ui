namespace BlazorUI.Components.Motion;

/// <summary>
/// Represents a single keyframe in an animation sequence.
/// </summary>
public class MotionKeyframe
{
    /// <summary>
    /// Opacity value (0-1).
    /// </summary>
    public double? Opacity { get; set; }

    /// <summary>
    /// Scale transformation.
    /// </summary>
    public double? Scale { get; set; }

    /// <summary>
    /// X-axis translation in pixels or percentage.
    /// </summary>
    public string? X { get; set; }

    /// <summary>
    /// Y-axis translation in pixels or percentage.
    /// </summary>
    public string? Y { get; set; }

    /// <summary>
    /// Rotation in degrees.
    /// </summary>
    public double? Rotate { get; set; }

    /// <summary>
    /// ScaleX transformation.
    /// </summary>
    public double? ScaleX { get; set; }

    /// <summary>
    /// ScaleY transformation.
    /// </summary>
    public double? ScaleY { get; set; }

    /// <summary>
    /// RotateX in degrees (3D rotation).
    /// </summary>
    public double? RotateX { get; set; }

    /// <summary>
    /// RotateY in degrees (3D rotation).
    /// </summary>
    public double? RotateY { get; set; }

    /// <summary>
    /// Z-axis translation (3D transform).
    /// </summary>
    public string? Z { get; set; }

    /// <summary>
    /// Skew transformation on X-axis in degrees.
    /// </summary>
    public double? SkewX { get; set; }

    /// <summary>
    /// Skew transformation on Y-axis in degrees.
    /// </summary>
    public double? SkewY { get; set; }

    /// <summary>
    /// Filter effects (e.g., "blur(5px)").
    /// </summary>
    public string? Filter { get; set; }

    /// <summary>
    /// Background color.
    /// </summary>
    public string? BackgroundColor { get; set; }

    /// <summary>
    /// Text color.
    /// </summary>
    public string? Color { get; set; }

    /// <summary>
    /// Border radius.
    /// </summary>
    public string? BorderRadius { get; set; }

    /// <summary>
    /// Width of the element.
    /// </summary>
    public string? Width { get; set; }

    /// <summary>
    /// Height of the element.
    /// </summary>
    public string? Height { get; set; }
}
