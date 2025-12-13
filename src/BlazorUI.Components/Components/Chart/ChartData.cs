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
