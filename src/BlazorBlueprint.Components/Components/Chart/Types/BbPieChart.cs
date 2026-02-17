namespace BlazorBlueprint.Components;

/// <summary>
/// A pie/donut chart component using the declarative composition API with Apache ECharts.
/// </summary>
/// <remarks>
/// <para>
/// The PieChart component provides circular visualizations for showing proportions
/// and part-to-whole relationships. Control the inner/outer radius on the Pie series
/// to create donut variants. Use CenterLabel for rich center text.
/// </para>
/// <para>
/// Usage:
/// <code>
/// &lt;BbPieChart Data="@data" Height="350px"&gt;
///     &lt;BbChartTooltip /&gt;
///     &lt;BbPie DataKey="value" NameKey="name" InnerRadius="60%" OuterRadius="80%"&gt;
///         &lt;BbCenterLabel Title="Total" Value="1,234" /&gt;
///     &lt;/BbPie&gt;
/// &lt;/BbPieChart&gt;
/// </code>
/// </para>
/// </remarks>
public class BbPieChart : BbChartBase
{
    internal override string DataSlot => "pie-chart";

    internal override string SeriesType => "pie";

    internal override void ApplyChartDefaults(EChartsOption option)
    {
        // Pie charts don't use XAxis, YAxis, or Grid
        option.XAxis = null;
        option.YAxis = null;
        option.Grid = null;
    }
}
