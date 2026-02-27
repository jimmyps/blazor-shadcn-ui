namespace NeoUI.Demo.Shared.Pages.Components.Charts;

partial class ComposedChartExamples
{
    private static readonly IReadOnlyList<DemoPropRow> _composedChartProps =
    [
        new("Data",            "IEnumerable<TData>", "required",  "The data collection to render."),
        new("Padding",         "Padding",            "32,16,24,16","Plot-area padding (Top, Right, Bottom, Left)."),
        new("StackOffset",     "StackOffset",        "None",      "Stack offset mode for all stacked series."),
        new("EnableAnimation", "bool",               "true",      "Whether to animate on first render."),
        new("Class",           "string?",            "null",      "Additional CSS classes on the root element."),
        new("Bar.DataKey",     "string",             "required",  "Property name on the data item for bar values."),
        new("Bar.Radius",      "int",                "0",         "Corner radius of bar tops in pixels."),
        new("Bar.StackId",     "string?",            "null",      "Stack group identifier. Bars with the same StackId are stacked."),
        new("Line.DataKey",    "string",             "required",  "Property name on the data item for line values."),
        new("Line.Dashed",     "bool",               "false",     "Whether to render the line as a dashed stroke."),
        new("Area.DataKey",    "string",             "required",  "Property name on the data item for area values."),
        new("Area.Opacity",    "double",             "1.0",       "Fill area opacity (0–1)."),
    ];

    private const string _revenueTargetCode =
        """
        <ChartContainer Height="200">
            <ComposedChart Data="@revenueData">
                <Grid Stroke="var(--border)" />
                <XAxis DataKey="month" BoundaryGap="true" Color="var(--border)">
                    <AxisLabel Color="var(--muted-foreground)" />
                </XAxis>
                <YAxis Color="var(--border)">
                    <AxisLabel Color="var(--muted-foreground)" />
                </YAxis>
                <ChartTooltip />
                <Legend TextColor="var(--foreground)" />
                <Bar DataKey="actual" Name="Actual Revenue" Radius="4" Color="var(--chart-1)" />
                <Line DataKey="target" Name="Target" Color="var(--chart-2)" />
            </ComposedChart>
        </ChartContainer>
        """;

    private const string _salesForecastCode =
        """
        <ChartContainer Height="200">
            <ComposedChart Data="@salesData">
                <Grid Stroke="var(--border)" />
                <XAxis DataKey="month" BoundaryGap="true" Color="var(--border)">
                    <AxisLabel Color="var(--muted-foreground)" />
                </XAxis>
                <YAxis Color="var(--border)">
                    <AxisLabel Color="var(--muted-foreground)" />
                </YAxis>
                <ChartTooltip />
                <Legend TextColor="var(--foreground)" />
                <Bar DataKey="sales" Name="Sales" Radius="4" Color="var(--chart-1)" />
                <Line DataKey="forecast" Name="Forecast" Dashed="true" Color="var(--chart-2)" />
            </ComposedChart>
        </ChartContainer>
        """;

    private const string _inventoryCode =
        """
        <ChartContainer Height="200">
            <ComposedChart Data="@inventoryData">
                <Grid Stroke="var(--border)" />
                <XAxis DataKey="month" BoundaryGap="true" Color="var(--border)">
                    <AxisLabel Color="var(--muted-foreground)" />
                </XAxis>
                <YAxis Color="var(--border)">
                    <AxisLabel Color="var(--muted-foreground)" />
                </YAxis>
                <ChartTooltip />
                <Legend TextColor="var(--foreground)" />
                <Bar DataKey="stock" Name="Stock Level" Radius="4" Color="var(--chart-1)" />
                <Line DataKey="reorder" Name="Reorder Point" Color="var(--chart-3)" />
                <Area DataKey="safety" Name="Safety Stock" Opacity="0.3" Color="var(--chart-2)" />
            </ComposedChart>
        </ChartContainer>
        """;

    private const string _customerMetricsCode =
        """
        <ChartContainer Height="200">
            <ComposedChart Data="@customerData">
                <Grid Stroke="var(--border)" />
                <XAxis DataKey="month" BoundaryGap="true" Color="var(--border)">
                    <AxisLabel Color="var(--muted-foreground)" />
                </XAxis>
                <YAxis Color="var(--border)">
                    <AxisLabel Color="var(--muted-foreground)" />
                </YAxis>
                <ChartTooltip />
                <Legend TextColor="var(--foreground)" />
                <Bar DataKey="newCustomers" Name="New Customers" Radius="4" Color="var(--chart-4)" />
                <Line DataKey="retention" Name="Retention Rate" Color="var(--chart-5)" />
            </ComposedChart>
        </ChartContainer>
        """;

    private const string _financialCode =
        """
        <ChartContainer Height="200">
            <ComposedChart Data="@financialData">
                <Grid Stroke="var(--border)" />
                <XAxis DataKey="month" BoundaryGap="true" Color="var(--border)">
                    <AxisLabel Color="var(--muted-foreground)" />
                </XAxis>
                <YAxis Color="var(--border)">
                    <AxisLabel Color="var(--muted-foreground)" />
                </YAxis>
                <ChartTooltip />
                <Legend TextColor="var(--foreground)" />
                <Bar DataKey="expenses" Name="Expenses" Radius="4" Color="var(--chart-3)" />
                <Line DataKey="revenue" Name="Revenue" Color="var(--chart-1)" />
                <Area DataKey="profit" Name="Profit Margin" Opacity="0.3" Color="var(--chart-2)" />
            </ComposedChart>
        </ChartContainer>
        """;

    private const string _marketingROICode =
        """
        <ChartContainer Height="200">
            <ComposedChart Data="@marketingData">
                <Grid Stroke="var(--border)" />
                <XAxis DataKey="month" BoundaryGap="true" Color="var(--border)">
                    <AxisLabel Color="var(--muted-foreground)" />
                </XAxis>
                <YAxis Color="var(--border)">
                    <AxisLabel Color="var(--muted-foreground)" />
                </YAxis>
                <ChartTooltip />
                <Legend TextColor="var(--foreground)" />
                <Bar DataKey="adSpend" Name="Ad Spend" Radius="4" Color="var(--chart-5)" />
                <Line DataKey="conversions" Name="Conversions" Color="var(--chart-1)" />
            </ComposedChart>
        </ChartContainer>
        """;

    private const string _analyticsCode =
        """
        <ChartContainer Height="200">
            <ComposedChart Data="@analyticsData">
                <Grid Stroke="var(--border)" />
                <XAxis DataKey="month" BoundaryGap="true" Color="var(--border)">
                    <AxisLabel Color="var(--muted-foreground)" />
                </XAxis>
                <YAxis Color="var(--border)">
                    <AxisLabel Color="var(--muted-foreground)" />
                </YAxis>
                <ChartTooltip />
                <Legend TextColor="var(--foreground)" />
                <Bar DataKey="pageViews" Name="Page Views" Radius="4" Color="var(--chart-2)" />
                <Line DataKey="bounceRate" Name="Bounce Rate" Color="var(--chart-3)" />
                <Area DataKey="engagement" Name="Engagement" Opacity="0.3" Color="var(--chart-4)" />
            </ComposedChart>
        </ChartContainer>
        """;

    private const string _kpiThresholdsCode =
        """
        <ChartContainer Height="200">
            <ComposedChart Data="@kpiData">
                <Grid Stroke="var(--border)" />
                <XAxis DataKey="month" BoundaryGap="true" Color="var(--border)">
                    <AxisLabel Color="var(--muted-foreground)" />
                </XAxis>
                <YAxis Color="var(--border)">
                    <AxisLabel Color="var(--muted-foreground)" />
                </YAxis>
                <ChartTooltip />
                <Legend TextColor="var(--foreground)" />
                <Bar DataKey="actual" Name="Actual" Radius="4" Color="var(--chart-1)" />
                <Line DataKey="threshold" Name="Threshold" Dashed="true" Color="var(--chart-3)" />
            </ComposedChart>
        </ChartContainer>
        """;
}
