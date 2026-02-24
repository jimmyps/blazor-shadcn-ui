namespace NeoUI.Blazor.Chart;

/// <summary>Implemented by all chart roots. Used by Tooltip and Legend.</summary>
public interface IChartParent
{
    void RegisterTooltip(Tooltip tooltip);
    void RegisterLegend(Legend legend);
}

/// <summary>Charts that accept an X-axis. Extends IChartParent.</summary>
public interface IXAxisParent : IChartParent
{
    void RegisterXAxis(XAxis xAxis);
}

/// <summary>Cartesian charts (full axis + grid support). Extends IXAxisParent.</summary>
public interface ICartesianChartParent : IXAxisParent
{
    void RegisterYAxis(YAxis yAxis);
    void RegisterGrid(Grid grid);
}

/// <summary>PieChart only.</summary>
public interface IPieChartParent : IChartParent
{
    void RegisterPie(Pie pie);
}

/// <summary>BarChart and ComposedChart.</summary>
public interface IBarSeriesParent : ICartesianChartParent
{
    void RegisterBar(Bar bar);
}

/// <summary>LineChart and ComposedChart.</summary>
public interface ILineSeriesParent : ICartesianChartParent
{
    void RegisterLine(Line line);
}

/// <summary>AreaChart and ComposedChart.</summary>
public interface IAreaSeriesParent : ICartesianChartParent
{
    void RegisterArea(Area area);
}

/// <summary>ScatterChart and ComposedChart.</summary>
public interface IScatterSeriesParent : ICartesianChartParent
{
    void RegisterScatter(Scatter scatter);
}

/// <summary>RadarChart only.</summary>
public interface IRadarChartParent : IXAxisParent
{
    void RegisterRadar(Radar radar);
    void RegisterRadarGrid(RadarGrid radarGrid);
}

/// <summary>RadialBarChart only.</summary>
public interface IRadialBarChartParent : IXAxisParent
{
    void RegisterRadialBar(RadialBar radialBar);
    void RegisterPolarGrid(PolarGrid polarGrid);
    void RegisterCenterLabel(CenterLabel centerLabel);
}

/// <summary>XAxis and YAxis. Used by AxisLabel.</summary>
public interface IAxisParent
{
    void RegisterAxisLabel(AxisLabel axisLabel);
}

/// <summary>Line and Area series. Used by Fill.</summary>
public interface IFillParent
{
    void RegisterFill(Fill fill);
}
