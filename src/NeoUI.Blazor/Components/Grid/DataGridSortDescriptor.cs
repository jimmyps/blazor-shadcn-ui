using System.Text.Json.Serialization;

namespace NeoUI.Blazor;

/// <summary>
/// Describes a sort operation applied to a grid column.
/// </summary>
public class DataGridSortDescriptor
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
    public DataGridSortDirection Direction { get; set; } = DataGridSortDirection.None;

    /// <summary>
    /// Gets or sets the order of this sort descriptor when multiple sorts are applied.
    /// </summary>
    [JsonPropertyName("order")]
    public int Order { get; set; }
}
