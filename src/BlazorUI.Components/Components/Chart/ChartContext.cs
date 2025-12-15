using System.Collections.Concurrent;
using System.Collections.Generic;
using Microsoft.AspNetCore.Components;

namespace BlazorUI.Components.Chart;

internal sealed class ChartContext<TData>
{
    public ChartContext(string chartKind)
    {
        ChartKind = chartKind;
    }
    
    public string ChartKind { get; }
    
    public GridOptions? Grid { get; private set; }
    public AxisOptions? XAxis { get; private set; }
    public AxisOptions? YAxis { get; private set; }
    public TooltipOptions? Tooltip { get; private set; }
    public LegendOptions? Legend { get; private set; }
    
    public List<LineSeriesOptions> Lines { get; } = new();
    public List<AreaSeriesOptions> Areas { get; } = new();
    public List<BarSeriesOptions> Bars { get; } = new();
    public List<PieSeriesOptions> Pies { get; } = new();
    public List<ScatterSeriesOptions> Scatters { get; } = new();
    public List<RadarSeriesOptions> Radars { get; } = new();
    
    public void SetGrid(GridOptions options) => Grid = options;
    public void SetXAxis(AxisOptions options) => XAxis = options;
    public void SetYAxis(AxisOptions options) => YAxis = options;
    public void SetTooltip(TooltipOptions options) => Tooltip = options;
    public void SetLegend(LegendOptions options) => Legend = options;
    
    public void AddLine(LineSeriesOptions options) => Lines.Add(options);
    public void AddArea(AreaSeriesOptions options) => Areas.Add(options);
    public void AddBar(BarSeriesOptions options) => Bars.Add(options);
    public void AddPie(PieSeriesOptions options) => Pies.Add(options);
    public void AddScatter(ScatterSeriesOptions options) => Scatters.Add(options);
    public void AddRadar(RadarSeriesOptions options) => Radars.Add(options);
    
    public void EnsureImplicitPrimitives(TooltipMode defaultTooltipMode)
    {
        Grid ??= new GridOptions();
        XAxis ??= new AxisOptions();
        YAxis ??= new AxisOptions();
        Tooltip ??= new TooltipOptions { Mode = defaultTooltipMode };
        Legend ??= new LegendOptions();
    }
}

internal record GridOptions
{
    public bool Show { get; init; } = true;
    public bool Horizontal { get; init; } = true;
    public bool Vertical { get; init; } = true;
    public string? Stroke { get; init; }
}

internal record AxisOptions
{
    public bool Show { get; init; } = true;
    public string? DataKey { get; init; }
    public AxisScale Scale { get; init; } = AxisScale.Auto;
    public bool AxisLine { get; init; } = true;
    public bool TickLine { get; init; } = true;
}

internal record TooltipOptions
{
    public bool Show { get; init; } = true;
    public TooltipMode Mode { get; init; } = TooltipMode.Axis;
    public TooltipCursor Cursor { get; init; } = TooltipCursor.None;
}

internal record LegendOptions
{
    public bool Show { get; init; } = true;
    public LegendLayout Layout { get; init; } = LegendLayout.Horizontal;
    public LegendAlign Align { get; init; } = LegendAlign.Center;
    public LegendVerticalAlign VerticalAlign { get; init; } = LegendVerticalAlign.Top;
    public int MarginTop { get; init; } = 4;
}

internal record SeriesBaseOptions
{
    public string DataKey { get; init; } = string.Empty;
    public string? Name { get; init; }
    public string? Color { get; init; }
    public bool Emphasis { get; init; }
    public Focus Focus { get; init; } = Focus.None;
}

internal record LineSeriesOptions : SeriesBaseOptions
{
    public bool ShowDots { get; init; }
    public int LineWidth { get; init; } = 2;
    public bool Dashed { get; init; }
}

internal record AreaSeriesOptions : SeriesBaseOptions
{
    public bool ShowDots { get; init; }
    public int LineWidth { get; init; } = 2;
    public string? StackId { get; init; }
    public FillOptions? Fill { get; init; }
}

internal record BarSeriesOptions : SeriesBaseOptions
{
    public string? StackId { get; init; }
    public int? Radius { get; init; }
}

internal record PieSeriesOptions
{
    public string DataKey { get; init; } = string.Empty;
    public string NameKey { get; init; } = string.Empty;
    public string? InnerRadius { get; init; }
    public string? Name { get; init; }
    public string? Color { get; init; }
}

internal record ScatterSeriesOptions
{
    public string? Name { get; init; }
    public string? Color { get; init; }
    public bool Emphasis { get; init; }
    public Focus Focus { get; init; } = Focus.None;
}

internal record RadarSeriesOptions : SeriesBaseOptions
{
    public bool Fill { get; init; }
}

internal record FillOptions
{
    public string? Color { get; init; }
    public double? Opacity { get; init; }
    public LinearGradientOptions? LinearGradient { get; init; }
}

internal record LinearGradientOptions
{
    public GradientDirection Direction { get; init; } = GradientDirection.Vertical;
    public List<GradientStopOptions> Stops { get; init; } = new();
}

internal record GradientStopOptions
{
    public double Offset { get; init; }
    public string Color { get; init; } = string.Empty;
    public double? Opacity { get; init; }
}
