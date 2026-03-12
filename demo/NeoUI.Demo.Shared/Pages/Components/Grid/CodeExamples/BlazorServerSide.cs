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

    public const string SelectionExample = """
// IdField is the key — it lets the grid track which rows are "the same" across page fetches.
// Without IdField the grid can't match previously selected rows to newly loaded rows.
<DataGrid TItem="Order"
          RowModelType="DataGridRowModelType.BlazorServerSide"
          SelectionMode="DataGridSelectionMode.Multiple"
          @bind-SelectedItems="_selectedOrders"
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
    private IReadOnlyCollection<Order> _selectedOrders = Array.Empty<Order>();

    private Task<DataGridDataResponse<Order>> HandleOrderRequest(DataGridDataRequest<Order> request)
    {
        var query = _allOrders.AsQueryable();

        foreach (var filter in request.FilterDescriptors)
        {
            var value = filter.Value?.ToString() ?? "";
            query = filter.Field?.ToLowerInvariant() switch {
                "customername" => query.Where(o => o.CustomerName.Contains(value, StringComparison.OrdinalIgnoreCase)),
                "status"       => query.Where(o => o.Status.Contains(value, StringComparison.OrdinalIgnoreCase)),
                _              => query
            };
        }

        IOrderedQueryable<Order>? sorted = null;
        foreach (var sort in request.SortDescriptors.OrderBy(s => s.Order))
        {
            var asc = sort.Direction == DataGridSortDirection.Ascending;
            sorted = sort.Field?.ToLowerInvariant() switch {
                "id"           => sorted == null ? (asc ? query.OrderBy(o => o.Id)           : query.OrderByDescending(o => o.Id))           : (asc ? sorted.ThenBy(o => o.Id)           : sorted.ThenByDescending(o => o.Id)),
                "customername" => sorted == null ? (asc ? query.OrderBy(o => o.CustomerName) : query.OrderByDescending(o => o.CustomerName)) : (asc ? sorted.ThenBy(o => o.CustomerName) : sorted.ThenByDescending(o => o.CustomerName)),
                "orderdate"    => sorted == null ? (asc ? query.OrderBy(o => o.OrderDate)    : query.OrderByDescending(o => o.OrderDate))    : (asc ? sorted.ThenBy(o => o.OrderDate)    : sorted.ThenByDescending(o => o.OrderDate)),
                "total"        => sorted == null ? (asc ? query.OrderBy(o => o.Total)        : query.OrderByDescending(o => o.Total))        : (asc ? sorted.ThenBy(o => o.Total)        : sorted.ThenByDescending(o => o.Total)),
                _              => sorted
            };
        }
        if (sorted != null) query = sorted;

        var total = query.Count();
        var items = query.Skip(request.StartIndex).Take(request.Count).ToList();
        return Task.FromResult(new DataGridDataResponse<Order> { Items = items, TotalCount = total });
    }
}
""";

    public const string FilterBuilderIntegration = """
// FilterBuilder sits above the grid; no column has Filterable="true"
<FilterBuilder TData="Order"
               @bind-Filters="_filterBuilderFilters"
               OnFilterChange="HandleFilterBuilderFilterChange"
               Class="mb-4">
    <FilterFields>
        <FilterField Field="CustomerName" Label="Customer" Icon="user"        Type="FilterFieldType.Text" />
        <FilterField Field="Status"       Label="Status"   Icon="activity"    Type="FilterFieldType.Select"
                     Options="@_orderStatusOptions" />
        <FilterField Field="Total"        Label="Total"    Icon="dollar-sign" Type="FilterFieldType.Number"
                     EditorType="FilterEditorType.Currency" Min="0" />
        <FilterField Field="OrderDate"    Label="Date"     Icon="calendar"    Type="FilterFieldType.Date" />
    </FilterFields>
    <FilterPresets>
        <FilterPreset Name="High-Value Pending" Icon="trending-up"  Filters="@_highValuePendingPreset" />
        <FilterPreset Name="Delivered"          Icon="circle-check" Filters="@_deliveredPreset" />
    </FilterPresets>
</FilterBuilder>

<DataGrid TItem="Order"
          @ref="_filterBuilderGrid"
          RowModelType="DataGridRowModelType.BlazorServerSide"
          OnServerDataRequest="HandleFilterBuilderOrderRequest"
          PagingMode="DataGridPagingMode.Server"
          PageSize="15"
          @bind-TotalServerRowCount="_totalFilterBuilderOrders"
          IdField="@nameof(Order.Id)"
          FillWidth="true"
          Height="380px">
    <Columns>
        <DataGridColumn Field="@nameof(Order.Id)"           Header="ID"       Sortable="true" Width="80px" />
        <DataGridColumn Field="@nameof(Order.CustomerName)" Header="Customer" Sortable="true" />
        <DataGridColumn Field="@nameof(Order.OrderDate)"    Header="Date"     Sortable="true" DataFormatString="{0:dd MMM yyyy}" />
        <DataGridColumn Field="@nameof(Order.Total)"        Header="Total"    Sortable="true" DataFormatString="{0:C}" />
        <DataGridColumn Field="@nameof(Order.Status)"       Header="Status"   Sortable="true" />
    </Columns>
</DataGrid>

@code {
    private DataGrid<Order>? _filterBuilderGrid;
    private FilterGroup      _filterBuilderFilters     = new();
    private int              _totalFilterBuilderOrders = 0;

    private readonly List<SelectOption> _orderStatusOptions =
    [
        new("Pending",    "Pending"),
        new("Processing", "Processing"),
        new("Shipped",    "Shipped"),
        new("Delivered",  "Delivered"),
        new("Cancelled",  "Cancelled"),
    ];

    private readonly FilterGroup _highValuePendingPreset = new()
    {
        Conditions =
        [
            new FilterCondition { Field = "Status", Operator = FilterOperator.Equals,      Value = "Pending" },
            new FilterCondition { Field = "Total",  Operator = FilterOperator.GreaterThan, Value = 5000m }
        ]
    };

    private readonly FilterGroup _deliveredPreset = new()
    {
        Conditions =
        [
            new FilterCondition { Field = "Status", Operator = FilterOperator.Equals, Value = "Delivered" }
        ]
    };

    private async Task HandleFilterBuilderFilterChange(FilterGroup filters)
    {
        _filterBuilderFilters = filters;
        if (_filterBuilderGrid != null)
            await _filterBuilderGrid.RefreshAsync();
    }

    private Task<DataGridDataResponse<Order>> HandleFilterBuilderOrderRequest(DataGridDataRequest<Order> request)
    {
        // ApplyFilter() is a built-in LINQ extension — it handles all operator types
        // using the FilterCondition objects emitted by FilterBuilder directly.
        var query = _allOrders.AsQueryable().ApplyFilter(_filterBuilderFilters);

        IOrderedQueryable<Order>? sorted = null;
        foreach (var sort in request.SortDescriptors.OrderBy(s => s.Order))
        {
            var asc = sort.Direction == DataGridSortDirection.Ascending;
            sorted = sort.Field?.ToLowerInvariant() switch {
                "id"           => sorted == null ? (asc ? query.OrderBy(o => o.Id)           : query.OrderByDescending(o => o.Id))           : (asc ? sorted.ThenBy(o => o.Id)           : sorted.ThenByDescending(o => o.Id)),
                "customername" => sorted == null ? (asc ? query.OrderBy(o => o.CustomerName) : query.OrderByDescending(o => o.CustomerName)) : (asc ? sorted.ThenBy(o => o.CustomerName) : sorted.ThenByDescending(o => o.CustomerName)),
                "orderdate"    => sorted == null ? (asc ? query.OrderBy(o => o.OrderDate)    : query.OrderByDescending(o => o.OrderDate))    : (asc ? sorted.ThenBy(o => o.OrderDate)    : sorted.ThenByDescending(o => o.OrderDate)),
                "total"        => sorted == null ? (asc ? query.OrderBy(o => o.Total)        : query.OrderByDescending(o => o.Total))        : (asc ? sorted.ThenBy(o => o.Total)        : sorted.ThenByDescending(o => o.Total)),
                _              => sorted
            };
        }
        if (sorted != null) query = sorted;

        var total = query.Count();
        var items = query.Skip(request.StartIndex).Take(request.Count).ToList();
        return Task.FromResult(new DataGridDataResponse<Order> { Items = items, TotalCount = total });
    }
}
""";
}
