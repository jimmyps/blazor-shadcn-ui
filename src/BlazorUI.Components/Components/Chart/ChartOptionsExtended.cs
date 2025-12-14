using System.Text.Json.Serialization;

namespace BlazorUI.Components.Chart;

/// <summary>
/// Grid configuration with dashboard defaults.
/// </summary>
public sealed class ChartGrid
{
    /// <summary>
    /// Distance between grid component and the left side of the container.
    /// Default: 16 (dashboard-optimized)
    /// </summary>
    [JsonPropertyName("left")]
    public int? Left { get; init; } = 16;
    
    /// <summary>
    /// Distance between grid component and the right side of the container.
    /// Default: 16 (dashboard-optimized)
    /// </summary>
    [JsonPropertyName("right")]
    public int? Right { get; init; } = 16;
    
    /// <summary>
    /// Distance between grid component and the top of the container.
    /// Default: 32 (dashboard-optimized)
    /// </summary>
    [JsonPropertyName("top")]
    public int? Top { get; init; } = 32;
    
    /// <summary>
    /// Distance between grid component and the bottom of the container.
    /// Default: 24 (dashboard-optimized)
    /// </summary>
    [JsonPropertyName("bottom")]
    public int? Bottom { get; init; } = 24;
    
    /// <summary>
    /// Whether the grid region contains axis tick labels.
    /// Default: true (dashboard-optimized)
    /// </summary>
    [JsonPropertyName("containLabel")]
    public bool ContainLabel { get; init; } = true;
}

/// <summary>
/// Legend configuration with dashboard defaults.
/// </summary>
public sealed class ChartLegendExtended
{
    /// <summary>
    /// Whether to show the legend.
    /// </summary>
    [JsonPropertyName("show")]
    public bool Show { get; init; } = true;
    
    /// <summary>
    /// Distance from top of the container.
    /// Default: 4 (dashboard-optimized)
    /// </summary>
    [JsonPropertyName("top")]
    public int? Top { get; init; } = 4;
    
    /// <summary>
    /// Horizontal alignment.
    /// Default: "center" (dashboard-optimized)
    /// </summary>
    [JsonPropertyName("left")]
    public string? Left { get; init; } = "center";
    
    /// <summary>
    /// Legend icon shape.
    /// Default: Circle (dashboard-optimized)
    /// </summary>
    [JsonPropertyName("icon")]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public LegendIcon Icon { get; init; } = LegendIcon.Circle;
    
    /// <summary>
    /// Legend orientation.
    /// Default: Horizontal (dashboard-optimized)
    /// </summary>
    [JsonPropertyName("orient")]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public Orient Orient { get; init; } = Orient.Horizontal;
}

/// <summary>
/// Tooltip configuration with dashboard defaults.
/// </summary>
public sealed class ChartTooltipExtended
{
    /// <summary>
    /// Whether to show the tooltip.
    /// </summary>
    [JsonPropertyName("show")]
    public bool Show { get; init; } = true;
    
    /// <summary>
    /// Tooltip trigger type.
    /// Default: Axis for axis-based charts
    /// </summary>
    [JsonPropertyName("trigger")]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public TooltipTrigger? Trigger { get; init; }
    
    /// <summary>
    /// Axis pointer configuration.
    /// </summary>
    [JsonPropertyName("axisPointer")]
    public AxisPointerConfig? AxisPointer { get; init; }
    
    /// <summary>
    /// Custom formatter function (raw JavaScript).
    /// Example: "function (params) { return params.value; }"
    /// </summary>
    [JsonPropertyName("formatter")]
    [JsonConverter(typeof(JsRawJsonConverter))]
    public JsRaw? Formatter { get; init; }
}

/// <summary>
/// Axis pointer configuration for tooltips.
/// </summary>
public sealed class AxisPointerConfig
{
    /// <summary>
    /// Axis pointer type.
    /// Default: None (dashboard-optimized)
    /// </summary>
    [JsonPropertyName("type")]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public AxisPointerType Type { get; init; } = AxisPointerType.None;
}

/// <summary>
/// Axis configuration with dashboard defaults.
/// </summary>
public sealed class ChartAxisExtended
{
    /// <summary>
    /// Axis type.
    /// </summary>
    [JsonPropertyName("type")]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public AxisType? Type { get; init; }
    
    /// <summary>
    /// Whether to show the axis.
    /// </summary>
    [JsonPropertyName("show")]
    public bool Show { get; init; } = true;
    
    /// <summary>
    /// Category data for category axis.
    /// </summary>
    [JsonPropertyName("data")]
    public string[]? Data { get; init; }
    
    /// <summary>
    /// Axis line configuration.
    /// </summary>
    [JsonPropertyName("axisLine")]
    public AxisLineConfig? AxisLine { get; init; }
    
    /// <summary>
    /// Axis tick configuration.
    /// </summary>
    [JsonPropertyName("axisTick")]
    public AxisTickConfig? AxisTick { get; init; }
    
    /// <summary>
    /// Axis label configuration.
    /// </summary>
    [JsonPropertyName("axisLabel")]
    public AxisLabelConfig? AxisLabel { get; init; }
    
    /// <summary>
    /// Split line (grid line) configuration.
    /// </summary>
    [JsonPropertyName("splitLine")]
    public SplitLineConfig? SplitLine { get; init; }
    
    /// <summary>
    /// Whether to leave blank space at both ends of the axis.
    /// Important: false for line charts, true for bar charts.
    /// </summary>
    [JsonPropertyName("boundaryGap")]
    public bool? BoundaryGap { get; init; }
}

/// <summary>
/// Axis line configuration.
/// </summary>
public sealed class AxisLineConfig
{
    [JsonPropertyName("show")]
    public bool Show { get; init; } = true;
}

/// <summary>
/// Axis tick configuration.
/// </summary>
public sealed class AxisTickConfig
{
    [JsonPropertyName("show")]
    public bool Show { get; init; } = true;
}

/// <summary>
/// Axis label configuration with formatter support.
/// </summary>
public sealed class AxisLabelConfig
{
    /// <summary>
    /// Whether to show axis labels.
    /// </summary>
    [JsonPropertyName("show")]
    public bool Show { get; init; } = true;
    
    /// <summary>
    /// Custom formatter function (raw JavaScript).
    /// Example: "function (value) { return value + '%'; }"
    /// </summary>
    [JsonPropertyName("formatter")]
    [JsonConverter(typeof(JsRawJsonConverter))]
    public JsRaw? Formatter { get; init; }
    
    /// <summary>
    /// Whether to hide overlapping labels.
    /// </summary>
    [JsonPropertyName("hideOverlap")]
    public bool HideOverlap { get; init; } = true;
    
    /// <summary>
    /// Margin between label and axis line.
    /// </summary>
    [JsonPropertyName("margin")]
    public int? Margin { get; init; }
}

/// <summary>
/// Split line (grid line) configuration.
/// </summary>
public sealed class SplitLineConfig
{
    [JsonPropertyName("show")]
    public bool Show { get; init; } = true;
}

/// <summary>
/// Emphasis configuration for series items.
/// </summary>
public sealed class EmphasisConfig
{
    /// <summary>
    /// Whether emphasis is disabled.
    /// Default: true (disabled by default per spec)
    /// </summary>
    [JsonPropertyName("disabled")]
    public bool Disabled { get; init; } = true;
    
    /// <summary>
    /// Focus behavior when emphasized.
    /// </summary>
    [JsonPropertyName("focus")]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public EmphasisFocus? Focus { get; init; }
}
