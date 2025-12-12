namespace BlazorUI.Components.Chart;

/// <summary>
/// Represents line style types for chart lines.
/// </summary>
public enum LineType
{
    /// <summary>
    /// Solid continuous line.
    /// </summary>
    Solid,
    
    /// <summary>
    /// Dashed line with regular gaps.
    /// </summary>
    Dashed,
    
    /// <summary>
    /// Dotted line with small gaps.
    /// </summary>
    Dotted
}

/// <summary>
/// Represents point marker symbols for data points in charts.
/// </summary>
public enum PointSymbol
{
    /// <summary>
    /// Circular marker.
    /// </summary>
    Circle,
    
    /// <summary>
    /// Rectangular marker.
    /// </summary>
    Rect,
    
    /// <summary>
    /// Triangular marker.
    /// </summary>
    Triangle,
    
    /// <summary>
    /// Diamond-shaped marker.
    /// </summary>
    Diamond,
    
    /// <summary>
    /// Pin/teardrop-shaped marker.
    /// </summary>
    Pin,
    
    /// <summary>
    /// Arrow-shaped marker.
    /// </summary>
    Arrow,
    
    /// <summary>
    /// No marker (hidden).
    /// </summary>
    None
}

/// <summary>
/// Represents gradient directions for linear gradients.
/// </summary>
public enum GradientDirection
{
    /// <summary>
    /// Vertical gradient from top to bottom.
    /// </summary>
    Vertical,
    
    /// <summary>
    /// Horizontal gradient from left to right.
    /// </summary>
    Horizontal,
    
    /// <summary>
    /// Diagonal gradient from top-left to bottom-right.
    /// </summary>
    TopLeftToBottomRight,
    
    /// <summary>
    /// Diagonal gradient from top-right to bottom-left.
    /// </summary>
    TopRightToBottomLeft,
    
    /// <summary>
    /// Diagonal gradient from bottom-left to top-right.
    /// </summary>
    BottomLeftToTopRight,
    
    /// <summary>
    /// Diagonal gradient from bottom-right to top-left.
    /// </summary>
    BottomRightToTopLeft
}
