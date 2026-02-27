namespace NeoUI.Demo.Shared.Pages.Components.Charts;

partial class RadarChartExamples
{
    private static readonly IReadOnlyList<DemoPropRow> _radarChartProps =
    [
        new("Data",               "IEnumerable<TData>", "required", "The data collection to render."),
        new("DataKey",            "string",             "required", "Property on each data item mapped to the series value (on Radar)."),
        new("Name",               "string?",            "DataKey",  "Legend and tooltip series name."),
        new("Color",              "string?",            "auto",     "Line and fill color for the series."),
        new("FillArea",           "bool",               "false",    "Whether to fill the enclosed area."),
        new("AreaOpacity",        "double",             "0.3",      "Fill opacity when FillArea is true (0–1)."),
        new("Emphasis",           "bool",               "false",    "Whether to highlight the series on legend hover."),
        new("Shape",              "RadarShape",         "Polygon",  "Grid shape: Polygon or Circle (on RadarGrid)."),
        new("SplitNumber",        "int",                "5",        "Number of concentric rings in the grid (on RadarGrid)."),
        new("ShowIndicatorLabels","bool",               "true",     "Whether to render axis indicator labels (on RadarGrid)."),
        new("IndicatorColor",     "string?",            "null",     "CSS color for indicator label text (on RadarGrid)."),
        new("IndicatorFontSize",  "int",                "12",       "Font size for indicator labels in px (on RadarGrid)."),
    ];

    private const string _basicCode =
        """
        <ChartContainer Height="200">
            <RadarChart Data="@skillData">
                <XAxis DataKey="skill" />
                <ChartTooltip Show="false" />
                <Legend Show="false" />
                <Radar DataKey="score" FillArea="true" AreaOpacity="0.3" Color="var(--chart-1)" />
            </RadarChart>
        </ChartContainer>
        """;

    private const string _multipleSeriesCode =
        """
        <ChartContainer Height="200">
            <RadarChart Data="@teamSkillData">
                <XAxis DataKey="skill" />
                <ChartTooltip />
                <Legend TextColor="var(--foreground)" />
                <RadarGrid Radius="50%" Center="@(new string[] { "50%", "65%" })" />
                <Radar DataKey="developer1" Name="Dev A" FillArea="true" AreaOpacity="0.2" Color="var(--chart-1)" />
                <Radar DataKey="developer2" Name="Dev B" FillArea="true" AreaOpacity="0.2" Color="var(--chart-2)" />
            </RadarChart>
        </ChartContainer>
        """;

    private const string _filledCode =
        """
        <ChartContainer Height="200">
            <RadarChart Data="@skillData">
                <XAxis DataKey="skill" />
                <ChartTooltip Show="false" />
                <Legend Show="false" />
                <Radar DataKey="score" FillArea="true" AreaOpacity="0.6" Color="var(--chart-3)" />
            </RadarChart>
        </ChartContainer>
        """;

    private const string _circleGridCode =
        """
        <ChartContainer Height="200">
            <RadarChart Data="@skillData">
                <RadarGrid Shape="RadarShape.Circle" SplitNumber="4" />
                <XAxis DataKey="skill" />
                <ChartTooltip Show="false" />
                <Legend Show="false" />
                <Radar DataKey="score" FillArea="true" AreaOpacity="0.3" Color="var(--chart-4)" />
            </RadarChart>
        </ChartContainer>
        """;

    private const string _performanceCode =
        """
        <ChartContainer Height="200">
            <RadarChart Data="@performanceData">
                <XAxis DataKey="metric" />
                <ChartTooltip />
                <Legend TextColor="var(--foreground)" />
                <RadarGrid Radius="60%" Center="@(new string[] { "50%", "65%" })" />
                <Radar DataKey="sales" Name="Sales" FillArea="true" AreaOpacity="0.25" Color="var(--chart-1)" />
                <Radar DataKey="marketing" Name="Marketing" FillArea="true" AreaOpacity="0.25" Color="var(--chart-2)" />
            </RadarChart>
        </ChartContainer>
        """;

    private const string _legendEmphasisCode =
        """
        <ChartContainer Height="200">
            <RadarChart Data="@productComparison">
                <XAxis DataKey="feature" />
                <ChartTooltip />
                <Legend TextColor="var(--foreground)" VerticalAlign="LegendVerticalAlign.Bottom" Icon="LegendIcon.Diamond" />
                <RadarGrid Radius="50%" />
                <Radar DataKey="productA" Name="Product A" Emphasis="true" FillArea="true" AreaOpacity="0.2" Color="var(--chart-3)" />
                <Radar DataKey="productB" Name="Product B" Emphasis="true" FillArea="true" AreaOpacity="0.2" Color="var(--chart-4)" />
                <Radar DataKey="productC" Name="Product C" Emphasis="true" FillArea="true" AreaOpacity="0.2" Color="var(--chart-5)" />
            </RadarChart>
        </ChartContainer>
        """;

    private const string _polygonGridCode =
        """
        <ChartContainer Height="200">
            <RadarChart Data="@skillData">
                <RadarGrid Shape="RadarShape.Polygon" SplitNumber="5" />
                <XAxis DataKey="skill" />
                <ChartTooltip Show="false" />
                <Legend Show="false" />
                <Radar DataKey="score" FillArea="true" AreaOpacity="0.3" Color="var(--chart-5)" />
            </RadarChart>
        </ChartContainer>
        """;

    private const string _lineOnlyCode =
        """
        <ChartContainer Height="200">
            <RadarChart Data="@skillData">
                <XAxis DataKey="skill" />
                <ChartTooltip Show="false" />
                <Legend Show="false" />
                <Radar DataKey="score" FillArea="false" Color="var(--chart-2)" />
            </RadarChart>
        </ChartContainer>
        """;

    private const string _styledIndicatorsCode =
        """
        <ChartContainer Height="200">
            <RadarChart Data="@skillData">
                <XAxis DataKey="skill" />
                <RadarGrid
                    IndicatorColor="var(--foreground)"
                    IndicatorFontSize="12"
                    IndicatorFontWeight="bold" />
                <ChartTooltip Show="false" />
                <Legend Show="false" />
                <Radar DataKey="score" FillArea="true" AreaOpacity="0.3" Color="var(--chart-3)" />
            </RadarChart>
        </ChartContainer>
        """;

    private const string _hiddenIndicatorsCode =
        """
        <ChartContainer Height="200">
            <RadarChart Data="@skillData">
                <XAxis DataKey="skill" />
                <RadarGrid ShowIndicatorLabels="false" />
                <ChartTooltip Show="false" />
                <Legend Show="false" />
                <Radar DataKey="score" FillArea="true" AreaOpacity="0.4" Color="var(--chart-4)" />
            </RadarChart>
        </ChartContainer>
        """;
}
