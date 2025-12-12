using System.Text.Json.Serialization;

namespace BlazorUI.Components.Chart;

/// <summary>
/// Fluent builder for creating ECharts options with a strongly-typed, intuitive API.
/// </summary>
/// <example>
/// <code>
/// var option = new EChartsOptionBuilder()
///     .WithTooltip(trigger: "axis")
///     .WithXAxis(axis => axis
///         .SetType("category")
///         .SetData("Jan", "Feb", "Mar"))
///     .WithYAxis(axis => axis.SetType("value"))
///     .AddLineSeries(series => series
///         .SetName("Sales")
///         .SetData(120, 200, 150)
///         .SetSmooth(true)
///         .WithGradientFill(
///             EChartsLinearGradient.Vertical(
///                 new EChartsColorStop(0, "hsla(var(--chart-1), 0.8)"),
///                 new EChartsColorStop(1, "hsla(var(--chart-1), 0.1)")
///             )
///         ))
///     .Build();
/// </code>
/// </example>
public class EChartsOptionBuilder
{
    private readonly EChartsOption _option = new();
    
    /// <summary>
    /// Configures the tooltip.
    /// </summary>
    public EChartsOptionBuilder WithTooltip(Action<EChartsTooltip>? configure = null, string? trigger = null)
    {
        _option.Tooltip = new EChartsTooltip();
        if (trigger != null)
            _option.Tooltip.Trigger = trigger;
        configure?.Invoke(_option.Tooltip);
        return this;
    }
    
    /// <summary>
    /// Configures the legend.
    /// </summary>
    public EChartsOptionBuilder WithLegend(Action<EChartsLegend>? configure = null)
    {
        _option.Legend = new EChartsLegend();
        configure?.Invoke(_option.Legend);
        return this;
    }
    
    /// <summary>
    /// Configures the X-axis.
    /// </summary>
    public EChartsOptionBuilder WithXAxis(Action<AxisBuilder> configure)
    {
        var builder = new AxisBuilder();
        configure(builder);
        _option.XAxis = builder.Build();
        return this;
    }
    
    /// <summary>
    /// Configures the Y-axis.
    /// </summary>
    public EChartsOptionBuilder WithYAxis(Action<AxisBuilder> configure)
    {
        var builder = new AxisBuilder();
        configure(builder);
        _option.YAxis = builder.Build();
        return this;
    }
    
    /// <summary>
    /// Configures the grid.
    /// </summary>
    public EChartsOptionBuilder WithGrid(Action<EChartsGrid>? configure = null)
    {
        _option.Grid = new EChartsGrid();
        configure?.Invoke(_option.Grid);
        return this;
    }
    
    /// <summary>
    /// Adds a line series.
    /// </summary>
    public EChartsOptionBuilder AddLineSeries(Action<LineSeriesBuilder> configure)
    {
        var builder = new LineSeriesBuilder();
        configure(builder);
        _option.Series.Add(builder.Build());
        return this;
    }
    
    /// <summary>
    /// Adds a bar series.
    /// </summary>
    public EChartsOptionBuilder AddBarSeries(Action<BarSeriesBuilder> configure)
    {
        var builder = new BarSeriesBuilder();
        configure(builder);
        _option.Series.Add(builder.Build());
        return this;
    }
    
    /// <summary>
    /// Adds a pie series.
    /// </summary>
    public EChartsOptionBuilder AddPieSeries(Action<PieSeriesBuilder> configure)
    {
        var builder = new PieSeriesBuilder();
        configure(builder);
        _option.Series.Add(builder.Build());
        return this;
    }
    
    /// <summary>
    /// Adds a scatter series.
    /// </summary>
    public EChartsOptionBuilder AddScatterSeries(Action<ScatterSeriesBuilder> configure)
    {
        var builder = new ScatterSeriesBuilder();
        configure(builder);
        _option.Series.Add(builder.Build());
        return this;
    }
    
    /// <summary>
    /// Sets animation options.
    /// </summary>
    public EChartsOptionBuilder WithAnimation(bool enabled = true, int? duration = null, string? easing = null)
    {
        _option.Animation = enabled;
        if (duration.HasValue)
            _option.AnimationDuration = duration;
        if (easing != null)
            _option.AnimationEasing = easing;
        return this;
    }
    
    /// <summary>
    /// Builds the final EChartsOption.
    /// </summary>
    public EChartsOption Build() => _option;
}

/// <summary>
/// Builder for axis configuration.
/// </summary>
public class AxisBuilder
{
    private readonly EChartsAxis _axis = new();
    
    public AxisBuilder SetType(string type)
    {
        _axis.Type = type;
        return this;
    }
    
    public AxisBuilder SetData(params string[] data)
    {
        _axis.Data = new List<string>(data);
        return this;
    }
    
    public AxisBuilder SetName(string name)
    {
        _axis.Name = name;
        return this;
    }
    
    public AxisBuilder SetBoundaryGap(bool boundaryGap)
    {
        _axis.BoundaryGap = boundaryGap;
        return this;
    }
    
    internal EChartsAxis Build() => _axis;
}

/// <summary>
/// Builder for line series configuration.
/// </summary>
public class LineSeriesBuilder
{
    private readonly EChartsSeries _series = new() { Type = "line" };
    
    public LineSeriesBuilder SetName(string name)
    {
        _series.Name = name;
        return this;
    }
    
    public LineSeriesBuilder SetData(params object[] data)
    {
        _series.Data = data;
        return this;
    }
    
    public LineSeriesBuilder SetSmooth(bool smooth)
    {
        _series.Smooth = smooth;
        return this;
    }
    
    public LineSeriesBuilder SetStack(string stack)
    {
        _series.Stack = stack;
        return this;
    }
    
    public LineSeriesBuilder ShowSymbols(bool show, int? size = null)
    {
        _series.ShowSymbol = show;
        if (size.HasValue)
            _series.SymbolSize = size;
        return this;
    }
    
    public LineSeriesBuilder WithLineStyle(string? color = null, int? width = null)
    {
        _series.LineStyle = new EChartsLineStyle
        {
            Color = color,
            Width = width
        };
        return this;
    }
    
    public LineSeriesBuilder WithGradientFill(EChartsLinearGradient gradient)
    {
        _series.AreaStyle = new EChartsAreaStyle
        {
            Color = gradient
        };
        return this;
    }
    
    public LineSeriesBuilder WithAreaFill(string? color = null, double? opacity = null)
    {
        _series.AreaStyle = new EChartsAreaStyle
        {
            Color = color,
            Opacity = opacity
        };
        return this;
    }
    
    internal EChartsSeries Build() => _series;
}

/// <summary>
/// Builder for bar series configuration.
/// </summary>
public class BarSeriesBuilder
{
    private readonly EChartsSeries _series = new() { Type = "bar" };
    
    public BarSeriesBuilder SetName(string name)
    {
        _series.Name = name;
        return this;
    }
    
    public BarSeriesBuilder SetData(params object[] data)
    {
        _series.Data = data;
        return this;
    }
    
    public BarSeriesBuilder SetStack(string stack)
    {
        _series.Stack = stack;
        return this;
    }
    
    public BarSeriesBuilder WithItemStyle(object? color = null, object? borderRadius = null)
    {
        _series.ItemStyle = new EChartsItemStyle
        {
            Color = color,
            BorderRadius = borderRadius
        };
        return this;
    }
    
    public BarSeriesBuilder WithGradientFill(EChartsLinearGradient gradient)
    {
        _series.ItemStyle = new EChartsItemStyle
        {
            Color = gradient
        };
        return this;
    }
    
    public BarSeriesBuilder SetMaxWidth(int maxWidth)
    {
        _series.BarMaxWidth = maxWidth;
        return this;
    }
    
    internal EChartsSeries Build() => _series;
}

/// <summary>
/// Builder for pie series configuration.
/// </summary>
public class PieSeriesBuilder
{
    private readonly EChartsSeries _series = new() { Type = "pie" };
    
    public PieSeriesBuilder SetName(string name)
    {
        _series.Name = name;
        return this;
    }
    
    public PieSeriesBuilder SetData(params object[] data)
    {
        _series.Data = data;
        return this;
    }
    
    public PieSeriesBuilder SetRadius(string radius)
    {
        _series.Radius = radius;
        return this;
    }
    
    public PieSeriesBuilder SetRadius(string innerRadius, string outerRadius)
    {
        _series.Radius = new[] { innerRadius, outerRadius };
        return this;
    }
    
    internal EChartsSeries Build() => _series;
}

/// <summary>
/// Builder for scatter series configuration.
/// </summary>
public class ScatterSeriesBuilder
{
    private readonly EChartsSeries _series = new() { Type = "scatter" };
    
    public ScatterSeriesBuilder SetName(string name)
    {
        _series.Name = name;
        return this;
    }
    
    public ScatterSeriesBuilder SetData(params object[] data)
    {
        _series.Data = data;
        return this;
    }
    
    public ScatterSeriesBuilder SetSymbolSize(int size)
    {
        _series.SymbolSize = size;
        return this;
    }
    
    public ScatterSeriesBuilder WithItemStyle(string? color = null)
    {
        _series.ItemStyle = new EChartsItemStyle { Color = color };
        return this;
    }
    
    internal EChartsSeries Build() => _series;
}
