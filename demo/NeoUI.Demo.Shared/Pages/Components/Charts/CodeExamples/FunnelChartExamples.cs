namespace NeoUI.Demo.Shared.Pages.Components.Charts;

partial class FunnelChartExamples
{
    public const string SalesCode =
        """
        <ChartContainer Height="300" Class="w-full">
            <FunnelChart Data="@_salesPipeline">
                <ChartTooltip />
                <Legend TextColor="var(--foreground)" />
                <Funnel DataKey="count" NameKey="stage" ShowLabel="true" />
            </FunnelChart>
        </ChartContainer>

        @code {
            private record PipelineStage(string stage, int count);
            private static readonly PipelineStage[] _salesPipeline =
            [
                new("Leads",       12_400),
                new("Prospects",    8_200),
                new("Qualified",    4_700),
                new("Proposals",    2_100),
                new("Closed Won",     780),
            ];
        }
        """;

    public const string ConversionCode =
        """
        <ChartContainer Height="300" Class="w-full">
            <FunnelChart Data="@_conversionData">
                <ChartTooltip />
                <Funnel DataKey="pct" NameKey="step" ShowLabel="true" />
            </FunnelChart>
        </ChartContainer>

        @code {
            private record ConversionStep(string step, double pct);
            private static readonly ConversionStep[] _conversionData =
            [
                new("Page Views",    100.0),
                new("Sign-up Click",  34.2),
                new("Registration",   18.7),
                new("Activated",       9.4),
                new("Paid",            3.1),
            ];
        }
        """;

    public const string PyramidCode =
        """
        <ChartContainer Height="300" Class="w-full">
            <FunnelChart Data="@_pyramidData">
                <ChartTooltip />
                <Funnel DataKey="value" NameKey="tier" Sort="FunnelSort.Ascending" ShowLabel="true" />
            </FunnelChart>
        </ChartContainer>
        """;
}
