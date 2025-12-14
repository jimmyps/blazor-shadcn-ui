using System.Collections.Concurrent;

namespace BlazorUI.Components.Chart;

/// <summary>
/// Registry service for managing and refreshing all charts globally.
/// Allows manual refresh of all registered charts (e.g., on theme change).
/// </summary>
public interface IChartRefreshRegistry
{
    /// <summary>
    /// Registers a chart instance for global refresh operations.
    /// </summary>
    void Register(string chartId, IChartRenderer renderer);
    
    /// <summary>
    /// Unregisters a chart instance from global refresh operations.
    /// </summary>
    void Unregister(string chartId);
    
    /// <summary>
    /// Refreshes all registered charts by re-resolving CSS variables.
    /// </summary>
    Task RefreshAllAsync();
}

/// <summary>
/// Default implementation of chart refresh registry.
/// </summary>
public class ChartRefreshRegistry : IChartRefreshRegistry
{
    private readonly ConcurrentDictionary<string, IChartRenderer> _registeredCharts = new();
    
    public void Register(string chartId, IChartRenderer renderer)
    {
        _registeredCharts.TryAdd(chartId, renderer);
    }
    
    public void Unregister(string chartId)
    {
        _registeredCharts.TryRemove(chartId, out _);
    }
    
    public async Task RefreshAllAsync()
    {
        var tasks = _registeredCharts.Select(kvp => 
            kvp.Value.RefreshAsync(kvp.Key)
        );
        
        await Task.WhenAll(tasks);
    }
}
