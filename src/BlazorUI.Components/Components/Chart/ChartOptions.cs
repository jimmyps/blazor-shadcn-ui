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
    public AxisConfig X { get; init; } = new();
    
    [JsonPropertyName("y")]
    public AxisConfig Y { get; init; } = new();
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
/// Animation configuration for charts.
/// </summary>
public sealed class ChartAnimationConfig
{
    [JsonPropertyName("duration")]
    public int Duration { get; init; } = 750;
    
    [JsonPropertyName("easing")]
    public AnimationEasing Easing { get; init; } = AnimationEasing.EaseInOutQuart;
}
