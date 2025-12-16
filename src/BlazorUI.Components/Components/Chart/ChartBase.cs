using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using BlazorUI.Components.Chart.Internal;
using System.Text.Json;

namespace BlazorUI.Components.Chart;

/// <summary>
/// Base class for chart root components with common rendering logic.
/// </summary>
/// <typeparam name="TData">The type of data items in the chart.</typeparam>
public abstract class ChartBase<TData> : ComponentBase, IAsyncDisposable
{
    [Inject] protected IJSRuntime JSRuntime { get; set; } = default!;
    
    /// <summary>
    /// Gets or sets the data to display in the chart.
    /// </summary>
    [Parameter, EditorRequired]
    public IEnumerable<TData> Data { get; set; } = Enumerable.Empty<TData>();
    
    /// <summary>
    /// Gets or sets the child content (primitives and series).
    /// </summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }
    
    /// <summary>
    /// Gets or sets the chart ID.
    /// </summary>
    [Parameter]
    public string? Id { get; set; }
    
    /// <summary>
    /// Gets or sets additional CSS classes.
    /// </summary>
    [Parameter]
    public string? Class { get; set; }
    
    /// <summary>
    /// Gets or sets the plot area padding (cartesian charts only).
    /// </summary>
    [Parameter]
    public Padding Padding { get; set; } = new Padding(32, 16, 24, 16);
    
    /// <summary>
    /// Gets or sets whether axis labels should be contained within the grid.
    /// </summary>
    [Parameter]
    public bool ContainLabel { get; set; } = true;
    
    protected ElementReference _canvasRef;
    protected IChartRenderer? _renderer;
    protected string? _chartId;
    protected bool _isInitialized;
    
    // Collected primitives (overridden by explicit child components)
    protected Grid? _grid;
    protected XAxis? _xAxis;
    protected YAxis? _yAxis;
    protected Tooltip? _tooltip;
    protected Legend? _legend;
    
    // Collected series
    protected List<object> _series = new();
    
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender && Data != null && Data.Any())
        {
            await InitializeChartAsync();
        }
    }
    
    protected override async Task OnParametersSetAsync()
    {
        if (_isInitialized && Data != null)
        {
            await RefreshAsync();
        }
    }
    
    private async Task InitializeChartAsync()
    {
        try
        {
            Console.WriteLine($"[{GetType().Name}] InitializeChartAsync called");
            Console.WriteLine($"[{GetType().Name}] Data count: {Data?.Count() ?? 0}");
            
            _renderer = ChartRendererFactory.CreateRenderer(ChartEngine.ECharts, JSRuntime);
            Console.WriteLine($"[{GetType().Name}] Renderer created");
            
            var option = BuildEChartsOption();
            Console.WriteLine($"[{GetType().Name}] EChartsOption built:");
            Console.WriteLine($"[{GetType().Name}]   Series count: {option.Series?.Count ?? 0}");
            Console.WriteLine($"[{GetType().Name}]   XAxis data count: {option.XAxis?.Data?.Count ?? 0}");
            Console.WriteLine($"[{GetType().Name}]   YAxis configured: {option.YAxis != null}");
            
            var config = new ChartConfig
            {
                Type = GetChartType(),
                Data = option,
                Options = new { }
            };
            
            Console.WriteLine($"[{GetType().Name}] Calling renderer.InitializeAsync");
            _chartId = await _renderer.InitializeAsync(_canvasRef, config);
            Console.WriteLine($"[{GetType().Name}] Chart initialized with ID: {_chartId}");
            _isInitialized = true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[{GetType().Name}] Failed to initialize chart: {ex.Message}");
            Console.WriteLine($"[{GetType().Name}] Stack trace: {ex.StackTrace}");
        }
    }
    
    /// <summary>
    /// Refreshes the chart with current data and primitives.
    /// </summary>
    public async Task RefreshAsync()
    {
        if (!_isInitialized || _renderer == null || string.IsNullOrEmpty(_chartId))
        {
            Console.WriteLine($"[{GetType().Name}] RefreshAsync skipped - not initialized");
            return;
        }
        
        try
        {
            Console.WriteLine($"[{GetType().Name}] RefreshAsync called");
            var option = BuildEChartsOption();
            Console.WriteLine($"[{GetType().Name}] EChartsOption rebuilt, series count: {option.Series?.Count ?? 0}");
            await _renderer.UpdateOptionsAsync(_chartId, option);
            Console.WriteLine($"[{GetType().Name}] Chart refreshed successfully");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[{GetType().Name}] Failed to refresh chart: {ex.Message}");
            Console.WriteLine($"[{GetType().Name}] Stack trace: {ex.StackTrace}");
        }
    }
    
    /// <summary>
    /// Builds the ECharts option object from primitives and data.
    /// </summary>
    protected abstract EChartsOption BuildEChartsOption();
    
    /// <summary>
    /// Gets the chart type for this root component.
    /// </summary>
    protected abstract ChartType GetChartType();
    
    /// <summary>
    /// Gets the default tooltip mode for this chart type.
    /// </summary>
    protected abstract TooltipMode GetDefaultTooltipMode();
    
    /// <summary>
    /// Builds the grid configuration.
    /// </summary>
    protected EChartsGrid BuildGrid()
    {
        return new EChartsGrid
        {
            Top = Padding.Top,
            Right = Padding.Right,
            Bottom = Padding.Bottom,
            Left = Padding.Left,
            ContainLabel = ContainLabel
        };
    }
    
    /// <summary>
    /// Builds the legend configuration.
    /// </summary>
    protected EChartsLegend BuildLegend()
    {
        var legend = _legend ?? new Legend();
        
        return new EChartsLegend
        {
            Show = legend.Show,
            Orient = legend.Layout == LegendLayout.Vertical ? "vertical" : "horizontal",
            Left = legend.Align switch
            {
                LegendAlign.Left => "left",
                LegendAlign.Right => "right",
                _ => "center"
            },
            Top = legend.VerticalAlign switch
            {
                LegendVerticalAlign.Top => legend.MarginTop.ToString(),
                LegendVerticalAlign.Middle => "middle",
                _ => "bottom"
            }
        };
    }
    
    /// <summary>
    /// Builds the tooltip configuration.
    /// </summary>
    protected EChartsTooltip BuildTooltip()
    {
        var tooltip = _tooltip ?? new Tooltip { Mode = GetDefaultTooltipMode() };
        var mode = tooltip.Mode ?? GetDefaultTooltipMode();
        
        return new EChartsTooltip
        {
            Show = tooltip.Show,
            Trigger = mode == TooltipMode.Axis ? "axis" : "item",
            AxisPointer = mode == TooltipMode.Axis ? new EChartsAxisPointer
            {
                Type = tooltip.Cursor switch
                {
                    TooltipCursor.Cross => "cross",
                    TooltipCursor.Line => "line",
                    TooltipCursor.Shadow => "shadow",
                    _ => "none"
                }
            } : null,
            Formatter = tooltip.Formatter
        };
    }
    
    /// <summary>
    /// Builds axis configuration.
    /// </summary>
    protected EChartsAxis BuildAxis(XAxis? xAxis, YAxis? yAxis, bool isXAxis)
    {
        object? axisObj = isXAxis ? (object?)xAxis : yAxis;
        
        var show = axisObj switch
        {
            XAxis x => x.Show,
            YAxis y => y.Show,
            _ => true
        };
        
        var scale = axisObj switch
        {
            XAxis x => x.Scale,
            YAxis y => y.Scale,
            _ => AxisScale.Auto
        };
        
        var axisLine = axisObj switch
        {
            XAxis x => x.AxisLine,
            YAxis y => y.AxisLine,
            _ => true
        };
        
        var tickLine = axisObj switch
        {
            XAxis x => x.TickLine,
            YAxis y => y.TickLine,
            _ => true
        };
        
        var type = scale switch
        {
            AxisScale.Category => "category",
            AxisScale.Value => "value",
            AxisScale.Time => "time",
            AxisScale.Log => "log",
            _ => "category" // Auto defaults to category for X, value for Y
        };
        
        // For auto scale, determine based on axis
        if (scale == AxisScale.Auto)
        {
            type = isXAxis ? "category" : "value";
        }
        
        // Get AxisLabel component if nested in XAxis or YAxis
        AxisLabel? axisLabel = axisObj switch
        {
            XAxis x => x.AxisLabelComponent,
            YAxis y => y.AxisLabelComponent,
            _ => null
        };
        
        var result = new EChartsAxis
        {
            Type = type,
            AxisLine = new EChartsAxisLine { Show = show && axisLine },
            AxisTick = new EChartsAxisTick { Show = show && tickLine },
            SplitLine = BuildSplitLine(isXAxis),
            AxisLabel = BuildAxisLabel(axisLabel)
        };
        
        return result;
    }
    
    /// <summary>
    /// Builds ECharts axis label configuration from AxisLabel component.
    /// </summary>
    protected EChartsAxisLabel? BuildAxisLabel(AxisLabel? axisLabel)
    {
        if (axisLabel == null)
        {
            return null;
        }
        
        return new EChartsAxisLabel
        {
            Show = axisLabel.Show,
            Rotate = axisLabel.Rotate,
            Formatter = axisLabel.Formatter,
            Interval = axisLabel.Interval,
            Inside = axisLabel.Inside,
            Margin = axisLabel.Margin,
            HideOverlap = axisLabel.HideOverlap,
            Color = axisLabel.Color,
            FontSize = axisLabel.FontSize,
            FontFamily = axisLabel.FontFamily,
            FontWeight = axisLabel.FontWeight,
            LineHeight = axisLabel.LineHeight,
            Align = axisLabel.Align,
            VerticalAlign = axisLabel.VerticalAlign,
            Overflow = axisLabel.Overflow,
            Width = axisLabel.Width,
            Ellipsis = axisLabel.Ellipsis
        };
    }
    
    /// <summary>
    /// Builds gradient configuration from Fill component.
    /// </summary>
    protected object? BuildGradient(Fill? fill)
    {
        if (fill == null)
        {
            return null;
        }
        
        // If there's a LinearGradient child with stops
        if (fill.LinearGradient != null && fill.LinearGradient.Stops.Count > 0)
        {
            var gradient = fill.LinearGradient;
            var stops = new List<object>();
            
            foreach (var stop in gradient.Stops)
            {
                var color = stop.Color;
                if (stop.Opacity.HasValue)
                {
                    // Convert to rgba if opacity specified
                    color = $"rgba({color}, {stop.Opacity.Value})";
                }
                
                stops.Add(new
                {
                    offset = stop.Offset,
                    color = color
                });
            }
            
            // Return ECharts linear gradient object
            return new
            {
                type = "linear",
                x = gradient.Direction == GradientDirection.Horizontal ? 0 : 0,
                y = gradient.Direction == GradientDirection.Horizontal ? 0 : 0,
                x2 = gradient.Direction == GradientDirection.Horizontal ? 1 : 0,
                y2 = gradient.Direction == GradientDirection.Horizontal ? 0 : 1,
                colorStops = stops
            };
        }
        
        // Solid fill with optional opacity
        if (!string.IsNullOrEmpty(fill.Color))
        {
            if (fill.Opacity.HasValue)
            {
                return $"rgba({fill.Color}, {fill.Opacity.Value})";
            }
            return fill.Color;
        }
        
        return null;
    }
    
    /// <summary>
    /// Builds split line (grid line) configuration.
    /// </summary>
    protected EChartsSplitLine BuildSplitLine(bool isXAxis)
    {
        var grid = _grid ?? new Grid();
        var show = grid.Show && (isXAxis ? grid.Vertical : grid.Horizontal);
        
        return new EChartsSplitLine
        {
            Show = show,
            LineStyle = grid.Stroke != null ? new EChartsLineStyle { Color = grid.Stroke } : null
        };
    }
    
    /// <summary>
    /// Gets color for series at index.
    /// </summary>
    protected string GetSeriesColor(int index, string? customColor)
    {
        return customColor ?? $"var(--chart-{(index % 5) + 1})";
    }
    
    public async ValueTask DisposeAsync()
    {
        if (_renderer != null && !string.IsNullOrEmpty(_chartId))
        {
            try
            {
                await _renderer.DestroyAsync(_chartId);
            }
            catch (JSDisconnectedException)
            {
                // Circuit disconnected, nothing to clean up
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{GetType().Name}: Error during disposal: {ex.Message}");
            }
        }
        
        if (_renderer != null)
        {
            await _renderer.DisposeAsync();
        }
    }
}
