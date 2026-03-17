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
                new("ServerData", "Func<DataTableRequest, Task<DataTableResult<TData>>>?", "null", "Server-side paged data callback. Request carries Page, PageSize, SortDescriptors, and SearchText."),
                new("ItemsProvider", "DataTableVirtualProvider<TData>?", "null", "Server-side virtualised provider for infinite scroll. Request carries StartIndex, Count, SortDescriptors, SearchText, and CancellationToken. Overrides Data and ServerData when set."),
                new("Virtualize", "bool", "false", "Client-side DOM-windowing via Virtualize. All rows stay in memory; only visible nodes are rendered. Pagination is hidden."),
                new("ItemHeight", "float", "40", "Row height in px passed to the virtualizer as ItemSize. Must match your actual row height when Virtualize=true or ItemsProvider is set."),
                new("Height", "string", "\"400px\"", "CSS height of the scroll container. Required when Virtualize=true or ItemsProvider is set."),
                new("VirtualizeOverscanCount", "int", "3", "Extra rows rendered beyond the viewport to reduce blank flicker during fast scrolling."),
                new("OnSort", "EventCallback<(string, SortDirection)>", "—", "Fires when the user changes the sort column or direction."),
                new("OnFilter", "EventCallback<string?>", "—", "Fires when the global search value changes."),
                new("PreprocessData", "Func<IEnumerable<TData>, Task<IEnumerable<TData>>>?", "null", "Async hook to transform data before filtering and sorting."),
                new("ChildrenProperty", "Func<TData, IEnumerable<TData>?>?", "null", "Returns child items for each row, activating tree mode. Children render inline with depth indentation. Incompatible with ItemsProvider (virtualised server mode)."),
                new("LoadChildrenAsync", "Func<TData, Task<IEnumerable<TData>>>?", "null", "Lazy async child loader — called on first expand of a node. Activates tree mode. Use with HasChildrenField to control whether the expander is shown before loading."),
                new("HasChildrenField", "Func<TData, bool>?", "null", "Hints whether a node has children without loading them. In lazy mode, the expander is shown for all nodes unless this is provided."),
                new("ValueField", "Func<TData, string>?", "null", "Returns a stable unique key per item for tracking expanded state. Strongly recommended in tree mode; falls back to RuntimeHelpers.GetHashCode when omitted."),
                new("ExpandedValues", "HashSet<string>?", "null", "Two-way bindable set of expanded row keys. Use @bind-ExpandedValues to persist or restore the expand state across renders."),
                new("Resizable", "bool", "false", "Adds drag handles on column headers so users can adjust widths at runtime. Activates table-layout:fixed automatically. Per-column Resizable on DataTableColumn overrides this."),
                new("MinColumnWidth", "int", "50", "Minimum column width in pixels enforced during drag-to-resize."),
                new("OnColumnResize", "EventCallback<(string, string)>", "—", "Fires when the user finishes resizing a column. Provides (ColumnId, NewCssWidth)."),
                new("Reorderable", "bool", "false", "Lets users drag column headers to reorder them. Columns animate into place as you drag. Pinned and selection columns are excluded. Per-column Reorderable overrides this."),
                new("OnColumnReorder", "EventCallback<(string, int)>", "—", "Fires when the user drops a column into a new position. Provides (ColumnId, NewIndex)."),
                new("RowContextMenu", "RenderFragment<DataTableRowMenuContext<TData>>?", "null", "Template for the context menu shown on row right-click. Receives DataTableRowMenuContext with Item, SelectedItems, and VisibleColumns."),
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
                new("Pinned", "ColumnPinnedSide", "None", "Sticks the column to the Left or Right edge during horizontal scroll. A shadow separator marks the boundary. Requires an explicit px Width for reliable offset calculation."),
                new("Resizable", "bool?", "null", "Per-column resize override. null inherits the table-level Resizable setting."),
                new("Reorderable", "bool?", "null", "Per-column reorder override. null inherits the table-level Reorderable setting. Pinned columns and the selection column are always excluded."),
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

        private const string _columnPinningCode = """
                <DataTable TData="Employee" Data="@_employees" ShowPagination="false">
                    <Columns>
                        <DataTableColumn TData="Employee" TValue="string" Property="@(e => e.Name)"
                            Header="Name" Width="200px" Pinned="ColumnPinnedSide.Left" Sortable />
                        <DataTableColumn TData="Employee" TValue="string" Property="@(e => e.Department)" Header="Department" Width="140px" Sortable />
                        <DataTableColumn TData="Employee" TValue="string" Property="@(e => e.Title)"      Header="Title"      Width="180px" />
                        <DataTableColumn TData="Employee" TValue="string" Property="@(e => e.Location)"   Header="Location"   Width="130px" />
                        <DataTableColumn TData="Employee" TValue="string" Property="@(e => e.Manager)"    Header="Manager"    Width="160px" />
                        <DataTableColumn TData="Employee" TValue="string" Property="@(e => e.StartDate)"  Header="Start Date" Width="120px" />
                        <DataTableColumn TData="Employee" TValue="string" Property="@(e => e.Salary)"     Header="Salary"     Width="110px" Alignment="ColumnAlignment.Right" />
                        <DataTableColumn TData="Employee" TValue="string" Property="@(e => e.Status)"     Header="Status"     Width="100px" />
                        <DataTableColumn TData="Employee" TValue="string" Property="@(e => e.Name)"
                            Header="Actions" Width="90px" Pinned="ColumnPinnedSide.Right" Alignment="ColumnAlignment.Center">
                            <CellTemplate Context="e">
                                <!-- action buttons -->
                            </CellTemplate>
                        </DataTableColumn>
                    </Columns>
                </DataTable>
                """;

        private const string _treeRowsCode = """
                <DataTable TData="CoaAccount" Data="@_coaAccounts"
                           ChildrenProperty="@(a => a.Children)"
                           ValueField="@(a => a.Code)"
                           ShowPagination="false">
                    <Columns>
                        <DataTableColumn TData="CoaAccount" TValue="string" Property="@(a => a.Name)"
                            Header="Account Name" Width="220px" Pinned="ColumnPinnedSide.Left" Sortable />
                        <DataTableColumn TData="CoaAccount" TValue="string" Property="@(a => a.Code)"          Header="Code"           Width="80px" />
                        <DataTableColumn TData="CoaAccount" TValue="string" Property="@(a => a.Type)"          Header="Type"           Width="120px" />
                        <DataTableColumn TData="CoaAccount" TValue="string" Property="@(a => a.DetailType)"    Header="Detail Type"    Width="190px" />
                        <DataTableColumn TData="CoaAccount" TValue="string" Property="@(a => a.Description)"   Header="Description"    Width="240px" />
                        <DataTableColumn TData="CoaAccount" TValue="string" Property="@(a => a.Currency)"      Header="Currency"       Width="90px" />
                        <DataTableColumn TData="CoaAccount" TValue="string" Property="@(a => a.TaxLine)"       Header="Tax Line"       Width="180px" />
                        <DataTableColumn TData="CoaAccount" TValue="string" Property="@(a => a.NormalBalance)" Header="Normal Balance" Width="140px" />
                        <DataTableColumn TData="CoaAccount" TValue="string" Property="@(a => a.Balance)"       Header="Balance"        Width="130px" Alignment="ColumnAlignment.Right" />
                        <DataTableColumn TData="CoaAccount" TValue="string" Property="@(a => a.Status)"        Header="Status"         Width="90px" />
                        <DataTableColumn TData="CoaAccount" TValue="string" Property="@(a => a.CreatedBy)"     Header="Created By"     Width="130px" />
                        <DataTableColumn TData="CoaAccount" TValue="string" Property="@(a => a.LastModified)"  Header="Last Modified"  Width="130px" />
                        <DataTableColumn TData="CoaAccount" TValue="string" Property="@(a => a.Code)"
                            Header="Actions" Width="80px" Pinned="ColumnPinnedSide.Right" Alignment="ColumnAlignment.Center">
                            <CellTemplate Context="a">
                                <!-- action buttons for leaf accounts -->
                            </CellTemplate>
                        </DataTableColumn>
                    </Columns>
                </DataTable>

                @code {
                    record CoaAccount(string Code, string Name, string Type, string DetailType,
                        string Description, string Currency, string TaxLine, string NormalBalance,
                        string Balance, string Status, string CreatedBy, string LastModified,
                        List<CoaAccount> Children);
                }
                """;

        private const string _serverDataCode = """
                <DataTable TData="Order" ServerData="LoadOrders" InitialPageSize="10"
                           PageSizes="@(new[]{ 10, 25, 50 })">
                    <Columns>
                        <DataTableColumn TData="Order" TValue="int"     Property="@(o => o.Id)"       Header="ID"       Sortable />
                        <DataTableColumn TData="Order" TValue="string"  Property="@(o => o.Customer)" Header="Customer" Sortable Filterable />
                        <DataTableColumn TData="Order" TValue="string"  Property="@(o => o.Product)"  Header="Product"  Sortable Filterable />
                        <DataTableColumn TData="Order" TValue="string"  Property="@(o => o.Status)"   Header="Status" />
                        <DataTableColumn TData="Order" TValue="decimal" Property="@(o => o.Amount)"   Header="Amount"   Sortable />
                    </Columns>
                </DataTable>

                @code {
                    private record Order(int Id, string Customer, string Product, string Status, decimal Amount);

                    private async Task<DataTableResult<Order>> LoadOrders(DataTableRequest req)
                    {
                        await Task.Delay(300);
                        var query = _allOrders.AsEnumerable();

                        if (!string.IsNullOrWhiteSpace(req.SearchText))
                            query = query.Where(o =>
                                o.Customer.Contains(req.SearchText, StringComparison.OrdinalIgnoreCase) ||
                                o.Product.Contains(req.SearchText, StringComparison.OrdinalIgnoreCase));

                        var sort = req.SortDescriptors.FirstOrDefault();
                        if (sort is not null)
                            query = (sort.ColumnId, sort.Direction) switch
                            {
                                ("id",       SortDirection.Ascending) => query.OrderBy(o => o.Id),
                                ("id",       _)                       => query.OrderByDescending(o => o.Id),
                                ("customer", SortDirection.Ascending) => query.OrderBy(o => o.Customer),
                                ("customer", _)                       => query.OrderByDescending(o => o.Customer),
                                ("amount",   SortDirection.Ascending) => query.OrderBy(o => o.Amount),
                                ("amount",   _)                       => query.OrderByDescending(o => o.Amount),
                                _                                     => query
                            };

                        var total = query.Count();
                        var items = query.Skip((req.Page - 1) * req.PageSize).Take(req.PageSize);
                        return new DataTableResult<Order> { Items = items, TotalCount = total };
                    }
                }
                """;

        private const string _infiniteScrollServerCode = """
                <DataTable TData="Order"
                           ItemsProvider="@LoadOrdersVirtual"
                           ItemHeight="40"
                           Height="360px">
                    <Columns>
                        <DataTableColumn TData="Order" TValue="int"     Property="@(o => o.Id)"       Header="ID"       Sortable />
                        <DataTableColumn TData="Order" TValue="string"  Property="@(o => o.Customer)" Header="Customer" Sortable Filterable />
                        <DataTableColumn TData="Order" TValue="string"  Property="@(o => o.Product)"  Header="Product"  Sortable Filterable />
                        <DataTableColumn TData="Order" TValue="string"  Property="@(o => o.Status)"   Header="Status" />
                        <DataTableColumn TData="Order" TValue="decimal" Property="@(o => o.Amount)"   Header="Amount"   Sortable />
                    </Columns>
                </DataTable>

                @code {
                    private async ValueTask<ItemsProviderResult<Order>> LoadOrdersVirtual(DataTableVirtualRequest req)
                    {
                        await Task.Delay(150); // simulate network latency
                        var query = _allOrders.AsEnumerable();

                        if (!string.IsNullOrWhiteSpace(req.SearchText))
                            query = query.Where(o =>
                                o.Customer.Contains(req.SearchText, StringComparison.OrdinalIgnoreCase) ||
                                o.Product.Contains(req.SearchText, StringComparison.OrdinalIgnoreCase));

                        // SortDescriptors holds the active sort — currently at most one entry,
                        // designed for multi-column sort in the future.
                        var sort = req.SortDescriptors.FirstOrDefault();
                        if (sort is not null)
                            query = (sort.ColumnId, sort.Direction) switch
                            {
                                ("id",       SortDirection.Ascending) => query.OrderBy(o => o.Id),
                                ("id",       _)                       => query.OrderByDescending(o => o.Id),
                                ("customer", SortDirection.Ascending) => query.OrderBy(o => o.Customer),
                                ("customer", _)                       => query.OrderByDescending(o => o.Customer),
                                ("amount",   SortDirection.Ascending) => query.OrderBy(o => o.Amount),
                                ("amount",   _)                       => query.OrderByDescending(o => o.Amount),
                                _                                     => query
                            };

                        var total = query.Count();
                        var items = query.Skip(req.StartIndex).Take(req.Count).ToList();
                        return new(items, total);
                    }
                }
                """;

        private const string _virtualizeClientCode = """
                @* All rows in memory — only visible DOM nodes are rendered *@
                <DataTable TData="Order"
                           Data="@_allOrders"
                           Virtualize="true"
                           ItemHeight="40"
                           Height="360px">
                    <Columns>
                        <DataTableColumn TData="Order" TValue="int"     Property="@(o => o.Id)"       Header="ID"       Sortable />
                        <DataTableColumn TData="Order" TValue="string"  Property="@(o => o.Customer)" Header="Customer" Sortable Filterable />
                        <DataTableColumn TData="Order" TValue="string"  Property="@(o => o.Product)"  Header="Product"  Sortable Filterable />
                        <DataTableColumn TData="Order" TValue="string"  Property="@(o => o.Status)"   Header="Status" />
                        <DataTableColumn TData="Order" TValue="decimal" Property="@(o => o.Amount)"   Header="Amount"   Sortable />
                    </Columns>
                </DataTable>
                """;

        private const string _resizeReorderCode = """
                <DataTable TData="Employee"
                           Data="@employees"
                           Resizable="true"
                           Reorderable="true"
                           OnColumnResize="@(args => Console.WriteLine($"{args.ColumnId}: {args.Width}"))"
                           OnColumnReorder="@(args => Console.WriteLine($"{args.ColumnId} → index {args.NewIndex}"))">
                    <Columns>
                        <DataTableColumn TData="Employee" TValue="string" Property="@(e => e.Name)"       Header="Name"       Width="160px" Sortable />
                        <DataTableColumn TData="Employee" TValue="string" Property="@(e => e.Department)" Header="Department" Width="140px" Sortable />
                        <DataTableColumn TData="Employee" TValue="string" Property="@(e => e.Title)"      Header="Title"      Width="180px" />
                        @* Disable resize/reorder on a specific column *@
                        <DataTableColumn TData="Employee" TValue="string" Property="@(e => e.Status)"     Header="Status"
                                         Resizable="false" Reorderable="false" Width="100px" />
                    </Columns>
                </DataTable>
                """;

        private const string _rowContextMenuCode = """
                <DataTable TData="Employee"
                           Data="@employees"
                           SelectionMode="DataTableSelectionMode.Multiple">
                    <Columns>
                        <DataTableColumn TData="Employee" TValue="string" Property="@(e => e.Name)"   Header="Name"   Sortable />
                        <DataTableColumn TData="Employee" TValue="string" Property="@(e => e.Status)" Header="Status" />
                    </Columns>
                    <RowContextMenu Context="ctx">
                        <ContextMenuItem OnClick="@(() => Edit(ctx.Item))">
                            <LucideIcon Name="pencil" Size="14" Class="mr-2" /> Edit
                        </ContextMenuItem>
                        <ContextMenuSeparator />
                        @if (ctx.SelectedItems.Count > 1)
                        {
                            <ContextMenuItem OnClick="@(() => ExportSelected(ctx.SelectedItems))">
                                Export @ctx.SelectedItems.Count Selected
                            </ContextMenuItem>
                        }
                        <ContextMenuItem Class="text-destructive focus:text-destructive"
                                         OnClick="@(() => Delete(ctx.Item))">
                            <LucideIcon Name="trash-2" Size="14" Class="mr-2" /> Delete
                        </ContextMenuItem>
                    </RowContextMenu>
                </DataTable>
                """;
    }
}
