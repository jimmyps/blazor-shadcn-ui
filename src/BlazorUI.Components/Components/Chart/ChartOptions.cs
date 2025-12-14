using System.Text.Json.Serialization;

namespace BlazorUI.Components.Chart;

/// <summary>
/// Strongly-typed chart options structure (universal format).
/// </summary>
public sealed class ChartOptions
{
    [JsonPropertyName("responsive")]
    public bool Responsive { get; init; } = true;
    
    [JsonPropertyName("maintainAspectRatio")]
    public bool MaintainAspectRatio { get; init; } = false;
    
    [JsonPropertyName("indexAxis")]
    public string? IndexAxis { get; init; }
    
    [JsonPropertyName("plugins")]
    public ChartPlugins Plugins { get; init; } = new();
    
    [JsonPropertyName("scales")]
    public ChartScales? Scales { get; init; }
    
    [JsonPropertyName("animation")]
    public ChartAnimationConfig? Animation { get; init; }
    
    [JsonPropertyName("cutout")]
    public string? Cutout { get; init; }
    
    /// <summary>
    /// VisualMap configuration for heat maps and other visual encoding.
    /// </summary>
    [JsonPropertyName("visualMap")]
    public VisualMapConfig? VisualMap { get; init; }
    
    /// <summary>
    /// Gauge-specific configuration.
    /// </summary>
    [JsonPropertyName("gauge")]
    public GaugeConfig? Gauge { get; init; }
    
    /// <summary>
    /// Radar coordinate system configuration.
    /// </summary>
    [JsonPropertyName("radar")]
    public RadarCoordinateConfig? Radar { get; init; }
    
    /// <summary>
    /// Dataset source for tabular data (alternative to labels + datasets pattern).
    /// </summary>
    [JsonPropertyName("dataset")]
    public DatasetConfig? Dataset { get; init; }
    
    /// <summary>
    /// Grid configuration with dashboard defaults.
    /// If not specified, dashboard defaults will be applied.
    /// </summary>
    [JsonPropertyName("grid")]
    public ChartGrid? Grid { get; init; }
    
    /// <summary>
    /// Legend configuration with dashboard defaults.
    /// If not specified, dashboard defaults will be applied.
    /// Note: Also accessible via Plugins.Legend for compatibility.
    /// </summary>
    [JsonPropertyName("legend")]
    public LegendConfig? Legend { get; init; }
    
    /// <summary>
    /// Tooltip configuration with dashboard defaults.
    /// If not specified, dashboard defaults will be applied.
    /// Note: Also accessible via Plugins.Tooltip for compatibility.
    /// </summary>
    [JsonPropertyName("tooltip")]
    public TooltipConfig? Tooltip { get; init; }
}

/// <summary>
/// Plugin configurations for charts.
/// </summary>
public sealed class ChartPlugins
{
    [JsonPropertyName("legend")]
    public LegendConfig Legend { get; init; } = new();
    
    [JsonPropertyName("tooltip")]
    public TooltipConfig Tooltip { get; init; } = new();
}

/// <summary>
/// Legend configuration with dashboard defaults.
/// </summary>
public sealed class LegendConfig
{
    [JsonPropertyName("display")]
    public bool Display { get; init; } = true;
    
    [JsonPropertyName("position")]
    public string? Position { get; init; }
    
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
public sealed class TooltipConfig
{
    [JsonPropertyName("enabled")]
    public bool Enabled { get; init; } = true;
    
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
/// Scales configuration (axes).
/// </summary>
public sealed class ChartScales
{
    [JsonPropertyName("x")]
    public AxisConfig? X { get; init; }
    
    [JsonPropertyName("y")]
    public AxisConfig? Y { get; init; }
    
    [JsonPropertyName("r")]
    public RadarAxisConfig? R { get; init; }
}

/// <summary>
/// Individual axis configuration.
/// </summary>
public sealed class AxisConfig
{
    [JsonPropertyName("display")]
    public bool Display { get; init; } = true;
    
    [JsonPropertyName("stacked")]
    public bool Stacked { get; init; } = false;
    
    [JsonPropertyName("grid")]
    public GridConfig Grid { get; init; } = new();
}

/// <summary>
/// Grid configuration for axes.
/// </summary>
public sealed class GridConfig
{
    [JsonPropertyName("display")]
    public bool Display { get; init; } = true;
}

/// <summary>
/// Radar axis configuration (for radar charts).
/// </summary>
public sealed class RadarAxisConfig
{
    [JsonPropertyName("beginAtZero")]
    public bool BeginAtZero { get; init; } = true;
    
    [JsonPropertyName("grid")]
    public GridConfig Grid { get; init; } = new();
    
    [JsonPropertyName("pointLabels")]
    public PointLabelsConfig PointLabels { get; init; } = new();
}

/// <summary>
/// Point labels configuration (for radar charts).
/// </summary>
public sealed class PointLabelsConfig
{
    [JsonPropertyName("display")]
    public bool Display { get; init; } = true;
}

/// <summary>
/// Animation configuration for charts.
/// </summary>
public sealed class ChartAnimationConfig
{
    [JsonPropertyName("duration")]
    public int Duration { get; init; } = 750;
    
    [JsonPropertyName("easing")]
    public AnimationEasing Easing { get; init; } = AnimationEasing.EaseInOutQuart;
}

/// <summary>
/// VisualMap configuration for heatmaps and continuous visual encoding.
/// </summary>
public sealed class VisualMapConfig
{
    [JsonPropertyName("min")]
    public double Min { get; init; }
    
    [JsonPropertyName("max")]
    public double Max { get; init; }
    
    [JsonPropertyName("calculable")]
    public bool Calculable { get; init; } = true;
    
    [JsonPropertyName("orient")]
    public string Orient { get; init; } = "vertical";
    
    [JsonPropertyName("left")]
    public string? Left { get; init; }
    
    [JsonPropertyName("bottom")]
    public string? Bottom { get; init; }
    
    [JsonPropertyName("inRange")]
    public VisualMapInRange? InRange { get; init; }
}

/// <summary>
/// Visual encoding range for VisualMap.
/// </summary>
public sealed class VisualMapInRange
{
    [JsonPropertyName("color")]
    public string[]? Color { get; init; }
}

/// <summary>
/// Gauge chart specific configuration.
/// </summary>
public sealed class GaugeConfig
{
    [JsonPropertyName("min")]
    public double Min { get; init; } = 0;
    
    [JsonPropertyName("max")]
    public double Max { get; init; } = 100;
    
    [JsonPropertyName("splitNumber")]
    public int SplitNumber { get; init; } = 10;
    
    [JsonPropertyName("axisLine")]
    public GaugeAxisLine? AxisLine { get; init; }
    
    [JsonPropertyName("detail")]
    public GaugeDetail? Detail { get; init; }
}

/// <summary>
/// Gauge axis line configuration.
/// </summary>
public sealed class GaugeAxisLine
{
    [JsonPropertyName("lineStyle")]
    public GaugeLineStyle? LineStyle { get; init; }
}

/// <summary>
/// Gauge line style configuration.
/// </summary>
public sealed class GaugeLineStyle
{
    [JsonPropertyName("width")]
    public int Width { get; init; } = 30;
    
    [JsonPropertyName("color")]
    public object? Color { get; init; } // Can be string or color stops array
}

/// <summary>
/// Gauge detail (value display) configuration.
/// </summary>
public sealed class GaugeDetail
{
    [JsonPropertyName("formatter")]
    public string? Formatter { get; init; }
    
    [JsonPropertyName("fontSize")]
    public int FontSize { get; init; } = 20;
}

/// <summary>
/// Radar coordinate system configuration (for radar charts).
/// </summary>
public sealed class RadarCoordinateConfig
{
    [JsonPropertyName("indicator")]
    public RadarIndicator[]? Indicator { get; init; }
    
    [JsonPropertyName("shape")]
    public string Shape { get; init; } = "polygon"; // "polygon" or "circle"
}

/// <summary>
/// Radar chart indicator (axis) configuration.
/// </summary>
public sealed class RadarIndicator
{
    [JsonPropertyName("name")]
    public string Name { get; init; } = string.Empty;
    
    [JsonPropertyName("max")]
    public double Max { get; init; }
    
    [JsonPropertyName("min")]
    public double? Min { get; init; }
}

/// <summary>
/// Dataset configuration for tabular data pattern.
/// </summary>
public sealed class DatasetConfig
{
    [JsonPropertyName("source")]
    public object? Source { get; init; } // Can be array of arrays or array of objects
    
    [JsonPropertyName("dimensions")]
    public string[]? Dimensions { get; init; }
}

/// <summary>
/// Encode configuration for mapping dataset columns to visual channels.
/// </summary>
public sealed class EncodeConfig
{
    [JsonPropertyName("x")]
    public string? X { get; init; }
    
    [JsonPropertyName("y")]
    public string? Y { get; init; }
    
    [JsonPropertyName("tooltip")]
    public string[]? Tooltip { get; init; }
    
    [JsonPropertyName("seriesName")]
    public string? SeriesName { get; init; }
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
