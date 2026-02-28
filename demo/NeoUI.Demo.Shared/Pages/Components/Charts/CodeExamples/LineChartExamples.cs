namespace NeoUI.Demo.Shared.Pages.Components.Charts;

partial class LineChartExamples
{
    private static readonly IReadOnlyList<DemoPropRow> _lineChartProps =
    [
        new("Data",            "IEnumerable<TData>", "required",   "The data collection to render."),
        new("Padding",         "Padding",            "32,16,24,16","Plot-area padding (Top, Right, Bottom, Left)."),
        new("EnableAnimation", "bool",               "true",       "Whether to animate on first render."),
        new("Class",           "string?",            "null",       "Additional CSS classes on the root element."),
        new("DataKey",         "string",             "required",   "Property name for line values (on Line series)."),
        new("Name",            "string?",            "DataKey",    "Legend and tooltip series name."),
        new("Color",           "string?",            "auto",       "Line color. Accepts any CSS color value."),
        new("LineWidth",       "int",                "2",          "Stroke width in pixels."),
        new("Dashed",          "bool",               "false",      "Whether to render the line as a dashed stroke."),
        new("ShowDots",        "bool",               "false",      "Show symbols at each data point."),
        new("DotSize",         "int",                "6",          "Symbol size in pixels when ShowDots is true."),
        new("DotShape",        "SymbolShape",        "Circle",     "Symbol shape when ShowDots is true."),
        new("Interpolation",   "InterpolationType",  "Natural",    "Curve interpolation: Natural, Monotone, Linear, Step."),
        new("ShowLabel",       "bool",               "false",      "Whether to show data labels on each point."),
        new("LabelPosition",   "LabelPosition",      "Top",        "Position of the data label relative to the point."),
    ];

    private const string _interactiveCode =
        """
        <div class="flex items-center gap-2 space-y-0 border-b pb-4 sm:flex-row">
            <div class="grid flex-1 gap-1">
                <p class="text-xl font-semibold">Line Chart — Interactive</p>
                <p class="text-sm text-muted-foreground">Showing trends for the last 3 months</p>
            </div>
            <Select @bind-Value="@timeRange" TValue="string" Class="w-[160px] rounded-lg sm:ml-auto">
                <SelectTrigger><SelectValue Placeholder="Last 3 months" /></SelectTrigger>
                <SelectContent>
                    @foreach (var range in timeRanges)
                    {
                        <SelectItem Value="@range.Key" TValue="string" Text="@range.Value">@range.Value</SelectItem>
                    }
                </SelectContent>
            </Select>
        </div>
        <ChartContainer Height="250" Class="w-full">
            <LineChart Data="@FilteredData" Padding="@(new Padding(32, 16, 0, 16))">
                <Grid Vertical="false" Stroke="var(--border)" />
                <XAxis DataKey="date" TickLine="false" Color="var(--border)">
                    <AxisLabel Color="var(--muted-foreground)" Formatter="@dateFormatter" />
                </XAxis>
                
                <ChartTooltip Mode="TooltipMode.Axis" Cursor="TooltipCursor.Cross" Formatter="@tooltipFormatter" />
                <Legend TextColor="var(--foreground)" />
                <Line DataKey="desktop" Name="Desktop" Color="var(--chart-1)" />
                <Line DataKey="mobile" Name="Mobile" Color="var(--chart-2)" />
            </LineChart>
        </ChartContainer>
        """;

    private const string _defaultCode =
        """
        <ChartContainer Height="200">
            <LineChart Data="@simpleData" Padding="new Padding(8, 0, 0, 0)">
                <Grid Stroke="var(--border)" />
                <XAxis DataKey="month" Color="var(--border)"><AxisLabel Color="var(--muted-foreground)" /></XAxis>
                <YAxis Color="var(--border)"><AxisLabel Color="var(--muted-foreground)" /></YAxis>
                <ChartTooltip />
                <Line DataKey="sales" Name="Sales" Color="var(--chart-1)" />
            </LineChart>
        </ChartContainer>
        """;

    private const string _multipleCode =
        """
        <ChartContainer Height="200">
            <LineChart Data="@multiData" Padding="@(new Padding(6, 0, 32, 0))">
                <Grid Show="false" />
                <XAxis DataKey="month" Color="var(--border)"><AxisLabel Color="var(--muted-foreground)" /></XAxis>
                <YAxis Interval="100" Position="YAxisPosition.Right" Color="var(--border)">
                    <AxisLabel Color="var(--muted-foreground)" />
                </YAxis>
                <ChartTooltip />
                <Legend TextColor="var(--foreground)" VerticalAlign="LegendVerticalAlign.Bottom" Icon="LegendIcon.Circle" />
                <Line DataKey="product1" Name="Product A" Color="var(--chart-1)" />
                <Line DataKey="product2" Name="Product B" Color="var(--chart-2)" />
            </LineChart>
        </ChartContainer>
        """;

    private const string _withDotsCode =
        """
        <ChartContainer Height="200">
            <LineChart Data="@simpleData" Padding="new Padding(8, 0, 0, 0)">
                <Grid Stroke="var(--border)" />
                <XAxis DataKey="month" Color="var(--border)"><AxisLabel Color="var(--muted-foreground)" /></XAxis>
                <YAxis Color="var(--border)"><AxisLabel Color="var(--muted-foreground)" /></YAxis>
                <ChartTooltip />
                <Line DataKey="sales" Name="Sales" ShowDots="true" Color="var(--chart-3)" />
            </LineChart>
        </ChartContainer>
        """;

    private const string _customDotColorsCode =
        """
        <ChartContainer Height="200">
            <LineChart Data="@multiData" Padding="new Padding(8, 0, 0, 0)">
                <Grid Stroke="var(--border)" />
                <XAxis DataKey="month" Color="var(--border)"><AxisLabel Color="var(--muted-foreground)" /></XAxis>
                <YAxis Color="var(--border)"><AxisLabel Color="var(--muted-foreground)" /></YAxis>
                <ChartTooltip />
                <Legend TextColor="var(--foreground)" />
                <Line DataKey="product1" Name="Product A" ShowDots="true" Color="var(--chart-1)" />
                <Line DataKey="product2" Name="Product B" ShowDots="true" Color="var(--chart-2)" />
            </LineChart>
        </ChartContainer>
        """;

    private const string _customDotsCode =
        """
        <ChartContainer Height="200">
            <LineChart Data="@simpleData" Padding="new Padding(8, 0, 0, 0)">
                <Grid Stroke="var(--border)" />
                <XAxis DataKey="month" Color="var(--border)"><AxisLabel Color="var(--muted-foreground)" /></XAxis>
                <YAxis Color="var(--border)"><AxisLabel Color="var(--muted-foreground)" /></YAxis>
                <ChartTooltip />
                <Line DataKey="sales" Name="Sales" ShowDots="true" DotSize="8" DotShape="SymbolShape.Diamond" Color="var(--chart-4)" />
            </LineChart>
        </ChartContainer>
        """;

    private const string _linearCode =
        """
        <ChartContainer Height="200">
            <LineChart Data="@simpleData" Padding="new Padding(8, 0, 0, 0)">
                <Grid Stroke="var(--border)" />
                <XAxis DataKey="month" Color="var(--border)"><AxisLabel Color="var(--muted-foreground)" /></XAxis>
                <YAxis Color="var(--border)"><AxisLabel Color="var(--muted-foreground)" /></YAxis>
                <ChartTooltip />
                <Line DataKey="sales" Name="Sales" Interpolation="InterpolationType.Linear" Color="var(--chart-5)" />
            </LineChart>
        </ChartContainer>
        """;

    private const string _stepCode =
        """
        <ChartContainer Height="200">
            <LineChart Data="@simpleData" Padding="new Padding(8, 0, 0, 0)">
                <Grid Stroke="var(--border)" />
                <XAxis DataKey="month" Color="var(--border)"><AxisLabel Color="var(--muted-foreground)" /></XAxis>
                <YAxis Color="var(--border)"><AxisLabel Color="var(--muted-foreground)" /></YAxis>
                <ChartTooltip />
                <Line DataKey="sales" Name="Sales" Interpolation="InterpolationType.Step" Color="var(--chart-1)" />
            </LineChart>
        </ChartContainer>
        """;

    private const string _withLabelsCode =
        """
        <ChartContainer Height="200">
            <LineChart Data="@simpleData" Padding="new Padding(8, 0, 0, 0)">
                <Grid Stroke="var(--border)" />
                <XAxis DataKey="month" Color="var(--border)"><AxisLabel Color="var(--muted-foreground)" /></XAxis>
                <YAxis Color="var(--border)"><AxisLabel Color="var(--muted-foreground)" /></YAxis>
                <ChartTooltip />
                <Line DataKey="sales" Name="Sales" ShowDots="true" ShowLabel="true" LabelPosition="LabelPosition.Top" Color="var(--chart-2)">
                    <LabelList Position="LabelPosition.Top" Formatter="{c}" FontSize="10" Color="#666" />
                </Line>
            </LineChart>
        </ChartContainer>
        """;

    private const string _customLabelsCode =
        """
        <ChartContainer Height="200">
            <LineChart Data="@simpleData" Padding="new Padding(8, 0, 0, 0)">
                <Grid Stroke="var(--border)" />
                <XAxis DataKey="month" Color="var(--border)"><AxisLabel Color="var(--muted-foreground)" /></XAxis>
                <YAxis Color="var(--border)"><AxisLabel Color="var(--muted-foreground)" /></YAxis>
                <ChartTooltip />
                <Line DataKey="sales" Name="Sales" ShowDots="true" ShowLabel="true" Color="var(--chart-3)">
                    <LabelList Position="LabelPosition.Top" Formatter="function(params) { return '$' + params.value; }" FontSize="9" Color="#2563eb" />
                </Line>
            </LineChart>
        </ChartContainer>
        """;

    private const string _dashedCode =
        """
        <ChartContainer Height="200">
            <LineChart Data="@simpleData" Padding="new Padding(8, 0, 0, 0)">
                <Grid Stroke="var(--border)" />
                <XAxis DataKey="month" Color="var(--border)"><AxisLabel Color="var(--muted-foreground)" /></XAxis>
                <YAxis Color="var(--border)"><AxisLabel Color="var(--muted-foreground)" /></YAxis>
                <ChartTooltip />
                <Line DataKey="sales" Name="Forecast" Dashed="true" ShowDots="true" Color="var(--chart-4)" />
            </LineChart>
        </ChartContainer>
        """;
}
