namespace NeoUI.Blazor.Services.Grid;

/// <summary>
/// Abstraction for providing server-side data to the DataGrid.
/// Implement this interface to wire any external data source (REST API, GraphQL, etc.)
/// as the grid's server-side data provider.
/// </summary>
/// <typeparam name="TItem">The type of items in the grid.</typeparam>
public interface IDataGridServerDataProvider<TItem>
{
    /// <summary>
    /// Fetches a page of data based on the given request (page, sort, filter).
    /// </summary>
    /// <param name="request">The data request containing pagination, sort, and filter parameters.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A task that resolves to the data response with items and total count.</returns>
    Task<DataGridDataResponse<TItem>> GetDataAsync(DataGridDataRequest<TItem> request, CancellationToken cancellationToken = default);
}
