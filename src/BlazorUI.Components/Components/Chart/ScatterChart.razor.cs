using BlazorUI.Components.Utilities;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace BlazorUI.Components.Chart;

/// <summary>
/// Scatter chart component for visualizing correlation between two variables.
/// Supports bubble chart variant by adding size dimension.
/// </summary>
/// <typeparam name="TData">The type of data items in the chart.</typeparam>
public class ScatterChartBase<TData> : ComponentBase
{
    [Inject] protected IJSRuntime JSRuntime { get; set; } = default!;
    
    /// <summary>
    /// Gets or sets the data to display in the chart.
    /// </summary>
    [Parameter, EditorRequired]
    public IEnumerable<TData> Data { get; set; } = Enumerable.Empty<TData>();
    
    /// <summary>
    /// Gets or sets the height of the chart in pixels.
    /// </summary>
    [Parameter]
    public int Height { get; set; } = 350;
    
    /// <summary>
    /// Gets or sets the property name for X-axis values.
    /// </summary>
    [Parameter, EditorRequired]
    public string XAxisDataKey { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the property name for Y-axis values.
    /// </summary>
    [Parameter, EditorRequired]
    public string YAxisDataKey { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the property name for bubble size (optional, for bubble charts).
    /// </summary>
    [Parameter]
    public string? SizeDataKey { get; set; }
    
    /// <summary>
    /// Gets or sets the label for the data series.
    /// </summary>
    [Parameter]
    public string? Label { get; set; }
    
    /// <summary>
    /// Gets or sets the color of the points.
    /// </summary>
    [Parameter]
    public string? Color { get; set; }
    
    /// <summary>
    /// Gets or sets the size of the points in pixels.
    /// </summary>
    [Parameter]
    public int PointSize { get; set; } = 10;
    
    /// <summary>
    /// Gets or sets whether to show tooltips.
    /// </summary>
    [Parameter]
    public bool ShowTooltip { get; set; } = true;
    
    /// <summary>
    /// Gets or sets whether to show the legend.
    /// </summary>
    [Parameter]
    public bool ShowLegend { get; set; } = true;
    
    /// <summary>
    /// Gets or sets whether to show grid lines.
    /// </summary>
    [Parameter]
    public bool ShowGrid { get; set; } = true;
    
    /// <summary>
    /// Gets or sets additional CSS classes.
    /// </summary>
    [Parameter]
    public string? Class { get; set; }
    
    /// <summary>
    /// Gets or sets the rendering engine to use.
    /// </summary>
    [Parameter]
    public ChartEngine Engine { get; set; } = ChartEngine.ECharts;
    
    /// <summary>
    /// Gets the computed CSS classes for the container.
    /// </summary>
    protected string ContainerClass => ClassNames.cn(
        "chart-container",
        Class
    );
}
