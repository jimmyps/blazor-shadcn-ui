namespace NeoUI.Demo.Shared.Pages.Components.Charts;

partial class GaugeChartExamples
{
    public const string SingleCode =
        """
        <ChartContainer Height="260" Class="w-full">
            <GaugeChart Data="@_kpiData">
                <ChartTooltip />
                <Gauge DataKey="value" Name="Performance" Min="0" Max="100"
                       StartAngle="225" EndAngle="-45" />
            </GaugeChart>
        </ChartContainer>

        @code {
            private record KpiPoint(double value);
            private static readonly KpiPoint[] _kpiData = [new(72.5)];
        }
        """;

    public const string MultipleCode =
        """
        <ChartContainer Height="280" Class="w-full">
            <GaugeChart Data="@_serverMetrics">
                <ChartTooltip />
                <Gauge DataKey="cpu"    Name="CPU"    Fill="var(--chart-1)" />
                <Gauge DataKey="memory" Name="Memory" Fill="var(--chart-2)" />
                <Gauge DataKey="disk"   Name="Disk"   Fill="var(--chart-3)" />
            </GaugeChart>
        </ChartContainer>

        @code {
            private record ServerMetrics(double cpu, double memory, double disk);
            private static readonly ServerMetrics[] _serverMetrics = [new(68, 45, 82)];
        }
        """;

    public const string ProgressCode =
        """
        <div class="flex gap-10 justify-center flex-wrap py-4">
            @foreach (var task in _tasks)
            {
                <div class="flex flex-col items-center gap-3">
                    <ChartContainer Height="200" Class="w-[200px]">
                        <GaugeChart Data="@(new[] { task })">
                            <Gauge DataKey="progress" Name="@task.label"
                                   Fill="@task.color"
                                   StartAngle="90" EndAngle="-270"
                                   Min="0" Max="100"
                                   SplitNumber="0"
                                   ShowPointer="false"
                                   ShowSplitLine="false"
                                   ShowAxisLabel="false"
                                   ShowTitle="false"
                                   ProgressWidth="14"
                                   AxisLineWidth="14"
                                   DetailFontSize="28"
                                   DetailOffsetY="0%" />
                        </GaugeChart>
                    </ChartContainer>
                    <p class="text-sm font-medium text-center">@task.label</p>
                    <p class="text-xs text-muted-foreground text-center">@task.progress%</p>
                </div>
            }
        </div>

        @code {
            private record TaskProgress(string label, double progress, string color);
            private static readonly TaskProgress[] _tasks =
            [
                new("Design", 88, "var(--chart-1)"),
                new("Dev",    61, "var(--chart-2)"),
                new("QA",     34, "var(--chart-4)"),
            ];
        }
        """;
}
