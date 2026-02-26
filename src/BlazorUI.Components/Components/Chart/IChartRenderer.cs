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
    /// <param name="element">The HTML element reference to render the chart.</param>
    /// <param name="config">The chart configuration.</param>
    /// <returns>A unique identifier for the created chart instance.</returns>
    Task<string> InitializeAsync(ElementReference element, ChartConfig config);
    
    /// <summary>
    /// Updates the chart with new data.
    /// </summary>
    /// <param name="chartId">The unique identifier of the chart to update.</param>
    /// <param name="data">The new data to display in the chart.</param>
    Task UpdateDataAsync(string chartId, object data);
    
    /// <summary>
    /// Updates chart configuration without recreating the chart.
    /// </summary>
    /// <param name="chartId">The unique identifier of the chart to update.</param>
    /// <param name="options">The new options to apply to the chart.</param>
    Task UpdateOptionsAsync(string chartId, object options);
    
    /// <summary>
    /// Applies theme colors to the chart.
    /// </summary>
    /// <param name="chartId">The unique identifier of the chart to theme.</param>
    /// <param name="theme">The theme configuration to apply.</param>
    Task ApplyThemeAsync(string chartId, ChartTheme theme);
    
    /// <summary>
    /// Exports the chart as an image (PNG/SVG).
    /// </summary>
    /// <param name="chartId">The unique identifier of the chart to export.</param>
    /// <param name="format">The image format (PNG or SVG).</param>
    /// <returns>A base64-encoded image string.</returns>
    Task<string> ExportAsImageAsync(string chartId, ImageFormat format);
    
    /// <summary>
    /// Destroys the chart instance and cleans up resources.
    /// </summary>
    /// <param name="chartId">The unique identifier of the chart to destroy.</param>
    Task DestroyAsync(string chartId);
}

/// <summary>
/// Chart configuration passed to renderer.
/// </summary>
public class ChartConfig
{
    /// <summary>
    /// Gets or sets the type of chart to render.
    /// </summary>
    [JsonPropertyName("type")]
    public ChartType Type { get; set; }
    
    /// <summary>
    /// Gets or sets a value indicating whether the chart should be responsive.
    /// </summary>
    [JsonPropertyName("responsive")]
    public bool Responsive { get; set; } = true;
    
    /// <summary>
    /// Gets or sets a value indicating whether the chart should maintain its aspect ratio.
    /// </summary>
    [JsonPropertyName("maintainAspectRatio")]
    public bool MaintainAspectRatio { get; set; } = false;
    
    /// <summary>
    /// Gets or sets the data to display in the chart.
    /// </summary>
    [JsonPropertyName("data")]
    public object Data { get; set; } = new { };
    
    /// <summary>
    /// Gets or sets the configuration options for the chart.
    /// </summary>
    [JsonPropertyName("options")]
    public object Options { get; set; } = new { };
}

/// <summary>
/// Image export format.
/// </summary>
public enum ImageFormat
{
    /// <summary>
    /// Portable Network Graphics format.
    /// </summary>
    Png,
    
    /// <summary>
    /// Scalable Vector Graphics format.
    /// </summary>
    Svg
}
