namespace BlazorUI.Components.Chart;

/// <summary>
/// Represents padding values for chart elements.
/// </summary>
public readonly record struct Padding
{
    /// <summary>
    /// Gets the top padding.
    /// </summary>
    public int Top { get; init; }
    
    /// <summary>
    /// Gets the right padding.
    /// </summary>
    public int Right { get; init; }
    
    /// <summary>
    /// Gets the bottom padding.
    /// </summary>
    public int Bottom { get; init; }
    
    /// <summary>
    /// Gets the left padding.
    /// </summary>
    public int Left { get; init; }
    
    /// <summary>
    /// Initializes a new instance of the <see cref="Padding"/> struct with uniform padding.
    /// </summary>
    /// <param name="all">The padding value to apply to all sides.</param>
    public Padding(int all) : this(all, all, all, all)
    {
    }
    
    /// <summary>
    /// Initializes a new instance of the <see cref="Padding"/> struct with vertical and horizontal padding.
    /// </summary>
    /// <param name="vertical">The padding value for top and bottom.</param>
    /// <param name="horizontal">The padding value for left and right.</param>
    public Padding(int vertical, int horizontal) : this(vertical, horizontal, vertical, horizontal)
    {
    }
    
    /// <summary>
    /// Initializes a new instance of the <see cref="Padding"/> struct with individual padding values.
    /// </summary>
    /// <param name="top">The top padding.</param>
    /// <param name="right">The right padding.</param>
    /// <param name="bottom">The bottom padding.</param>
    /// <param name="left">The left padding.</param>
    public Padding(int top, int right, int bottom, int left)
    {
        Top = top;
        Right = right;
        Bottom = bottom;
        Left = left;
    }
    
    /// <summary>
    /// Gets a <see cref="Padding"/> instance with zero padding on all sides.
    /// </summary>
    public static Padding Zero => new(0);
    
    /// <summary>
    /// Converts the padding to an array format [top, right, bottom, left].
    /// </summary>
    public int[] ToArray() => new[] { Top, Right, Bottom, Left };
}
