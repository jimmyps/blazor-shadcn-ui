using Microsoft.AspNetCore.Components;

namespace BlazorBlueprint.Components;

/// <summary>
/// A radar chart component using the declarative composition API with Apache ECharts.
/// </summary>
/// <remarks>
/// <para>
/// The RadarChart component (also known as spider or web chart) displays multivariate
/// data on axes starting from the same point. Configure radar indicators via the
/// <see cref="IndicatorKey"/> parameter to auto-extract from data, or provide explicit
/// <see cref="Indicators"/> for full control.
/// </para>
/// <para>
/// Usage:
/// <code>
/// &lt;BbRadarChart Data="@data" Height="350px" IndicatorKey="skill" MaxValue="100"&gt;
///     &lt;BbChartTooltip /&gt;
///     &lt;BbRadar DataKey="score" Name="Team A" Color="var(--chart-1)" FillOpacity="0.3" /&gt;
/// &lt;/BbRadarChart&gt;
/// </code>
/// </para>
/// </remarks>
public class BbRadarChart : BbChartBase
{
    /// <summary>
    /// Gets or sets the radar shape.
    /// </summary>
    /// <remarks>
    /// Use <see cref="RadarShape.Polygon"/> for angular shapes or <see cref="RadarShape.Circle"/> for circular shapes.
    /// Default is <see cref="RadarShape.Polygon"/>.
    /// </remarks>
    [Parameter]
    public RadarShape Shape { get; set; } = RadarShape.Polygon;

    /// <summary>
    /// Gets or sets explicit radar indicators.
    /// </summary>
    /// <remarks>
    /// When provided, these override auto-extraction from data.
    /// Each item should have a "name" and optionally a "max" property.
    /// </remarks>
    [Parameter]
    public IEnumerable<EChartsRadarIndicator>? Indicators { get; set; }

    /// <summary>
    /// Gets or sets the property name used to extract indicator names from data.
    /// </summary>
    /// <remarks>
    /// Used for auto-building radar indicators from the chart data.
    /// Each distinct value of this property becomes an indicator axis.
    /// </remarks>
    [Parameter]
    public string? IndicatorKey { get; set; }

    /// <summary>
    /// Gets or sets the maximum value for all radar axes.
    /// </summary>
    /// <remarks>
    /// Applied to all auto-generated indicators. When using explicit <see cref="Indicators"/>,
    /// set max on each indicator individually.
    /// </remarks>
    [Parameter]
    public double? MaxValue { get; set; }

    /// <summary>
    /// Gets or sets whether the axis lines from center to each vertex are shown.
    /// </summary>
    [Parameter]
    public bool ShowAxisLines { get; set; } = true;

    /// <summary>
    /// Gets or sets whether the polygon/circle grid lines are shown.
    /// </summary>
    [Parameter]
    public bool ShowGridLines { get; set; } = true;

    /// <summary>
    /// Gets or sets whether the split areas between grid lines are filled.
    /// </summary>
    [Parameter]
    public bool GridFill { get; set; }

    internal override string DataSlot => "radar-chart";

    internal override string SeriesType => "radar";

    internal override void ApplyChartDefaults(EChartsOption option)
    {
        // Radar charts don't use standard XAxis, YAxis, or Grid
        option.XAxis = null;
        option.YAxis = null;
        option.Grid = null;

        // Build the radar coordinate system
        var indicators = BuildIndicators();
        var hasRichText = indicators.Exists(i => i.Name.Contains("{a|", StringComparison.Ordinal));

        var axisName = new EChartsAxisLabelOption
        {
            Color = "var(--muted-foreground)"
        };

        if (hasRichText)
        {
            axisName.Rich = new Dictionary<string, EChartsRichStyleOption>
            {
                ["a"] = new EChartsRichStyleOption
                {
                    FontSize = 12,
                    FontWeight = "bold",
                    Color = "var(--foreground)",
                    LineHeight = 22,
                    Align = "center"
                },
                ["b"] = new EChartsRichStyleOption
                {
                    FontSize = 12,
                    Color = "var(--muted-foreground)",
                    Align = "center"
                }
            };
        }

        var radar = new EChartsRadarOption
        {
            Shape = Shape switch
            {
                RadarShape.Circle => "circle",
                _ => "polygon"
            },
            Indicator = indicators,
            AxisLine = new EChartsAxisLineOption
            {
                Show = ShowAxisLines,
                LineStyle = new EChartsLineStyleOption
                {
                    Color = "var(--border)"
                }
            },
            SplitLine = new EChartsSplitLineOption
            {
                Show = ShowGridLines,
                LineStyle = new EChartsLineStyleOption
                {
                    Color = "var(--border)"
                }
            },
            AxisName = axisName
        };

        if (GridFill)
        {
            radar.SplitArea = new EChartsSplitAreaOption
            {
                Show = true,
                AreaStyle = new EChartsAreaStyleOption
                {
                    Color = new object[] { "var(--muted)", "transparent" }
                }
            };
        }

        option.Radar = radar;
    }

    private List<EChartsRadarIndicator> BuildIndicators()
    {
        if (Indicators != null)
        {
            return [.. Indicators];
        }

        if (string.IsNullOrEmpty(IndicatorKey))
        {
            return [];
        }

        var names = DataExtractor.ExtractStringValues(Data, IndicatorKey);
        var indicators = new List<EChartsRadarIndicator>(names.Count);

        foreach (var name in names)
        {
            indicators.Add(new EChartsRadarIndicator
            {
                Name = name,
                Max = MaxValue
            });
        }

        return indicators;
    }
}
