using BlazorUI.Components.Utilities;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace BlazorUI.Components.Chart;

/// <summary>
/// Radar chart component for visualizing multivariate data across multiple axes.
/// </summary>
/// <typeparam name="TData">The type of data items in the chart.</typeparam>
public class RadarChartBase<TData> : ComponentBase
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
    /// Gets or sets the property name to use for axis labels.
    /// </summary>
    [Parameter, EditorRequired]
    public string LabelDataKey { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the series configurations.
    /// </summary>
    [Parameter, EditorRequired]
    public IEnumerable<RadarSeriesConfig> Series { get; set; } = Enumerable.Empty<RadarSeriesConfig>();
    
    /// <summary>
    /// Gets or sets the fill opacity for radar areas (0.0 to 1.0).
    /// </summary>
    [Parameter]
    public double FillOpacity { get; set; } = 0.2;
    
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
    /// Gets or sets whether to show axis labels.
    /// </summary>
    [Parameter]
    public bool ShowLabels { get; set; } = true;
    
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
    /// Gets the computed CSS classes for the container.
    /// </summary>
    protected string ContainerClass => ClassNames.cn(
        "chart-container",
        Class
    );
}

/// <summary>
/// Configuration for a radar chart data series.
/// </summary>
public class RadarSeriesConfig
{
    /// <summary>
    /// Gets or sets the property name to use for data values.
    /// </summary>
    public string DataKey { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the label for the series.
    /// </summary>
    public string? Label { get; set; }
    
    /// <summary>
    /// Gets or sets the color of the line and points.
    /// </summary>
    public string? Color { get; set; }
    
    /// <summary>
    /// Gets or sets the width of the line.
    /// </summary>
    public int LineWidth { get; set; } = 2;
}
