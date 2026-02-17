namespace BlazorBlueprint.Components;

/// <summary>
/// A line chart component using the declarative composition API with Apache ECharts.
/// </summary>
/// <remarks>
/// <para>
/// The LineChart component provides line-based visualizations with support for
/// smooth curves, step lines, dashed lines, dot customization, and gradient fills.
/// </para>
/// <para>
/// Usage:
/// <code>
/// &lt;BbLineChart Data="@data" Height="350px"&gt;
///     &lt;BbXAxis DataKey="month" /&gt;
///     &lt;BbYAxis /&gt;
///     &lt;BbChartTooltip /&gt;
///     &lt;BbChartLegend /&gt;
///     &lt;BbLine DataKey="desktop" Name="Desktop" Color="var(--chart-1)" /&gt;
///     &lt;BbLine DataKey="mobile" Name="Mobile" Color="var(--chart-2)" Curve="CurveType.Smooth" /&gt;
/// &lt;/BbLineChart&gt;
/// </code>
/// </para>
/// </remarks>
public class BbLineChart : BbChartBase
{
    internal override string DataSlot => "line-chart";

    internal override string SeriesType => "line";

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
