namespace BlazorBlueprint.Components;

internal interface IChartComponent
{
    public void ApplyTo(EChartsOption option);
}

internal interface IChartSeries : IChartComponent
{
    public EChartsSeriesOption BuildSeries();
}

internal interface IFillComponent
{
    public object BuildFill();
}
