namespace BlazorBlueprint.Components;

/// <summary>
/// Specifies the shape of a radar chart's grid.
/// </summary>
public enum RadarShape
{
    /// <summary>
    /// Angular polygon shape (default). The number of sides matches the number of indicators.
    /// </summary>
    Polygon,

    /// <summary>
    /// Circular shape with concentric rings.
    /// </summary>
    Circle
}
