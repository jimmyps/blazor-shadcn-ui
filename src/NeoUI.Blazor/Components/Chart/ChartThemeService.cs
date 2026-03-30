using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;

namespace NeoUI.Blazor.Charts;

/// <summary>
/// Service for managing chart themes and coordinating theme changes across all charts.
/// </summary>
public class ChartThemeService : IAsyncDisposable
{
    private static readonly Action<ILogger, string, Exception?> LogApplyThemeToRendererFailed =
        LoggerMessage.Define<string>(LogLevel.Error, new EventId(1, nameof(LogApplyThemeToRendererFailed)),
            "ChartThemeService: Failed to apply theme to renderer - {Message}");

    private static readonly Action<ILogger, string, Exception?> LogApplyThemeFailed =
        LoggerMessage.Define<string>(LogLevel.Error, new EventId(2, nameof(LogApplyThemeFailed)),
            "ChartThemeService: Failed to apply theme - {Message}");

    private static readonly Action<ILogger, string, Exception?> LogModuleLoadFailed =
        LoggerMessage.Define<string>(LogLevel.Error, new EventId(3, nameof(LogModuleLoadFailed)),
            "ChartThemeService: Failed to load module - {Message}");

    private readonly IJSRuntime _jsRuntime;
    private readonly ILogger<ChartThemeService> _logger;
    private readonly List<IChartRenderer> _renderers = new();
    private IJSObjectReference? _moduleRef;
    private bool _disposed;
    
    /// <summary>
    /// Initializes a new instance of the <see cref="ChartThemeService"/> class.
    /// </summary>
    /// <param name="jsRuntime">The JavaScript runtime for interop calls.</param>
    /// <param name="logger">The logger instance.</param>
    public ChartThemeService(IJSRuntime jsRuntime, ILogger<ChartThemeService> logger)
    {
        _jsRuntime = jsRuntime;
        _logger = logger;
    }
    
    /// <summary>
    /// Registers a chart renderer with the theme service.
    /// </summary>
    /// <param name="renderer">The chart renderer to register.</param>
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
    /// <param name="renderer">The chart renderer to unregister.</param>
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
                }
                catch (Exception ex)
                {
                    LogApplyThemeToRendererFailed(_logger, ex.Message, ex);
                }
            }
        }
        catch (Exception ex)
        {
            LogApplyThemeFailed(_logger, ex.Message, ex);
        }
    }
    
    /// <summary>
    /// Gets the current theme colors from CSS variables.
    /// </summary>
    /// <returns>An array of color strings for chart data series.</returns>
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
    
    /// <summary>
    /// Ensures the JavaScript module is loaded for theme operations.
    /// </summary>
    private async Task EnsureModuleLoadedAsync()
    {
        if (_moduleRef == null && !_disposed)
        {
            try
            {
                _moduleRef = await _jsRuntime.InvokeAsync<IJSObjectReference>(
                    "import",
                    "./_content/NeoUI.Blazor/js/chart-theme.js"
                );
            }
            catch (Exception ex)
            {
                LogModuleLoadFailed(_logger, ex.Message, ex);
            }
        }
    }
    
    /// <summary>
    /// Disposes the service and releases JavaScript module resources.
    /// </summary>
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
