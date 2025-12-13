namespace BlazorUI.Components.Chart;

/// <summary>
/// Configuration for line style appearance.
/// </summary>
public class LineStyleConfig
{
    /// <summary>
    /// Gets or sets the line color.
    /// </summary>
    public string? Color { get; set; }
    
    /// <summary>
    /// Gets or sets the line width in pixels.
    /// </summary>
    public int Width { get; set; } = 2;
    
    /// <summary>
    /// Gets or sets the line type (solid, dashed, dotted).
    /// </summary>
    public LineType Type { get; set; } = LineType.Solid;
    
    /// <summary>
    /// Gets or sets whether the line should be smooth (curved).
    /// </summary>
    public bool Smooth { get; set; }
    
    /// <summary>
    /// Gets or sets the line cap style.
    /// </summary>
    public string? Cap { get; set; }
    
    /// <summary>
    /// Gets or sets the line join style.
    /// </summary>
    public string? Join { get; set; }
    
    /// <summary>
    /// Gets or sets the opacity (0.0 to 1.0).
    /// </summary>
    public double Opacity { get; set; } = 1.0;
}

/// <summary>
/// Configuration for area fill style.
/// </summary>
public class AreaStyleConfig
{
    /// <summary>
    /// Gets or sets whether to show the area fill.
    /// </summary>
    public bool Show { get; set; } = true;
    
    /// <summary>
    /// Gets or sets the fill color (solid color).
    /// </summary>
    public string? Color { get; set; }
    
    /// <summary>
    /// Gets or sets the opacity (0.0 to 1.0).
    /// </summary>
    public double Opacity { get; set; } = 1.0;
    
    /// <summary>
    /// Gets or sets the gradient configuration.
    /// </summary>
    public LinearGradientConfig? Gradient { get; set; }
}

/// <summary>
/// Configuration for linear gradient fill.
/// </summary>
public class LinearGradientConfig
{
    /// <summary>
    /// Gets or sets the gradient direction.
    /// </summary>
    public GradientDirection Direction { get; set; } = GradientDirection.Vertical;
    
    /// <summary>
    /// Gets or sets the color stops.
    /// </summary>
    public List<ColorStopConfig> ColorStops { get; set; } = new();
}

/// <summary>
/// Configuration for a color stop in a gradient.
/// </summary>
public class ColorStopConfig
{
    /// <summary>
    /// Gets or sets the position (0.0 to 1.0).
    /// </summary>
    public double Offset { get; set; }
    
    /// <summary>
    /// Gets or sets the color at this position.
    /// </summary>
    public string Color { get; set; } = string.Empty;
}

/// <summary>
/// Configuration for data point markers.
/// </summary>
public class PointStyleConfig
{
    /// <summary>
    /// Gets or sets whether to show point markers.
    /// </summary>
    public bool Show { get; set; } = true;
    
    /// <summary>
    /// Gets or sets the point symbol/shape.
    /// </summary>
    public PointSymbol Symbol { get; set; } = PointSymbol.Circle;
    
    /// <summary>
    /// Gets or sets the size of the point in pixels.
    /// </summary>
    public int Size { get; set; } = 4;
    
    /// <summary>
    /// Gets or sets the color of the point.
    /// </summary>
    public string? Color { get; set; }
    
    /// <summary>
    /// Gets or sets the border width.
    /// </summary>
    public int BorderWidth { get; set; } = 0;
    
    /// <summary>
    /// Gets or sets the border color.
    /// </summary>
    public string? BorderColor { get; set; }
    
    /// <summary>
    /// Gets or sets the hover size.
    /// </summary>
    public int HoverSize { get; set; } = 6;
}

/// <summary>
/// Configuration for bar style (placeholder for future bar chart styling).
/// </summary>
public class BarStyleConfig
{
    /// <summary>
    /// Gets or sets the bar color.
    /// </summary>
    public string? Color { get; set; }
    
    /// <summary>
    /// Gets or sets the border width.
    /// </summary>
    public int BorderWidth { get; set; } = 0;
    
    /// <summary>
    /// Gets or sets the border color.
    /// </summary>
    public string? BorderColor { get; set; }
    
    /// <summary>
    /// Gets or sets the gradient configuration.
    /// </summary>
    public LinearGradientConfig? Gradient { get; set; }
}
