using BlazorUI.Components.Utilities;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace BlazorUI.Components.Chart;

/// <summary>
/// Bar chart component for comparing discrete categories.
/// </summary>
/// <typeparam name="TData">The type of data items in the chart.</typeparam>
public class BarChartBase<TData> : ComponentBase
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
    /// Gets or sets the orientation of the bars.
    /// </summary>
    [Parameter]
    public BarChartOrientation Orientation { get; set; } = BarChartOrientation.Vertical;
    
    /// <summary>
    /// Gets or sets how multiple bar series are displayed.
    /// </summary>
    [Parameter]
    public BarChartMode Mode { get; set; } = BarChartMode.Grouped;
    
    /// <summary>
    /// Gets or sets the property name to use for X-axis values.
    /// </summary>
    [Parameter]
    public string XAxisDataKey { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the property name to use for Y-axis values.
    /// </summary>
    [Parameter, EditorRequired]
    public string YAxisDataKey { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the label for the data series.
    /// </summary>
    [Parameter]
    public string? Label { get; set; }
    
    /// <summary>
    /// Gets or sets the color of the bars.
    /// </summary>
    [Parameter]
    public string? BarColor { get; set; }
    
    /// <summary>
    /// Gets or sets the border radius of the bars.
    /// </summary>
    [Parameter]
    public int BorderRadius { get; set; } = 4;
    
    /// <summary>
    /// Gets or sets the thickness of the bars in pixels (0 = automatic).
    /// </summary>
    [Parameter]
    public int BarThickness { get; set; } = 0;
    
    /// <summary>
    /// Gets or sets whether the chart should be responsive.
    /// </summary>
    [Parameter]
    public bool Responsive { get; set; } = true;
    
    /// <summary>
    /// Gets or sets whether to show the legend.
    /// </summary>
    [Parameter]
    public bool ShowLegend { get; set; } = true;
    
    /// <summary>
    /// Gets or sets whether to show tooltips.
    /// </summary>
    [Parameter]
    public bool ShowTooltip { get; set; } = true;
    
    /// <summary>
    /// Gets or sets whether to show the X axis.
    /// </summary>
    [Parameter]
    public bool ShowXAxis { get; set; } = true;
    
    /// <summary>
    /// Gets or sets whether to show the Y axis.
    /// </summary>
    [Parameter]
    public bool ShowYAxis { get; set; } = true;
    
    /// <summary>
    /// Gets or sets whether to show grid lines.
    /// </summary>
    [Parameter]
    public bool ShowGrid { get; set; } = true;
    
    /// <summary>
    /// Gets or sets the animation configuration.
    /// </summary>
    [Parameter]
    public ChartAnimation? Animation { get; set; }
    
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
    /// Gets the computed CSS classes for the container.
    /// </summary>
    protected string ContainerClass => ClassNames.cn(
        "chart-container",
        Class
    );
}
