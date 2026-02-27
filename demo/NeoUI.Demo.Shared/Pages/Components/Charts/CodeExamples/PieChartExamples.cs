namespace NeoUI.Demo.Shared.Pages.Components.Charts;

partial class PieChartExamples
{
    private static readonly IReadOnlyList<DemoPropRow> _pieChartProps =
    [
        new("Data",             "IEnumerable<TData>", "required",  "The data collection to render."),
        new("DataKey",          "string",             "required",  "Property name mapped to each slice's numeric value (on Pie series)."),
        new("NameKey",          "string?",            "null",      "Property name mapped to each slice's display name."),
        new("InnerRadius",      "string?",            "0",         "Inner radius for donut charts, e.g. \"40%\"."),
        new("OuterRadius",      "string?",            "75%",       "Outer radius as a percentage or pixel value."),
        new("Center",           "string[]?",          "50%, 50%",  "Center position [x, y] as percentage strings."),
        new("StartAngle",       "int",                "90",        "Starting angle in degrees (counterclockwise from 3 o'clock)."),
        new("EndAngle",         "int",                "-270",      "Ending angle in degrees."),
        new("PadAngle",         "int",                "0",         "Gap angle in degrees between slices."),
        new("RoseType",         "string?",            "null",      "Set to \"radius\" to render a Nightingale rose chart."),
        new("ShowLabel",        "bool",               "false",     "Whether to show slice labels."),
        new("EmphasisScaleSize","int",                "0",         "Scale-up amount on hover for emphasis effect."),
        new("Color",            "string?",            "auto",      "Override the first slice color. Usually left to the palette."),
        new("Class",            "string?",            "null",      "Additional CSS classes on the root element."),
    ];

    private const string _interactiveCode =
        """
        <div class="flex items-center gap-2 space-y-0 border-b py-5 sm:flex-row">
            <div class="grid flex-1 gap-1">
                <p class="text-xl font-semibold">Pie Chart — Interactive</p>
                <p class="text-sm text-muted-foreground">Showing browser usage statistics for January</p>
            </div>
            <Select @bind-Value="@selectedMonth" Class="hidden w-[160px] rounded-lg sm:ml-auto sm:flex">
                <SelectTrigger><SelectValue Placeholder="January" /></SelectTrigger>
                <SelectContent>
                    @foreach (var month in monthDescriptions)
                    {
                        <SelectItem Value="@month.Key" Text="@month.Value">@month.Value</SelectItem>
                    }
                </SelectContent>
            </Select>
        </div>
        <ChartContainer Height="250" Class="w-full">
            <PieChart Data="@FilteredBrowserData">
                <ChartTooltip />
                <Legend Show="false" />
                <Pie DataKey="value" NameKey="name" Color="var(--chart-1)">
                    <LabelList Color="var(--muted-foreground)" />
                </Pie>
            </PieChart>
        </ChartContainer>
        """;

    private const string _simpleCode =
        """
        <ChartContainer Height="200">
            <PieChart Data="@browserData">
                <ChartTooltip />
                <Legend Show="false" />
                <Pie DataKey="value" NameKey="name">
                    <LabelList Show="false" />
                </Pie>
            </PieChart>
        </ChartContainer>
        """;

    private const string _donutCode =
        """
        <ChartContainer Height="200">
            <PieChart Data="@browserData">
                <ChartTooltip />
                <Legend Show="false" />
                <Pie DataKey="value" NameKey="name" InnerRadius="30%">
                    <LabelList Color="var(--muted-foreground)" />
                </Pie>
            </PieChart>
        </ChartContainer>
        """;

    private const string _ringCode =
        """
        <ChartContainer Height="200">
            <PieChart Data="@browserData">
                <ChartTooltip />
                <Legend Show="false" />
                <Pie DataKey="value" NameKey="name" InnerRadius="60%" EmphasisScaleSize="10">
                    <LabelList Color="var(--muted-foreground)" />
                </Pie>
            </PieChart>
        </ChartContainer>
        """;

    private const string _roseCode =
        """
        <ChartContainer Height="200">
            <PieChart Data="@browserData">
                <ChartTooltip />
                <Legend Show="false" />
                <Pie DataKey="value" NameKey="name" RoseType="radius">
                    <LabelList Color="var(--muted-foreground)" />
                </Pie>
            </PieChart>
        </ChartContainer>
        """;

    private const string _withLabelsCode =
        """
        <ChartContainer Height="200">
            <PieChart Data="@browserData">
                <ChartTooltip />
                <Legend Show="false" />
                <Pie DataKey="value" NameKey="name" ShowLabel="true">
                    <LabelList Position="LabelPosition.Outside" Formatter="{b}: {c}%" FontSize="10" Color="var(--muted-foreground)" />
                </Pie>
            </PieChart>
        </ChartContainer>
        """;

    private const string _customLabelsCode =
        """
        <ChartContainer Height="200">
            <PieChart Data="@browserData">
                <ChartTooltip />
                <Legend Show="false" />
                <Pie DataKey="value" NameKey="name" ShowLabel="true">
                    <LabelList Position="LabelPosition.Outside" Formatter="function(params) { return params.name + '\n' + params.value + '%'; }" FontSize="9" Color="var(--muted-foreground)" />
                </Pie>
            </PieChart>
        </ChartContainer>
        """;

    private const string _labelLinesCode =
        """
        <ChartContainer Height="200">
            <PieChart Data="@browserData">
                <ChartTooltip />
                <Legend Show="false" />
                <Pie DataKey="value" NameKey="name" ShowLabel="true">
                    <LabelList Position="LabelPosition.Outside" Formatter="{b}" FontSize="10" Color="var(--muted-foreground)" />
                    <LabelLine Length="10" Length2="15" Smooth="true" />
                </Pie>
            </PieChart>
        </ChartContainer>
        """;

    private const string _labelListCode =
        """
        <ChartContainer Height="200">
            <PieChart Data="@browserData">
                <ChartTooltip />
                <Legend Show="false" />
                <Pie DataKey="value" NameKey="name" ShowLabel="true">
                    <LabelList Position="LabelPosition.Inside" Formatter="{c}%" FontSize="11" Color="var(--color-white)" />
                </Pie>
            </PieChart>
        </ChartContainer>
        """;

    private const string _withLegendCode =
        """
        <ChartContainer Height="200">
            <PieChart Data="@browserData">
                <ChartTooltip />
                <Legend VerticalAlign="LegendVerticalAlign.Bottom" Align="LegendAlign.Center" />
                <Pie DataKey="value" NameKey="name" OuterRadius="40%" Center="@(new string[] {"50%", "40%"})">
                    <LabelList Color="var(--muted-foreground)" />
                </Pie>
            </PieChart>
        </ChartContainer>
        """;

    private const string _sliceSpacingCode =
        """
        <ChartContainer Height="200">
            <PieChart Data="@browserData">
                <ChartTooltip />
                <Legend Show="false" />
                <Pie DataKey="value" NameKey="name" PadAngle="5">
                    <LabelList Color="var(--muted-foreground)" />
                </Pie>
            </PieChart>
        </ChartContainer>
        """;

    private const string _semiCircleCode =
        """
        <ChartContainer Height="200">
            <PieChart Data="@browserData">
                <ChartTooltip />
                <Legend Show="false" />
                <Pie DataKey="value" NameKey="name" StartAngle="180" EndAngle="360" InnerRadius="50%">
                    <LabelList Color="var(--muted-foreground)" />
                </Pie>
            </PieChart>
        </ChartContainer>
        """;
}
