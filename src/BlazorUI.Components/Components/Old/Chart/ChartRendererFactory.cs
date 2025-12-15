using Microsoft.JSInterop;

namespace BlazorUI.Components.Old.Chart;

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
            ChartEngine.ECharts => new EChartsRenderer(jsRuntime),
            _ => new EChartsRenderer(jsRuntime) // Default to ECharts
        };
    }
}
