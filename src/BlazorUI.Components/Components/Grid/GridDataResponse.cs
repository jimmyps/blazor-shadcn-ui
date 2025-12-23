using System.Text.Json.Serialization;

namespace BlazorUI.Components.Grid;

/// <summary>
/// Represents a response containing grid data from a server-side data source.
/// </summary>
/// <typeparam name="TItem">The type of items in the grid.</typeparam>
public class GridDataResponse<TItem>
{
    /// <summary>
    /// Gets or sets the items for the current page.
    /// </summary>
    [JsonPropertyName("items")]
    public IEnumerable<TItem> Items { get; set; } = Array.Empty<TItem>();

    /// <summary>
    /// Gets or sets the total number of items (before filtering).
    /// </summary>
    [JsonPropertyName("totalCount")]
    public int TotalCount { get; set; }

    /// <summary>
    /// Gets or sets the number of items after filtering (null if not applicable).
    /// </summary>
    [JsonPropertyName("filteredCount")]
    public int? FilteredCount { get; set; }
}
