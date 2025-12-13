using System.Text.Json.Serialization;

namespace BlazorUI.Components.Chart;

/// <summary>
/// Strongly-typed chart data structure (universal format).
/// </summary>
public sealed class ChartData
{
    [JsonPropertyName("labels")]
    public string[] Labels { get; init; } = Array.Empty<string>();
    
    [JsonPropertyName("datasets")]
    public ChartDataset[] Datasets { get; init; } = Array.Empty<ChartDataset>();
}

/// <summary>
/// Strongly-typed dataset within chart data.
/// </summary>
public sealed class ChartDataset
{
    [JsonPropertyName("label")]
    public string? Label { get; init; }
    
    [JsonPropertyName("data")]
    public double[] Data { get; init; } = Array.Empty<double>();
    
    [JsonPropertyName("borderColor")]
    public string? BorderColor { get; init; }
    
    [JsonPropertyName("backgroundColor")]
    public string? BackgroundColor { get; init; }
    
    [JsonPropertyName("borderWidth")]
    public int? BorderWidth { get; init; }
    
    [JsonPropertyName("tension")]
    public double? Tension { get; init; }
    
    [JsonPropertyName("fill")]
    public bool? Fill { get; init; }
    
    [JsonPropertyName("pointRadius")]
    public int? PointRadius { get; init; }
    
    [JsonPropertyName("pointHoverRadius")]
    public int? PointHoverRadius { get; init; }
    
    [JsonPropertyName("borderDash")]
    public int[]? BorderDash { get; init; }
    
    [JsonPropertyName("borderRadius")]
    public int? BorderRadius { get; init; }
    
    [JsonPropertyName("barThickness")]
    public int? BarThickness { get; init; }
    
    /// <summary>
    /// Optional gradient configuration for area fill.
    /// </summary>
    [JsonPropertyName("gradient")]
    public GradientConfig? Gradient { get; init; }
    
    /// <summary>
    /// Array of background colors (for pie charts with multiple slices).
    /// </summary>
    [JsonPropertyName("backgroundColors")]
    public string[]? BackgroundColors { get; init; }
    
    [JsonPropertyName("pointBackgroundColor")]
    public string? PointBackgroundColor { get; init; }
    
    [JsonPropertyName("pointBorderColor")]
    public string? PointBorderColor { get; init; }
    
    [JsonPropertyName("pointHoverBackgroundColor")]
    public string? PointHoverBackgroundColor { get; init; }
    
    [JsonPropertyName("pointHoverBorderColor")]
    public string? PointHoverBorderColor { get; init; }
    
    /// <summary>
    /// 2D scatter data for scatter and bubble charts (array of [x, y] or [x, y, size] arrays).
    /// </summary>
    [JsonPropertyName("scatterData")]
    public object[][]? ScatterData { get; init; }
    
    // === Pie Chart Specific Properties ===
    
    /// <summary>
    /// Name property for pie chart data items (used with Value for {name, value} format).
    /// </summary>
    [JsonPropertyName("name")]
    public string? Name { get; init; }
    
    /// <summary>
    /// Value property for pie chart data items (used with Name for {name, value} format).
    /// </summary>
    [JsonPropertyName("value")]
    public double? Value { get; init; }
    
    /// <summary>
    /// Radius of pie chart (can be percentage string like "70%" or array ["40%", "70%"] for donut).
    /// </summary>
    [JsonPropertyName("radius")]
    public object? Radius { get; init; }
    
    /// <summary>
    /// Center position of pie chart [x, y] as percentages (default: ["50%", "50%"]).
    /// </summary>
    [JsonPropertyName("center")]
    public string[]? Center { get; init; }
    
    /// <summary>
    /// Whether to avoid label overlap in pie charts.
    /// </summary>
    [JsonPropertyName("avoidLabelOverlap")]
    public bool? AvoidLabelOverlap { get; init; }
    
    // === Dataset + Encode Pattern ===
    
    /// <summary>
    /// Column/field name for encoding X-axis values in dataset pattern.
    /// </summary>
    [JsonPropertyName("encodeX")]
    public string? EncodeX { get; init; }
    
    /// <summary>
    /// Column/field name for encoding Y-axis values in dataset pattern.
    /// </summary>
    [JsonPropertyName("encodeY")]
    public string? EncodeY { get; init; }
}

/// <summary>
/// Gradient configuration for chart fills.
/// </summary>
public sealed class GradientConfig
{
    [JsonPropertyName("direction")]
    public string Direction { get; init; } = "Vertical";
    
    [JsonPropertyName("colorStops")]
    public GradientColorStop[] ColorStops { get; init; } = Array.Empty<GradientColorStop>();
}

/// <summary>
/// Color stop within a gradient (distinct from Style ColorStopConfig).
/// </summary>
public sealed class GradientColorStop
{
    [JsonPropertyName("offset")]
    public double Offset { get; init; }
    
    [JsonPropertyName("color")]
    public string Color { get; init; } = string.Empty;
}
