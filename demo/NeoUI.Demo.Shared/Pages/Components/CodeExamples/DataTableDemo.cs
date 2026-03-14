namespace NeoUI.Demo.Shared.Pages.Components
{
    partial class DataTableDemo
    {

        private static readonly IReadOnlyList<DemoPropRow> _dataTableProps =
            [
                new("Data", "IEnumerable<TData>", "—", "Required. The data source for the table."),
                new("Columns", "RenderFragment?", "null", "Slot for DataTableColumn child components."),
                new("SelectionMode", "DataTableSelectionMode", "None", "Row selection mode: None, Single, Multiple."),
                new("SelectedItems", "IReadOnlyCollection<TData>", "[ ]", "Currently selected items. Use @bind-SelectedItems for two-way binding."),
                new("EnableKeyboardNavigation", "bool", "true", "Arrow key navigation and Enter/Space row selection."),
                new("ShowToolbar", "bool", "true", "Whether to show the toolbar (search + column visibility)."),
                new("ShowPagination", "bool", "true", "Whether to show pagination controls."),
                new("ColumnsVisibility", "bool", "true", "Whether to show the column visibility toggle button in the toolbar."),
                new("IsLoading", "bool", "false", "Shows a loading indicator instead of table content."),
                new("InitialPageSize", "int", "5", "The initial number of rows per page."),
                new("PageSizes", "int[]", "[5,10,20,50,100]", "Available page size options in the pagination selector."),
                new("Dense", "bool", "true", "Compact cell padding (header h-9, body py-2 px-4). When false, uses h-12 / p-4."),
                new("HeaderBackground", "bool", "true", "Applies bg-muted/50 to the header row."),
                new("HeaderBorder", "bool", "false", "Vertical dividers between header cells (divide-x divide-border)."),
                new("CellBorder", "bool", "false", "Vertical dividers between body cells (divide-x divide-border)."),
                new("HeaderClass", "string?", "null", "Extra CSS classes on the &lt;thead&gt; element."),
                new("HeaderRowClass", "string?", "null", "Extra CSS classes on the header &lt;tr&gt;."),
                new("BodyRowClass", "string?", "null", "Extra CSS classes on each body &lt;tr&gt;."),
                new("ToolbarActions", "RenderFragment?", "null", "Custom action buttons rendered inside the toolbar."),
                new("EmptyTemplate", "RenderFragment?", "null", "Custom content shown when the table has no rows."),
                new("LoadingTemplate", "RenderFragment?", "null", "Custom content shown when IsLoading is true."),
                new("AriaLabel", "string?", "null", "ARIA label applied to the &lt;table&gt; element."),
                new("Class", "string?", "null", "Extra CSS classes on the outer container div."),
                new("OnSort", "EventCallback<(string, SortDirection)>", "—", "Fires when the user changes the sort column or direction."),
                new("OnFilter", "EventCallback<string?>", "—", "Fires when the global search value changes."),
                new("PreprocessData", "Func<IEnumerable<TData>, Task<IEnumerable<TData>>>?", "null", "Async hook to transform data before filtering and sorting."),
            ];

        private static readonly IReadOnlyList<DemoPropRow> _dataTableColumnProps =
            [
                new("Property", "Func<TData, TValue>", "—", "Required. Expression that returns the column value from a row."),
                new("Header", "string", "—", "Required. Column header label text."),
                new("Id", "string?", "null", "Unique column ID. Auto-generated from Header when omitted."),
                new("Sortable", "bool", "false", "Whether clicking the header sorts by this column."),
                new("Filterable", "bool", "false", "Whether this column is searched by the global search filter."),
                new("Alignment", "ColumnAlignment", "Left", "Cell alignment: Left, Center, Right."),
                new("CellTemplate", "RenderFragment<TData>?", "null", "Custom cell render template. Receives the row item as context."),
                new("Visible", "bool", "true", "Whether the column is shown. Can be toggled via the column visibility menu."),
                new("Width", "string?", "null", "Fixed column width (e.g. \"200px\", \"20%\")."),
                new("MinWidth", "string?", "null", "Minimum column width CSS value."),
                new("MaxWidth", "string?", "null", "Maximum column width CSS value."),
                new("CellClass", "string?", "null", "Extra CSS classes on every body cell in this column."),
                new("HeaderClass", "string?", "null", "Extra CSS classes on this column's header cell."),
            ];

        private const string _basicTableCode = """
                <DataTable TData="Person"
                           Data="people"
                           Dense="@_dense"
                           HeaderBackground="@_headerBackground"
                           HeaderBorder="@_headerBorder"
                           CellBorder="@_cellBorder"
                           ColumnsVisibility="@_columnsVisibility"
                           InitialPageSize="5">
                    <Columns>
                        <DataTableColumn TData="Person" TValue="int" Property="@(p => p.Id)" Header="ID" Sortable Alignment="ColumnAlignment.Right" />
                        <DataTableColumn TData="Person" TValue="string" Property="@(p => p.Name)" Header="Name" Sortable />
                        <DataTableColumn TData="Person" TValue="string" Property="@(p => p.Email)" Header="Email" Sortable />
                        <DataTableColumn TData="Person" TValue="int" Property="@(p => p.Age)" Header="Age" Sortable Alignment="ColumnAlignment.Right" />
                    </Columns>
                </DataTable>
                """;

        private const string _rowSelectionCode = """
                <DataTable TData="Person"
                           Data="people"
                           SelectionMode="DataTableSelectionMode.Multiple"
                           EnableKeyboardNavigation="true"
                           InitialPageSize="5">
                    <Columns>
                        <DataTableColumn TData="Person" TValue="string" Property="@(p => p.Name)" Header="Name" Sortable />
                        <DataTableColumn TData="Person" TValue="string" Property="@(p => p.Email)" Header="Email" />
                        <DataTableColumn TData="Person" TValue="string" Property="@(p => p.Role)" Header="Role" />
                    </Columns>
                </DataTable>
                """;

        private const string _customCellCode = """
                <DataTable TData="Person" Data="people" ShowPagination="false">
                    <Columns>
                        <DataTableColumn TData="Person" TValue="string" Property="@(p => p.Name)" Header="Name" Sortable />
                        <DataTableColumn TData="Person" TValue="string" Property="@(p => p.Status)" Header="Status" Sortable>
                            <CellTemplate Context="person">
                                <Badge Variant="@(person.Status == "Active" ? BadgeVariant.Default : BadgeVariant.Outline)">
                                    @person.Status
                                </Badge>
                            </CellTemplate>
                        </DataTableColumn>
                        <DataTableColumn TData="Person" TValue="string" Property="@(p => p.Email)" Header="Actions">
                            <CellTemplate Context="person">
                                <div class="flex gap-2">
                                    <Button Variant="ButtonVariant.Outline" Size="ButtonSize.Small">View</Button>
                                    <Button Variant="ButtonVariant.Ghost" Size="ButtonSize.Small">Edit</Button>
                                </div>
                            </CellTemplate>
                        </DataTableColumn>
                    </Columns>
                </DataTable>
                """;

        private const string _searchCode = """
                <DataTable TData="Person" Data="people" InitialPageSize="10">
                    <Columns>
                        <DataTableColumn TData="Person" TValue="int" Property="@(p => p.Id)" Header="ID" Sortable />
                        <DataTableColumn TData="Person" TValue="string" Property="@(p => p.Name)" Header="Name" Sortable />
                        <DataTableColumn TData="Person" TValue="string" Property="@(p => p.Email)" Header="Email" />
                        <DataTableColumn TData="Person" TValue="int" Property="@(p => p.Age)" Header="Age" Sortable />
                        <DataTableColumn TData="Person" TValue="string" Property="@(p => p.Role)" Header="Role" />
                    </Columns>
                </DataTable>
                """;

        private const string _emptyStateCode = """
                <DataTable TData="Person" Data="emptyPeople" InitialPageSize="5">
                    <Columns>
                        <DataTableColumn TData="Person" TValue="string" Property="@(p => p.Name)" Header="Name" />
                        <DataTableColumn TData="Person" TValue="string" Property="@(p => p.Email)" Header="Email" />
                    </Columns>
                </DataTable>
                """;

        private const string _loadingCode = """
                <DataTable TData="Person" Data="people" IsLoading="true" InitialPageSize="5">
                    <Columns>
                        <DataTableColumn TData="Person" TValue="string" Property="@(p => p.Name)" Header="Name" Sortable />
                        <DataTableColumn TData="Person" TValue="string" Property="@(p => p.Email)" Header="Email" />
                        <DataTableColumn TData="Person" TValue="string" Property="@(p => p.Role)" Header="Role" />
                    </Columns>
                </DataTable>
                """;

        private const string _withoutToolbarCode = """
                <DataTable TData="Person" Data="people" ShowToolbar="false" InitialPageSize="5">
                    <Columns>
                        <DataTableColumn TData="Person" TValue="string" Property="@(p => p.Name)" Header="Name" Sortable />
                        <DataTableColumn TData="Person" TValue="string" Property="@(p => p.Email)" Header="Email" />
                        <DataTableColumn TData="Person" TValue="int" Property="@(p => p.Age)" Header="Age" Sortable />
                    </Columns>
                </DataTable>
                """;

        private const string _customToolbarCode = """
                <DataTable TData="Person" Data="people" InitialPageSize="5">
                    <ToolbarActions>
                        <Button Variant="ButtonVariant.Default" Size="ButtonSize.Small">Add Person</Button>
                        <Button Variant="ButtonVariant.Outline" Size="ButtonSize.Small">Export</Button>
                    </ToolbarActions>
                    <Columns>
                        <DataTableColumn TData="Person" TValue="string" Property="@(p => p.Name)" Header="Name" Sortable />
                        <DataTableColumn TData="Person" TValue="string" Property="@(p => p.Email)" Header="Email" />
                        <DataTableColumn TData="Person" TValue="string" Property="@(p => p.Role)" Header="Role" />
                    </Columns>
                </DataTable>
                """;

        private const string _serverDataCode = """
                <DataTable TData="Order" ServerData="LoadOrders" InitialPageSize="10"
                           PageSizes="@(new[]{ 10, 25, 50 })">
                    <Columns>
                        <DataTableColumn TData="Order" TValue="int"    Property="@(o => o.Id)"       Header="ID"       Sortable />
                        <DataTableColumn TData="Order" TValue="string" Property="@(o => o.Customer)" Header="Customer" Sortable Filterable />
                        <DataTableColumn TData="Order" TValue="string" Property="@(o => o.Product)"  Header="Product"  Sortable Filterable />
                        <DataTableColumn TData="Order" TValue="string" Property="@(o => o.Status)"   Header="Status" />
                        <DataTableColumn TData="Order" TValue="decimal" Property="@(o => o.Amount)"  Header="Amount"   Sortable />
                    </Columns>
                </DataTable>

                @code {
                    private record Order(int Id, string Customer, string Product, string Status, decimal Amount);

                    private static readonly Order[] _allOrders = Enumerable.Range(1, 1000).Select(i => new Order(
                        i,
                        $"Customer {i}",
                        i % 3 == 0 ? "Widget Pro" : i % 3 == 1 ? "Gadget Lite" : "Device Max",
                        i % 4 == 0 ? "Pending" : i % 4 == 1 ? "Shipped" : i % 4 == 2 ? "Delivered" : "Cancelled",
                        Math.Round((decimal)(new Random(i).NextDouble() * 500 + 10), 2)
                    )).ToArray();

                    private async Task<DataTableResult<Order>> LoadOrders(DataTableRequest req)
                    {
                        await Task.Delay(300); // simulate network latency

                        var query = _allOrders.AsEnumerable();

                        if (!string.IsNullOrWhiteSpace(req.SearchText))
                            query = query.Where(o =>
                                o.Customer.Contains(req.SearchText, StringComparison.OrdinalIgnoreCase) ||
                                o.Product.Contains(req.SearchText, StringComparison.OrdinalIgnoreCase));

                        if (!string.IsNullOrWhiteSpace(req.SortColumn))
                            query = (req.SortColumn, req.SortDirection) switch
                            {
                                ("id", SortDirection.Ascending)          => query.OrderBy(o => o.Id),
                                ("id", _)                                => query.OrderByDescending(o => o.Id),
                                ("customer", SortDirection.Ascending)    => query.OrderBy(o => o.Customer),
                                ("customer", _)                          => query.OrderByDescending(o => o.Customer),
                                ("amount", SortDirection.Ascending)      => query.OrderBy(o => o.Amount),
                                ("amount", _)                            => query.OrderByDescending(o => o.Amount),
                                _                                        => query
                            };

                        var total = query.Count();
                        var items = query.Skip((req.Page - 1) * req.PageSize).Take(req.PageSize);
                        return new DataTableResult<Order> { Items = items, TotalCount = total };
                    }
                }
                """;
    }
}
