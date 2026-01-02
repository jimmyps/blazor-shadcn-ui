using System.Text.Json.Serialization;

namespace BlazorUI.Components.Grid;

/// <summary>
/// Specifies how pagination is handled in a grid.
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum GridPagingMode
{
    /// <summary>All data displayed, no pagination.</summary>
    None,
    
    /// <summary>All data loaded, paginated client-side.</summary>
    Client,
    
    /// <summary>Data fetched page-by-page from server.</summary>
    Server,
    
    /// <summary>Incremental loading as user scrolls.</summary>
    InfiniteScroll
}
