using System.Text.Json.Serialization;

namespace BlazorUI.Components.Chart;

/// <summary>
/// Represents the complete ECharts option configuration with strongly-typed properties.
/// </summary>
public class EChartsOption
{
    /// <summary>
    /// Gets or sets the tooltip configuration.
    /// </summary>
    [JsonPropertyName("tooltip")]
    public EChartsTooltip? Tooltip { get; set; }
    
    /// <summary>
    /// Gets or sets the legend configuration.
    /// </summary>
    [JsonPropertyName("legend")]
    public EChartsLegend? Legend { get; set; }
    
    /// <summary>
    /// Gets or sets the X-axis configuration.
    /// </summary>
    [JsonPropertyName("xAxis")]
    public EChartsAxis? XAxis { get; set; }
    
    /// <summary>
    /// Gets or sets the Y-axis configuration.
    /// </summary>
    [JsonPropertyName("yAxis")]
    public EChartsAxis? YAxis { get; set; }
    
    /// <summary>
    /// Gets or sets the grid configuration.
    /// </summary>
    [JsonPropertyName("grid")]
    public EChartsGrid? Grid { get; set; }
    
    /// <summary>
    /// Gets or sets the series data.
    /// </summary>
    [JsonPropertyName("series")]
    public List<EChartsSeries> Series { get; set; } = new();
    
    /// <summary>
    /// Gets or sets whether animation is enabled.
    /// </summary>
    [JsonPropertyName("animation")]
    public bool? Animation { get; set; }
    
    /// <summary>
    /// Gets or sets the animation duration in milliseconds.
    /// </summary>
    [JsonPropertyName("animationDuration")]
    public int? AnimationDuration { get; set; }
    
    /// <summary>
    /// Gets or sets the animation easing function.
    /// </summary>
    [JsonPropertyName("animationEasing")]
    public string? AnimationEasing { get; set; }
}

/// <summary>
/// Represents tooltip configuration.
/// </summary>
public class EChartsTooltip
{
    /// <summary>
    /// Gets or sets the trigger type ("item", "axis", "none").
    /// </summary>
    [JsonPropertyName("trigger")]
    public string? Trigger { get; set; }
    
    /// <summary>
    /// Gets or sets the axis pointer configuration.
    /// </summary>
    [JsonPropertyName("axisPointer")]
    public EChartsAxisPointer? AxisPointer { get; set; }
    
    /// <summary>
    /// Gets or sets the background color.
    /// </summary>
    [JsonPropertyName("backgroundColor")]
    public string? BackgroundColor { get; set; }
    
    /// <summary>
    /// Gets or sets the border color.
    /// </summary>
    [JsonPropertyName("borderColor")]
    public string? BorderColor { get; set; }
    
    /// <summary>
    /// Gets or sets the border width.
    /// </summary>
    [JsonPropertyName("borderWidth")]
    public int? BorderWidth { get; set; }
}

/// <summary>
/// Represents axis pointer configuration for tooltips.
/// </summary>
public class EChartsAxisPointer
{
    /// <summary>
    /// Gets or sets the type ("line", "shadow", "cross", "none").
    /// </summary>
    [JsonPropertyName("type")]
    public string? Type { get; set; }
}

/// <summary>
/// Represents legend configuration.
/// </summary>
public class EChartsLegend
{
    /// <summary>
    /// Gets or sets whether the legend is shown.
    /// </summary>
    [JsonPropertyName("show")]
    public bool? Show { get; set; }
    
    /// <summary>
    /// Gets or sets the legend data (series names).
    /// </summary>
    [JsonPropertyName("data")]
    public List<string>? Data { get; set; }
    
    /// <summary>
    /// Gets or sets the top position.
    /// </summary>
    [JsonPropertyName("top")]
    public string? Top { get; set; }
    
    /// <summary>
    /// Gets or sets the left position.
    /// </summary>
    [JsonPropertyName("left")]
    public string? Left { get; set; }
    
    /// <summary>
    /// Gets or sets the right position.
    /// </summary>
    [JsonPropertyName("right")]
    public string? Right { get; set; }
    
    /// <summary>
    /// Gets or sets the bottom position.
    /// </summary>
    [JsonPropertyName("bottom")]
    public string? Bottom { get; set; }
    
    /// <summary>
    /// Gets or sets the orientation ("horizontal", "vertical").
    /// </summary>
    [JsonPropertyName("orient")]
    public string? Orient { get; set; }
}

/// <summary>
/// Represents axis configuration.
/// </summary>
public class EChartsAxis
{
    /// <summary>
    /// Gets or sets the axis type ("category", "value", "time", "log").
    /// </summary>
    [JsonPropertyName("type")]
    public string? Type { get; set; }
    
    /// <summary>
    /// Gets or sets the axis data (for category type).
    /// </summary>
    [JsonPropertyName("data")]
    public List<string>? Data { get; set; }
    
    /// <summary>
    /// Gets or sets the axis name.
    /// </summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }
    
    /// <summary>
    /// Gets or sets whether to use boundary gap.
    /// </summary>
    [JsonPropertyName("boundaryGap")]
    public bool? BoundaryGap { get; set; }
    
    /// <summary>
    /// Gets or sets the axis line style.
    /// </summary>
    [JsonPropertyName("axisLine")]
    public EChartsAxisLine? AxisLine { get; set; }
}

/// <summary>
/// Represents axis line style.
/// </summary>
public class EChartsAxisLine
{
    /// <summary>
    /// Gets or sets the line style.
    /// </summary>
    [JsonPropertyName("lineStyle")]
    public EChartsLineStyle? LineStyle { get; set; }
}

/// <summary>
/// Represents grid configuration.
/// </summary>
public class EChartsGrid
{
    /// <summary>
    /// Gets or sets the left position.
    /// </summary>
    [JsonPropertyName("left")]
    public string? Left { get; set; }
    
    /// <summary>
    /// Gets or sets the right position.
    /// </summary>
    [JsonPropertyName("right")]
    public string? Right { get; set; }
    
    /// <summary>
    /// Gets or sets the top position.
    /// </summary>
    [JsonPropertyName("top")]
    public string? Top { get; set; }
    
    /// <summary>
    /// Gets or sets the bottom position.
    /// </summary>
    [JsonPropertyName("bottom")]
    public string? Bottom { get; set; }
    
    /// <summary>
    /// Gets or sets whether to contain labels.
    /// </summary>
    [JsonPropertyName("containLabel")]
    public bool? ContainLabel { get; set; }
}
