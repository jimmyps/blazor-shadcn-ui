using System.Text.Json.Serialization;

namespace NeoUI.Blazor;

/// <summary>
/// Represents a response from a server-side data source containing the requested data and metadata.
/// </summary>
/// <typeparam name="TItem">The type of items in the grid.</typeparam>
public class DataGridDataResponse<TItem>
{
    /// <summary>
    /// Gets or sets the collection of items for the current page or request.
    /// </summary>
    [JsonPropertyName("items")]
    public IEnumerable<TItem> Items { get; set; } = Array.Empty<TItem>();

    /// <summary>
    /// Gets or sets the total number of items in the complete dataset (before filtering).
    /// </summary>
    [JsonPropertyName("totalCount")]
    public int TotalCount { get; set; }

    /// <summary>
    /// Gets or sets the number of items after applying filters (optional).
    /// If null, assumes it equals TotalCount.
    /// </summary>
    [JsonPropertyName("filteredCount")]
    public int? FilteredCount { get; set; }
}
