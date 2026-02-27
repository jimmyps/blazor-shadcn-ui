namespace NeoUI.Demo.Shared.Pages.Components.Charts;

partial class ScatterChartExamples
{
    private static readonly IReadOnlyList<DemoPropRow> _scatterChartProps =
    [
        new("Data",        "IEnumerable<TData>", "required",  "The data collection to render."),
        new("DataKey",     "string",             "required",  "Property name mapped to each point's primary value (on Scatter series)."),
        new("Name",        "string?",            "DataKey",   "Legend and tooltip series name."),
        new("Color",       "string?",            "auto",      "Symbol color. Accepts any CSS color value."),
        new("Symbol",      "SymbolShape",        "Circle",    "Symbol shape: Circle, Rect, RoundRect, Triangle, Diamond, Pin, Arrow."),
        new("SymbolSize",  "int",                "8",         "Symbol size in pixels."),
        new("Opacity",     "double",             "1.0",       "Series opacity (0–1)."),
        new("Class",       "string?",            "null",      "Additional CSS classes on the root element."),
    ];

    private const string _basicCode =
        """
        <ChartContainer Height="200">
            <ScatterChart Data="@heightWeightData">
                <Grid Stroke="var(--border)" />
                <XAxis DataKey="height" Color="var(--border)">
                    <AxisLabel Color="var(--muted-foreground)" />
                </XAxis>
                <YAxis DataKey="weight" Color="var(--border)">
                    <AxisLabel Color="var(--muted-foreground)" />
                </YAxis>
                <ChartTooltip />
                <Legend Show="false" />
                <Scatter DataKey="weight" SymbolSize="8" Symbol="SymbolShape.Circle" Color="var(--chart-1)" />
            </ScatterChart>
        </ChartContainer>
        """;

    private const string _multipleSeriesCode =
        """
        <ChartContainer Height="200">
            <ScatterChart Data="@customerData">
                <Grid Stroke="var(--border)" />
                <XAxis DataKey="age" Color="var(--border)">
                    <AxisLabel Color="var(--muted-foreground)" />
                </XAxis>
                <YAxis DataKey="spending" Color="var(--border)">
                    <AxisLabel Color="var(--muted-foreground)" />
                </YAxis>
                <ChartTooltip />
                <Legend TextColor="var(--foreground)" />
                <Scatter DataKey="premium" Name="Premium" SymbolSize="8" Color="var(--chart-1)" />
                <Scatter DataKey="standard" Name="Standard" SymbolSize="8" Color="var(--chart-2)" />
            </ScatterChart>
        </ChartContainer>
        """;

    private const string _bubbleCode =
        """
        <ChartContainer Height="200">
            <ScatterChart Data="@salesData">
                <Grid Stroke="var(--border)" />
                <XAxis DataKey="sales" Color="var(--border)">
                    <AxisLabel Color="var(--muted-foreground)" />
                </XAxis>
                <YAxis DataKey="customers" Color="var(--border)">
                    <AxisLabel Color="var(--muted-foreground)" />
                </YAxis>
                <ChartTooltip />
                <Legend Show="false" />
                <Scatter DataKey="customers" SymbolSize="12" Color="var(--chart-3)" />
            </ScatterChart>
        </ChartContainer>
        """;

    private const string _customSymbolsCode =
        """
        <ChartContainer Height="200">
            <ScatterChart Data="@symbolData">
                <Grid Stroke="var(--border)" />
                <XAxis DataKey="x" Color="var(--border)">
                    <AxisLabel Color="var(--muted-foreground)" />
                </XAxis>
                <YAxis DataKey="y" Color="var(--border)">
                    <AxisLabel Color="var(--muted-foreground)" />
                </YAxis>
                <ChartTooltip />
                <Legend TextColor="var(--foreground)" />
                <Scatter DataKey="diamonds" Name="Diamond" SymbolSize="10" Symbol="SymbolShape.Diamond" Color="var(--chart-1)" />
                <Scatter DataKey="triangles" Name="Triangle" SymbolSize="10" Symbol="SymbolShape.Triangle" Color="var(--chart-2)" />
            </ScatterChart>
        </ChartContainer>
        """;

    private const string _withGridCode =
        """
        <ChartContainer Height="200">
            <ScatterChart Data="@heightWeightData">
                <Grid Horizontal="true" Vertical="true" Stroke="var(--border)" />
                <XAxis DataKey="height" Color="var(--border)">
                    <AxisLabel Color="var(--muted-foreground)" />
                </XAxis>
                <YAxis DataKey="weight" Color="var(--border)">
                    <AxisLabel Color="var(--muted-foreground)" />
                </YAxis>
                <ChartTooltip />
                <Legend Show="false" />
                <Scatter DataKey="weight" SymbolSize="8" Color="var(--chart-4)" />
            </ScatterChart>
        </ChartContainer>
        """;

    private const string _largeDatasetCode =
        """
        <ChartContainer Height="200">
            <ScatterChart Data="@largeDataset">
                <Grid Stroke="var(--border)" />
                <XAxis DataKey="x" Color="var(--border)">
                    <AxisLabel Color="var(--muted-foreground)" />
                </XAxis>
                <YAxis DataKey="y" Color="var(--border)">
                    <AxisLabel Color="var(--muted-foreground)" />
                </YAxis>
                <ChartTooltip />
                <Legend Show="false" />
                <Scatter DataKey="y" SymbolSize="4" Color="var(--chart-5)" />
            </ScatterChart>
        </ChartContainer>
        """;

    private const string _trendCode =
        """
        <ChartContainer Height="200">
            <ScatterChart Data="@trendData">
                <Grid Stroke="var(--border)" />
                <XAxis DataKey="experience" Color="var(--border)">
                    <AxisLabel Color="var(--muted-foreground)" />
                </XAxis>
                <YAxis DataKey="salary" Color="var(--border)">
                    <AxisLabel Color="var(--muted-foreground)" />
                </YAxis>
                <ChartTooltip />
                <Legend Show="false" />
                <Scatter DataKey="salary" SymbolSize="8" Color="var(--chart-1)" />
            </ScatterChart>
        </ChartContainer>
        """;

    private const string _colorVariationsCode =
        """
        <ChartContainer Height="200">
            <ScatterChart Data="@colorData">
                <Grid Stroke="var(--border)" />
                <XAxis DataKey="x" Color="var(--border)">
                    <AxisLabel Color="var(--muted-foreground)" />
                </XAxis>
                <YAxis DataKey="y" Color="var(--border)">
                    <AxisLabel Color="var(--muted-foreground)" />
                </YAxis>
                <ChartTooltip />
                <Legend TextColor="var(--foreground)" />
                <Scatter DataKey="series1" Name="Series A" SymbolSize="8" Color="var(--chart-3)" />
                <Scatter DataKey="series2" Name="Series B" SymbolSize="8" Color="var(--chart-4)" />
                <Scatter DataKey="series3" Name="Series C" SymbolSize="8" Color="var(--chart-5)" />
            </ScatterChart>
        </ChartContainer>
        """;
}
