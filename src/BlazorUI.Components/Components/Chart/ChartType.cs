using System.Text.Json.Serialization;

namespace BlazorUI.Components.Chart;

/// <summary>
/// Specifies the type of chart to render.
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
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
