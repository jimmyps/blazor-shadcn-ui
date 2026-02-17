using Microsoft.AspNetCore.Components;

namespace BlazorBlueprint.Components;

/// <summary>
/// A bar chart component using the declarative composition API with Apache ECharts.
/// </summary>
/// <remarks>
/// <para>
/// The BarChart component provides vertical and horizontal bar visualizations
/// with support for stacking, custom border radius, and bar width control.
/// </para>
/// <para>
/// Usage:
/// <code>
/// &lt;BbBarChart Data="@data" Height="350px"&gt;
///     &lt;BbXAxis DataKey="month" /&gt;
///     &lt;BbYAxis /&gt;
///     &lt;BbChartTooltip /&gt;
///     &lt;BbBar DataKey="desktop" Name="Desktop" Color="var(--chart-1)" /&gt;
///     &lt;BbBar DataKey="mobile" Name="Mobile" Color="var(--chart-2)" /&gt;
/// &lt;/BbBarChart&gt;
/// </code>
/// </para>
/// </remarks>
public class BbBarChart : BbChartBase
{
    /// <summary>
    /// Gets or sets whether to render bars horizontally.
    /// </summary>
    /// <remarks>
    /// When true, the X axis becomes the value axis and Y axis becomes the category axis.
    /// Default is false (vertical bars).
    /// </remarks>
    [Parameter]
    public bool Horizontal { get; set; }

    internal override string DataSlot => "bar-chart";

    internal override string SeriesType => "bar";

    internal override bool SwapAxes => Horizontal;

    internal override void ApplyChartDefaults(EChartsOption option)
    {
        option.Grid ??= new EChartsGridOption
        {
            Left = "12",
            Right = "12",
            Top = "10",
            Bottom = "0",
            ContainLabel = true
        };

        if (Horizontal)
        {
            option.XAxis ??= new EChartsAxisOption
            {
                Type = "value",
                SplitLine = new EChartsSplitLineOption
                {
                    Show = true,
                    LineStyle = new EChartsLineStyleOption
                    {
                        Color = "var(--border)",
                        Type = "dashed"
                    }
                },
                AxisLabel = new EChartsAxisLabelOption
                {
                    Color = "var(--muted-foreground)"
                }
            };

            option.YAxis ??= new EChartsAxisOption
            {
                Type = "category",
                Inverse = true,
                AxisLabel = new EChartsAxisLabelOption
                {
                    Color = "var(--muted-foreground)"
                }
            };
        }
        else
        {
            option.YAxis ??= new EChartsAxisOption
            {
                Type = "value",
                SplitLine = new EChartsSplitLineOption
                {
                    Show = true,
                    LineStyle = new EChartsLineStyleOption
                    {
                        Color = "var(--border)",
                        Type = "dashed"
                    }
                },
                AxisLabel = new EChartsAxisLabelOption
                {
                    Color = "var(--muted-foreground)"
                }
            };
        }
    }
}
