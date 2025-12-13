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
/// Legend configuration.
/// </summary>
public sealed class LegendConfig
{
    [JsonPropertyName("display")]
    public bool Display { get; init; } = true;
    
    [JsonPropertyName("position")]
    public string? Position { get; init; }
}

/// <summary>
/// Tooltip configuration.
/// </summary>
public sealed class TooltipConfig
{
    [JsonPropertyName("enabled")]
    public bool Enabled { get; init; } = true;
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
