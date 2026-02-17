namespace BlazorBlueprint.Components;

/// <summary>
/// Specifies the position of a data label relative to its visual element.
/// </summary>
public enum LabelPosition
{
    /// <summary>
    /// Label is placed above the element (default for vertical bar charts).
    /// </summary>
    Top,

    /// <summary>
    /// Label is placed below the element.
    /// </summary>
    Bottom,

    /// <summary>
    /// Label is placed to the left of the element.
    /// </summary>
    Left,

    /// <summary>
    /// Label is placed to the right of the element.
    /// </summary>
    Right,

    /// <summary>
    /// Label is placed inside the element, centered.
    /// </summary>
    Inside,

    /// <summary>
    /// Label is placed inside the element, aligned to the left.
    /// </summary>
    InsideLeft,

    /// <summary>
    /// Label is placed inside the element, aligned to the right.
    /// </summary>
    InsideRight,

    /// <summary>
    /// Label is placed inside the element, aligned to the top.
    /// </summary>
    InsideTop,

    /// <summary>
    /// Label is placed inside the element, aligned to the bottom.
    /// </summary>
    InsideBottom,

    /// <summary>
    /// Label is placed outside the element (default for pie chart labels).
    /// </summary>
    Outside,

    /// <summary>
    /// Label is placed at the center of the element (used for pie/donut center labels).
    /// </summary>
    Center,

    /// <summary>
    /// Label is placed at the middle of the element (used for radial bars).
    /// </summary>
    Middle
}
