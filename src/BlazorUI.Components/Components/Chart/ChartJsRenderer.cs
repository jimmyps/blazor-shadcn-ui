using System.Text.Json;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace BlazorUI.Components.Chart;

/// <summary>
/// Chart.js renderer implementation (canvas-based).
/// </summary>
public class ChartJsRenderer : IChartRenderer
{
    private readonly IJSRuntime _jsRuntime;
    private IJSObjectReference? _jsModule;
    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = false
    };
    
    /// <summary>
    /// Initializes a new instance of the <see cref="ChartJsRenderer"/> class.
    /// </summary>
    /// <param name="jsRuntime">The JavaScript runtime for interop calls.</param>
    public ChartJsRenderer(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }
    
    /// <summary>
    /// Initializes a Chart.js chart instance with the specified configuration.
    /// </summary>
    /// <param name="element">The HTML element reference to render the chart.</param>
    /// <param name="config">The chart configuration.</param>
    /// <returns>A unique identifier for the created chart instance.</returns>
    public async Task<string> InitializeAsync(ElementReference element, ChartConfig config)
    {
        _jsModule ??= await _jsRuntime.InvokeAsync<IJSObjectReference>(
            "import", "./_content/NeoBlazorUI.Components/js/chartjs-renderer.js");
        
        // Serialize config to JSON with camelCase to ensure proper property names in JS
        var json = JsonSerializer.Serialize(config, _jsonOptions);
        var configObj = JsonSerializer.Deserialize<object>(json, _jsonOptions);
        
        var chartId = await _jsModule.InvokeAsync<string>("createChart", element, configObj);
        return chartId;
    }
    
    /// <summary>
    /// Updates the data of an existing chart without recreating it.
    /// </summary>
    /// <param name="chartId">The unique identifier of the chart to update.</param>
    /// <param name="data">The new data to display in the chart.</param>
    public async Task UpdateDataAsync(string chartId, object data)
    {
        if (_jsModule != null)
        {
            // Serialize data to ensure camelCase properties for Chart.js
            var json = JsonSerializer.Serialize(data, _jsonOptions);
            var dataObj = JsonSerializer.Deserialize<object>(json, _jsonOptions);
            await _jsModule.InvokeVoidAsync("updateData", chartId, dataObj);
        }
    }
    
    /// <summary>
    /// Updates the configuration options of an existing chart.
    /// </summary>
    /// <param name="chartId">The unique identifier of the chart to update.</param>
    /// <param name="options">The new options to apply to the chart.</param>
    public async Task UpdateOptionsAsync(string chartId, object options)
    {
        if (_jsModule != null)
        {
            // Serialize options to ensure camelCase properties for Chart.js
            var json = JsonSerializer.Serialize(options, _jsonOptions);
            var optionsObj = JsonSerializer.Deserialize<object>(json, _jsonOptions);
            await _jsModule.InvokeVoidAsync("updateOptions", chartId, optionsObj);
        }
    }
    
    /// <summary>
    /// Applies theme colors and styles to an existing chart.
    /// </summary>
    /// <param name="chartId">The unique identifier of the chart to theme.</param>
    /// <param name="theme">The theme configuration to apply.</param>
    public async Task ApplyThemeAsync(string chartId, ChartTheme theme)
    {
        if (_jsModule != null)
        {
            await _jsModule.InvokeVoidAsync("applyTheme", chartId, theme);
        }
    }
    
    /// <summary>
    /// Exports the chart as a base64-encoded image.
    /// </summary>
    /// <param name="chartId">The unique identifier of the chart to export.</param>
    /// <param name="format">The image format (only PNG is supported).</param>
    /// <returns>A base64-encoded image string.</returns>
    /// <exception cref="NotSupportedException">Thrown when format is not PNG.</exception>
    public async Task<string> ExportAsImageAsync(string chartId, ImageFormat format)
    {
        if (format != ImageFormat.Png)
        {
            throw new NotSupportedException("Chart.js only supports PNG export");
        }
        
        if (_jsModule != null)
        {
            return await _jsModule.InvokeAsync<string>("toBase64Image", chartId);
        }
        
        return string.Empty;
    }
    
    /// <summary>
    /// Destroys a chart instance and releases its resources.
    /// </summary>
    /// <param name="chartId">The unique identifier of the chart to destroy.</param>
    public async Task DestroyAsync(string chartId)
    {
        if (_jsModule != null)
        {
            await _jsModule.InvokeVoidAsync("destroy", chartId);
        }
    }
    
    /// <summary>
    /// Disposes the renderer and releases JavaScript module resources.
    /// </summary>
    public async ValueTask DisposeAsync()
    {
        if (_jsModule != null)
        {
            await _jsModule.DisposeAsync();
        }
    }
}
