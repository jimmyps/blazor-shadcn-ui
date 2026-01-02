using System.Text.Json.Serialization;

namespace BlazorUI.Components.Grid;

/// <summary>
/// Represents the state of a single grid column.
/// </summary>
public class GridColumnState
{
    /// <summary>
    /// Gets or sets the field name of the column.
    /// </summary>
    [JsonPropertyName("field")]
    public string Field { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets whether the column is visible.
    /// </summary>
    [JsonPropertyName("visible")]
    public bool Visible { get; set; } = true;

    /// <summary>
    /// Gets or sets the column width (e.g., "100px", "20%").
    /// </summary>
    [JsonPropertyName("width")]
    public string? Width { get; set; }

    /// <summary>
    /// Gets or sets the column pinning position.
    /// </summary>
    [JsonPropertyName("pinned")]
    public GridColumnPinPosition Pinned { get; set; } = GridColumnPinPosition.None;

    /// <summary>
    /// Gets or sets the column display order.
    /// </summary>
    [JsonPropertyName("order")]
    public int Order { get; set; }
}
