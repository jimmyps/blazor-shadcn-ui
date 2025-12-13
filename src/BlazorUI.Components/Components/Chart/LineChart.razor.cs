using BlazorUI.Components.Utilities;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace BlazorUI.Components.Chart;

/// <summary>
/// Line chart component for visualizing trends over time or continuous data.
/// </summary>
/// <typeparam name="TData">The type of data items in the chart.</typeparam>
public class LineChartBase<TData> : ComponentBase, IChartStyleContainer
{
    [Inject] protected IJSRuntime JSRuntime { get; set; } = default!;
    
    /// <summary>
    /// Internal ChartStyle reference from child content.
    /// </summary>
    protected ChartStyle? _chartStyle;
    
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
    /// Gets or sets the color of the line.
    /// </summary>
    [Parameter]
    public string? LineColor { get; set; }
    
    /// <summary>
    /// Gets or sets the width of the line.
    /// </summary>
    [Parameter]
    public int LineWidth { get; set; } = 2;
    
    /// <summary>
    /// Gets or sets whether the line should be curved.
    /// </summary>
    [Parameter]
    public bool Curved { get; set; } = false;
    
    /// <summary>
    /// Gets or sets whether to fill the area under the line.
    /// </summary>
    [Parameter]
    public bool FillArea { get; set; } = false;
    
    /// <summary>
    /// Gets or sets whether to use a gradient fill for the area (requires FillArea=true).
    /// </summary>
    [Parameter]
    public bool GradientFill { get; set; } = false;
    
    /// <summary>
    /// Gets or sets the opacity at the top of the gradient (0.0 to 1.0). Default is 0.8.
    /// Only applies when GradientFill=true.
    /// </summary>
    [Parameter]
    public double GradientStartOpacity { get; set; } = 0.8;
    
    /// <summary>
    /// Gets or sets the opacity at the bottom of the gradient (0.0 to 1.0). Default is 0.1.
    /// Only applies when GradientFill=true.
    /// </summary>
    [Parameter]
    public double GradientEndOpacity { get; set; } = 0.1;
    
    /// <summary>
    /// Gets or sets whether to show dots at data points.
    /// </summary>
    [Parameter]
    public bool ShowDots { get; set; } = true;
    
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
    /// Gets or sets the rendering engine to use.
    /// </summary>
    [Parameter]
    public ChartEngine Engine { get; set; } = ChartEngine.ECharts;
    
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
    /// Gets or sets whether to show an export button for downloading the chart.
    /// </summary>
    [Parameter]
    public bool ShowExportButton { get; set; }
    
    /// <summary>
    /// Gets or sets the filename to use when exporting (without extension).
    /// </summary>
    [Parameter]
    public string ExportFileName { get; set; } = "chart";
    
    /// <summary>
    /// Gets or sets the child content containing ChartStyle configuration.
    /// </summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }
    
    /// <summary>
    /// Gets the computed CSS classes for the container.
    /// </summary>
    protected string ContainerClass => ClassNames.cn(
        "chart-container",
        Class
    );
    
    /// <summary>
    /// Registers a ChartStyle component.
    /// </summary>
    public void RegisterStyle(ChartStyle style)
    {
        _chartStyle = style;
    }
}
