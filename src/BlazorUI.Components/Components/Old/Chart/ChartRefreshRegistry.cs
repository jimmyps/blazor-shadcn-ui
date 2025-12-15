using System.Collections.Concurrent;

namespace BlazorUI.Components.Old.Chart;

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
        // Create tasks with error handling to ensure all charts are processed
        var tasks = _registeredCharts.Select(async kvp =>
        {
            try
            {
                await kvp.Value.RefreshAsync(kvp.Key);
            }
            catch (Exception ex)
            {
                // Log but don't throw - allow other charts to refresh
                Console.WriteLine($"Failed to refresh chart {kvp.Key}: {ex.Message}");
            }
        });
        
        await Task.WhenAll(tasks);
    }
}
