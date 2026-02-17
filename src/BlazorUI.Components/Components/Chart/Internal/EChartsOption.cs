using System.Text.Json;
using System.Text.Json.Serialization;

namespace BlazorUI.Components.Chart.Internal;

/// <summary>
/// Root ECharts option object - represents the complete configuration for an ECharts instance.
/// This is a 1:1 representation of the ECharts option JSON structure (MVP subset).
/// </summary>
public sealed class EChartsOption
{
    /// <summary>
    /// Grid component configuration for positioning chart elements in the cartesian coordinate system.
    /// </summary>
    [JsonPropertyName("grid")]
    public EChartsGrid? Grid { get; set; }

    /// <summary>
    /// MVP: single axis object (not array).
    /// </summary>
    [JsonPropertyName("xAxis")]
    public EChartsAxis? XAxis { get; set; }

    /// <summary>
    /// MVP: single axis object (not array).
    /// </summary>
    [JsonPropertyName("yAxis")]
    public EChartsAxis? YAxis { get; set; }

    /// <summary>
    /// Legend component configuration for displaying series names and toggling series visibility.
    /// </summary>
    [JsonPropertyName("legend")]
    public EChartsLegend? Legend { get; set; }

    /// <summary>
    /// Tooltip component configuration for displaying data information on hover.
    /// </summary>
    [JsonPropertyName("tooltip")]
    public EChartsTooltip? Tooltip { get; set; }

    /// <summary>
    /// Radar coordinate system configuration for radar charts.
    /// </summary>
    [JsonPropertyName("radar")]
    public EChartsRadar? Radar { get; set; }

    /// <summary>
    /// Polar coordinate system configuration for radial charts.
    /// </summary>
    [JsonPropertyName("polar")]
    public EChartsPolar? PolarCoordinateSystem { get; set; }

    /// <summary>
    /// Angle axis for polar coordinate system.
    /// </summary>
    [JsonPropertyName("angleAxis")]
    public EChartsAxis? AngleAxis { get; set; }

    /// <summary>
    /// Radius axis for polar coordinate system.
    /// </summary>
    [JsonPropertyName("radiusAxis")]
    public EChartsAxis? RadiusAxis { get; set; }

    /// <summary>
    /// Series list containing one or more data series configurations.
    /// </summary>
    [JsonPropertyName("series")]
    public List<EChartsSeries>? Series { get; set; }

    /// <summary>
    /// Gets or sets whether to enable animations globally.
    /// </summary>
    [JsonPropertyName("animation")]
    public bool? Animation { get; set; }

    /// <summary>
    /// Gets or sets the global animation duration in milliseconds.
    /// </summary>
    [JsonPropertyName("animationDuration")]
    public int? AnimationDuration { get; set; }

    /// <summary>
    /// Gets or sets the global animation easing function.
    /// </summary>
    [JsonPropertyName("animationEasing")]
    public string? AnimationEasing { get; set; }

    /// <summary>
    /// Gets or sets the global animation delay in milliseconds.
    /// </summary>
    [JsonPropertyName("animationDelay")]
    public int? AnimationDelay { get; set; }

    /// <summary>
    /// Gets or sets the animation duration for data updates.
    /// </summary>
    [JsonPropertyName("animationDurationUpdate")]
    public int? AnimationDurationUpdate { get; set; }

    /// <summary>
    /// Gets or sets the animation easing for data updates.
    /// </summary>
    [JsonPropertyName("animationEasingUpdate")]
    public string? AnimationEasingUpdate { get; set; }

    /// <summary>
    /// Extension data for additional properties not explicitly defined.
    /// </summary>
    [JsonExtensionData]
    public Dictionary<string, JsonElement>? ExtensionData { get; set; }
}

/// <summary>
/// Grid component for positioning chart elements in the cartesian coordinate system.
/// </summary>
public sealed class EChartsGrid
{
    /// <summary>
    /// ECharts grid paddings accept numbers.
    /// </summary>
    [JsonPropertyName("top")]
    public int? Top { get; set; }

    /// <summary>
    /// Distance between grid component and the right side of the container.
    /// </summary>
    [JsonPropertyName("right")]
    public int? Right { get; set; }

    /// <summary>
    /// Distance between grid component and the bottom side of the container.
    /// </summary>
    [JsonPropertyName("bottom")]
    public int? Bottom { get; set; }

    /// <summary>
    /// Distance between grid component and the left side of the container.
    /// </summary>
    [JsonPropertyName("left")]
    public int? Left { get; set; }

    /// <summary>
    /// Whether to contain axis labels inside the grid. Useful to prevent labels from being cut off.
    /// </summary>
    [JsonPropertyName("containLabel")]
    public bool? ContainLabel { get; set; }
}

/// <summary>
/// Axis component configuration for cartesian, polar, radar coordinate systems.
/// </summary>
public sealed class EChartsAxis
{
    /// <summary>
    /// Axis type: "category" | "value" | "time" | "log"
    /// </summary>
    [JsonPropertyName("type")]
    public string? Type { get; set; }

    /// <summary>
    /// Labels for category axis.
    /// </summary>
    [JsonPropertyName("data")]
    public List<string>? Data { get; set; }

    /// <summary>
    /// Axis position: "left" | "right" | "top" | "bottom"
    /// </summary>
    [JsonPropertyName("position")]
    public string? Position { get; set; }

    /// <summary>
    /// Minimum value for the axis.
    /// </summary>
    [JsonPropertyName("min")]
    public double? Min { get; set; }

    /// <summary>
    /// Maximum value for the axis.
    /// </summary>
    [JsonPropertyName("max")]
    public double? Max { get; set; }

    /// <summary>
    /// Interval of axis tick. Maps to axis.interval in ECharts.
    /// </summary>
    [JsonPropertyName("interval")]
    public double? Interval { get; set; }
    
    /// <summary>
    /// Whether there is a gap on both sides of the axis.
    /// For category axes, true leaves space on both sides (useful for bar charts).
    /// For line/area charts, false is typically used to have data points at the edges.
    /// Maps to axis.boundaryGap in ECharts.
    /// </summary>
    [JsonPropertyName("boundaryGap")]
    public bool? BoundaryGap { get; set; }

    
    /// <summary>
    /// Start angle in degrees (for angle axis).
    /// </summary>
    [JsonPropertyName("startAngle")]
    public int? StartAngle { get; set; }
    
    /// <summary>
    /// End angle in degrees (for angle axis).
    /// </summary>
    [JsonPropertyName("endAngle")]
    public int? EndAngle { get; set; }
    
    /// <summary>
    /// Clockwise direction (for angle axis).
    /// </summary>
    [JsonPropertyName("clockwise")]
    public bool? Clockwise { get; set; }
    
    /// <summary>
    /// Split number (for radius axis).
    /// </summary>
    [JsonPropertyName("splitNumber")]
    public int? SplitNumber { get; set; }

    [JsonPropertyName("axisLine")]
    public EChartsAxisLine? AxisLine { get; set; }

    /// <summary>
    /// Axis tick configuration.
    /// </summary>
    [JsonPropertyName("axisTick")]
    public EChartsAxisTick? AxisTick { get; set; }

    /// <summary>
    /// Axis label configuration.
    /// </summary>
    [JsonPropertyName("axisLabel")]
    public EChartsAxisLabel? AxisLabel { get; set; }

    /// <summary>
    /// Split line configuration for grid lines.
    /// </summary>
    [JsonPropertyName("splitLine")]
    public EChartsSplitLine? SplitLine { get; set; }
}

/// <summary>
/// Axis line configuration.
/// </summary>
public sealed class EChartsAxisLine
{
    /// <summary>
    /// Whether to show the axis line.
    /// </summary>
    [JsonPropertyName("show")]
    public bool? Show { get; set; }

    /// <summary>
    /// Line style configuration for the axis line.
    /// </summary>
    [JsonPropertyName("lineStyle")]
    public EChartsLineStyle? LineStyle { get; set; }
}

/// <summary>
/// Axis tick mark configuration.
/// </summary>
public sealed class EChartsAxisTick
{
    /// <summary>
    /// Whether to show axis tick marks.
    /// </summary>
    [JsonPropertyName("show")]
    public bool? Show { get; set; }

    /// <summary>
    /// Line style configuration for tick marks.
    /// </summary>
    [JsonPropertyName("lineStyle")]
    public EChartsLineStyle? LineStyle { get; set; }
}

/// <summary>
/// Axis label configuration for text labels on axes.
/// </summary>
public sealed class EChartsAxisLabel
{
    /// <summary>
    /// Whether to show axis labels.
    /// </summary>
    [JsonPropertyName("show")]
    public bool? Show { get; set; }

    /// <summary>
    /// Text color for axis labels.
    /// </summary>
    [JsonPropertyName("color")]
    public string? Color { get; set; }

    /// <summary>
    /// Formatter string - can be either an ECharts template string or a JavaScript function expressed as a string.
    /// Examples:
    /// - Template: "{value}"
    /// - Function: "function(value) { return value + '%'; }"
    /// The JS layer interprets function strings and converts them to actual functions.
    /// </summary>
    [JsonPropertyName("formatter")]
    public string? Formatter { get; set; }

    /// <summary>
    /// Rotation angle for axis labels in degrees.
    /// </summary>
    [JsonPropertyName("rotate")]
    public int? Rotate { get; set; }

    /// <summary>
    /// Interval for displaying axis labels (e.g., 0 = all labels, 1 = every other label).
    /// </summary>
    [JsonPropertyName("interval")]
    public int? Interval { get; set; }

    /// <summary>
    /// Whether to display labels inside the chart area.
    /// </summary>
    [JsonPropertyName("inside")]
    public bool? Inside { get; set; }

    /// <summary>
    /// Margin between labels and axis line in pixels.
    /// </summary>
    [JsonPropertyName("margin")]
    public int? Margin { get; set; }

    /// <summary>
    /// Whether to hide overlapping labels automatically.
    /// </summary>
    [JsonPropertyName("hideOverlap")]
    public bool? HideOverlap { get; set; }

    /// <summary>
    /// Font size for axis labels in pixels.
    /// </summary>
    [JsonPropertyName("fontSize")]
    public int? FontSize { get; set; }

    /// <summary>
    /// Font family for axis labels.
    /// </summary>
    [JsonPropertyName("fontFamily")]
    public string? FontFamily { get; set; }

    /// <summary>
    /// Font weight for axis labels (e.g., "normal", "bold", "bolder", "lighter").
    /// </summary>
    [JsonPropertyName("fontWeight")]
    public string? FontWeight { get; set; }

    /// <summary>
    /// Line height for axis labels in pixels.
    /// </summary>
    [JsonPropertyName("lineHeight")]
    public int? LineHeight { get; set; }

    /// <summary>
    /// Horizontal alignment for axis labels (e.g., "left", "center", "right").
    /// </summary>
    [JsonPropertyName("align")]
    public string? Align { get; set; }

    /// <summary>
    /// Vertical alignment for axis labels (e.g., "top", "middle", "bottom").
    /// </summary>
    [JsonPropertyName("verticalAlign")]
    public string? VerticalAlign { get; set; }

    /// <summary>
    /// How to handle text overflow (e.g., "truncate", "break", "breakAll").
    /// </summary>
    [JsonPropertyName("overflow")]
    public string? Overflow { get; set; }

    /// <summary>
    /// Maximum width for axis labels in pixels.
    /// </summary>
    [JsonPropertyName("width")]
    public int? Width { get; set; }

    /// <summary>
    /// Ellipsis string to use when text is truncated.
    /// </summary>
    [JsonPropertyName("ellipsis")]
    public string? Ellipsis { get; set; }
}

/// <summary>
/// Split line configuration for grid lines on axes.
/// </summary>
public sealed class EChartsSplitLine
{
    /// <summary>
    /// Whether to show split lines.
    /// </summary>
    [JsonPropertyName("show")]
    public bool? Show { get; set; }

    /// <summary>
    /// Line style configuration for split lines.
    /// </summary>
    [JsonPropertyName("lineStyle")]
    public EChartsLineStyle? LineStyle { get; set; }
}

/// <summary>
/// Line style configuration for various line elements.
/// </summary>
public sealed class EChartsLineStyle
{
    /// <summary>
    /// Line width in pixels.
    /// </summary>
    [JsonPropertyName("width")]
    public int? Width { get; set; }

    /// <summary>
    /// Line type: "solid" | "dashed" | "dotted"
    /// </summary>
    [JsonPropertyName("type")]
    public string? Type { get; set; }

    /// <summary>
    /// String color or gradient object.
    /// </summary>
    [JsonPropertyName("color")]
    public object? Color { get; set; }

    /// <summary>
    /// Opacity (0.0 to 1.0).
    /// </summary>
    [JsonPropertyName("opacity")]
    public double? Opacity { get; set; }
}

/// <summary>
/// Series configuration for different chart types (line, bar, pie, scatter, radar).
/// </summary>
public sealed class EChartsSeries
{
    /// <summary>
    /// Series type: "line" | "bar" | "pie" | "scatter" | "radar"
    /// </summary>
    [JsonPropertyName("type")]
    public string? Type { get; set; }

    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>
    /// Coordinate system: "cartesian2d" (default) | "polar" | "geo" | "radar"
    /// </summary>
    [JsonPropertyName("coordinateSystem")]
    public string? CoordinateSystem { get; set; }

    /// <summary>
    /// Polymorphic by chart type.
    /// </summary>
    [JsonPropertyName("data")]
    public object? Data { get; set; }

    /// <summary>
    /// Item style configuration for data points, bars, or pie slices.
    /// </summary>
    [JsonPropertyName("itemStyle")]
    public EChartsItemStyle? ItemStyle { get; set; }

    /// <summary>
    /// Line style configuration for line series.
    /// </summary>
    [JsonPropertyName("lineStyle")]
    public EChartsLineStyle? LineStyle { get; set; }

    /// <summary>
    /// Area style configuration for area charts.
    /// </summary>
    [JsonPropertyName("areaStyle")]
    public EChartsAreaStyle? AreaStyle { get; set; }

    /// <summary>
    /// Line-specific: smooth interpolation.
    /// </summary>
    [JsonPropertyName("smooth")]
    public bool? Smooth { get; set; }

    /// <summary>
    /// Line-specific: show data point markers.
    /// </summary>
    [JsonPropertyName("showSymbol")]
    public bool? ShowSymbol { get; set; }

    /// <summary>
    /// Symbol/marker shape: "circle" | "rect" | "roundRect" | "triangle" | "diamond" | "pin" | "arrow" | "none"
    /// </summary>
    [JsonPropertyName("symbol")]
    public string? Symbol { get; set; }

    /// <summary>
    /// Symbol/marker size in pixels.
    /// </summary>
    [JsonPropertyName("symbolSize")]
    public int? SymbolSize { get; set; }
    
    /// <summary>
    /// JavaScript function string for dynamic symbol sizing.
    /// </summary>
    [JsonPropertyName("symbolSizeFunction")]
    public string? SymbolSizeFunction { get; set; }
    
    /// <summary>
    /// Symbol rotation in degrees.
    /// </summary>
    [JsonPropertyName("symbolRotate")]
    public int? SymbolRotate { get; set; }
    
    /// <summary>
    /// Enable large dataset mode.
    /// </summary>
    [JsonPropertyName("large")]
    public bool? Large { get; set; }
    
    /// <summary>
    /// Threshold for automatic large mode.
    /// </summary>
    [JsonPropertyName("largeThreshold")]
    public int? LargeThreshold { get; set; }
    
    /// <summary>
    /// Progressive rendering chunk size.
    /// </summary>
    [JsonPropertyName("progressive")]
    public int? Progressive { get; set; }
    
    /// <summary>
    /// Clip overflow points.
    /// </summary>
    [JsonPropertyName("clip")]
    public bool? Clip { get; set; }

    /// <summary>
    /// Stack group name (bar/area).
    /// </summary>
    [JsonPropertyName("stack")]
    public string? Stack { get; set; }

    /// <summary>
    /// Pie-specific: "70%" or ["50%","70%"]
    /// </summary>
    [JsonPropertyName("radius")]
    public object? Radius { get; set; }
    
    /// <summary>
    /// Start angle in degrees.
    /// </summary>
    [JsonPropertyName("startAngle")]
    public int? StartAngle { get; set; }
    
    /// <summary>
    /// End angle in degrees.
    /// </summary>
    [JsonPropertyName("endAngle")]
    public int? EndAngle { get; set; }
    
    /// <summary>
    /// Rose chart type: "radius", "area", or null.
    /// </summary>
    [JsonPropertyName("roseType")]
    public string? RoseType { get; set; }
    
    /// <summary>
    /// Padding angle between slices in degrees.
    /// </summary>
    [JsonPropertyName("padAngle")]
    public int? PadAngle { get; set; }
    
    /// <summary>
    /// Minimum angle for slice visibility.
    /// </summary>
    [JsonPropertyName("minAngle")]
    public int? MinAngle { get; set; }
    
    /// <summary>
    /// Center position [x, y].
    /// </summary>
    [JsonPropertyName("center")]
    public string[]? Center { get; set; }
    
    /// <summary>
    /// Selection mode: "single", "multiple", false.
    /// </summary>
    [JsonPropertyName("selectedMode")]
    public string? SelectedMode { get; set; }
    
    /// <summary>
    /// Selected slice offset distance.
    /// </summary>
    [JsonPropertyName("selectedOffset")]
    public int? SelectedOffset { get; set; }

    /// <summary>
    /// Emphasis (hover state) configuration.
    /// </summary>
    [JsonPropertyName("emphasis")]
    public EChartsEmphasis? Emphasis { get; set; }

    /// <summary>
    /// Label configuration for data points.
    /// </summary>
    [JsonPropertyName("label")]
    public EChartsLabel? Label { get; set; }

    /// <summary>
    /// Label line configuration (for pie charts).
    /// </summary>
    [JsonPropertyName("labelLine")]
    public EChartsLabelLine? LabelLine { get; set; }

    /// <summary>
    /// Step interpolation: true | "start" | "middle" | "end" | false
    /// For line/area charts.
    /// </summary>
    [JsonPropertyName("step")]
    public object? Step { get; set; }

    /// <summary>
    /// Gets or sets whether to enable animation for this series (overrides global).
    /// </summary>
    [JsonPropertyName("animation")]
    public bool? Animation { get; set; }

    /// <summary>
    /// Gets or sets the animation duration for this series (overrides global).
    /// </summary>
    [JsonPropertyName("animationDuration")]
    public int? AnimationDuration { get; set; }

    /// <summary>
    /// Gets or sets the animation easing for this series (overrides global).
    /// </summary>
    [JsonPropertyName("animationEasing")]
    public string? AnimationEasing { get; set; }

    /// <summary>
    /// Gets or sets the animation delay for this series (overrides global).
    /// </summary>
    [JsonPropertyName("animationDelay")]
    public int? AnimationDelay { get; set; }

    /// <summary>
    /// Gets or sets the animation duration for updates (overrides global).
    /// </summary>
    [JsonPropertyName("animationDurationUpdate")]
    public int? AnimationDurationUpdate { get; set; }

    /// <summary>
    /// Gets or sets the animation easing for updates (overrides global).
    /// </summary>
    [JsonPropertyName("animationEasingUpdate")]
    public string? AnimationEasingUpdate { get; set; }
    
    /// <summary>
    /// Show background (for radial bars).
    /// </summary>
    [JsonPropertyName("showBackground")]
    public bool? ShowBackground { get; set; }
    
    /// <summary>
    /// Background style.
    /// </summary>
    [JsonPropertyName("backgroundStyle")]
    public EChartsBackgroundStyle? BackgroundStyle { get; set; }

    /// <summary>
    /// Extension data for additional properties not explicitly defined.
    /// </summary>
    [JsonExtensionData]
    public Dictionary<string, JsonElement>? ExtensionData { get; set; }
}

/// <summary>
/// Emphasis (hover) state configuration for series items.
/// </summary>
public sealed class EChartsEmphasis
{
    /// <summary>
    /// Whether to disable emphasis (hover) effect.
    /// </summary>
    [JsonPropertyName("disabled")]
    public bool? Disabled { get; set; }

    /// <summary>
    /// Focus mode: "self", "series", "none", etc.
    /// </summary>
    [JsonPropertyName("focus")]
    public string? Focus { get; set; }

    /// <summary>
    /// Whether to scale the item on hover (pie charts).
    /// </summary>
    [JsonPropertyName("scale")]
    public bool? Scale { get; set; }

    /// <summary>
    /// Scale size in pixels when hovering (pie charts). Default is typically 5-10.
    /// </summary>
    [JsonPropertyName("scaleSize")]
    public int? ScaleSize { get; set; }

    /// <summary>
    /// Label configuration when item is emphasized (hovered).
    /// </summary>
    [JsonPropertyName("label")]
    public EChartsLabel? Label { get; set; }

    /// <summary>
    /// Item style when emphasized (colors, borders, shadows).
    /// </summary>
    [JsonPropertyName("itemStyle")]
    public EChartsItemStyle? ItemStyle { get; set; }
}

/// <summary>
/// Item style configuration for data points, bars, or pie slices.
/// </summary>
public sealed class EChartsItemStyle
{
    /// <summary>
    /// String color or gradient object.
    /// </summary>
    [JsonPropertyName("color")]
    public object? Color { get; set; }

    /// <summary>
    /// Bar radius: number or [tl,tr,br,bl]
    /// </summary>
    [JsonPropertyName("borderRadius")]
    public object? BorderRadius { get; set; }

    /// <summary>
    /// Opacity (0.0 to 1.0).
    /// </summary>
    [JsonPropertyName("opacity")]
    public double? Opacity { get; set; }
    
    /// <summary>
    /// Border color.
    /// </summary>
    [JsonPropertyName("borderColor")]
    public string? BorderColor { get; set; }
    
    /// <summary>
    /// Border width.
    /// </summary>
    [JsonPropertyName("borderWidth")]
    public int? BorderWidth { get; set; }
}

/// <summary>
/// Area style configuration for area charts.
/// </summary>
public sealed class EChartsAreaStyle
{
    /// <summary>
    /// Opacity (0.0 to 1.0).
    /// </summary>
    [JsonPropertyName("opacity")]
    public double? Opacity { get; set; }

    /// <summary>
    /// String color or gradient object.
    /// </summary>
    [JsonPropertyName("color")]
    public object? Color { get; set; }
}

/// <summary>
/// Label configuration for data points in series.
/// </summary>
public sealed class EChartsLabel
{
    /// <summary>
    /// Whether to show labels.
    /// </summary>
    [JsonPropertyName("show")]
    public bool? Show { get; set; }

    /// <summary>
    /// Label position: "top" | "bottom" | "left" | "right" | "inside" | "insideTop" | "insideBottom" | "insideLeft" | "insideRight" | "center" | "outside"
    /// </summary>
    [JsonPropertyName("position")]
    public string? Position { get; set; }

    /// <summary>
    /// Label formatter (template string or JS function string).
    /// </summary>
    [JsonPropertyName("formatter")]
    public string? Formatter { get; set; }

    /// <summary>
    /// Text color for labels.
    /// </summary>
    [JsonPropertyName("color")]
    public string? Color { get; set; }

    /// <summary>
    /// Font size for labels in pixels.
    /// </summary>
    [JsonPropertyName("fontSize")]
    public double? FontSize { get; set; }

    /// <summary>
    /// Offset distance for labels.
    /// </summary>
    [JsonPropertyName("offset")]
    public double? Offset { get; set; }
}

/// <summary>
/// Label line configuration for pie chart labels.
/// </summary>
public sealed class EChartsLabelLine
{
    /// <summary>
    /// Whether to show label lines.
    /// </summary>
    [JsonPropertyName("show")]
    public bool? Show { get; set; }

    /// <summary>
    /// Length of the first segment of the label line.
    /// </summary>
    [JsonPropertyName("length")]
    public double? Length { get; set; }

    /// <summary>
    /// Length of the second segment of the label line.
    /// </summary>
    [JsonPropertyName("length2")]
    public double? Length2 { get; set; }

    /// <summary>
    /// Smooth curve configuration.
    /// - false: straight lines
    /// - 0 to 1: numeric curve intensity (0.2 = slight, 0.5 = moderate, 1 = very curved)
    /// </summary>
    [JsonPropertyName("smooth")]
    public object? Smooth { get; set; }
}

/// <summary>
/// Tooltip configuration for displaying data information on hover.
/// </summary>
public sealed class EChartsTooltip
{
    /// <summary>
    /// Whether to show the tooltip.
    /// </summary>
    [JsonPropertyName("show")]
    public bool? Show { get; set; }

    /// <summary>
    /// Trigger type: "axis" | "item"
    /// </summary>
    [JsonPropertyName("trigger")]
    public string? Trigger { get; set; }

    /// <summary>
    /// Axis pointer configuration for tooltip.
    /// </summary>
    [JsonPropertyName("axisPointer")]
    public EChartsAxisPointer? AxisPointer { get; set; }

    /// <summary>
    /// Formatter string - can be either an ECharts template string or a JavaScript function expressed as a string.
    /// Examples:
    /// - Template: "{b}: {c}"
    /// - Function: "function(params) { return params.name + ': ' + params.value; }"
    /// The JS layer interprets function strings and converts them to actual functions.
    /// </summary>
    [JsonPropertyName("formatter")]
    public string? Formatter { get; set; }

    /// <summary>
    /// Background color for the tooltip.
    /// </summary>
    [JsonPropertyName("backgroundColor")]
    public string? BackgroundColor { get; set; }

    /// <summary>
    /// Border color for the tooltip.
    /// </summary>
    [JsonPropertyName("borderColor")]
    public string? BorderColor { get; set; }

    /// <summary>
    /// Border width for the tooltip in pixels.
    /// </summary>
    [JsonPropertyName("borderWidth")]
    public int? BorderWidth { get; set; }

    /// <summary>
    /// Text style configuration for tooltip text.
    /// </summary>
    [JsonPropertyName("textStyle")]
    public EChartsTextStyle? TextStyle { get; set; }
}

/// <summary>
/// Text style configuration for various text elements.
/// </summary>
public sealed class EChartsTextStyle
{
    /// <summary>
    /// Text color.
    /// </summary>
    [JsonPropertyName("color")]
    public string? Color { get; set; }
    
    /// <summary>
    /// Font size in pixels.
    /// </summary>
    [JsonPropertyName("fontSize")]
    public int? FontSize { get; set; }
    
    /// <summary>
    /// Font weight (e.g., "normal", "bold", "bolder", "lighter", or numeric values like "400", "700").
    /// </summary>
    [JsonPropertyName("fontWeight")]
    public string? FontWeight { get; set; }
    
    /// <summary>
    /// Font family.
    /// </summary>
    [JsonPropertyName("fontFamily")]
    public string? FontFamily { get; set; }
}

/// <summary>
/// Axis pointer configuration for tooltip and axis interaction.
/// </summary>
public sealed class EChartsAxisPointer
{
    /// <summary>
    /// Pointer type: "line" | "shadow" | "cross" | "none"
    /// </summary>
    [JsonPropertyName("type")]
    public string? Type { get; set; }
}

/// <summary>
/// Legend component configuration for displaying series names and toggling series visibility.
/// </summary>
public sealed class EChartsLegend
{
    [JsonPropertyName("show")]
    public bool? Show { get; set; }

    /// <summary>
    /// Legend orientation: "horizontal" | "vertical"
    /// </summary>
    [JsonPropertyName("orient")]
    public string? Orient { get; set; }

    /// <summary>
    /// MVP: string-only placement fields.
    /// "left" | "center" | "right" | "10" | "10%"
    /// </summary>
    [JsonPropertyName("left")]
    public string? Left { get; set; }

    /// <summary>
    /// MVP: string-only placement fields.
    /// "top" | "middle" | "bottom" | "4" | "10%"
    /// </summary>
    [JsonPropertyName("top")]
    public string? Top { get; set; }

    /// <summary>
    /// Icon shape for legend items (e.g., "circle", "rect", "roundRect", "triangle", "diamond").
    /// </summary>
    [JsonPropertyName("icon")]
    public string? Icon { get; set; }
    
    /// <summary>
    /// Text style configuration for legend labels.
    /// </summary>
    [JsonPropertyName("textStyle")]
    public EChartsTextStyle? TextStyle { get; set; }
}

/// <summary>
/// Radar coordinate system configuration for radar charts.
/// </summary>
public sealed class EChartsRadar
{
    /// <summary>
    /// Radar shape: "polygon" or "circle".
    /// </summary>
    [JsonPropertyName("shape")]
    public string? Shape { get; set; }
    
    /// <summary>
    /// Number of split levels.
    /// </summary>
    [JsonPropertyName("splitNumber")]
    public int? SplitNumber { get; set; }
    
    /// <summary>
    /// Radius.
    /// </summary>
    [JsonPropertyName("radius")]
    public string? Radius { get; set; }
    
    /// <summary>
    /// Center position [x, y].
    /// </summary>
    [JsonPropertyName("center")]
    public string[]? Center { get; set; }
    
    /// <summary>
    /// Radar indicator configuration (axes) for each dimension.
    /// </summary>
    [JsonPropertyName("indicator")]
    public List<EChartsRadarIndicator>? Indicator { get; set; }
    
    /// <summary>
    /// Axis name (indicator label) configuration.
    /// </summary>
    [JsonPropertyName("axisName")]
    public EChartsRadarAxisName? AxisName { get; set; }
    
    /// <summary>
    /// Axis line configuration.
    /// </summary>
    [JsonPropertyName("axisLine")]
    public EChartsAxisLine? AxisLine { get; set; }
    
    /// <summary>
    /// Split line configuration.
    /// </summary>
    [JsonPropertyName("splitLine")]
    public EChartsSplitLine? SplitLine { get; set; }
}

/// <summary>
/// Radar indicator configuration for a single axis dimension in radar charts.
/// </summary>
public sealed class EChartsRadarIndicator
{
    /// <summary>
    /// Name of the radar indicator.
    /// </summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }
    
    /// <summary>
    /// Minimum value.
    /// </summary>
    [JsonPropertyName("min")]
    public double? Min { get; set; }

    /// <summary>
    /// Maximum value for this indicator.
    /// </summary>
    [JsonPropertyName("max")]
    public double? Max { get; set; }
}

/// <summary>
/// Radar axis name (indicator label) configuration.
/// Controls the styling of indicator labels (e.g., "Coding", "Design") on radar charts.
/// </summary>
public sealed class EChartsRadarAxisName
{
    /// <summary>
    /// Whether to show the axis names (indicator labels).
    /// </summary>
    [JsonPropertyName("show")]
    public bool? Show { get; set; }
    
    /// <summary>
    /// Text color for axis names.
    /// </summary>
    [JsonPropertyName("color")]
    public string? Color { get; set; }
    
    /// <summary>
    /// Font size for axis names.
    /// </summary>
    [JsonPropertyName("fontSize")]
    public int? FontSize { get; set; }
    
    /// <summary>
    /// Font weight for axis names.
    /// </summary>
    [JsonPropertyName("fontWeight")]
    public string? FontWeight { get; set; }
    
    /// <summary>
    /// Font family for axis names.
    /// </summary>
    [JsonPropertyName("fontFamily")]
    public string? FontFamily { get; set; }
}

/// <summary>
/// Linear gradient configuration for colors.
/// </summary>
public sealed class EChartsLinearGradient
{
    /// <summary>
    /// Gradient type (always "linear" for linear gradients).
    /// </summary>
    [JsonPropertyName("type")]
    public string Type { get; set; } = "linear";

    /// <summary>
    /// Starting x coordinate (0.0 to 1.0).
    /// </summary>
    [JsonPropertyName("x")]
    public double X { get; set; }

    /// <summary>
    /// Starting y coordinate (0.0 to 1.0).
    /// </summary>
    [JsonPropertyName("y")]
    public double Y { get; set; }

    /// <summary>
    /// Ending x coordinate (0.0 to 1.0).
    /// </summary>
    [JsonPropertyName("x2")]
    public double X2 { get; set; }

    /// <summary>
    /// Ending y coordinate (0.0 to 1.0).
    /// </summary>
    [JsonPropertyName("y2")]
    public double Y2 { get; set; }

    /// <summary>
    /// Color stops defining the gradient colors and their positions.
    /// </summary>
    [JsonPropertyName("colorStops")]
    public List<EChartsColorStop> ColorStops { get; set; } = new();
}

/// <summary>
/// Color stop configuration for gradients.
/// </summary>
public sealed class EChartsColorStop
{
    /// <summary>
    /// Position in the gradient (0..1).
    /// </summary>
    [JsonPropertyName("offset")]
    public double Offset { get; set; }

    /// <summary>
    /// Color value at this stop.
    /// </summary>
    [JsonPropertyName("color")]
    public string Color { get; set; } = "";
}

/// <summary>
/// Polar coordinate system configuration for radial charts.
/// </summary>
public sealed class EChartsPolar
{
    /// <summary>
    /// Center position as [x, y] where x and y can be percentages or absolute values.
    /// </summary>
    [JsonPropertyName("center")]
    public object[]? Center { get; set; }

    /// <summary>
    /// Radius of the polar coordinate system (percentage or absolute value).
    /// </summary>
    [JsonPropertyName("radius")]
    public object? Radius { get; set; }
}

/// <summary>
/// Background style for radial bars.
/// </summary>
public sealed class EChartsBackgroundStyle
{
    /// <summary>
    /// Background color.
    /// </summary>
    [JsonPropertyName("color")]
    public string? Color { get; set; }
    
    /// <summary>
    /// Background opacity.
    /// </summary>
    [JsonPropertyName("opacity")]
    public double? Opacity { get; set; }
}
