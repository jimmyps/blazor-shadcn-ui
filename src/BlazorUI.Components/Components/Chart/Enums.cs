using System.Text.Json.Serialization;

namespace BlazorUI.Components.Chart;

/// <summary>
/// Scale type for chart axes.
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum AxisScale
{
    /// <summary>
    /// Automatic scale detection.
    /// </summary>
    Auto,
    
    /// <summary>
    /// Category scale for discrete values.
    /// </summary>
    Category,
    
    /// <summary>
    /// Value scale for numeric data.
    /// </summary>
    Value,
    
    /// <summary>
    /// Time scale for temporal data.
    /// </summary>
    Time,
    
    /// <summary>
    /// Logarithmic scale.
    /// </summary>
    Log
}

/// <summary>
/// Tooltip trigger mode.
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum TooltipMode
{
    /// <summary>
    /// Tooltip triggered by axis (shows all series at that position).
    /// </summary>
    Axis,
    
    /// <summary>
    /// Tooltip triggered by individual data points.
    /// </summary>
    Item
}

/// <summary>
/// Tooltip axis pointer type (crosshair).
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum TooltipCursor
{
    /// <summary>
    /// No cursor/pointer.
    /// </summary>
    None,
    
    /// <summary>
    /// Single line cursor.
    /// </summary>
    Line,
    
    /// <summary>
    /// Cross cursor (both horizontal and vertical lines).
    /// </summary>
    Cross,
    
    /// <summary>
    /// Shadow/highlight region.
    /// </summary>
    Shadow
}

/// <summary>
/// Legend layout orientation.
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum LegendLayout
{
    /// <summary>
    /// Horizontal layout.
    /// </summary>
    Horizontal,
    
    /// <summary>
    /// Vertical layout.
    /// </summary>
    Vertical
}

/// <summary>
/// Legend horizontal alignment.
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum LegendAlign
{
    /// <summary>
    /// Align to the left.
    /// </summary>
    Left,
    
    /// <summary>
    /// Align to the center.
    /// </summary>
    Center,
    
    /// <summary>
    /// Align to the right.
    /// </summary>
    Right
}

/// <summary>
/// Legend vertical alignment.
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum LegendVerticalAlign
{
    /// <summary>
    /// Align to the top.
    /// </summary>
    Top,
    
    /// <summary>
    /// Align to the middle.
    /// </summary>
    Middle,
    
    /// <summary>
    /// Align to the bottom.
    /// </summary>
    Bottom
}

/// <summary>
/// Series emphasis/focus behavior.
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum Focus
{
    /// <summary>
    /// No focus effect (emphasis disabled).
    /// </summary>
    None,
    
    /// <summary>
    /// Focus on self only.
    /// </summary>
    Self
}

/// <summary>
/// Bar chart layout direction.
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum BarLayout
{
    /// <summary>
    /// Vertical bars (default).
    /// </summary>
    Vertical,
    
    /// <summary>
    /// Horizontal bars.
    /// </summary>
    Horizontal
}

/// <summary>
/// Gradient direction for linear gradients.
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum GradientDirection
{
    /// <summary>
    /// Top to bottom gradient.
    /// </summary>
    Vertical,
    
    /// <summary>
    /// Left to right gradient.
    /// </summary>
    Horizontal
}
