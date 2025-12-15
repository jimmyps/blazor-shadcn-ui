namespace BlazorUI.Components.Old.Chart;

/// <summary>
/// Interface for chart components that can accept ChartStyle child components.
/// </summary>
public interface IChartStyleContainer
{
    /// <summary>
    /// Registers a ChartStyle component with this chart.
    /// </summary>
    void RegisterStyle(ChartStyle style);
}
