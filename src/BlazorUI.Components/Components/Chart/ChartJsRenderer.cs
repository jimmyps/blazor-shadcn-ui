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
    
    public ChartJsRenderer(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }
    
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
    
    public async Task ApplyThemeAsync(string chartId, ChartTheme theme)
    {
        if (_jsModule != null)
        {
            await _jsModule.InvokeVoidAsync("applyTheme", chartId, theme);
        }
    }
    
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
    
    public async Task DestroyAsync(string chartId)
    {
        if (_jsModule != null)
        {
            await _jsModule.InvokeVoidAsync("destroy", chartId);
        }
    }
    
    public async ValueTask DisposeAsync()
    {
        if (_jsModule != null)
        {
            await _jsModule.DisposeAsync();
        }
    }
}
