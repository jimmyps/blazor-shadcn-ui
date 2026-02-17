using Microsoft.AspNetCore.Components;

namespace BlazorBlueprint.Components;

/// <summary>
/// Configures the legend for a chart. Must be placed inside a chart component.
/// </summary>
/// <remarks>
/// <para>
/// ChartLegend is named for consistency with other chart composable components.
/// It maps the <see cref="LegendPosition"/> enum to ECharts orient/positioning properties.
/// </para>
/// <para>
/// The <see cref="LegendPosition.Hidden"/> position hides the legend entirely.
/// </para>
/// </remarks>
/// <example>
/// <code>
/// &lt;LineChart Data="@data"&gt;
///     &lt;ChartLegend Position="LegendPosition.Top" /&gt;
///     &lt;XAxis DataKey="month" /&gt;
///     &lt;YAxis /&gt;
/// &lt;/LineChart&gt;
/// </code>
/// </example>
public partial class BbChartLegend : ComponentBase, IChartComponent, IDisposable
{
    [CascadingParameter]
    private BbChartBase? ParentChart { get; set; }

    /// <summary>
    /// Gets or sets the legend position relative to the chart.
    /// </summary>
    /// <remarks>
    /// <see cref="LegendPosition.Hidden"/> hides the legend entirely.
    /// Horizontal positions (Top, Bottom) use horizontal orientation.
    /// Vertical positions (Left, Right) use vertical orientation.
    /// </remarks>
    [Parameter]
    public LegendPosition Position { get; set; } = LegendPosition.Bottom;

    /// <summary>
    /// Gets or sets the legend text color.
    /// </summary>
    /// <remarks>
    /// Defaults to <c>var(--foreground)</c> when not specified, matching the design system foreground color.
    /// </remarks>
    [Parameter]
    public string? TextColor { get; set; }

    /// <summary>
    /// Gets or sets the gap between legend items in pixels.
    /// </summary>
    [Parameter]
    public int ItemGap { get; set; } = 16;

    protected override void OnInitialized() =>
        ParentChart?.RegisterComponent(this);

    void IChartComponent.ApplyTo(EChartsOption option)
    {
        if (Position == LegendPosition.Hidden)
        {
            option.Legend = new EChartsLegendOption
            {
                Show = false
            };
            return;
        }

        var legend = new EChartsLegendOption
        {
            Show = true,
            Type = "scroll",
            ItemGap = ItemGap,
            TextStyle = new EChartsTextStyleOption
            {
                Color = TextColor ?? "var(--foreground)"
            }
        };

        switch (Position)
        {
            case LegendPosition.Top:
                legend.Orient = "horizontal";
                legend.Left = "center";
                legend.Top = "0";
                break;

            case LegendPosition.Bottom:
                legend.Orient = "horizontal";
                legend.Left = "center";
                legend.Bottom = "0";
                break;

            case LegendPosition.Left:
                legend.Orient = "vertical";
                legend.Left = "0";
                legend.Top = "center";
                break;

            case LegendPosition.Right:
                legend.Orient = "vertical";
                legend.Right = "0";
                legend.Top = "center";
                break;
        }

        option.Legend = legend;
    }

    public void Dispose()
    {
        ParentChart?.UnregisterComponent(this);
        GC.SuppressFinalize(this);
    }
}
