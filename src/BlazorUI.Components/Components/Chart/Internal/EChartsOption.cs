using System.Text.Json;
using System.Text.Json.Serialization;

namespace BlazorUI.Components.Chart.Internal;

/// <summary>
/// Root ECharts option object - represents the complete configuration for an ECharts instance.
/// This is a 1:1 representation of the ECharts option JSON structure (MVP subset).
/// </summary>
public sealed class EChartsOption
{
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

    [JsonPropertyName("legend")]
    public EChartsLegend? Legend { get; set; }

    [JsonPropertyName("tooltip")]
    public EChartsTooltip? Tooltip { get; set; }

    [JsonPropertyName("radar")]
    public EChartsRadar? Radar { get; set; }

    [JsonPropertyName("series")]
    public List<EChartsSeries>? Series { get; set; }

    [JsonPropertyName("animation")]
    public bool? Animation { get; set; }

    [JsonPropertyName("animationDuration")]
    public int? AnimationDuration { get; set; }

    [JsonPropertyName("animationEasing")]
    public string? AnimationEasing { get; set; }

    [JsonExtensionData]
    public Dictionary<string, JsonElement>? ExtensionData { get; set; }
}

public sealed class EChartsGrid
{
    /// <summary>
    /// ECharts grid paddings accept numbers.
    /// </summary>
    [JsonPropertyName("top")]
    public int? Top { get; set; }

    [JsonPropertyName("right")]
    public int? Right { get; set; }

    [JsonPropertyName("bottom")]
    public int? Bottom { get; set; }

    [JsonPropertyName("left")]
    public int? Left { get; set; }

    [JsonPropertyName("containLabel")]
    public bool? ContainLabel { get; set; }
}

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

    [JsonPropertyName("axisLine")]
    public EChartsAxisLine? AxisLine { get; set; }

    [JsonPropertyName("axisTick")]
    public EChartsAxisTick? AxisTick { get; set; }

    [JsonPropertyName("axisLabel")]
    public EChartsAxisLabel? AxisLabel { get; set; }

    [JsonPropertyName("splitLine")]
    public EChartsSplitLine? SplitLine { get; set; }
}

public sealed class EChartsAxisLine
{
    [JsonPropertyName("show")]
    public bool? Show { get; set; }

    [JsonPropertyName("lineStyle")]
    public EChartsLineStyle? LineStyle { get; set; }
}

public sealed class EChartsAxisTick
{
    [JsonPropertyName("show")]
    public bool? Show { get; set; }

    [JsonPropertyName("lineStyle")]
    public EChartsLineStyle? LineStyle { get; set; }
}

public sealed class EChartsAxisLabel
{
    [JsonPropertyName("show")]
    public bool? Show { get; set; }

    [JsonPropertyName("color")]
    public string? Color { get; set; }

    /// <summary>
    /// MVP: string-only formatter (templates), no function formatters.
    /// </summary>
    [JsonPropertyName("formatter")]
    public string? Formatter { get; set; }
}

public sealed class EChartsSplitLine
{
    [JsonPropertyName("show")]
    public bool? Show { get; set; }

    [JsonPropertyName("lineStyle")]
    public EChartsLineStyle? LineStyle { get; set; }
}

public sealed class EChartsLineStyle
{
    [JsonPropertyName("width")]
    public int? Width { get; set; }

    /// <summary>
    /// Line type: "solid" | "dashed" | "dotted"
    /// </summary>
    [JsonPropertyName("type")]
    public string? Type { get; set; }

    [JsonPropertyName("color")]
    public string? Color { get; set; }
}

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
    /// Polymorphic by chart type.
    /// </summary>
    [JsonPropertyName("data")]
    public object? Data { get; set; }

    [JsonPropertyName("itemStyle")]
    public EChartsItemStyle? ItemStyle { get; set; }

    [JsonPropertyName("lineStyle")]
    public EChartsLineStyle? LineStyle { get; set; }

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
    /// Emphasis (hover state) configuration.
    /// </summary>
    [JsonPropertyName("emphasis")]
    public EChartsEmphasis? Emphasis { get; set; }

    [JsonExtensionData]
    public Dictionary<string, JsonElement>? ExtensionData { get; set; }
}

public sealed class EChartsEmphasis
{
    [JsonPropertyName("disabled")]
    public bool? Disabled { get; set; }

    /// <summary>
    /// Focus mode: "self" etc.
    /// </summary>
    [JsonPropertyName("focus")]
    public string? Focus { get; set; }
}

public sealed class EChartsItemStyle
{
    [JsonPropertyName("color")]
    public string? Color { get; set; }

    /// <summary>
    /// Bar radius: number or [tl,tr,br,bl]
    /// </summary>
    [JsonPropertyName("borderRadius")]
    public object? BorderRadius { get; set; }
}

public sealed class EChartsAreaStyle
{
    [JsonPropertyName("opacity")]
    public double? Opacity { get; set; }

    /// <summary>
    /// String color or gradient object.
    /// </summary>
    [JsonPropertyName("color")]
    public object? Color { get; set; }
}

public sealed class EChartsTooltip
{
    [JsonPropertyName("show")]
    public bool? Show { get; set; }

    /// <summary>
    /// Trigger type: "axis" | "item"
    /// </summary>
    [JsonPropertyName("trigger")]
    public string? Trigger { get; set; }

    [JsonPropertyName("axisPointer")]
    public EChartsAxisPointer? AxisPointer { get; set; }

    /// <summary>
    /// MVP: string-only formatter (templates), no function formatters.
    /// </summary>
    [JsonPropertyName("formatter")]
    public string? Formatter { get; set; }
}

public sealed class EChartsAxisPointer
{
    /// <summary>
    /// Pointer type: "line" | "shadow" | "cross" | "none"
    /// </summary>
    [JsonPropertyName("type")]
    public string? Type { get; set; }
}

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

    [JsonPropertyName("icon")]
    public string? Icon { get; set; }
}

public sealed class EChartsRadar
{
    [JsonPropertyName("indicator")]
    public List<EChartsRadarIndicator>? Indicator { get; set; }
}

public sealed class EChartsRadarIndicator
{
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("max")]
    public double? Max { get; set; }
}

public sealed class EChartsLinearGradient
{
    [JsonPropertyName("type")]
    public string Type { get; set; } = "linear";

    [JsonPropertyName("x")]
    public double X { get; set; }

    [JsonPropertyName("y")]
    public double Y { get; set; }

    [JsonPropertyName("x2")]
    public double X2 { get; set; }

    [JsonPropertyName("y2")]
    public double Y2 { get; set; }

    [JsonPropertyName("colorStops")]
    public List<EChartsColorStop> ColorStops { get; set; } = new();
}

public sealed class EChartsColorStop
{
    /// <summary>
    /// Position in the gradient (0..1).
    /// </summary>
    [JsonPropertyName("offset")]
    public double Offset { get; set; }

    [JsonPropertyName("color")]
    public string Color { get; set; } = "";
}
