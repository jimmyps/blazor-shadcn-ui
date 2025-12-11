using System.Text.Json;
using System.Text.Json.Serialization;

namespace BlazorUI.Components.Chart;

/// <summary>
/// Specifies the type of chart to render.
/// </summary>
[JsonConverter(typeof(ChartTypeJsonConverter))]
public enum ChartType
{
    /// <summary>
    /// Line chart for showing trends over time or continuous data.
    /// </summary>
    Line,
    
    /// <summary>
    /// Bar chart for comparing discrete categories.
    /// </summary>
    Bar,
    
    /// <summary>
    /// Pie chart for showing composition of a whole.
    /// </summary>
    Pie,
    
    /// <summary>
    /// Donut chart, a variant of pie chart with a hole in the center.
    /// </summary>
    Donut,
    
    /// <summary>
    /// Radar chart for multivariate data.
    /// </summary>
    Radar,
    
    /// <summary>
    /// Scatter chart for showing correlation between two variables.
    /// </summary>
    Scatter,
    
    /// <summary>
    /// Bubble chart for showing three dimensions of data.
    /// </summary>
    Bubble,
    
    /// <summary>
    /// Area chart, a filled line chart.
    /// </summary>
    Area
}

/// <summary>
/// Custom JSON converter for ChartType enum that serializes to lowercase strings for Chart.js compatibility.
/// </summary>
public class ChartTypeJsonConverter : JsonConverter<ChartType>
{
    public override ChartType Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var value = reader.GetString();
        if (string.IsNullOrEmpty(value))
        {
            return ChartType.Line;
        }
        
        // Support both lowercase (Chart.js format) and PascalCase
        return value.ToLowerInvariant() switch
        {
            "line" => ChartType.Line,
            "bar" => ChartType.Bar,
            "pie" => ChartType.Pie,
            "donut" => ChartType.Donut,
            "doughnut" => ChartType.Donut, // Chart.js uses "doughnut"
            "radar" => ChartType.Radar,
            "scatter" => ChartType.Scatter,
            "bubble" => ChartType.Bubble,
            "area" => ChartType.Area,
            _ => ChartType.Line
        };
    }

    public override void Write(Utf8JsonWriter writer, ChartType value, JsonSerializerOptions options)
    {
        // Write as lowercase for Chart.js compatibility
        var stringValue = value switch
        {
            ChartType.Line => "line",
            ChartType.Bar => "bar",
            ChartType.Pie => "pie",
            ChartType.Donut => "doughnut", // Chart.js uses "doughnut"
            ChartType.Radar => "radar",
            ChartType.Scatter => "scatter",
            ChartType.Bubble => "bubble",
            ChartType.Area => "line", // Area is a line chart with fill
            _ => "line"
        };
        
        writer.WriteStringValue(stringValue);
    }
}
