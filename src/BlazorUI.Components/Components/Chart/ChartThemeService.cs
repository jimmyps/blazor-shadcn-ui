using Microsoft.JSInterop;

namespace BlazorUI.Components.Chart;

/// <summary>
/// Service for managing chart themes and coordinating theme changes across all charts.
/// </summary>
public class ChartThemeService : IAsyncDisposable
{
    private readonly IJSRuntime _jsRuntime;
    private readonly List<IChartRenderer> _renderers = new();
    private IJSObjectReference? _moduleRef;
    private bool _disposed;
    
    public ChartThemeService(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }
    
    /// <summary>
    /// Registers a chart renderer with the theme service.
    /// </summary>
    public void RegisterRenderer(IChartRenderer renderer)
    {
        if (!_renderers.Contains(renderer))
        {
            _renderers.Add(renderer);
        }
    }
    
    /// <summary>
    /// Unregisters a chart renderer from the theme service.
    /// </summary>
    public void UnregisterRenderer(IChartRenderer renderer)
    {
        _renderers.Remove(renderer);
    }
    
    /// <summary>
    /// Applies the current theme to all registered charts.
    /// </summary>
    public async Task ApplyCurrentThemeAsync()
    {
        if (_disposed) return;
        
        try
        {
            await EnsureModuleLoadedAsync();
            
            if (_moduleRef == null) return;
            
            // Get current theme from DOM
            var colors = await GetThemeColorsAsync();
            var theme = new ChartTheme
            {
                ChartColors = colors
            };
            
            // Apply theme to all renderers
            foreach (var renderer in _renderers.ToList())
            {
                try
                {
                    // Note: ApplyThemeAsync requires a chartId, but we don't track them here
                    // This is a limitation - theme changes would need to be handled per-chart
                    // For now, this method is a placeholder for future enhancement
                    Console.WriteLine("ChartThemeService: Theme changed - charts will use new theme on next render");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"ChartThemeService: Failed to apply theme to renderer - {ex.Message}");
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"ChartThemeService: Failed to apply theme - {ex.Message}");
        }
    }
    
    /// <summary>
    /// Gets the current theme colors from CSS variables.
    /// </summary>
    private async Task<string[]> GetThemeColorsAsync()
    {
        if (_moduleRef == null)
        {
            await EnsureModuleLoadedAsync();
        }
        
        if (_moduleRef == null)
        {
            return new[]
            {
                "hsl(var(--chart-1))",
                "hsl(var(--chart-2))",
                "hsl(var(--chart-3))",
                "hsl(var(--chart-4))",
                "hsl(var(--chart-5))"
            };
        }
        
        try
        {
            return await _moduleRef.InvokeAsync<string[]>("getThemeColors");
        }
        catch
        {
            return new[]
            {
                "hsl(var(--chart-1))",
                "hsl(var(--chart-2))",
                "hsl(var(--chart-3))",
                "hsl(var(--chart-4))",
                "hsl(var(--chart-5))"
            };
        }
    }
    
    private async Task EnsureModuleLoadedAsync()
    {
        if (_moduleRef == null && !_disposed)
        {
            try
            {
                _moduleRef = await _jsRuntime.InvokeAsync<IJSObjectReference>(
                    "import",
                    "./_content/NeoBlazorUI.Components/js/chart-theme.js"
                );
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ChartThemeService: Failed to load module - {ex.Message}");
            }
        }
    }
    
    public async ValueTask DisposeAsync()
    {
        if (_disposed) return;
        
        _disposed = true;
        _renderers.Clear();
        
        if (_moduleRef != null)
        {
            try
            {
                await _moduleRef.DisposeAsync();
            }
            catch
            {
                // Ignore disposal errors
            }
        }
    }
}
