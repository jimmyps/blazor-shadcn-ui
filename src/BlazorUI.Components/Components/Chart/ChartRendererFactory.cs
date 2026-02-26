using Microsoft.JSInterop;

namespace BlazorUI.Components.Chart;

/// <summary>
/// Factory for creating chart renderers based on engine type.
/// </summary>
public static class ChartRendererFactory
{
    /// <summary>
    /// Creates a chart renderer instance for the specified engine.
    /// </summary>
    public static IChartRenderer CreateRenderer(ChartEngine engine, IJSRuntime jsRuntime)
    {
        return engine switch
        {
            ChartEngine.ChartJs => new ChartJsRenderer(jsRuntime),
            ChartEngine.ECharts => new EChartsRenderer(jsRuntime),
            _ => new ChartJsRenderer(jsRuntime) // Default to Chart.js
        };
    }
}
