namespace BlazorUI.Components.Chart;

/// <summary>
/// Specifies the rendering engine for charts.
/// </summary>
/// <remarks>
/// Chart engines provide different rendering capabilities:
/// - ChartJs: Canvas-based rendering (default), fast and lightweight
/// - ECharts: SVG-based rendering, rich features and themeable
/// </remarks>
public enum ChartEngine
{
    /// <summary>
    /// Chart.js engine - Canvas-based rendering with excellent performance.
    /// Best for interactive dashboards and large datasets.
    /// </summary>
    ChartJs,
    
    /// <summary>
    /// ECharts engine - SVG-based rendering with rich features.
    /// Best for print-quality charts and complex visualizations.
    /// </summary>
    ECharts
}
