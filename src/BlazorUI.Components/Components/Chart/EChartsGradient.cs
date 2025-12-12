using System.Text.Json.Serialization;

namespace BlazorUI.Components.Chart;

/// <summary>
/// Represents ECharts linear gradient configuration.
/// </summary>
public class EChartsLinearGradient
{
    /// <summary>
    /// Gets or sets the type of gradient (always "linear").
    /// </summary>
    [JsonPropertyName("type")]
    public string Type { get; set; } = "linear";
    
    /// <summary>
    /// Gets or sets the x coordinate of the start point (0-1).
    /// </summary>
    [JsonPropertyName("x")]
    public double X { get; set; } = 0;
    
    /// <summary>
    /// Gets or sets the y coordinate of the start point (0-1).
    /// </summary>
    [JsonPropertyName("y")]
    public double Y { get; set; } = 0;
    
    /// <summary>
    /// Gets or sets the x coordinate of the end point (0-1).
    /// </summary>
    [JsonPropertyName("x2")]
    public double X2 { get; set; } = 0;
    
    /// <summary>
    /// Gets or sets the y coordinate of the end point (0-1).
    /// </summary>
    [JsonPropertyName("y2")]
    public double Y2 { get; set; } = 1;
    
    /// <summary>
    /// Gets or sets the color stops for the gradient.
    /// </summary>
    [JsonPropertyName("colorStops")]
    public List<EChartsColorStop> ColorStops { get; set; } = new();
    
    /// <summary>
    /// Creates a vertical gradient (top to bottom).
    /// </summary>
    public static EChartsLinearGradient Vertical(params EChartsColorStop[] stops)
    {
        return new EChartsLinearGradient
        {
            X = 0, Y = 0, X2 = 0, Y2 = 1,
            ColorStops = new List<EChartsColorStop>(stops)
        };
    }
    
    /// <summary>
    /// Creates a horizontal gradient (left to right).
    /// </summary>
    public static EChartsLinearGradient Horizontal(params EChartsColorStop[] stops)
    {
        return new EChartsLinearGradient
        {
            X = 0, Y = 0, X2 = 1, Y2 = 0,
            ColorStops = new List<EChartsColorStop>(stops)
        };
    }
}

/// <summary>
/// Represents a color stop in a gradient.
/// </summary>
public class EChartsColorStop
{
    /// <summary>
    /// Gets or sets the offset position (0-1).
    /// </summary>
    [JsonPropertyName("offset")]
    public double Offset { get; set; }
    
    /// <summary>
    /// Gets or sets the color at this stop.
    /// </summary>
    [JsonPropertyName("color")]
    public string Color { get; set; } = string.Empty;
    
    /// <summary>
    /// Creates a new color stop.
    /// </summary>
    public EChartsColorStop() { }
    
    /// <summary>
    /// Creates a new color stop with the specified offset and color.
    /// </summary>
    public EChartsColorStop(double offset, string color)
    {
        Offset = offset;
        Color = color;
    }
}
