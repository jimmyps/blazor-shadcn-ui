namespace BlazorUI.Components.Grid;

/// <summary>
/// Specifies the row model type for the grid.
/// </summary>
public enum GridRowModelType
{
    /// <summary>
    /// Client-side row model. All data is loaded into the grid at once.
    /// Best for datasets under 10,000 rows.
    /// </summary>
    ClientSide,
    
    /// <summary>
    /// Server-side row model. Data is fetched on-demand from the server.
    /// Supports server-side sorting, filtering, and pagination.
    /// Best for large datasets (100,000+ rows) or when data must remain on the server.
    /// </summary>
    ServerSide,
    
    /// <summary>
    /// Infinite scroll row model. Data is fetched in blocks as the user scrolls.
    /// Best for medium-sized datasets (10,000-100,000 rows) with simple requirements.
    /// </summary>
    Infinite
}
