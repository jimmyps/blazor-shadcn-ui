namespace NeoUI.Demo.Shared.Pages.Components.Charts;

partial class AreaChartExamples
{
    private static readonly IReadOnlyList<DemoPropRow> _areaChartProps =
    [
        new("Data",          "IEnumerable<TData>", "required",     "The data collection to render."),
        new("Padding",       "Padding",            "32,16,24,16",  "Plot-area padding (Top, Right, Bottom, Left)."),
        new("StackOffset",   "StackOffset",        "None",         "Stack offset mode. Use Expand for 100% stacking."),
        new("EnableAnimation","bool",              "true",         "Whether to animate on first render."),
        new("Class",         "string?",            "null",         "Additional CSS classes on the root element."),
        new("DataKey",       "string",             "required",     "Property name on the data item mapped to the X axis (on Area series)."),
        new("Name",          "string?",            "DataKey",      "Legend and tooltip series name."),
        new("Color",         "string?",            "auto",         "Line and fill color. Accepts any CSS color value."),
        new("StackId",       "string?",            "null",         "Stack group identifier. Areas sharing the same StackId are stacked."),
        new("ShowDots",      "bool",               "false",        "Show symbols at each data point."),
        new("LineWidth",     "int",                "2",            "Stroke width in pixels."),
        new("Opacity",       "double",             "1.0",          "Series opacity (0–1)."),
        new("Interpolation", "InterpolationType",  "Natural",      "Curve interpolation: Natural, Monotone, Linear, Step."),
    ];

    private const string _interactiveCode =
        """
        <div class="flex items-center gap-2 space-y-0 border-b pb-4 sm:flex-row">
            <div class="grid flex-1 gap-1">
                <p class="text-xl font-semibold">Area Chart — Interactive</p>
                <p class="text-sm text-muted-foreground">Showing total visitors for the last 3 months</p>
            </div>
            <Select @bind-Value="@timeRange" Class="w-[160px] rounded-lg sm:ml-auto sm:flex">
                <SelectTrigger>
                    <SelectValue Placeholder="Last 3 months" />
                </SelectTrigger>
                <SelectContent>
                    @foreach (var range in timeRanges)
                    {
                        <SelectItem Value="@range.Key" Text="@range.Value">@range.Value</SelectItem>
                    }
                </SelectContent>
            </Select>
        </div>
        <ChartContainer Height="250" Class="w-full">
            <AreaChart Data="@FilteredData" Padding="@(new Padding(32, 16, 0, 16))">
                <Grid Vertical="false" Stroke="var(--border)" />
                <XAxis DataKey="date" TickLine="false" Color="var(--border)">
                    <AxisLabel Color="var(--muted-foreground)" Formatter="@dateFormatter" />
                </XAxis>
                <YAxis Show="false" TickLine="false">
                    <AxisLabel Show="false" />
                </YAxis>
                <ChartTooltip Mode="TooltipMode.Axis" Cursor="TooltipCursor.Line" Formatter="@tooltipFormatter" />
                <Legend TextColor="var(--foreground)" />
                <Area DataKey="desktop" Name="Desktop" StackId="Total">
                    <Fill>
                        <LinearGradient Direction="GradientDirection.Vertical">
                            <Stop Offset="0.05" Color="var(--chart-1)" Opacity="0.8" />
                            <Stop Offset="0.95" Color="var(--chart-1)" Opacity="0.1" />
                        </LinearGradient>
                    </Fill>
                </Area>
                <Area DataKey="mobile" Name="Mobile" StackId="Total">
                    <Fill>
                        <LinearGradient Direction="GradientDirection.Vertical">
                            <Stop Offset="0.05" Color="var(--chart-2)" Opacity="0.8" />
                            <Stop Offset="0.95" Color="var(--chart-2)" Opacity="0.1" />
                        </LinearGradient>
                    </Fill>
                </Area>
            </AreaChart>
        </ChartContainer>
        """;

    private const string _defaultCode =
        """
        <ChartContainer Height="200">
            <AreaChart Data="@simpleData" Padding="ChartDefaults.DefaultPadding">
                <XAxis DataKey="month" Color="var(--border)">
                    <AxisLabel Color="var(--muted-foreground)" />
                </XAxis>
                <YAxis Color="var(--border)">
                    <AxisLabel Color="var(--muted-foreground)" />
                </YAxis>
                <ChartTooltip />
                <Area DataKey="sales" Name="Sales" Color="var(--chart-1)" />
            </AreaChart>
        </ChartContainer>
        """;

    private const string _stackedCode =
        """
        <ChartContainer Height="200">
            <AreaChart Data="@multiData" Padding="ChartDefaults.DefaultPadding">
                <XAxis DataKey="month" Color="var(--border)">
                    <AxisLabel Color="var(--muted-foreground)" />
                </XAxis>
                <YAxis Color="var(--border)">
                    <AxisLabel Color="var(--muted-foreground)" />
                </YAxis>
                <ChartTooltip />
                <Legend TextColor="var(--foreground)" />
                <Area DataKey="product1" Name="Product A" StackId="total" Color="var(--chart-1)" />
                <Area DataKey="product2" Name="Product B" StackId="total" Color="var(--chart-2)" />
            </AreaChart>
        </ChartContainer>
        """;

    private const string _gradientCode =
        """
        <ChartContainer Height="200">
            <AreaChart Data="@simpleData" Padding="ChartDefaults.DefaultPadding">
                <XAxis DataKey="month" Color="var(--border)">
                    <AxisLabel Color="var(--muted-foreground)" />
                </XAxis>
                <YAxis Color="var(--border)">
                    <AxisLabel Color="var(--muted-foreground)" />
                </YAxis>
                <ChartTooltip Cursor="TooltipCursor.Shadow" />
                <Area DataKey="sales" Name="Sales">
                    <Fill>
                        <LinearGradient Direction="GradientDirection.Vertical">
                            <Stop Offset="0" Color="var(--chart-1)" Opacity="0.8" />
                            <Stop Offset="1" Color="var(--chart-1)" Opacity="0.1" />
                        </LinearGradient>
                    </Fill>
                </Area>
            </AreaChart>
        </ChartContainer>
        """;

    private const string _withLegendCode =
        """
        <ChartContainer Height="200">
            <AreaChart Data="@multiData" Padding="@(new Padding(6, 0, 32, 0))">
                <XAxis DataKey="month" Color="var(--border)">
                    <AxisLabel Color="var(--muted-foreground)" />
                </XAxis>
                <YAxis Color="var(--border)">
                    <AxisLabel Color="var(--muted-foreground)" />
                </YAxis>
                <ChartTooltip />
                <Legend TextColor="var(--foreground)" Align="LegendAlign.Right" VerticalAlign="LegendVerticalAlign.Bottom" />
                <Area DataKey="product1" Name="Product A" Color="var(--chart-1)" />
                <Area DataKey="product2" Name="Product B" Color="var(--chart-2)" />
            </AreaChart>
        </ChartContainer>
        """;

    private const string _multipleCode =
        """
        <ChartContainer Height="200">
            <AreaChart Data="@multiData" Padding="ChartDefaults.DefaultPadding">
                <XAxis DataKey="month" Color="var(--border)">
                    <AxisLabel Color="var(--muted-foreground)" />
                </XAxis>
                <YAxis Color="var(--border)">
                    <AxisLabel Color="var(--muted-foreground)" />
                </YAxis>
                <ChartTooltip />
                <Legend TextColor="var(--foreground)" />
                <Area DataKey="product1" Name="Product A" Color="var(--chart-1)" />
                <Area DataKey="product2" Name="Product B" Color="var(--chart-2)" />
            </AreaChart>
        </ChartContainer>
        """;

    private const string _withDotsCode =
        """
        <ChartContainer Height="200">
            <AreaChart Data="@simpleData" Padding="ChartDefaults.DefaultPadding">
                <XAxis DataKey="month" Color="var(--border)">
                    <AxisLabel Color="var(--muted-foreground)" />
                </XAxis>
                <YAxis Color="var(--border)">
                    <AxisLabel Color="var(--muted-foreground)" />
                </YAxis>
                <ChartTooltip />
                <Area DataKey="sales" Name="Sales" ShowDots="true" Color="var(--chart-3)" />
            </AreaChart>
        </ChartContainer>
        """;

    private const string _withGridCode =
        """
        <ChartContainer Height="200">
            <AreaChart Data="@simpleData" Padding="ChartDefaults.DefaultPadding">
                <Grid Horizontal="true" Vertical="false" Stroke="var(--border)" />
                <XAxis DataKey="month" Color="var(--border)">
                    <AxisLabel Color="var(--muted-foreground)" />
                </XAxis>
                <YAxis Color="var(--border)">
                    <AxisLabel Color="var(--muted-foreground)" />
                </YAxis>
                <ChartTooltip />
                <Area DataKey="sales" Name="Sales" Color="var(--chart-4)" />
            </AreaChart>
        </ChartContainer>
        """;

    private const string _customAxesCode =
        """
        <ChartContainer Height="200">
            <AreaChart Data="@simpleData" Padding="ChartDefaults.DefaultPadding">
                <XAxis DataKey="month" Show="true" AxisLine="true" TickLine="true" Color="var(--border)">
                    <AxisLabel Color="var(--muted-foreground)" />
                </XAxis>
                <YAxis Show="true" Position="YAxisPosition.Right" AxisLine="true" TickLine="true" Color="var(--border)">
                    <AxisLabel Color="var(--muted-foreground)" />
                </YAxis>
                <ChartTooltip />
                <Area DataKey="sales" Name="Sales" Color="var(--chart-2)" Opacity="0.8" />
            </AreaChart>
        </ChartContainer>
        """;

    private const string _linearCode =
        """
        <ChartContainer Height="200">
            <AreaChart Data="@simpleData" Padding="ChartDefaults.DefaultPadding">
                <XAxis DataKey="month" Color="var(--border)">
                    <AxisLabel Color="var(--muted-foreground)" />
                </XAxis>
                <YAxis Color="var(--border)">
                    <AxisLabel Color="var(--muted-foreground)" />
                </YAxis>
                <ChartTooltip />
                <Area DataKey="sales" Name="Sales" LineWidth="3" Color="var(--chart-5)" />
            </AreaChart>
        </ChartContainer>
        """;

    private const string _stepCode =
        """
        <ChartContainer Height="200">
            <AreaChart Data="@simpleData" Padding="ChartDefaults.DefaultPadding">
                <XAxis DataKey="month" Color="var(--border)">
                    <AxisLabel Color="var(--muted-foreground)" />
                </XAxis>
                <YAxis Color="var(--border)">
                    <AxisLabel Color="var(--muted-foreground)" />
                </YAxis>
                <ChartTooltip />
                <Area DataKey="sales" Name="Sales" Interpolation="InterpolationType.Step" Color="var(--chart-1)" />
            </AreaChart>
        </ChartContainer>
        """;

    private const string _expandedCode =
        """
        <ChartContainer Height="200">
            <AreaChart Data="@multiData" StackOffset="StackOffset.Expand" Padding="ChartDefaults.DefaultPadding">
                <XAxis DataKey="month" Color="var(--border)">
                    <AxisLabel Color="var(--muted-foreground)" />
                </XAxis>
                <YAxis Color="var(--border)">
                    <AxisLabel Color="var(--muted-foreground)" />
                </YAxis>
                <ChartTooltip Formatter="function(params) { return params.seriesName + ': ' + params.value.toFixed(1) + '%'; }" />
                <Legend TextColor="var(--foreground)" />
                <Area DataKey="product1" Name="Product A" StackId="total" Color="var(--chart-1)" />
                <Area DataKey="product2" Name="Product B" StackId="total" Color="var(--chart-2)" />
                <Area DataKey="product3" Name="Product C" StackId="total" Color="var(--chart-3)" />
            </AreaChart>
        </ChartContainer>
        """;
}
