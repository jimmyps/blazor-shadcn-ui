using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace BlazorUI.Components.Chart;

/// <summary>
/// Base ECharts component with direct option support for maximum flexibility.
/// </summary>
/// <remarks>
/// This component provides direct access to ECharts' native option object,
/// allowing you to use any ECharts feature without wrapper limitations.
/// 
/// Example with strongly-typed builder:
/// <code>
/// &lt;EChart Option="@chartOption" Height="400" /&gt;
/// 
/// @code {
///     private EChartsOption chartOption = new EChartsOptionBuilder()
///         .WithTooltip(trigger: "axis")
///         .WithXAxis(axis => axis.SetType("category").SetData("Mon", "Tue", "Wed"))
///         .WithYAxis(axis => axis.SetType("value"))
///         .AddLineSeries(series => series
///             .SetName("Sales")
///             .SetData(120, 200, 150)
///             .SetSmooth(true)
///             .WithGradientFill(
///                 EChartsLinearGradient.Vertical(
///                     new EChartsColorStop(0, "hsla(var(--chart-1), 0.8)"),
///                     new EChartsColorStop(1, "hsla(var(--chart-1), 0.1)")
///                 )
///             ))
///         .Build();
/// }
/// </code>
/// 
/// For type-safe access, see ECharts option documentation: https://echarts.apache.org/option.html
/// </remarks>
public partial class EChart : ComponentBase, IAsyncDisposable
{
    [Inject] private IJSRuntime JSRuntime { get; set; } = default!;
    
    /// <summary>
    /// Gets or sets the ECharts option object (strongly-typed).
    /// Use EChartsOptionBuilder for a fluent, type-safe API.
    /// </summary>
    [Parameter, EditorRequired]
    public object Option { get; set; } = default!;
    
    /// <summary>
    /// Gets or sets the height of the chart in pixels.
    /// </summary>
    [Parameter]
    public int Height { get; set; } = 350;
    
    /// <summary>
    /// Gets or sets the width of the chart. Defaults to 100% (responsive).
    /// </summary>
    [Parameter]
    public string Width { get; set; } = "100%";
    
    /// <summary>
    /// Gets or sets whether to disable animations.
    /// </summary>
    [Parameter]
    public bool DisableAnimations { get; set; }
    
    /// <summary>
    /// Gets or sets additional CSS classes.
    /// </summary>
    [Parameter]
    public string? Class { get; set; }
    
    /// <summary>
    /// Gets or sets the theme to use (light/dark or custom theme name).
    /// </summary>
    [Parameter]
    public string? Theme { get; set; }
    
    /// <summary>
    /// Event callback when chart is clicked.
    /// </summary>
    [Parameter]
    public EventCallback<ChartClickEventArgs> OnChartClick { get; set; }
    
    /// <summary>
    /// Event callback when legend selection changes.
    /// </summary>
    [Parameter]
    public EventCallback<LegendSelectChangedEventArgs> OnLegendSelectChanged { get; set; }
    
    private ElementReference _chartElement;
    private string? _chartId;
    private IChartRenderer? _renderer;
    private bool _isDisposed;
    
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender && !_isDisposed)
        {
            try
            {
                _renderer = new EChartsRenderer(JSRuntime);
                
                var config = new ChartConfig
                {
                    Type = ChartType.Line, // Doesn't matter for direct option mode
                    Data = new { }, // Will be overridden by Option
                    Options = Option
                };
                
                _chartId = await _renderer.InitializeAsync(_chartElement, config);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"EChart initialization error: {ex.Message}");
            }
        }
    }
    
    /// <summary>
    /// Updates the chart with new option data.
    /// </summary>
    public async Task UpdateOptionAsync(object newOption)
    {
        if (_renderer != null && _chartId != null)
        {
            await _renderer.UpdateOptionsAsync(_chartId, newOption);
        }
    }
    
    /// <summary>
    /// Exports the chart as an image.
    /// </summary>
    /// <param name="format">PNG or SVG format</param>
    /// <returns>Base64 encoded image string</returns>
    public async Task<string> ExportAsImageAsync(ImageFormat format = ImageFormat.Png)
    {
        if (_renderer != null && _chartId != null)
        {
            return await _renderer.ExportAsImageAsync(_chartId, format);
        }
        return string.Empty;
    }
    
    public async ValueTask DisposeAsync()
    {
        if (_isDisposed) return;
        _isDisposed = true;
        
        if (_renderer != null && _chartId != null)
        {
            try
            {
                await _renderer.DestroyAsync(_chartId);
            }
            catch (JSDisconnectedException)
            {
                // Circuit disconnected, nothing to clean up
            }
            catch (JSException)
            {
                // JavaScript error during disposal, safe to ignore
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"EChart disposal error: {ex.Message}");
            }
        }
        
        if (_renderer != null)
        {
            await _renderer.DisposeAsync();
        }
    }
}

/// <summary>
/// Event arguments for chart click events.
/// </summary>
public class ChartClickEventArgs
{
    public string? Name { get; set; }
    public object? Value { get; set; }
    public int SeriesIndex { get; set; }
    public int DataIndex { get; set; }
}

/// <summary>
/// Event arguments for legend selection changes.
/// </summary>
public class LegendSelectChangedEventArgs
{
    public string? Name { get; set; }
    public Dictionary<string, bool> Selected { get; set; } = new();
}
