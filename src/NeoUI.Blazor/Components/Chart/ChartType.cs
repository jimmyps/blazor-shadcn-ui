using System.Text.Json;
using System.Text.Json.Serialization;

namespace NeoUI.Blazor.Charts;

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
    
    /// <summary>Area chart, a filled line chart.</summary>
    Area,
    /// <summary>Candlestick (OHLC) chart for financial data.</summary>
    Candlestick,
    /// <summary>Heatmap chart for value-intensity grids.</summary>
    Heatmap,
    /// <summary>Gauge chart for KPI/progress display.</summary>
    Gauge,
    /// <summary>Funnel chart for conversion pipeline visualization.</summary>
    Funnel,
    /// <summary>Composite/mixed chart allowing multiple series types.</summary>
    Composite,
    /// <summary>Radial bar chart (polar bar chart).</summary>
    RadialBar
}

/// <summary>
/// Custom JSON converter for ChartType enum that serializes to lowercase strings for Chart.js compatibility.
/// </summary>
public class ChartTypeJsonConverter : JsonConverter<ChartType>
{
    /// <summary>
    /// Reads and converts JSON to a ChartType value.
    /// </summary>
    /// <param name="reader">The JSON reader.</param>
    /// <param name="typeToConvert">The type to convert.</param>
    /// <param name="options">Serialization options.</param>
    /// <returns>The converted ChartType value.</returns>
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
            "scatter"      => ChartType.Scatter,
            "bubble"       => ChartType.Bubble,
            "area"         => ChartType.Area,
            "candlestick"  => ChartType.Candlestick,
            "heatmap"      => ChartType.Heatmap,
            "gauge"        => ChartType.Gauge,
            "funnel"       => ChartType.Funnel,
            "composite"    => ChartType.Composite,
            "radialbar"    => ChartType.RadialBar,
            _              => ChartType.Line
        };
    }

    /// <summary>
    /// Writes a ChartType value as JSON.
    /// </summary>
    /// <param name="writer">The JSON writer.</param>
    /// <param name="value">The ChartType value to write.</param>
    /// <param name="options">Serialization options.</param>
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
            ChartType.Scatter      => "scatter",
            ChartType.Bubble       => "bubble",
            ChartType.Area         => "line",
            ChartType.Candlestick  => "candlestick",
            ChartType.Heatmap      => "heatmap",
            ChartType.Gauge        => "gauge",
            ChartType.Funnel       => "funnel",
            ChartType.Composite    => "line",
            ChartType.RadialBar    => "bar",
            _                      => "line"
        };
        
        writer.WriteStringValue(stringValue);
    }
}
