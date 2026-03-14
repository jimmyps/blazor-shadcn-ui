namespace NeoUI.Demo.Shared.Pages.Components.Charts;

partial class HeatmapChartExamples
{
    public const string ActivityCode =
        """
        <ChartContainer Height="220" Class="w-full">
            <HeatmapChart Data="@_activityData">
                <Grid Stroke="var(--border)" />
                <XAxis DataKey="day" TickLine="false" Color="var(--border)">
                    <AxisLabel Color="var(--muted-foreground)" />
                </XAxis>
                <YAxis DataKey="hour" Color="var(--border)">
                    <AxisLabel Color="var(--muted-foreground)" />
                </YAxis>
                <ChartTooltip />
                <Heatmap XKey="day" YKey="hour" ValueKey="commits" />
                <VisualMap Min="0" Max="10"
                           InRange="@(new[] { "var(--muted)", "var(--chart-1)" })" />
            </HeatmapChart>
        </ChartContainer>

        @code {
            private record ActivityPoint(string day, string hour, int commits);
            private static readonly ActivityPoint[] _activityData = GenerateActivity();
        }
        """;

    public const string CorrelationCode =
        """
        <ChartContainer Height="300" Class="w-full">
            <HeatmapChart Data="@_correlationData">
                <XAxis DataKey="x" /><YAxis DataKey="y" />
                <ChartTooltip />
                <Heatmap XKey="x" YKey="y" ValueKey="value" />
                <VisualMap Min="-1" Max="1"
                           InRange="@(new[] { "var(--chart-5)", "#ffffff", "var(--chart-2)" })"
                           ShowLegend="true" />
            </HeatmapChart>
        </ChartContainer>
        """;
}
