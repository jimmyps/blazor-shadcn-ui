namespace NeoUI.Demo.Shared.Pages.Components.Charts;

partial class CandlestickChartExamples
{
    public const string BasicCode =
        """
        <ChartContainer Height="280" Class="w-full">
            <CandlestickChart Data="@_ohlcData">
                <Grid Vertical="false" Stroke="var(--border)" />
                <XAxis DataKey="date" TickLine="false" Color="var(--border)">
                    <AxisLabel Color="var(--muted-foreground)" />
                </XAxis>
                <YAxis Scale="AxisScale.Value" Color="var(--border)">
                    <AxisLabel Color="var(--muted-foreground)" />
                </YAxis>
                <ChartTooltip />
                <Candlestick OpenKey="open" HighKey="high" LowKey="low" CloseKey="close"
                             RisingColor="var(--chart-2)" FallingColor="var(--chart-5)" />
            </CandlestickChart>
        </ChartContainer>

        @code {
            private record OhlcPoint(string date, double open, double high, double low, double close, long volume);
            private static readonly OhlcPoint[] _ohlcData = [ /* ... */ ];
        }
        """;
}
