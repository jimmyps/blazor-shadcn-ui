namespace BlazorUI.Components.Chart;

/// <summary>
/// Specifies the rendering engine for charts.
/// </summary>
/// <remarks>
/// BlazorUI uses ECharts as the primary charting engine, providing:
/// - SVG-based rendering for high-quality output
/// - Native OKLCH color space support for modern theming
/// - Rich features and polished default styles
/// - Superior gradient, shadow, and animation capabilities
/// </remarks>
public enum ChartEngine
{
    /// <summary>
    /// ECharts engine - SVG-based rendering with rich features and modern design.
    /// Provides native OKLCH color support, polished themes, and advanced visualization capabilities.
    /// This is the primary and recommended engine for all BlazorUI charts.
    /// </summary>
    ECharts
}
