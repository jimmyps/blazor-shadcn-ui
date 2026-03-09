namespace NeoUI.Demo.Shared.Pages.Components.Grid;

partial class BlazorServerSide
{
}

/// <summary>
/// Code example strings displayed in the BlazorServerSide demo page.
/// </summary>
public static class BlazorServerSideExamples
{
    public const string OnServerDataRequestUsage = """
// Minimal usage: pass an async Func to OnServerDataRequest
<DataGrid TItem="Order"
          RowModelType="DataGridRowModelType.BlazorServerSide"
          OnServerDataRequest="HandleOrderRequest"
          PagingMode="DataGridPagingMode.Server"
          PageSize="20"
          @bind-TotalServerRowCount="_totalOrders"
          IdField="@nameof(Order.Id)">
    <Columns>
        <DataGridColumn Field="@nameof(Order.Id)"           Header="ID"       Sortable="true" Width="80px" />
        <DataGridColumn Field="@nameof(Order.CustomerName)" Header="Customer" Sortable="true" Filterable="true" />
        <DataGridColumn Field="@nameof(Order.OrderDate)"    Header="Date"     Sortable="true" DataFormatString="{0:dd MMM yyyy}" />
        <DataGridColumn Field="@nameof(Order.Total)"        Header="Total"    Sortable="true" DataFormatString="{0:C}" />
        <DataGridColumn Field="@nameof(Order.Status)"       Header="Status"   Filterable="true" />
    </Columns>
</DataGrid>

@code {
    private int _totalOrders;

    private async Task<DataGridDataResponse<Order>> HandleOrderRequest(DataGridDataRequest<Order> request)
    {
        // Apply filters to your data source
        var query = _allOrders.AsQueryable();
        foreach (var filter in request.FilterDescriptors)
        {
            query = filter.Field?.ToLowerInvariant() switch {
                "customername" => query.Where(o => o.CustomerName.Contains(filter.Value ?? "")),
                "status"       => query.Where(o => o.Status == filter.Value),
                _              => query
            };
        }

        // Apply sorts
        foreach (var sort in request.SortDescriptors.OrderBy(s => s.Order))
        {
            query = sort.Field?.ToLowerInvariant() switch {
                "orderdate" => sort.Direction == DataGridSortDirection.Ascending
                    ? query.OrderBy(o => o.OrderDate) : query.OrderByDescending(o => o.OrderDate),
                "total"     => sort.Direction == DataGridSortDirection.Ascending
                    ? query.OrderBy(o => o.Total)     : query.OrderByDescending(o => o.Total),
                _           => query
            };
        }

        var total = query.Count();
        var items = query.Skip(request.StartIndex).Take(request.Count).ToList();

        return new DataGridDataResponse<Order> { Items = items, TotalCount = total };
    }
}
""";

    public const string HttpProviderSubclassExample = """
// 1. Wire the provider as a [Parameter] on the DataGrid
<DataGrid TItem="Product"
          RowModelType="DataGridRowModelType.BlazorServerSide"
          ServerDataProvider="@_productProvider"
          PagingMode="DataGridPagingMode.Server"
          PageSize="25"
          IdField="@nameof(Product.Id)">
    <Columns>
        <DataGridColumn Field="@nameof(Product.Name)"     Header="Name"     Sortable="true" Filterable="true" />
        <DataGridColumn Field="@nameof(Product.Category)" Header="Category" Sortable="true" Filterable="true" />
        <DataGridColumn Field="@nameof(Product.Price)"    Header="Price"    Sortable="true" DataFormatString="{0:C}" />
    </Columns>
</DataGrid>

// 2. Subclass HttpDataGridProvider<TItem> to target any REST API
public class ProductApiProvider : HttpDataGridProvider<Product>
{
    private readonly IAccessTokenProvider _tokenProvider;

    public ProductApiProvider(HttpClient http, IAccessTokenProvider tokenProvider)
        : base(http, "https://api.example.com/products/search")
    {
        _tokenProvider = tokenProvider;
    }

    // Override to inject auth headers, tenant ID, API version, etc.
    protected override async Task ConfigureRequestAsync(
        HttpRequestMessage request,
        DataGridDataRequest<Product> dataRequest,
        CancellationToken ct)
    {
        var token = await _tokenProvider.RequestAccessToken();
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token.Value);
        request.Headers.Add("X-Api-Version", "2");
    }

    // Override to handle a non-standard response envelope
    // (only needed if your API doesn't return DataGridDataResponse directly)
    protected override async Task<DataGridDataResponse<Product>> MapResponseAsync(
        HttpResponseMessage response,
        CancellationToken ct)
    {
        var envelope = await response.Content.ReadFromJsonAsync<ApiEnvelope<Product>>(cancellationToken: ct);
        return new DataGridDataResponse<Product>
        {
            Items      = envelope?.Data ?? [],
            TotalCount = envelope?.Total ?? 0
        };
    }
}
""";

    public const string SimulatedLatencyUsage = """
// Demonstrate loading overlay with an artificial delay
<DataGrid TItem="Order"
          RowModelType="DataGridRowModelType.BlazorServerSide"
          OnServerDataRequest="HandleSlowOrderRequest"
          PagingMode="DataGridPagingMode.Server"
          PageSize="10"
          IdField="@nameof(Order.Id)">
    <Columns>
        <DataGridColumn Field="@nameof(Order.CustomerName)" Header="Customer" Sortable="true" />
        <DataGridColumn Field="@nameof(Order.Total)"        Header="Total"    DataFormatString="{0:C}" />
    </Columns>
</DataGrid>

@code {
    private async Task<DataGridDataResponse<Order>> HandleSlowOrderRequest(DataGridDataRequest<Order> request)
    {
        await Task.Delay(800); // Simulate 800 ms server round-trip

        var items = _allOrders.Skip(request.StartIndex).Take(request.Count).ToList();
        return new DataGridDataResponse<Order>
        {
            Items      = items,
            TotalCount = _allOrders.Count
        };
    }
}
""";
}
