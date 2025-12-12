using System.Text.Json.Serialization;

namespace BlazorUI.Components.Chart;

/// <summary>
/// Represents a series in an ECharts chart.
/// </summary>
public class EChartsSeries
{
    /// <summary>
    /// Gets or sets the series name.
    /// </summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }
    
    /// <summary>
    /// Gets or sets the series type ("line", "bar", "pie", "scatter", "radar", etc.).
    /// </summary>
    [JsonPropertyName("type")]
    public string? Type { get; set; }
    
    /// <summary>
    /// Gets or sets the series data.
    /// </summary>
    [JsonPropertyName("data")]
    public object? Data { get; set; }
    
    /// <summary>
    /// Gets or sets whether the line is smooth (for line charts).
    /// </summary>
    [JsonPropertyName("smooth")]
    public bool? Smooth { get; set; }
    
    /// <summary>
    /// Gets or sets the stack name for stacking series.
    /// </summary>
    [JsonPropertyName("stack")]
    public string? Stack { get; set; }
    
    /// <summary>
    /// Gets or sets the area style (for area charts).
    /// </summary>
    [JsonPropertyName("areaStyle")]
    public EChartsAreaStyle? AreaStyle { get; set; }
    
    /// <summary>
    /// Gets or sets the line style.
    /// </summary>
    [JsonPropertyName("lineStyle")]
    public EChartsLineStyle? LineStyle { get; set; }
    
    /// <summary>
    /// Gets or sets the item style.
    /// </summary>
    [JsonPropertyName("itemStyle")]
    public EChartsItemStyle? ItemStyle { get; set; }
    
    /// <summary>
    /// Gets or sets whether to show symbols on the line.
    /// </summary>
    [JsonPropertyName("showSymbol")]
    public bool? ShowSymbol { get; set; }
    
    /// <summary>
    /// Gets or sets the symbol size.
    /// </summary>
    [JsonPropertyName("symbolSize")]
    public object? SymbolSize { get; set; }
    
    /// <summary>
    /// Gets or sets the emphasis configuration.
    /// </summary>
    [JsonPropertyName("emphasis")]
    public EChartsEmphasis? Emphasis { get; set; }
    
    /// <summary>
    /// Gets or sets the label configuration.
    /// </summary>
    [JsonPropertyName("label")]
    public EChartsLabel? Label { get; set; }
    
    /// <summary>
    /// Gets or sets the radius (for pie charts).
    /// </summary>
    [JsonPropertyName("radius")]
    public object? Radius { get; set; }
    
    /// <summary>
    /// Gets or sets the bar maximum width.
    /// </summary>
    [JsonPropertyName("barMaxWidth")]
    public int? BarMaxWidth { get; set; }
}

/// <summary>
/// Represents area style configuration.
/// </summary>
public class EChartsAreaStyle
{
    /// <summary>
    /// Gets or sets the fill color (can be string or gradient).
    /// </summary>
    [JsonPropertyName("color")]
    public object? Color { get; set; }
    
    /// <summary>
    /// Gets or sets the opacity.
    /// </summary>
    [JsonPropertyName("opacity")]
    public double? Opacity { get; set; }
}

/// <summary>
/// Represents line style configuration.
/// </summary>
public class EChartsLineStyle
{
    /// <summary>
    /// Gets or sets the line color.
    /// </summary>
    [JsonPropertyName("color")]
    public string? Color { get; set; }
    
    /// <summary>
    /// Gets or sets the line width.
    /// </summary>
    [JsonPropertyName("width")]
    public int? Width { get; set; }
    
    /// <summary>
    /// Gets or sets the line type ("solid", "dashed", "dotted").
    /// </summary>
    [JsonPropertyName("type")]
    public string? Type { get; set; }
}

/// <summary>
/// Represents item style configuration.
/// </summary>
public class EChartsItemStyle
{
    /// <summary>
    /// Gets or sets the item color (can be string or gradient).
    /// </summary>
    [JsonPropertyName("color")]
    public object? Color { get; set; }
    
    /// <summary>
    /// Gets or sets the border radius.
    /// </summary>
    [JsonPropertyName("borderRadius")]
    public object? BorderRadius { get; set; }
    
    /// <summary>
    /// Gets or sets the shadow blur.
    /// </summary>
    [JsonPropertyName("shadowBlur")]
    public int? ShadowBlur { get; set; }
    
    /// <summary>
    /// Gets or sets the shadow color.
    /// </summary>
    [JsonPropertyName("shadowColor")]
    public string? ShadowColor { get; set; }
    
    /// <summary>
    /// Gets or sets the shadow offset X.
    /// </summary>
    [JsonPropertyName("shadowOffsetX")]
    public int? ShadowOffsetX { get; set; }
    
    /// <summary>
    /// Gets or sets the shadow offset Y.
    /// </summary>
    [JsonPropertyName("shadowOffsetY")]
    public int? ShadowOffsetY { get; set; }
}

/// <summary>
/// Represents emphasis configuration (hover effects).
/// </summary>
public class EChartsEmphasis
{
    /// <summary>
    /// Gets or sets the focus type.
    /// </summary>
    [JsonPropertyName("focus")]
    public string? Focus { get; set; }
    
    /// <summary>
    /// Gets or sets the area style for emphasis.
    /// </summary>
    [JsonPropertyName("areaStyle")]
    public EChartsAreaStyle? AreaStyle { get; set; }
    
    /// <summary>
    /// Gets or sets the item style for emphasis.
    /// </summary>
    [JsonPropertyName("itemStyle")]
    public EChartsItemStyle? ItemStyle { get; set; }
}

/// <summary>
/// Represents label configuration.
/// </summary>
public class EChartsLabel
{
    /// <summary>
    /// Gets or sets whether to show labels.
    /// </summary>
    [JsonPropertyName("show")]
    public bool? Show { get; set; }
    
    /// <summary>
    /// Gets or sets the label position.
    /// </summary>
    [JsonPropertyName("position")]
    public string? Position { get; set; }
    
    /// <summary>
    /// Gets or sets the label formatter.
    /// </summary>
    [JsonPropertyName("formatter")]
    public string? Formatter { get; set; }
}
