using System.Text.Json.Serialization;

namespace BlazorUI.Components.Grid;

/// <summary>
/// Describes a sort operation on a grid column.
/// </summary>
public class GridSortDescriptor
{
    /// <summary>
    /// Gets or sets the field name to sort by.
    /// </summary>
    [JsonPropertyName("field")]
    public string Field { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the sort direction.
    /// </summary>
    [JsonPropertyName("direction")]
    public GridSortDirection Direction { get; set; } = GridSortDirection.None;

    /// <summary>
    /// Gets or sets the sort order (for multi-column sorting).
    /// </summary>
    [JsonPropertyName("order")]
    public int Order { get; set; }
}
