using System.Text.Json.Serialization;

namespace BlazorUI.Components.Grid;

/// <summary>
/// Represents a request for data from a server-side data source.
/// Used for server-side paging, sorting, and filtering.
/// </summary>
/// <typeparam name="TItem">The type of items in the grid.</typeparam>
public class GridDataRequest<TItem>
{
    /// <summary>
    /// Gets or sets the starting index of the data to retrieve (0-based).
    /// </summary>
    [JsonPropertyName("startIndex")]
    public int StartIndex { get; set; }

    /// <summary>
    /// Gets or sets the number of items to retrieve.
    /// </summary>
    [JsonPropertyName("count")]
    public int Count { get; set; }

    /// <summary>
    /// Gets or sets the list of sort descriptors to apply.
    /// </summary>
    [JsonPropertyName("sortDescriptors")]
    public List<GridSortDescriptor> SortDescriptors { get; set; } = new();

    /// <summary>
    /// Gets or sets the list of filter descriptors to apply.
    /// </summary>
    [JsonPropertyName("filterDescriptors")]
    public List<GridFilterDescriptor> FilterDescriptors { get; set; } = new();

    /// <summary>
    /// Gets or sets the current page number (1-based).
    /// </summary>
    [JsonPropertyName("pageNumber")]
    public int PageNumber { get; set; } = 1;

    /// <summary>
    /// Gets or sets the number of items per page.
    /// </summary>
    [JsonPropertyName("pageSize")]
    public int PageSize { get; set; } = 25;

    /// <summary>
    /// Gets or sets custom parameters that can be passed to the data source.
    /// </summary>
    [JsonPropertyName("customParameters")]
    public Dictionary<string, object?> CustomParameters { get; set; } = new();
}
