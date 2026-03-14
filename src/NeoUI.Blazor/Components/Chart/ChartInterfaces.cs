namespace NeoUI.Blazor.Charts;

/// <summary>Implemented by all chart roots. Used by Tooltip and Legend.</summary>
public interface IChartParent
{
    /// <summary>Registers a tooltip with this chart.</summary>
    void RegisterChartTooltip(ChartTooltip tooltip);
    /// <summary>Registers a legend with this chart.</summary>
    void RegisterLegend(Legend legend);
}

/// <summary>Charts that accept an X-axis. Extends IChartParent.</summary>
public interface IXAxisParent : IChartParent
{
    /// <summary>Registers an X-axis with this chart.</summary>
    void RegisterXAxis(XAxis xAxis);
}

/// <summary>Cartesian charts (full axis + grid support). Extends IXAxisParent.</summary>
public interface ICartesianChartParent : IXAxisParent
{
    /// <summary>Registers a Y-axis with this chart.</summary>
    void RegisterYAxis(YAxis yAxis);
    /// <summary>Registers a grid with this chart.</summary>
    void RegisterGrid(Grid grid);
}

/// <summary>PieChart only.</summary>
public interface IPieChartParent : IChartParent
{
    /// <summary>Registers a pie series with this chart.</summary>
    void RegisterPie(Pie pie);
}

/// <summary>BarChart and ComposedChart.</summary>
public interface IBarSeriesParent : ICartesianChartParent
{
    /// <summary>Registers a bar series with this chart.</summary>
    void RegisterBar(Bar bar);
}

/// <summary>LineChart and ComposedChart.</summary>
public interface ILineSeriesParent : ICartesianChartParent
{
    /// <summary>Registers a line series with this chart.</summary>
    void RegisterLine(Line line);
}

/// <summary>AreaChart and ComposedChart.</summary>
public interface IAreaSeriesParent : ICartesianChartParent
{
    /// <summary>Registers an area series with this chart.</summary>
    void RegisterArea(Area area);
}

/// <summary>ScatterChart and ComposedChart.</summary>
public interface IScatterSeriesParent : ICartesianChartParent
{
    /// <summary>Registers a scatter series with this chart.</summary>
    void RegisterScatter(Scatter scatter);
}

/// <summary>RadarChart only.</summary>
public interface IRadarChartParent : IXAxisParent
{
    /// <summary>Registers a radar series with this chart.</summary>
    void RegisterRadar(Radar radar);
    /// <summary>Registers a radar grid with this chart.</summary>
    void RegisterRadarGrid(RadarGrid radarGrid);
}

/// <summary>RadialBarChart only.</summary>
public interface IRadialBarChartParent : IXAxisParent
{
    /// <summary>Registers a radial bar series with this chart.</summary>
    void RegisterRadialBar(RadialBar radialBar);
    /// <summary>Registers a polar grid with this chart.</summary>
    void RegisterPolarGrid(PolarGrid polarGrid);
    /// <summary>Registers a center label with this chart.</summary>
    void RegisterCenterLabel(CenterLabel centerLabel);
}

/// <summary>XAxis and YAxis. Used by AxisLabel.</summary>
public interface IAxisParent
{
    /// <summary>Registers an axis label with this axis.</summary>
    void RegisterAxisLabel(AxisLabel axisLabel);
}

/// <summary>Line and Area series. Used by Fill.</summary>
public interface IFillParent
{
    /// <summary>Registers a fill with this series.</summary>
    void RegisterFill(Fill fill);
}

// ── New chart type parent interfaces ──────────────────────────────────────────

/// <summary>CandlestickChart.</summary>
public interface ICandlestickSeriesParent : ICartesianChartParent
{
    void RegisterCandlestick(Candlestick candlestick);
}

/// <summary>HeatmapChart.</summary>
public interface IHeatmapSeriesParent : ICartesianChartParent
{
    void RegisterHeatmap(Heatmap heatmap);
    void RegisterVisualMap(VisualMap visualMap);
}

/// <summary>GaugeChart.</summary>
public interface IGaugeSeriesParent : IChartParent
{
    void RegisterGauge(Gauge gauge);
}

/// <summary>FunnelChart.</summary>
public interface IFunnelSeriesParent : IChartParent
{
    void RegisterFunnel(Funnel funnel);
}
