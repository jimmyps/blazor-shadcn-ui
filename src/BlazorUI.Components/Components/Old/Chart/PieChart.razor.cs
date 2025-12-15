using BlazorUI.Components.Utilities;
using BlazorUI.Components.Chart;
using Microsoft.AspNetCore.Components;
using BlazorUI.Components.Chart;
using Microsoft.JSInterop;
using BlazorUI.Components.Chart;

namespace BlazorUI.Components.Old.Chart;

/// <summary>
/// Pie chart component for showing composition of a whole.
/// Can be configured as a donut chart with a hole in the center.
/// </summary>
/// <typeparam name="TData">The type of data items in the chart.</typeparam>
public class PieChartBase<TData> : ComponentBase
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
    /// Gets or sets whether to display as a donut chart.
    /// </summary>
    [Parameter]
    public bool IsDonut { get; set; } = false;
    
    /// <summary>
    /// Gets or sets the inner radius for donut charts (0.0 to 1.0).
    /// </summary>
    [Parameter]
    public double DonutInnerRadius { get; set; } = 0.6;
    
    /// <summary>
    /// Gets or sets the property name to use for labels.
    /// </summary>
    [Parameter, EditorRequired]
    public string LabelDataKey { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the property name to use for values.
    /// </summary>
    [Parameter, EditorRequired]
    public string ValueDataKey { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the position of the legend.
    /// </summary>
    [Parameter]
    public LegendPosition LegendPosition { get; set; } = LegendPosition.Bottom;
    
    /// <summary>
    /// Gets or sets content to display in the center of a donut chart.
    /// </summary>
    [Parameter]
    public RenderFragment? CenterContent { get; set; }
    
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
    /// Gets or sets the animation configuration.
    /// </summary>
    [Parameter]
    public ChartAnimation? Animation { get; set; }
    
    /// <summary>
    /// Gets or sets the rendering engine to use.
    /// </summary>
    [Parameter]
    public ChartEngine Engine { get; set; } = ChartEngine.ChartJs;
    
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
    /// Gets the computed CSS classes for the container.
    /// </summary>
    protected string ContainerClass => ClassNames.cn(
        "chart-container relative",
        Class
    );
}
