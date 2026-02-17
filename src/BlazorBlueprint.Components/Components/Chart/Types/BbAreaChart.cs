namespace BlazorBlueprint.Components;

/// <summary>
/// An area chart component using the declarative composition API with Apache ECharts.
/// </summary>
/// <remarks>
/// <para>
/// The AreaChart component provides area-based visualizations that emphasize volume
/// or magnitude. Area charts use the same ECharts "line" series type but with areaStyle
/// applied by the Area series component.
/// </para>
/// <para>
/// Usage:
/// <code>
/// &lt;BbAreaChart Data="@data" Height="350px"&gt;
///     &lt;BbXAxis DataKey="month" /&gt;
///     &lt;BbYAxis /&gt;
///     &lt;BbChartTooltip /&gt;
///     &lt;BbArea DataKey="desktop" Name="Desktop" Color="var(--chart-1)" /&gt;
///     &lt;BbArea DataKey="mobile" Name="Mobile" Color="var(--chart-2)" Stacked="true" /&gt;
/// &lt;/BbAreaChart&gt;
/// </code>
/// </para>
/// </remarks>
public class BbAreaChart : BbChartBase
{
    internal override string DataSlot => "area-chart";

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
