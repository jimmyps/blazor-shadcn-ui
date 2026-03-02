using System.Text.Json;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace BlazorUI.Components.Chart;

/// <summary>
/// ECharts renderer implementation (SVG-based).
/// </summary>
public class EChartsRenderer : IChartRenderer
{
    private readonly IJSRuntime _jsRuntime;
    private IJSObjectReference? _jsModule;
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
    };
    
    /// <summary>
    /// Initializes a new instance of the <see cref="EChartsRenderer"/> class.
    /// </summary>
    /// <param name="jsRuntime">The JavaScript runtime for interop calls.</param>
    public EChartsRenderer(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }
    
    /// <summary>
    /// Initializes an ECharts chart instance with the specified configuration.
    /// </summary>
    /// <param name="element">The HTML element reference to render the chart.</param>
    /// <param name="config">The chart configuration.</param>
    /// <returns>A unique identifier for the created chart instance.</returns>
    public async Task<string> InitializeAsync(ElementReference element, ChartConfig config)
    {
        Console.WriteLine("[EChartsRenderer] InitializeAsync called");
        
        _jsModule ??= await _jsRuntime.InvokeAsync<IJSObjectReference>(
            "import", "./_content/NeoBlazorUI.Components/js/echarts-renderer.js");
        Console.WriteLine("[EChartsRenderer] JS module loaded");
        
        // Serialize config with camelCase to ensure JavaScript property names match ECharts expectations
        var json = JsonSerializer.Serialize(config, JsonOptions);
        Console.WriteLine($"[EChartsRenderer] Serialized config (first 500 chars): {json.Substring(0, Math.Min(500, json.Length))}");
        
        var normalizedConfig = JsonSerializer.Deserialize<object>(json, JsonOptions);
        
        var chartId = await _jsModule.InvokeAsync<string>("createChart", element, normalizedConfig);
        Console.WriteLine($"[EChartsRenderer] Chart created with ID: {chartId}");
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
            // Serialize with camelCase to match ECharts expectations
            var json = JsonSerializer.Serialize(data, JsonOptions);
            var normalizedData = JsonSerializer.Deserialize<object>(json, JsonOptions);
            
            await _jsModule.InvokeVoidAsync("updateData", chartId, normalizedData);
        }
    }
    
    /// <summary>
    /// Updates the configuration options of an existing chart.
    /// </summary>
    /// <param name="chartId">The unique identifier of the chart to update.</param>
    /// <param name="options">The new options to apply to the chart.</param>
    public async Task UpdateOptionsAsync(string chartId, object options)
    {
        Console.WriteLine($"[EChartsRenderer] UpdateOptionsAsync called for chart: {chartId}");
        
        if (_jsModule != null)
        {
            // Serialize with camelCase to match ECharts expectations
            var json = JsonSerializer.Serialize(options, JsonOptions);
            Console.WriteLine($"[EChartsRenderer] Serialized options (first 500 chars): {json.Substring(0, Math.Min(500, json.Length))}");
            
            var normalizedOptions = JsonSerializer.Deserialize<object>(json, JsonOptions);
            
            await _jsModule.InvokeVoidAsync("updateOptions", chartId, normalizedOptions);
            Console.WriteLine($"[EChartsRenderer] Options updated successfully");
        }
        else
        {
            Console.WriteLine("[EChartsRenderer] JS module not loaded, cannot update options");
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
            var echartsTheme = MapToEChartsTheme(theme);
            await _jsModule.InvokeVoidAsync("applyTheme", chartId, echartsTheme);
        }
    }
    
    /// <summary>
    /// Exports the chart as a base64-encoded image.
    /// </summary>
    /// <param name="chartId">The unique identifier of the chart to export.</param>
    /// <param name="format">The image format (PNG or SVG).</param>
    /// <returns>A base64-encoded image string.</returns>
    public async Task<string> ExportAsImageAsync(string chartId, ImageFormat format)
    {
        if (_jsModule != null)
        {
            var type = format == ImageFormat.Svg ? "svg" : "png";
            return await _jsModule.InvokeAsync<string>("exportImage", chartId, type);
        }
        
        return string.Empty;
    }
    
    /// <summary>
    /// Maps BlazorUI theme configuration to ECharts theme format.
    /// </summary>
    /// <param name="theme">The BlazorUI theme to convert.</param>
    /// <returns>An ECharts-compatible theme object.</returns>
    private object MapToEChartsTheme(ChartTheme theme)
    {
        // Map BlazorUI theme to ECharts theme JSON format
        return new
        {
            color = theme.ChartColors,
            backgroundColor = theme.Background,
            textStyle = new
            {
                color = theme.Foreground,
                fontFamily = theme.FontFamily
            },
            grid = new
            {
                borderColor = theme.Border
            },
            categoryAxis = new
            {
                axisLine = new { lineStyle = new { color = theme.Border } },
                axisTick = new { lineStyle = new { color = theme.Border } },
                axisLabel = new { color = theme.MutedForeground },
                splitLine = new { lineStyle = new { color = theme.Muted } }
            },
            valueAxis = new
            {
                axisLine = new { lineStyle = new { color = theme.Border } },
                axisTick = new { lineStyle = new { color = theme.Border } },
                axisLabel = new { color = theme.MutedForeground },
                splitLine = new { lineStyle = new { color = theme.Muted } }
            }
        };
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
