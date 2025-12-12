namespace BlazorUI.Components.Chart;

/// <summary>
/// Specifies how multiple bar series are displayed.
/// </summary>
public enum BarChartMode
{
    /// <summary>
    /// Bars are displayed side by side (default).
    /// </summary>
    Grouped,
    
    /// <summary>
    /// Bars are stacked on top of each other.
    /// </summary>
    Stacked
}
