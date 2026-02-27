namespace NeoUI.Demo.Shared.Pages.Components.Grid;

partial class ServerSide
{
    private const string _serverSideSimulatedCode = """
private async Task<DataGridDataResponse<Product>> HandleSimulatedServerDataRequest(
    DataGridDataRequest<Product> request)
{
    // Simulate network delay
    await Task.Delay(300);

    var query = allProducts.AsQueryable();

    foreach (var filter in request.FilterDescriptors)
        query = ApplyFilter(query, filter);

    var totalCount = query.Count();

    if (request.SortDescriptors.Any())
        query = ApplySorting(query, request.SortDescriptors.First());

    var pagedData = query.Skip(request.StartIndex).Take(request.Count).ToList();

    return new DataGridDataResponse<Product>
    {
        Items = pagedData,
        TotalCount = totalCount
    };
}
""";

    private const string _movieDbCode = """
private async Task<DataGridDataResponse<Movie>> HandleMovieDataRequest(
    DataGridDataRequest<Movie> request)
{
    isMovieLoading = true;
    StateHasChanged();

    try
    {
        var page = (request.StartIndex / request.Count) + 1;
        var sortBy = MapSortField(request.SortDescriptors.FirstOrDefault());
        var url = $"https://api.themoviedb.org/3/discover/movie?page={page}";

        var response = await CallTmdbApiAsync<TmdbDiscoverResponse>(url);

        return new DataGridDataResponse<Movie>
        {
            Items = response.Results.Select(MapToMovie),
            TotalCount = response.TotalResults
        };
    }
    finally
    {
        isMovieLoading = false;
        StateHasChanged();
    }
}
""";
}
