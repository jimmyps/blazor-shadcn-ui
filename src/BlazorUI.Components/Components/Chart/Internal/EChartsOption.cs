using System.Text.Json;
using System.Text.Json.Serialization;

namespace BlazorUI.Components.Chart.Internal;

/// <summary>
/// Root ECharts option object - represents the complete configuration for an ECharts instance.
/// This is a 1:1 representation of the ECharts option JSON structure.
/// </summary>
internal class EChartsOption
{
    /// <summary>
    /// Global color palette for the chart.
    /// </summary>
    [JsonPropertyName("color")]
    public List<string>? Color { get; set; }
    
    /// <summary>
    /// Background color of the chart.
    /// </summary>
    [JsonPropertyName("backgroundColor")]
    public string? BackgroundColor { get; set; }
    
    /// <summary>
    /// Text style configuration.
    /// </summary>
    [JsonPropertyName("textStyle")]
    public EChartsTextStyle? TextStyle { get; set; }
    
    /// <summary>
    /// Animation configuration.
    /// </summary>
    [JsonPropertyName("animation")]
    public bool? Animation { get; set; }
    
    /// <summary>
    /// Animation duration in milliseconds.
    /// </summary>
    [JsonPropertyName("animationDuration")]
    public int? AnimationDuration { get; set; }
    
    /// <summary>
    /// Animation easing function.
    /// </summary>
    [JsonPropertyName("animationEasing")]
    public string? AnimationEasing { get; set; }
    
    /// <summary>
    /// Grid configuration (positioning of the chart area).
    /// </summary>
    [JsonPropertyName("grid")]
    public EChartsGrid? Grid { get; set; }
    
    /// <summary>
    /// X-axis configuration (can be single or array).
    /// </summary>
    [JsonPropertyName("xAxis")]
    public object? XAxis { get; set; }
    
    /// <summary>
    /// Y-axis configuration (can be single or array).
    /// </summary>
    [JsonPropertyName("yAxis")]
    public object? YAxis { get; set; }
    
    /// <summary>
    /// Series data configuration.
    /// </summary>
    [JsonPropertyName("series")]
    public List<EChartsSeries>? Series { get; set; }
    
    /// <summary>
    /// Tooltip configuration.
    /// </summary>
    [JsonPropertyName("tooltip")]
    public EChartsTooltip? Tooltip { get; set; }
    
    /// <summary>
    /// Legend configuration.
    /// </summary>
    [JsonPropertyName("legend")]
    public EChartsLegend? Legend { get; set; }
    
    /// <summary>
    /// Radar chart coordinate system.
    /// </summary>
    [JsonPropertyName("radar")]
    public EChartsRadar? Radar { get; set; }
    
    /// <summary>
    /// Extension data for forward-compatibility with additional ECharts options.
    /// </summary>
    [JsonExtensionData]
    public Dictionary<string, JsonElement>? ExtensionData { get; set; }
}

/// <summary>
/// Text style configuration.
/// </summary>
internal class EChartsTextStyle
{
    [JsonPropertyName("fontFamily")]
    public string? FontFamily { get; set; }
    
    [JsonPropertyName("fontSize")]
    public int? FontSize { get; set; }
    
    [JsonPropertyName("color")]
    public string? Color { get; set; }
    
    [JsonExtensionData]
    public Dictionary<string, JsonElement>? ExtensionData { get; set; }
}

/// <summary>
/// Grid configuration for chart positioning.
/// </summary>
internal class EChartsGrid
{
    /// <summary>
    /// Distance from the left of the container.
    /// </summary>
    [JsonPropertyName("left")]
    public object? Left { get; set; }
    
    /// <summary>
    /// Distance from the top of the container.
    /// </summary>
    [JsonPropertyName("top")]
    public object? Top { get; set; }
    
    /// <summary>
    /// Distance from the right of the container.
    /// </summary>
    [JsonPropertyName("right")]
    public object? Right { get; set; }
    
    /// <summary>
    /// Distance from the bottom of the container.
    /// </summary>
    [JsonPropertyName("bottom")]
    public object? Bottom { get; set; }
    
    /// <summary>
    /// Whether to contain axis labels within the grid.
    /// </summary>
    [JsonPropertyName("containLabel")]
    public bool? ContainLabel { get; set; }
    
    [JsonExtensionData]
    public Dictionary<string, JsonElement>? ExtensionData { get; set; }
}

/// <summary>
/// Axis configuration (X or Y axis).
/// </summary>
internal class EChartsAxis
{
    /// <summary>
    /// Axis type: 'value', 'category', 'time', 'log'.
    /// </summary>
    [JsonPropertyName("type")]
    public string? Type { get; set; }
    
    /// <summary>
    /// Axis name/label.
    /// </summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }
    
    /// <summary>
    /// Category data for category axis.
    /// </summary>
    [JsonPropertyName("data")]
    public List<string>? Data { get; set; }
    
    /// <summary>
    /// Axis line configuration.
    /// </summary>
    [JsonPropertyName("axisLine")]
    public EChartsAxisLine? AxisLine { get; set; }
    
    /// <summary>
    /// Axis label configuration.
    /// </summary>
    [JsonPropertyName("axisLabel")]
    public EChartsAxisLabel? AxisLabel { get; set; }
    
    /// <summary>
    /// Split line (grid line) configuration.
    /// </summary>
    [JsonPropertyName("splitLine")]
    public EChartsSplitLine? SplitLine { get; set; }
    
    /// <summary>
    /// Axis tick configuration.
    /// </summary>
    [JsonPropertyName("axisTick")]
    public EChartsAxisTick? AxisTick { get; set; }
    
    [JsonExtensionData]
    public Dictionary<string, JsonElement>? ExtensionData { get; set; }
}

/// <summary>
/// Axis line configuration.
/// </summary>
internal class EChartsAxisLine
{
    [JsonPropertyName("show")]
    public bool? Show { get; set; }
    
    [JsonPropertyName("lineStyle")]
    public EChartsLineStyle? LineStyle { get; set; }
    
    [JsonExtensionData]
    public Dictionary<string, JsonElement>? ExtensionData { get; set; }
}

/// <summary>
/// Axis label configuration.
/// </summary>
internal class EChartsAxisLabel
{
    [JsonPropertyName("show")]
    public bool? Show { get; set; }
    
    /// <summary>
    /// Label formatter (string only, not function).
    /// </summary>
    [JsonPropertyName("formatter")]
    public string? Formatter { get; set; }
    
    [JsonPropertyName("color")]
    public string? Color { get; set; }
    
    [JsonExtensionData]
    public Dictionary<string, JsonElement>? ExtensionData { get; set; }
}

/// <summary>
/// Split line (grid line) configuration.
/// </summary>
internal class EChartsSplitLine
{
    [JsonPropertyName("show")]
    public bool? Show { get; set; }
    
    [JsonPropertyName("lineStyle")]
    public EChartsLineStyle? LineStyle { get; set; }
    
    [JsonExtensionData]
    public Dictionary<string, JsonElement>? ExtensionData { get; set; }
}

/// <summary>
/// Axis tick configuration.
/// </summary>
internal class EChartsAxisTick
{
    [JsonPropertyName("show")]
    public bool? Show { get; set; }
    
    [JsonExtensionData]
    public Dictionary<string, JsonElement>? ExtensionData { get; set; }
}

/// <summary>
/// Line style configuration.
/// </summary>
internal class EChartsLineStyle
{
    [JsonPropertyName("color")]
    public string? Color { get; set; }
    
    [JsonPropertyName("width")]
    public int? Width { get; set; }
    
    [JsonPropertyName("type")]
    public string? Type { get; set; }
    
    [JsonExtensionData]
    public Dictionary<string, JsonElement>? ExtensionData { get; set; }
}

/// <summary>
/// Series configuration.
/// </summary>
internal class EChartsSeries
{
    /// <summary>
    /// Series type: 'line', 'bar', 'pie', 'scatter', 'radar', etc.
    /// </summary>
    [JsonPropertyName("type")]
    public string? Type { get; set; }
    
    /// <summary>
    /// Series name (for legend).
    /// </summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }
    
    /// <summary>
    /// Series data.
    /// </summary>
    [JsonPropertyName("data")]
    public List<object>? Data { get; set; }
    
    /// <summary>
    /// Line/area smooth interpolation.
    /// </summary>
    [JsonPropertyName("smooth")]
    public bool? Smooth { get; set; }
    
    /// <summary>
    /// Whether to connect null values.
    /// </summary>
    [JsonPropertyName("connectNulls")]
    public bool? ConnectNulls { get; set; }
    
    /// <summary>
    /// Line style configuration.
    /// </summary>
    [JsonPropertyName("lineStyle")]
    public EChartsLineStyle? LineStyle { get; set; }
    
    /// <summary>
    /// Area style configuration (for area charts).
    /// </summary>
    [JsonPropertyName("areaStyle")]
    public EChartsAreaStyle? AreaStyle { get; set; }
    
    /// <summary>
    /// Item style configuration (points, bars, pie slices).
    /// </summary>
    [JsonPropertyName("itemStyle")]
    public EChartsItemStyle? ItemStyle { get; set; }
    
    /// <summary>
    /// Emphasis (hover) configuration.
    /// </summary>
    [JsonPropertyName("emphasis")]
    public EChartsEmphasis? Emphasis { get; set; }
    
    /// <summary>
    /// Label configuration.
    /// </summary>
    [JsonPropertyName("label")]
    public EChartsLabel? Label { get; set; }
    
    /// <summary>
    /// Stack group name (for stacked charts).
    /// </summary>
    [JsonPropertyName("stack")]
    public string? Stack { get; set; }
    
    /// <summary>
    /// Bar width.
    /// </summary>
    [JsonPropertyName("barWidth")]
    public object? BarWidth { get; set; }
    
    /// <summary>
    /// Bar max width.
    /// </summary>
    [JsonPropertyName("barMaxWidth")]
    public object? BarMaxWidth { get; set; }
    
    /// <summary>
    /// Show symbol (data point marker).
    /// </summary>
    [JsonPropertyName("showSymbol")]
    public bool? ShowSymbol { get; set; }
    
    /// <summary>
    /// Symbol type.
    /// </summary>
    [JsonPropertyName("symbol")]
    public string? Symbol { get; set; }
    
    /// <summary>
    /// Symbol size.
    /// </summary>
    [JsonPropertyName("symbolSize")]
    public object? SymbolSize { get; set; }
    
    [JsonExtensionData]
    public Dictionary<string, JsonElement>? ExtensionData { get; set; }
}

/// <summary>
/// Area style configuration.
/// </summary>
internal class EChartsAreaStyle
{
    [JsonPropertyName("color")]
    public object? Color { get; set; }
    
    [JsonPropertyName("opacity")]
    public double? Opacity { get; set; }
    
    [JsonExtensionData]
    public Dictionary<string, JsonElement>? ExtensionData { get; set; }
}

/// <summary>
/// Item style configuration.
/// </summary>
internal class EChartsItemStyle
{
    [JsonPropertyName("color")]
    public object? Color { get; set; }
    
    [JsonPropertyName("borderRadius")]
    public object? BorderRadius { get; set; }
    
    [JsonExtensionData]
    public Dictionary<string, JsonElement>? ExtensionData { get; set; }
}

/// <summary>
/// Emphasis (hover state) configuration.
/// </summary>
internal class EChartsEmphasis
{
    [JsonPropertyName("focus")]
    public string? Focus { get; set; }
    
    [JsonPropertyName("disabled")]
    public bool? Disabled { get; set; }
    
    [JsonPropertyName("itemStyle")]
    public EChartsItemStyle? ItemStyle { get; set; }
    
    [JsonExtensionData]
    public Dictionary<string, JsonElement>? ExtensionData { get; set; }
}

/// <summary>
/// Label configuration.
/// </summary>
internal class EChartsLabel
{
    [JsonPropertyName("show")]
    public bool? Show { get; set; }
    
    [JsonPropertyName("position")]
    public string? Position { get; set; }
    
    /// <summary>
    /// Label formatter (string only, not function).
    /// </summary>
    [JsonPropertyName("formatter")]
    public string? Formatter { get; set; }
    
    [JsonExtensionData]
    public Dictionary<string, JsonElement>? ExtensionData { get; set; }
}

/// <summary>
/// Tooltip configuration.
/// </summary>
internal class EChartsTooltip
{
    /// <summary>
    /// Whether to show tooltip.
    /// </summary>
    [JsonPropertyName("show")]
    public bool? Show { get; set; }
    
    /// <summary>
    /// Trigger type: 'item' or 'axis'.
    /// </summary>
    [JsonPropertyName("trigger")]
    public string? Trigger { get; set; }
    
    /// <summary>
    /// Axis pointer configuration (for axis trigger mode).
    /// </summary>
    [JsonPropertyName("axisPointer")]
    public EChartsAxisPointer? AxisPointer { get; set; }
    
    /// <summary>
    /// Tooltip formatter (string only, not function).
    /// </summary>
    [JsonPropertyName("formatter")]
    public string? Formatter { get; set; }
    
    [JsonExtensionData]
    public Dictionary<string, JsonElement>? ExtensionData { get; set; }
}

/// <summary>
/// Axis pointer configuration (for tooltip crosshair).
/// </summary>
internal class EChartsAxisPointer
{
    /// <summary>
    /// Pointer type: 'line', 'shadow', 'cross', 'none'.
    /// </summary>
    [JsonPropertyName("type")]
    public string? Type { get; set; }
    
    [JsonExtensionData]
    public Dictionary<string, JsonElement>? ExtensionData { get; set; }
}

/// <summary>
/// Legend configuration.
/// </summary>
internal class EChartsLegend
{
    /// <summary>
    /// Whether to show legend.
    /// </summary>
    [JsonPropertyName("show")]
    public bool? Show { get; set; }
    
    /// <summary>
    /// Legend orientation: 'horizontal' or 'vertical'.
    /// </summary>
    [JsonPropertyName("orient")]
    public string? Orient { get; set; }
    
    /// <summary>
    /// Distance from left (string only: 'left', 'center', 'right', or '20px', '10%').
    /// </summary>
    [JsonPropertyName("left")]
    public string? Left { get; set; }
    
    /// <summary>
    /// Distance from top (string only: 'top', 'middle', 'bottom', or '20px', '10%').
    /// </summary>
    [JsonPropertyName("top")]
    public string? Top { get; set; }
    
    /// <summary>
    /// Distance from right (string only).
    /// </summary>
    [JsonPropertyName("right")]
    public string? Right { get; set; }
    
    /// <summary>
    /// Distance from bottom (string only).
    /// </summary>
    [JsonPropertyName("bottom")]
    public string? Bottom { get; set; }
    
    [JsonExtensionData]
    public Dictionary<string, JsonElement>? ExtensionData { get; set; }
}

/// <summary>
/// Radar chart coordinate system.
/// </summary>
internal class EChartsRadar
{
    /// <summary>
    /// Radar indicators (axes).
    /// </summary>
    [JsonPropertyName("indicator")]
    public List<EChartsRadarIndicator>? Indicator { get; set; }
    
    [JsonExtensionData]
    public Dictionary<string, JsonElement>? ExtensionData { get; set; }
}

/// <summary>
/// Radar indicator configuration.
/// </summary>
internal class EChartsRadarIndicator
{
    [JsonPropertyName("name")]
    public string? Name { get; set; }
    
    [JsonPropertyName("max")]
    public double? Max { get; set; }
    
    [JsonPropertyName("min")]
    public double? Min { get; set; }
    
    [JsonExtensionData]
    public Dictionary<string, JsonElement>? ExtensionData { get; set; }
}

/// <summary>
/// Linear gradient configuration for ECharts.
/// </summary>
internal class EChartsLinearGradient
{
    /// <summary>
    /// Gradient type (always 'linear').
    /// </summary>
    [JsonPropertyName("type")]
    public string Type => "linear";
    
    /// <summary>
    /// X coordinate of the start point (0-1).
    /// </summary>
    [JsonPropertyName("x")]
    public double X { get; set; }
    
    /// <summary>
    /// Y coordinate of the start point (0-1).
    /// </summary>
    [JsonPropertyName("y")]
    public double Y { get; set; }
    
    /// <summary>
    /// X coordinate of the end point (0-1).
    /// </summary>
    [JsonPropertyName("x2")]
    public double X2 { get; set; }
    
    /// <summary>
    /// Y coordinate of the end point (0-1).
    /// </summary>
    [JsonPropertyName("y2")]
    public double Y2 { get; set; }
    
    /// <summary>
    /// Color stops defining the gradient.
    /// </summary>
    [JsonPropertyName("colorStops")]
    public List<EChartsColorStop> ColorStops { get; set; } = new();
    
    [JsonExtensionData]
    public Dictionary<string, JsonElement>? ExtensionData { get; set; }
}

/// <summary>
/// Color stop for gradients.
/// </summary>
internal class EChartsColorStop
{
    /// <summary>
    /// Position in the gradient (0-1).
    /// </summary>
    [JsonPropertyName("offset")]
    public double Offset { get; set; }
    
    /// <summary>
    /// Color at this position.
    /// </summary>
    [JsonPropertyName("color")]
    public string Color { get; set; } = string.Empty;
}
