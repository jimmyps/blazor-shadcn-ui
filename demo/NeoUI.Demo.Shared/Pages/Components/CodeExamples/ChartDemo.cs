namespace NeoUI.Demo.Shared.Pages.Components;

partial class ChartDemo
{
    private const string _dataSnippetCode =
        """
        // Define a typed record (or class) for your data
        private record ChartPoint(string month, int desktop, int mobile);

        private static readonly List<ChartPoint> chartData =
        [
            new("Jan", 186, 80),
            new("Feb", 305, 200),
            new("Mar", 237, 120),
            new("Apr", 273, 190),
            new("May", 209, 130),
            new("Jun", 214, 140),
        ];
        """;

    private const string _step2Code =
        """
        <ChartContainer Height="250" Class="w-full">
            <BarChart Data="@chartData">
                <Bar DataKey="desktop" Name="Desktop" Color="var(--chart-1)" Radius="4" />
                <Bar DataKey="mobile" Name="Mobile" Color="var(--chart-2)" Radius="4" />
            </BarChart>
        </ChartContainer>
        """;

    private const string _step3Code =
        """
        <ChartContainer Height="250" Class="w-full">
            <BarChart Data="@chartData">
                <Grid Vertical="false" Stroke="var(--border)" />
                <Bar DataKey="desktop" Name="Desktop" Color="var(--chart-1)" Radius="4" />
                <Bar DataKey="mobile" Name="Mobile" Color="var(--chart-2)" Radius="4" />
            </BarChart>
        </ChartContainer>
        """;

    private const string _step4Code =
        """
        <ChartContainer Height="250" Class="w-full">
            <BarChart Data="@chartData">
                <Grid Vertical="false" Stroke="var(--border)" />
                <XAxis DataKey="month" Color="var(--border)">
                    <AxisLabel Color="var(--muted-foreground)" />
                </XAxis>
                <Bar DataKey="desktop" Name="Desktop" Color="var(--chart-1)" Radius="4" />
                <Bar DataKey="mobile" Name="Mobile" Color="var(--chart-2)" Radius="4" />
            </BarChart>
        </ChartContainer>
        """;

    private const string _step5Code =
        """
        <ChartContainer Height="250" Class="w-full">
            <BarChart Data="@chartData">
                <Grid Vertical="false" Stroke="var(--border)" />
                <XAxis DataKey="month" Color="var(--border)">
                    <AxisLabel Color="var(--muted-foreground)" />
                </XAxis>
                <ChartTooltip />
                <Bar DataKey="desktop" Name="Desktop" Color="var(--chart-1)" Radius="4" />
                <Bar DataKey="mobile" Name="Mobile" Color="var(--chart-2)" Radius="4" />
            </BarChart>
        </ChartContainer>
        """;

    private const string _step6Code =
        """
        <ChartContainer Height="250" Class="w-full">
            <BarChart Data="@chartData">
                <Grid Vertical="false" Stroke="var(--border)" />
                <XAxis DataKey="month" Color="var(--border)">
                    <AxisLabel Color="var(--muted-foreground)" />
                </XAxis>
                <ChartTooltip />
                <Legend TextColor="var(--foreground)" />
                <Bar DataKey="desktop" Name="Desktop" Color="var(--chart-1)" Radius="4" />
                <Bar DataKey="mobile" Name="Mobile" Color="var(--chart-2)" Radius="4" />
            </BarChart>
        </ChartContainer>
        """;

    private const string _themingCode =
        """
        @* Use CSS variable tokens for automatic theme / dark-mode support *@
        <Bar DataKey="desktop" Color="var(--chart-1)" />
        <Bar DataKey="mobile"  Color="var(--chart-2)" />
        <Bar DataKey="tablet"  Color="var(--chart-3)" />

        @* Or any explicit CSS color value *@
        <Line DataKey="revenue" Color="#2563eb" />
        <Line DataKey="costs"   Color="hsl(0 72% 51%)" />
        """;

    private static readonly IReadOnlyList<DemoPropRow> _tooltipProps =
    [
        new("Show",      "bool",         "true",                "Whether the tooltip is enabled."),
        new("Mode",      "TooltipMode",  "Item",                "Trigger mode: <code class=\"text-xs bg-muted px-1 rounded\">Item</code> (hover series) or <code class=\"text-xs bg-muted px-1 rounded\">Axis</code> (hover axis line)."),
        new("Cursor",    "TooltipCursor","Default",             "Cursor indicator style: Default, Line, Cross, Shadow, or None."),
        new("Formatter", "string?",      "null",                "Optional JavaScript function string for custom tooltip content."),
        new("Class",     "string?",      "null",                "Additional CSS classes on the tooltip container."),
    ];

    private static readonly IReadOnlyList<DemoPropRow> _legendProps =
    [
        new("Show",          "bool",                "true",    "Whether the legend is rendered."),
        new("TextColor",     "string?",             "null",    "CSS color for legend label text. Recommended: <code class=\"text-xs bg-muted px-1 rounded\">var(--foreground)</code>."),
        new("Align",         "LegendAlign",         "Center",  "Horizontal alignment: Left, Center, Right."),
        new("VerticalAlign", "LegendVerticalAlign", "Bottom",  "Vertical position: Top, Middle, Bottom."),
        new("Class",         "string?",             "null",    "Additional CSS classes."),
    ];

    private static readonly IReadOnlyList<DemoPropRow> _containerProps =
    [
        new("Height",  "int",     "300",  "Fixed pixel height of the chart area."),
        new("Class",   "string?", "null", "Additional CSS classes merged onto the container div."),
        new("ChildContent", "RenderFragment", "—", "The chart root component (e.g. BarChart, LineChart)."),
    ];
}
