using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Components;

namespace BlazorUI.Components.Chart;

/// <summary>
/// Abstraction for chart rendering engines.
/// </summary>
public interface IChartRenderer : IAsyncDisposable
{
    /// <summary>
    /// Initializes the chart renderer with the target element.
    /// </summary>
    /// <param name="element">The HTML element reference to render into</param>
    /// <param name="config">Chart configuration object (format depends on renderer - ECharts v6 format for EChartsRenderer)</param>
    Task<string> InitializeAsync(ElementReference element, object config);
    
    /// <summary>
    /// Updates the chart with new data.
    /// </summary>
    Task UpdateDataAsync(string chartId, object data);
    
    /// <summary>
    /// Updates chart configuration without recreating the chart.
    /// </summary>
    Task UpdateOptionsAsync(string chartId, object options);
    
    /// <summary>
    /// Applies theme colors to the chart.
    /// </summary>
    Task ApplyThemeAsync(string chartId, ChartTheme theme);
    
    /// <summary>
    /// Exports the chart as an image (PNG/SVG).
    /// </summary>
    Task<string> ExportAsImageAsync(string chartId, ImageFormat format);
    
    /// <summary>
    /// Destroys the chart instance and cleans up resources.
    /// </summary>
    Task DestroyAsync(string chartId);
}

/// <summary>
/// Chart configuration passed to renderer.
/// </summary>
public class ChartConfig
{
    [JsonPropertyName("type")]
    public ChartType Type { get; set; }
    
    [JsonPropertyName("responsive")]
    public bool Responsive { get; set; } = true;
    
    [JsonPropertyName("maintainAspectRatio")]
    public bool MaintainAspectRatio { get; set; } = false;
    
    [JsonPropertyName("data")]
    public object Data { get; set; } = new { };
    
    [JsonPropertyName("options")]
    public object Options { get; set; } = new { };
}

/// <summary>
/// Image export format.
/// </summary>
public enum ImageFormat
{
    Png,
    Svg
}
