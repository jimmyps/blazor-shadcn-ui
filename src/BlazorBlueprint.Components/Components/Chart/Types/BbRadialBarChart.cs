using Microsoft.AspNetCore.Components;

namespace BlazorBlueprint.Components;

/// <summary>
/// A radial bar chart component using the declarative composition API with Apache ECharts.
/// </summary>
/// <remarks>
/// <para>
/// The RadialBarChart component displays values as circular arcs around a center point
/// using ECharts polar coordinates. It's ideal for showing progress, completion percentages,
/// or comparative values.
/// </para>
/// <para>
/// Usage:
/// <code>
/// &lt;BbRadialBarChart Data="@data" Height="250px"&gt;
///     &lt;BbChartTooltip /&gt;
///     &lt;BbRadialBar DataKey="visitors" NameKey="browser" /&gt;
/// &lt;/BbRadialBarChart&gt;
/// </code>
/// </para>
/// </remarks>
public class BbRadialBarChart : BbChartBase
{
    /// <summary>
    /// Gets or sets the starting angle in degrees.
    /// </summary>
    /// <remarks>
    /// Default is 90 (top of the circle). ECharts measures angles counter-clockwise from 3 o'clock.
    /// </remarks>
    [Parameter]
    public int StartAngle { get; set; } = 90;

    /// <summary>
    /// Gets or sets the ending angle in degrees.
    /// </summary>
    /// <remarks>
    /// Default is -270 for a full circle. Set to 0 for a half circle.
    /// </remarks>
    [Parameter]
    public int EndAngle { get; set; } = -270;

    /// <summary>
    /// Gets or sets the inner radius of the polar coordinate system as a percentage.
    /// </summary>
    [Parameter]
    public string InnerRadius { get; set; } = "30%";

    /// <summary>
    /// Gets or sets the outer radius of the polar coordinate system as a percentage.
    /// </summary>
    [Parameter]
    public string OuterRadius { get; set; } = "80%";

    internal override string DataSlot => "radial-bar-chart";

    internal override string SeriesType => "bar";

    internal override void ApplyChartDefaults(EChartsOption option)
    {
        option.XAxis = null;
        option.YAxis = null;
        option.Grid = null;

        option.Polar = new EChartsPolarOption
        {
            Radius = new List<string> { InnerRadius, OuterRadius }
        };

        option.AngleAxis = new EChartsAngleAxisOption
        {
            StartAngle = StartAngle,
            Show = false
        };

        option.RadiusAxis = new EChartsRadiusAxisOption
        {
            Type = "category",
            Show = false
        };
    }
}
