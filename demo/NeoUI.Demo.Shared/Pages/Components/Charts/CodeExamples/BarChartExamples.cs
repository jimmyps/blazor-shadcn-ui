namespace NeoUI.Demo.Shared.Pages.Components.Charts;

partial class BarChartExamples
{
    private static readonly IReadOnlyList<DemoPropRow> _barChartProps =
    [
        new("Data",            "IEnumerable<TData>", "required",   "The data collection to render."),
        new("Padding",         "Padding",            "32,16,24,16","Plot-area padding (Top, Right, Bottom, Left)."),
        new("Layout",          "BarLayout",          "Vertical",   "Bar orientation: Vertical or Horizontal."),
        new("StackOffset",     "StackOffset",        "None",       "Stack offset mode. Use Expand for 100% stacking."),
        new("EnableAnimation", "bool",               "true",       "Whether to animate on first render."),
        new("Class",           "string?",            "null",       "Additional CSS classes on the root element."),
        new("DataKey",         "string",             "required",   "Property name mapped to bar values (on Bar series)."),
        new("Name",            "string?",            "DataKey",    "Legend and tooltip series name."),
        new("Color",           "string?",            "auto",       "Bar fill color. Accepts any CSS color value."),
        new("Radius",          "int",                "0",          "Corner radius of bar tops in pixels."),
        new("StackId",         "string?",            "null",       "Stack group identifier. Bars sharing the same StackId are stacked."),
        new("ShowLabel",       "bool",               "false",      "Whether to show data labels on bars."),
        new("LabelPosition",   "LabelPosition",      "Top",        "Position of the data label relative to the bar."),
    ];

    private const string _interactiveCode =
        """
        <div class="flex items-center gap-2 space-y-0 border-b pb-4 sm:flex-row">
            <div class="grid flex-1 gap-1">
                <p class="text-xl font-semibold">Bar Chart — Interactive</p>
                <p class="text-sm text-muted-foreground">Showing sales data for the last 30 days</p>
            </div>
            <Select @bind-Value="@timeRange" Class="w-[160px] rounded-lg sm:ml-auto">
                <SelectTrigger><SelectValue Placeholder="Last 30 days" /></SelectTrigger>
                <SelectContent>
                    @foreach (var range in timeRanges)
                    {
                        <SelectItem Value="@range.Key" Text="@range.Value">@range.Value</SelectItem>
                    }
                </SelectContent>
            </Select>
        </div>
        <ChartContainer Height="250" Class="w-full">
            <BarChart Data="@FilteredData" Padding="@(new Padding(32, 16, 0, 16))">
                <Grid Vertical="false" Stroke="var(--border)" />
                <XAxis DataKey="date" TickLine="false" Color="var(--border)">
                    <AxisLabel Color="var(--muted-foreground)" Formatter="@dateFormatter" />
                </XAxis>
                
                <ChartTooltip Mode="TooltipMode.Axis" Cursor="TooltipCursor.Shadow" />
                <Legend TextColor="var(--foreground)" />
                <Bar DataKey="desktop" Name="Desktop" Radius="4" Color="var(--chart-1)" />
                <Bar DataKey="mobile" Name="Mobile" Radius="4" Color="var(--chart-2)" />
            </BarChart>
        </ChartContainer>
        """;

    private const string _defaultCode =
        """
        <ChartContainer Height="200">
            <BarChart Data="@simpleData" Padding="ChartDefaults.DefaultPadding">
                <Grid Stroke="var(--border)" />
                <XAxis DataKey="month" Color="var(--border)"><AxisLabel Color="var(--muted-foreground)" /></XAxis>
                <YAxis Color="var(--border)"><AxisLabel Color="var(--muted-foreground)" /></YAxis>
                <ChartTooltip />
                <Bar DataKey="sales" Name="Sales" Radius="4" Color="var(--chart-1)" />
            </BarChart>
        </ChartContainer>
        """;

    private const string _horizontalCode =
        """
        <ChartContainer Height="200">
            <BarChart Data="@simpleData" Layout="BarLayout.Horizontal" Padding="ChartDefaults.DefaultPadding">
                <Grid Stroke="var(--border)" />
                <XAxis DataKey="sales" Color="var(--border)"><AxisLabel Color="var(--muted-foreground)" /></XAxis>
                <YAxis DataKey="month" Color="var(--border)"><AxisLabel Color="var(--muted-foreground)" /></YAxis>
                <ChartTooltip />
                <Bar DataKey="sales" Name="Sales" Radius="4" Color="var(--chart-2)" />
            </BarChart>
        </ChartContainer>
        """;

    private const string _multipleCode =
        """
        <ChartContainer Height="200">
            <BarChart Data="@multiData" Padding="ChartDefaults.DefaultPadding">
                <Grid Stroke="var(--border)" />
                <XAxis DataKey="month" Color="var(--border)"><AxisLabel Color="var(--muted-foreground)" /></XAxis>
                <YAxis Color="var(--border)"><AxisLabel Color="var(--muted-foreground)" /></YAxis>
                <ChartTooltip />
                <Legend TextColor="var(--foreground)" />
                <Bar DataKey="product1" Name="Product A" Radius="4" Color="var(--chart-1)" />
                <Bar DataKey="product2" Name="Product B" Radius="4" Color="var(--chart-2)" />
            </BarChart>
        </ChartContainer>
        """;

    private const string _stackedCode =
        """
        <ChartContainer Height="200">
            <BarChart Data="@multiData" Padding="ChartDefaults.DefaultPadding">
                <Grid Stroke="var(--border)" />
                <XAxis DataKey="month" Color="var(--border)"><AxisLabel Color="var(--muted-foreground)" /></XAxis>
                <YAxis Color="var(--border)"><AxisLabel Color="var(--muted-foreground)" /></YAxis>
                <ChartTooltip />
                <Legend TextColor="var(--foreground)" />
                <Bar DataKey="product1" Name="Product A" StackId="total" Radius="4" Color="var(--chart-1)" />
                <Bar DataKey="product2" Name="Product B" StackId="total" Radius="4" Color="var(--chart-2)" />
            </BarChart>
        </ChartContainer>
        """;

    private const string _withLabelsCode =
        """
        <ChartContainer Height="200">
            <BarChart Data="@simpleData" Padding="ChartDefaults.DefaultPadding">
                <Grid Stroke="var(--border)" />
                <XAxis DataKey="month" Color="var(--border)"><AxisLabel Color="var(--muted-foreground)" /></XAxis>
                <YAxis Color="var(--border)"><AxisLabel Color="var(--muted-foreground)" /></YAxis>
                <ChartTooltip />
                <Bar DataKey="sales" Name="Sales" Radius="4" ShowLabel="true" LabelPosition="LabelPosition.Top" Color="var(--chart-3)">
                    <LabelList Position="LabelPosition.Top" Formatter="{c}" FontSize="11" Color="#666" />
                </Bar>
            </BarChart>
        </ChartContainer>
        """;

    private const string _customLabelsCode =
        """
        <ChartContainer Height="200">
            <BarChart Data="@simpleData" Padding="ChartDefaults.DefaultPadding">
                <Grid Stroke="var(--border)" />
                <XAxis DataKey="month" Color="var(--border)"><AxisLabel Color="var(--muted-foreground)" /></XAxis>
                <YAxis Color="var(--border)"><AxisLabel Color="var(--muted-foreground)" /></YAxis>
                <ChartTooltip />
                <Bar DataKey="sales" Name="Sales" Radius="4" ShowLabel="true" Color="var(--chart-4)">
                    <LabelList Position="LabelPosition.Inside" Formatter="function(params) { return '$' + params.value; }" FontSize="10" Color="#fff" />
                </Bar>
            </BarChart>
        </ChartContainer>
        """;

    private const string _expandedCode =
        """
        <ChartContainer Height="200">
            <BarChart Data="@multiData" StackOffset="StackOffset.Expand" Padding="ChartDefaults.DefaultPadding">
                <Grid Stroke="var(--border)" />
                <XAxis DataKey="month" Color="var(--border)"><AxisLabel Color="var(--muted-foreground)" /></XAxis>
                <YAxis Color="var(--border)"><AxisLabel Color="var(--muted-foreground)" /></YAxis>
                <ChartTooltip Formatter="function(params) { return params.seriesName + ': ' + params.value.toFixed(1) + '%'; }" />
                <Legend TextColor="var(--foreground)" />
                <Bar DataKey="product1" Name="Product A" StackId="total" Radius="4" Color="var(--chart-1)" />
                <Bar DataKey="product2" Name="Product B" StackId="total" Radius="4" Color="var(--chart-2)" />
            </BarChart>
        </ChartContainer>
        """;

    private const string _activeCode =
        """
        <ChartContainer Height="200">
            <BarChart Data="@simpleData" Padding="ChartDefaults.DefaultPadding">
                <Grid Stroke="var(--border)" />
                <XAxis DataKey="month" Color="var(--border)"><AxisLabel Color="var(--muted-foreground)" /></XAxis>
                <YAxis Color="var(--border)"><AxisLabel Color="var(--muted-foreground)" /></YAxis>
                <ChartTooltip />
                <Bar DataKey="sales" Name="Sales" Radius="4" Color="var(--chart-5)" />
            </BarChart>
        </ChartContainer>
        """;

    private const string _mixedCode =
        """
        <ChartContainer Height="200">
            <ComposedChart Data="@multiData" Padding="ChartDefaults.DefaultPadding">
                <Grid Stroke="var(--border)" />
                <XAxis DataKey="month" Color="var(--border)"><AxisLabel Color="var(--muted-foreground)" /></XAxis>
                <YAxis Color="var(--border)"><AxisLabel Color="var(--muted-foreground)" /></YAxis>
                <ChartTooltip />
                <Legend TextColor="var(--foreground)" />
                <Bar DataKey="product1" Name="Product A" Radius="4" Color="var(--chart-1)" />
                <Line DataKey="product2" Name="Product B" Color="var(--chart-2)" />
            </ComposedChart>
        </ChartContainer>
        """;

    private const string _negativeCode =
        """
        <ChartContainer Height="200">
            <BarChart Data="@negativeData" Padding="ChartDefaults.DefaultPadding">
                <Grid Stroke="var(--border)" />
                <XAxis DataKey="month" Color="var(--border)"><AxisLabel Color="var(--muted-foreground)" /></XAxis>
                <YAxis Color="var(--border)"><AxisLabel Color="var(--muted-foreground)" /></YAxis>
                <ChartTooltip />
                <Bar DataKey="value" Name="Profit/Loss" Radius="4" Color="var(--chart-3)" />
            </BarChart>
        </ChartContainer>
        """;
}
