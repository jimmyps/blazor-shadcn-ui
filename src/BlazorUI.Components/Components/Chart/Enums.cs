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
/// Legend icon type for series symbols in the legend.
/// Maps to ECharts legend.icon property.
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum LegendIcon
{
    /// <summary>
    /// Circle icon.
    /// </summary>
    Circle,
    
    /// <summary>
    /// Rectangle icon.
    /// </summary>
    Rect,
    
    /// <summary>
    /// Rounded rectangle icon.
    /// </summary>
    RoundRect,
    
    /// <summary>
    /// Triangle icon.
    /// </summary>
    Triangle,
    
    /// <summary>
    /// Diamond icon.
    /// </summary>
    Diamond,
    
    /// <summary>
    /// Pin icon (droplet shape).
    /// </summary>
    Pin,
    
    /// <summary>
    /// Arrow icon.
    /// </summary>
    Arrow,
    
    /// <summary>
    /// No icon (empty space).
    /// </summary>
    None
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

/// <summary>
/// Label position for data labels on series.
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum LabelPosition
{
    /// <summary>
    /// Position label at top.
    /// </summary>
    Top,
    
    /// <summary>
    /// Position label at bottom.
    /// </summary>
    Bottom,
    
    /// <summary>
    /// Position label at left.
    /// </summary>
    Left,
    
    /// <summary>
    /// Position label at right.
    /// </summary>
    Right,
    
    /// <summary>
    /// Position label inside the data element.
    /// </summary>
    Inside,
    
    /// <summary>
    /// Position label inside at top.
    /// </summary>
    InsideTop,
    
    /// <summary>
    /// Position label inside at bottom.
    /// </summary>
    InsideBottom,
    
    /// <summary>
    /// Position label inside at left.
    /// </summary>
    InsideLeft,
    
    /// <summary>
    /// Position label inside at right.
    /// </summary>
    InsideRight,
    
    /// <summary>
    /// Position label at center.
    /// </summary>
    Center,
    
    /// <summary>
    /// Position label outside the data element.
    /// </summary>
    Outside
}

/// <summary>
/// Interpolation type for line and area series.
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum InterpolationType
{
    /// <summary>
    /// Natural/smooth curve interpolation (default).
    /// </summary>
    Natural,
    
    /// <summary>
    /// Linear interpolation (straight lines).
    /// </summary>
    Linear,
    
    /// <summary>
    /// Step interpolation (middle).
    /// </summary>
    Step,
    
    /// <summary>
    /// Step interpolation before the point.
    /// </summary>
    StepBefore,
    
    /// <summary>
    /// Step interpolation after the point.
    /// </summary>
    StepAfter,
    
    /// <summary>
    /// Monotone curve interpolation.
    /// </summary>
    Monotone
}

/// <summary>
/// Stack offset type for normalized stacking.
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum StackOffset
{
    /// <summary>
    /// No normalization (default stacking).
    /// </summary>
    None,
    
    /// <summary>
    /// Expand/normalize to 100% per X bucket.
    /// </summary>
    Expand
}

/// <summary>
/// Polar grid type for radial charts.
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum PolarGridType
{
    /// <summary>
    /// Circular grid lines.
    /// </summary>
    Circle,
    
    /// <summary>
    /// Polygon grid lines.
    /// </summary>
    Polygon
}

/// <summary>
/// Y axis position (left or right side of chart).
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum YAxisPosition
{
    /// <summary>
    /// Position axis on the left side (default).
    /// </summary>
    Left,
    
    /// <summary>
    /// Position axis on the right side.
    /// </summary>
    Right
}

/// <summary>
/// Line style/stroke type for grid lines, borders, etc.
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum LineStyleType
{
    /// <summary>
    /// Solid line (default).
    /// </summary>
    Solid,
    
    /// <summary>
    /// Dashed line.
    /// </summary>
    Dashed,
    
    /// <summary>
    /// Dotted line.
    /// </summary>
    Dotted
}

/// <summary>
/// Symbol/marker shape for line and scatter series.
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum SymbolShape
{
    /// <summary>
    /// Circle marker (default).
    /// </summary>
    Circle,
    
    /// <summary>
    /// Rectangle/square marker.
    /// </summary>
    Rect,
    
    /// <summary>
    /// Rounded rectangle marker.
    /// </summary>
    RoundRect,
    
    /// <summary>
    /// Triangle marker.
    /// </summary>
    Triangle,
    
    /// <summary>
    /// Diamond marker.
    /// </summary>
    Diamond,
    
    /// <summary>
    /// Pin/droplet marker.
    /// </summary>
    Pin,
    
    /// <summary>
    /// Arrow marker.
    /// </summary>
    Arrow,
    
    /// <summary>
    /// No marker/symbol.
    /// </summary>
    None
}
