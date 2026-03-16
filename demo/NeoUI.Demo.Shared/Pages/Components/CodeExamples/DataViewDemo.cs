using NeoUI.Blazor;

namespace NeoUI.Demo.Shared.Pages.Components;

partial class DataViewDemo
{
    private record UserRow(int Id, string Name, string Role, string Status, string Initials, string Color);
    private record ProductRow(int Id, string Name, string Category, decimal Price, bool InStock, string Icon);

    private static readonly ProductRow[] _emptyProducts = [];

    private static readonly UserRow[] _users =
    [
        new(1, "Alice Johnson",  "Senior Engineer",   "Active",  "AJ", "#7c3aed"),
        new(2, "Bob Martinez",   "Product Manager",   "Active",  "BM", "#2563eb"),
        new(3, "Carol Chen",     "UX Designer",       "Away",    "CC", "#db2777"),
        new(4, "David Kim",      "Backend Engineer",  "Active",  "DK", "#059669"),
        new(5, "Eva Rossi",      "Data Scientist",    "Offline", "ER", "#d97706"),
        new(6, "Frank Müller",   "DevOps Engineer",   "Active",  "FM", "#0891b2"),
        new(7, "Grace Nakamura", "Frontend Engineer", "Away",    "GN", "#e11d48"),
        new(8, "Hiro Tanaka",    "Tech Lead",         "Active",  "HT", "#4f46e5"),
    ];

    private static readonly ProductRow[] _products =
    [
        new(1,  "Mechanical Keyboard",         "Peripherals",  149.99m, true,  "keyboard"),
        new(2,  "4K Monitor",                  "Displays",     399.00m, true,  "monitor"),
        new(3,  "USB-C Hub",                   "Accessories",   49.95m, true,  "usb"),
        new(4,  "Webcam Pro",                  "Peripherals",   89.00m, false, "camera"),
        new(5,  "Desk Lamp",                   "Office",        35.50m, true,  "lamp-desk"),
        new(6,  "Noise Cancelling Headphones", "Audio",        249.99m, true,  "headphones"),
        new(7,  "Mouse Pad XL",                "Accessories",   19.99m, false, "layout-dashboard"),
        new(8,  "Laptop Stand",                "Accessories",   42.00m, true,  "laptop"),
        new(9,  "Ethernet Cable 3m",           "Networking",     8.99m, true,  "cable"),
        new(10, "Power Strip",                 "Accessories",   25.00m, true,  "zap"),
        new(11, "Cable Organizer",             "Accessories",   12.50m, true,  "boxes"),
        new(12, "Portable SSD 1TB",            "Storage",       79.99m, false, "hard-drive"),
    ];

    private static BadgeVariant StatusVariant(string status) => status switch
    {
        "Active"  => BadgeVariant.Default,
        "Away"    => BadgeVariant.Secondary,
        _         => BadgeVariant.Outline
    };

    private static readonly IReadOnlyList<DemoPropRow> _props =
    [
        new("Items",               "IEnumerable<TItem>?",    "null",           "Data source."),
        new("Layout",              "DataViewLayout",         "List",           "Initial layout mode: List or Grid."),
        new("PageSize",            "int",                    "0",              "Initial items per page. 0 = all items, no pagination."),
        new("PageSizes",           "int[]",                  "[10,25,50,100]", "Page-size options shown in the selector."),
        new("GridColumns",         "int",                    "3",              "Column count in Grid layout (1–6)."),
        new("GridColumnMinWidth",  "string?",                "null",           "Min tile width for auto-fill grid columns. Accepts any CSS length (e.g. \"160px\", \"10rem\") or a Tailwind spacing key (e.g. \"40\"). null/empty = disabled; falls back to GridColumns."),
        new("ShowToolbar",         "bool",                   "true",           "Whether the toolbar is visible."),
        new("ToolbarActions",      "RenderFragment?",        "null",           "Custom content on the right of the toolbar (after search/sort, before the layout toggle)."),
        new("Fields",              "RenderFragment?",        "null",           "DataViewColumn declarations that enable toolbar search and sort."),
        new("GroupBy",             "Func<TItem, object>?",   "null",           "Groups items by the returned key."),
        new("GroupHeaderTemplate", "RenderFragment<object>?","null",           "Custom group header. Defaults to the key's ToString()."),
        new("ItemKey",             "Func<TItem, object>?",   "null",           "Function to extract a unique key per item for diffing."),
        new("Loading",             "bool",                   "false",          "Shows a loading spinner."),
        new("LoadingText",         "string",                 "\"Loading…\"",   "Text shown in loading state."),
        new("EmptyText",           "string",                 "\"No items\"",   "Fallback text when empty and EmptyTemplate is null."),
        new("ShowPagination",      "bool",                   "true",           "Whether to render the pagination bar (requires PageSize > 0)."),
        new("SelectionMode",       "DataViewSelectionMode",  "None",           "None, Single, or Multiple item selection."),
        new("SelectedItem",        "TItem?",                 "null",           "Selected item in Single mode (two-way bindable)."),
        new("SelectedItems",       "IReadOnlyList<TItem>?",  "null",           "Selected items in Multiple mode (two-way bindable)."),
        new("CheckVariant",        "DataViewCheckVariant",   "CircleCheck",    "CircleCheck, Check (invisible when unselected), or None."),
        new("ListTemplate",        "RenderFragment<TItem>?", "null",           "List-specific item template. Provide with GridTemplate to enable the toggle."),
        new("GridTemplate",        "RenderFragment<TItem>?", "null",           "Grid-specific item template. Provide with ListTemplate to enable the toggle."),
        new("ItemTemplate",        "RenderFragment<TItem>?", "null",           "Shared template for both layouts when layout-specific templates are not set."),
        new("EmptyTemplate",       "RenderFragment?",        "null",           "Custom empty-state content."),
        new("Header",              "RenderFragment?",        "null",           "Optional header rendered above the toolbar and items."),
        new("ItemsProvider",       "ItemsProviderDelegate?", "null",           "Server-side data callback for infinite-scroll virtualization. When set, Items is ignored."),
        new("Virtualize",          "bool",                   "false",          "When true, renders client-side Items via <Virtualize> (DOM windowing). Pagination is hidden."),
        new("ItemHeight",          "float",                  "72",             "Per-item height in px passed to the virtualizer as ItemSize. Required when Virtualize=true or ItemsProvider is set."),
        new("Height",                 "string",                 "\"500px\"",      "CSS height of the scroll container. Required when Virtualize=true or ItemsProvider is set."),
        new("VirtualizeOverscanCount","int",                    "3",              "Extra items rendered beyond the visible viewport to reduce blank flicker during fast scrolling."),
    ];

    private static class DataViewDemoCode
    {
        public const string Basic =
            """
            <DataView Items="@_users" PageSize="5" ItemKey="@(u => u.Id)">
                <Fields>
                    <DataViewColumn TItem="UserRow" Header="Name" Property="@(u => u.Name)" Filterable="true" Sortable="true" />
                    <DataViewColumn TItem="UserRow" Header="Role" Property="@(u => u.Role)" Filterable="true" Sortable="true" />
                </Fields>
                <ListTemplate Context="u">
                    <div class="flex items-center gap-3 py-3 px-1">
                        <Avatar Size="AvatarSize.Small">
                            <AvatarFallback style="@($"background-color:{u.Color};color:#fff")">@u.Initials</AvatarFallback>
                        </Avatar>
                        <div class="flex-1 min-w-0">
                            <p class="text-sm font-medium truncate">@u.Name</p>
                            <p class="text-xs text-muted-foreground">@u.Role</p>
                        </div>
                        <Badge Variant="@StatusVariant(u.Status)" Class="text-xs">@u.Status</Badge>
                    </div>
                </ListTemplate>
            </DataView>

            @code {
                private record UserRow(int Id, string Name, string Role, string Status, string Initials, string Color);

                private static BadgeVariant StatusVariant(string s) => s switch
                {
                    "Active"  => BadgeVariant.Default,
                    "Away"    => BadgeVariant.Secondary,
                    _         => BadgeVariant.Outline
                };
            }
            """;

        public const string Switcher =
            """
            <DataView Items="@_products" PageSize="6" ItemKey="@(p => p.Id)">
                <Fields>
                    <DataViewColumn TItem="Product" Header="Name"     Property="@(p => p.Name)"     Filterable="true" Sortable="true" />
                    <DataViewColumn TItem="Product" Header="Category" Property="@(p => p.Category)" Filterable="true" />
                </Fields>
                <ListTemplate Context="p">...</ListTemplate>
                <GridTemplate Context="p">...</GridTemplate>
            </DataView>
            """;

        public const string SingleSelection =
            """
            <DataView Items="@_products" ItemKey="@(p => p.Id)"
                      SelectionMode="DataViewSelectionMode.Single"
                      @bind-SelectedItem="_selected">
                <Fields>
                    <DataViewColumn TItem="Product" Header="Name"     Property="@(p => p.Name)"     Filterable="true" Sortable="true" />
                    <DataViewColumn TItem="Product" Header="Category" Property="@(p => p.Category)" Filterable="true" />
                </Fields>
                <ListTemplate Context="p">...</ListTemplate>
                <GridTemplate Context="p">...</GridTemplate>
            </DataView>
            """;

        public const string MultipleSelection =
            """
            <DataView Items="@_products" ItemKey="@(p => p.Id)"
                      SelectionMode="DataViewSelectionMode.Multiple"
                      @bind-SelectedItems="_selected">
                <Fields>
                    <DataViewColumn TItem="Product" Header="Name"     Property="@(p => p.Name)"     Filterable="true" Sortable="true" />
                    <DataViewColumn TItem="Product" Header="Category" Property="@(p => p.Category)" Filterable="true" />
                </Fields>
                <ToolbarActions>
                    @if (_selected?.Count > 0)
                    {
                        <div class="flex items-center gap-2">
                            <Badge>@_selected.Count selected</Badge>
                            <Button Size="ButtonSize.Small" OnClick="@(() => _selected = null)">Clear</Button>
                        </div>
                    }
                </ToolbarActions>
                <ListTemplate Context="p">...</ListTemplate>
                <GridTemplate Context="p">...</GridTemplate>
            </DataView>

            @code {
                private IReadOnlyList<Product>? _selected;
            }
            """;

        public const string CheckVariant =
            """
            @* Plain checkmark, invisible when unselected (mobile-style) *@
            <DataView Items="@_items" SelectionMode="DataViewSelectionMode.Single"
                      CheckVariant="DataViewCheckVariant.Check">
                <ListTemplate Context="p">...</ListTemplate>
                <GridTemplate Context="p">...</GridTemplate>
            </DataView>

            @* No indicator — drive feedback entirely through your templates *@
            <DataView Items="@_items" SelectionMode="DataViewSelectionMode.Single"
                      CheckVariant="DataViewCheckVariant.None"
                      @bind-SelectedItem="_selected">
                <ListTemplate Context="p">
                    <div class="@($"py-2 {(p == _selected ? "text-primary font-semibold" : "")}")">
                        @p.Name
                    </div>
                </ListTemplate>
                <GridTemplate Context="p">
                    <Card Class="@($"h-full {(p == _selected ? "ring-2 ring-primary" : "")}")">
                        ...
                    </Card>
                </GridTemplate>
            </DataView>
            """;

        public const string Grouping =
            """
            <DataView Items="@_products" ItemKey="@(p => p.Id)"
                      GroupBy="@(p => p.Category)"
                      Layout="DataViewLayout.List">
                <Fields>
                    <DataViewColumn TItem="Product" Header="Name" Property="@(p => p.Name)" Filterable="true" Sortable="true" />
                </Fields>
                <GroupHeaderTemplate Context="key">
                    <span class="flex items-center gap-1.5">
                        <LucideIcon Name="folder" Size="13" />
                        @key
                    </span>
                </GroupHeaderTemplate>
                <ListTemplate Context="p">...</ListTemplate>
                <GridTemplate Context="p">...</GridTemplate>
            </DataView>
            """;

        public const string SearchSort =
            """
            <DataView Items="@_products" PageSize="10">
                <Fields>
                    <DataViewColumn TItem="Product" Header="Name"     Property="@(p => p.Name)"     Filterable="true" Sortable="true" />
                    <DataViewColumn TItem="Product" Header="Category" Property="@(p => p.Category)" Filterable="true" Sortable="true" />
                    <DataViewColumn TItem="Product" Header="Price"    Property="@(p => p.Price)"    Sortable="true" />
                </Fields>
                <ListTemplate Context="p">...</ListTemplate>
                <GridTemplate Context="p">...</GridTemplate>
            </DataView>
            """;

        public const string ToolbarActions =
            """
            @* ToolbarActions renders on the right — after search/sort, before the layout toggle *@
            <DataView Items="@_products" PageSize="6">
                <Fields>
                    <DataViewColumn TItem="Product" Header="Name" Property="@(p => p.Name)" Filterable="true" />
                </Fields>
                <ToolbarActions>
                    <Button Variant="ButtonVariant.Outline" Size="ButtonSize.Small" Class="h-8 gap-1.5">
                        <LucideIcon Name="download" Size="14" />
                        Export
                    </Button>
                    <Button Variant="ButtonVariant.Default" Size="ButtonSize.Small" Class="h-8 gap-1.5">
                        <LucideIcon Name="plus" Size="14" />
                        Add
                    </Button>
                </ToolbarActions>
                <ListTemplate Context="p">...</ListTemplate>
                <GridTemplate Context="p">...</GridTemplate>
            </DataView>
            """;

        public const string HeaderPagination =
            """
            <DataView Items="@_products" PageSize="4" PageSizes="@(new[]{ 4, 8, 12 })" ItemKey="@(p => p.Id)">
                <Fields>
                    <DataViewColumn TItem="Product" Header="Name" Property="@(p => p.Name)" Filterable="true" Sortable="true" />
                </Fields>
                <Header>
                    <p class="text-sm font-semibold pb-2">Products (@_products.Length total)</p>
                </Header>
                <ListTemplate Context="p">
                    <div class="flex items-center gap-3 py-2.5 px-1">
                        <span class="text-sm flex-1">@p.Name</span>
                        <span class="text-sm font-medium">$@p.Price.ToString("F2")</span>
                    </div>
                </ListTemplate>
            </DataView>
            """;

        public const string Loading =
            """
            <DataView Items="@_empty" Loading="true" LoadingText="Fetching products…" />
            """;

        public const string Empty =
            """
            <DataView Items="@_empty">
                <EmptyTemplate>
                    <LucideIcon Name="inbox" Size="36" Class="text-muted-foreground mb-2" />
                    <p class="text-sm font-medium">No products found</p>
                    <p class="text-xs text-muted-foreground">Try adjusting your filters.</p>
                </EmptyTemplate>
            </DataView>
            """;

        public const string ServerSide =
            """
            <DataView TItem="ProductRow" ItemsProvider="LoadProducts" ItemHeight="64" Height="480px">
                <ListTemplate Context="p">
                    <div class="flex items-center gap-3 py-2.5 px-1">
                        <LucideIcon Name="package" Size="16" Class="text-muted-foreground" />
                        <span class="text-sm flex-1">@p.Name</span>
                        <Badge>@p.Category</Badge>
                        <span class="text-sm font-medium">$@p.Price.ToString("F2")</span>
                    </div>
                </ListTemplate>
            </DataView>

            @code {
                private record ProductRow(int Id, string Name, string Category, decimal Price);

                private static readonly ProductRow[] _serverProducts = Enumerable.Range(1, 500)
                    .Select(i => new ProductRow(i, $"Product {i}",
                        i % 3 == 0 ? "Electronics" : i % 3 == 1 ? "Clothing" : "Home",
                        Math.Round((decimal)(new Random(i).NextDouble() * 200 + 5), 2)))
                    .ToArray();

                private async ValueTask<ItemsProviderResult<ProductRow>> LoadProducts(ItemsProviderRequest req)
                {
                    await Task.Delay(150); // simulate network latency
                    var items = _serverProducts
                        .Skip(req.StartIndex)
                        .Take(req.Count)
                        .ToList();
                    return new ItemsProviderResult<ProductRow>(items, _serverProducts.Length);
                }
            }
            """;
        public const string InfiniteColumns =
            """
            @* GridColumnMinWidth accepts any CSS length or a Tailwind spacing key *@
            @* "160px", "10rem", "40" (= 10 rem via Tailwind spacing) all work *@
            <DataView Items="@_products" PageSize="12" ItemKey="@(p => p.Id)"
                      Layout="DataViewLayout.Grid"
                      GridColumnMinWidth="160px">
                <Fields>
                    <DataViewColumn TItem="ProductRow" Header="Name" Property="@(p => p.Name)" Filterable="true" Sortable="true" />
                </Fields>
                <GridTemplate Context="p">
                    <Card Class="h-full">
                        <CardContent Class="pt-5">
                            <div class="flex flex-col gap-2 h-full">
                                <div class="h-8 w-8 rounded-md bg-muted flex items-center justify-center">
                                    <LucideIcon Name="@p.Icon" Size="16" Class="text-muted-foreground" />
                                </div>
                                <p class="font-medium text-sm truncate">@p.Name</p>
                                <p class="text-xs text-muted-foreground flex-1">@p.Category</p>
                                <span class="text-sm font-bold">$@p.Price.ToString("F2")</span>
                            </div>
                        </CardContent>
                    </Card>
                </GridTemplate>
            </DataView>
            """;

        public const string VirtualizeClient =
            """
            @* All 500 items in memory — only visible DOM nodes are rendered *@
            <DataView Items="@_bigList" Virtualize="true" ItemHeight="64" Height="480px">
                <Fields>
                    <DataViewColumn TItem="ProductRow" Header="Name" Property="@(p => p.Name)" Filterable Sortable />
                </Fields>
                <ListTemplate Context="p">
                    <div class="flex items-center gap-3 py-2.5 px-2">
                        <LucideIcon Name="package" Size="16" Class="text-muted-foreground" />
                        <span class="text-sm flex-1">@p.Name</span>
                        <Badge>@p.Category</Badge>
                        <span class="text-sm font-medium">$@p.Price.ToString("F2")</span>
                    </div>
                </ListTemplate>
            </DataView>
            """;
    }
}
