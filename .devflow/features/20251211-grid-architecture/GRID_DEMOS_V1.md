# Grid Demo Page Structure - Version 1

**Feature:** 20251211-grid-architecture  
**Status:** Planning  
**Created:** 2025-12-23

## Overview

This document defines the comprehensive demo page structure for the Grid component, featuring a hub page with tab-based navigation and 20 carefully designed examples that progressively demonstrate grid capabilities.

## Main Hub Page

**Route:** `/components/grid`

### Structure

The Grid demo page uses a tab-based navigation pattern to organize examples by feature category:

```razor
<Tabs DefaultValue="basic">
    <TabsList>
        <TabsTrigger Value="basic">Basic</TabsTrigger>
        <TabsTrigger Value="templating">Templating</TabsTrigger>
        <TabsTrigger Value="selection">Selection</TabsTrigger>
        <TabsTrigger Value="sorting-filtering">Sorting & Filtering</TabsTrigger>
        <TabsTrigger Value="state">State</TabsTrigger>
        <TabsTrigger Value="advanced">Advanced</TabsTrigger>
    </TabsList>
    
    <TabsContent Value="basic">
        @* Basic demos *@
    </TabsContent>
    @* Other tabs... *@
</Tabs>
```

## Demo Categories

### Tab 1: Basic (5 demos)

#### 1. grid-default
**Name:** Simple Grid  
**Description:** Basic 3-column grid with sortable columns displaying order data.  
**Key Features:**
- Minimal configuration
- Default column definitions
- Sortable columns (click header to sort)
- 100 rows of sample data

**shadcn/ui Reference:** Matches [Table](https://ui.shadcn.com/docs/components/table) simplicity

**Code Example:**
```razor
<Grid Items="@orders">
    <Columns>
        <GridColumn Field="Id" Header="Order ID" Sortable="true" Width="100px" />
        <GridColumn Field="Customer" Header="Customer" Sortable="true" />
        <GridColumn Field="Amount" Header="Amount" Sortable="true" Width="120px" />
    </Columns>
</Grid>
```

---

#### 2. grid-pagination
**Name:** Paginated Grid  
**Description:** Client-side pagination with page size selector and navigation controls.  
**Key Features:**
- Client-side pagination (`PagingMode.Client`)
- Configurable page size (10, 25, 50, 100)
- Page navigation controls
- Row count display (e.g., "Showing 1-25 of 1000")
- 1000 rows of sample data

**shadcn/ui Reference:** Similar to [Data Table Pagination](https://ui.shadcn.com/docs/components/data-table#pagination)

**Code Example:**
```razor
<Grid Items="@orders" PagingMode="GridPagingMode.Client" PageSize="25">
    <Columns>
        <GridColumn Field="Id" Header="Order ID" Sortable="true" />
        <GridColumn Field="Customer" Header="Customer" Sortable="true" />
        <GridColumn Field="Status" Header="Status" Sortable="true" />
        <GridColumn Field="Amount" Header="Amount" Sortable="true" />
        <GridColumn Field="OrderDate" Header="Date" Sortable="true" />
    </Columns>
</Grid>
```

---

#### 3. grid-height
**Name:** Fixed Height Grid  
**Description:** Grid with fixed 500px height and vertical scrolling, demonstrating row virtualization.  
**Key Features:**
- Fixed height container (500px)
- Vertical scrollbar
- Row virtualization for performance
- Smooth scrolling with 1000+ rows

**shadcn/ui Reference:** N/A (AG Grid specific feature)

**Code Example:**
```razor
<div style="height: 500px;">
    <Grid Items="@orders" VirtualizationMode="GridVirtualizationMode.RowOnly">
        <Columns>
            <GridColumn Field="Id" Header="Order ID" Sortable="true" />
            <GridColumn Field="Customer" Header="Customer" Sortable="true" />
            <GridColumn Field="Status" Header="Status" Sortable="true" />
            <GridColumn Field="Amount" Header="Amount" Sortable="true" />
            <GridColumn Field="OrderDate" Header="Date" Sortable="true" />
            <GridColumn Field="ShipTo" Header="Ship To" Sortable="true" />
        </Columns>
    </Grid>
</div>
```

---

#### 4. grid-auto-height
**Name:** Auto-Height Grid  
**Description:** Grid that automatically adjusts height to fit all rows without scrolling.  
**Key Features:**
- Auto-height layout (no virtualization)
- No scrollbar
- Best for small datasets (<100 rows)
- 50 rows of sample data

**shadcn/ui Reference:** Matches [Table](https://ui.shadcn.com/docs/components/table) default behavior

**Code Example:**
```razor
<Grid Items="@orders.Take(50)" VirtualizationMode="GridVirtualizationMode.None">
    <Columns>
        <GridColumn Field="Id" Header="Order ID" Sortable="true" />
        <GridColumn Field="Customer" Header="Customer" Sortable="true" />
        <GridColumn Field="Status" Header="Status" Sortable="true" />
        <GridColumn Field="Amount" Header="Amount" Sortable="true" />
    </Columns>
</Grid>
```

---

#### 5. grid-pinned-columns
**Name:** Pinned Columns  
**Description:** Grid with left-pinned ID column and right-pinned Actions column.  
**Key Features:**
- Left pinned column (Order ID)
- Right pinned column (Actions)
- Horizontal scrolling for middle columns
- 6+ columns to demonstrate scrolling

**shadcn/ui Reference:** N/A (AG Grid specific feature)

**Code Example:**
```razor
<Grid Items="@orders">
    <Columns>
        <GridColumn Field="Id" Header="Order ID" Pinned="GridColumnPinPosition.Left" Width="100px" />
        <GridColumn Field="Customer" Header="Customer" Width="200px" />
        <GridColumn Field="Status" Header="Status" Width="150px" />
        <GridColumn Field="Amount" Header="Amount" Width="120px" />
        <GridColumn Field="OrderDate" Header="Order Date" Width="150px" />
        <GridColumn Field="ShipTo" Header="Ship To" Width="200px" />
        <GridColumn Header="Actions" Pinned="GridColumnPinPosition.Right" Width="100px">
            <CellTemplate>
                <Button Size="ButtonSize.Sm" Variant="ButtonVariant.Ghost">Edit</Button>
            </CellTemplate>
        </GridColumn>
    </Columns>
</Grid>
```

---

### Tab 2: Templating (4 demos)

#### 6. grid-cell-template
**Name:** Custom Cell Templates  
**Description:** Custom cell rendering using Badge for status and formatted currency for amounts.  
**Key Features:**
- CellTemplate with RenderFragment<TItem>
- Badge component integration for status
- Custom formatting (currency, dates)
- Access to full row data in template context

**shadcn/ui Reference:** Similar to [Data Table Row Actions](https://ui.shadcn.com/docs/components/data-table#row-actions)

**Code Example:**
```razor
<Grid Items="@orders">
    <Columns>
        <GridColumn Field="Id" Header="Order ID" />
        <GridColumn Field="Customer" Header="Customer" />
        <GridColumn Field="Status" Header="Status">
            <CellTemplate Context="order">
                <Badge Variant="@GetStatusVariant(order.Status)">
                    @order.Status
                </Badge>
            </CellTemplate>
        </GridColumn>
        <GridColumn Field="Amount" Header="Amount">
            <CellTemplate Context="order">
                <span class="font-mono">@order.Amount.ToString("C")</span>
            </CellTemplate>
        </GridColumn>
    </Columns>
</Grid>

@code {
    private BadgeVariant GetStatusVariant(OrderStatus status) => status switch
    {
        OrderStatus.Delivered => BadgeVariant.Default,
        OrderStatus.Shipped => BadgeVariant.Secondary,
        OrderStatus.Processing => BadgeVariant.Outline,
        OrderStatus.Pending => BadgeVariant.Outline,
        OrderStatus.Cancelled => BadgeVariant.Destructive,
        _ => BadgeVariant.Default
    };
}
```

---

#### 7. grid-header-template
**Name:** Custom Header Templates  
**Description:** Custom header rendering with icons and tooltips.  
**Key Features:**
- HeaderTemplate with RenderFragment
- Icon integration (Lucide icons)
- Tooltip integration for column descriptions
- Custom header styling

**shadcn/ui Reference:** N/A (Custom extension)

**Code Example:**
```razor
<Grid Items="@orders">
    <Columns>
        <GridColumn Field="Id" Header="Order ID" />
        <GridColumn Field="Customer">
            <HeaderTemplate>
                <div class="flex items-center gap-2">
                    <Icon Name="user" Size="16" />
                    <span>Customer</span>
                </div>
            </HeaderTemplate>
        </GridColumn>
        <GridColumn Field="Amount">
            <HeaderTemplate>
                <div class="flex items-center gap-2">
                    <Icon Name="dollar-sign" Size="16" />
                    <span>Amount</span>
                    <TooltipProvider>
                        <Tooltip>
                            <TooltipTrigger>
                                <Icon Name="info" Size="14" Class="text-muted-foreground" />
                            </TooltipTrigger>
                            <TooltipContent>Total order amount in USD</TooltipContent>
                        </Tooltip>
                    </TooltipProvider>
                </div>
            </HeaderTemplate>
        </GridColumn>
    </Columns>
</Grid>
```

---

#### 8. grid-row-actions
**Name:** Row Actions  
**Description:** Action column with DropdownMenu for Edit, Duplicate, Delete operations.  
**Key Features:**
- Action column (no field binding)
- DropdownMenu component integration
- Event handlers for row-specific actions
- Icon buttons

**shadcn/ui Reference:** Exact match to [Data Table Row Actions](https://ui.shadcn.com/docs/components/data-table#row-actions)

**Code Example:**
```razor
<Grid Items="@orders">
    <Columns>
        <GridColumn Field="Id" Header="Order ID" />
        <GridColumn Field="Customer" Header="Customer" />
        <GridColumn Field="Status" Header="Status" />
        <GridColumn Field="Amount" Header="Amount" />
        <GridColumn Header="Actions" Width="70px">
            <CellTemplate Context="order">
                <DropdownMenu>
                    <DropdownMenuTrigger AsChild>
                        <Button Variant="ButtonVariant.Ghost" Size="ButtonSize.Icon">
                            <Icon Name="more-horizontal" Size="16" />
                        </Button>
                    </DropdownMenuTrigger>
                    <DropdownMenuContent Align="DropdownMenuAlign.End">
                        <DropdownMenuItem OnClick="@(() => EditOrder(order))">
                            <Icon Name="edit" Size="16" Class="mr-2" />
                            Edit
                        </DropdownMenuItem>
                        <DropdownMenuItem OnClick="@(() => DuplicateOrder(order))">
                            <Icon Name="copy" Size="16" Class="mr-2" />
                            Duplicate
                        </DropdownMenuItem>
                        <DropdownMenuSeparator />
                        <DropdownMenuItem OnClick="@(() => DeleteOrder(order))" Class="text-destructive">
                            <Icon Name="trash" Size="16" Class="mr-2" />
                            Delete
                        </DropdownMenuItem>
                    </DropdownMenuContent>
                </DropdownMenu>
            </CellTemplate>
        </GridColumn>
    </Columns>
</Grid>
```

---

#### 9. grid-conditional-styling
**Name:** Conditional Styling  
**Description:** Row and cell CSS classes applied based on data values.  
**Key Features:**
- Dynamic CSS classes based on row data
- Highlight rows (e.g., high-value orders)
- Color-coded cells (e.g., negative amounts in red)
- CellClass and custom logic

**shadcn/ui Reference:** N/A (Custom extension)

**Code Example:**
```razor
<Grid Items="@orders">
    <Columns>
        <GridColumn Field="Id" Header="Order ID" />
        <GridColumn Field="Customer" Header="Customer" />
        <GridColumn Field="Amount" Header="Amount">
            <CellTemplate Context="order">
                <span class="@GetAmountClass(order.Amount)">
                    @order.Amount.ToString("C")
                </span>
            </CellTemplate>
        </GridColumn>
        <GridColumn Field="Status" Header="Status" CellClass="@GetStatusCellClass" />
    </Columns>
</Grid>

@code {
    private string GetAmountClass(decimal amount)
    {
        return amount > 5000 ? "font-bold text-green-600 dark:text-green-400" : "";
    }
    
    private string GetStatusCellClass(Order order)
    {
        return order.Status == OrderStatus.Cancelled ? "bg-destructive/10" : "";
    }
}
```

---

### Tab 3: Selection (3 demos)

#### 10. grid-selection-single
**Name:** Single Row Selection  
**Description:** Radio-style single row selection with visual feedback.  
**Key Features:**
- SelectionMode.Single
- Click row to select (one at a time)
- Visual highlight for selected row
- OnSelectionChanged callback

**shadcn/ui Reference:** Similar to [Data Table Selection](https://ui.shadcn.com/docs/components/data-table#row-selection)

**Code Example:**
```razor
<Grid Items="@orders" SelectionMode="GridSelectionMode.Single" OnSelectionChanged="HandleSelectionChanged">
    <Columns>
        <GridColumn Field="Id" Header="Order ID" />
        <GridColumn Field="Customer" Header="Customer" />
        <GridColumn Field="Status" Header="Status" />
        <GridColumn Field="Amount" Header="Amount" />
    </Columns>
</Grid>

@if (selectedOrder != null)
{
    <div class="mt-4 p-4 border rounded">
        <h3 class="font-semibold">Selected Order</h3>
        <p>ID: @selectedOrder.Id - @selectedOrder.Customer - @selectedOrder.Amount.ToString("C")</p>
    </div>
}

@code {
    private Order? selectedOrder;
    
    private void HandleSelectionChanged(IReadOnlyCollection<Order> selection)
    {
        selectedOrder = selection.FirstOrDefault();
    }
}
```

---

#### 11. grid-selection-multiple
**Name:** Multiple Row Selection  
**Description:** Checkbox-based multiple row selection with select all.  
**Key Features:**
- SelectionMode.Multiple
- Checkboxes in first column
- Select all checkbox in header
- Display selected count

**shadcn/ui Reference:** Exact match to [Data Table Selection](https://ui.shadcn.com/docs/components/data-table#row-selection)

**Code Example:**
```razor
<Grid Items="@orders" SelectionMode="GridSelectionMode.Multiple" OnSelectionChanged="HandleSelectionChanged">
    <Columns>
        <GridColumn Field="Id" Header="Order ID" />
        <GridColumn Field="Customer" Header="Customer" />
        <GridColumn Field="Status" Header="Status" />
        <GridColumn Field="Amount" Header="Amount" />
    </Columns>
</Grid>

<div class="flex items-center gap-2 mt-4">
    <span class="text-sm text-muted-foreground">
        @selectedOrders.Count of @orders.Count row(s) selected
    </span>
    @if (selectedOrders.Count > 0)
    {
        <Button Size="ButtonSize.Sm" Variant="ButtonVariant.Destructive" OnClick="DeleteSelected">
            <Icon Name="trash" Size="16" Class="mr-2" />
            Delete Selected
        </Button>
    }
</div>

@code {
    private IReadOnlyCollection<Order> selectedOrders = Array.Empty<Order>();
    
    private void HandleSelectionChanged(IReadOnlyCollection<Order> selection)
    {
        selectedOrders = selection;
    }
}
```

---

#### 12. grid-selection-controlled
**Name:** Controlled Selection State  
**Description:** Programmatic control of selection with @bind-SelectedItems.  
**Key Features:**
- @bind-SelectedItems two-way binding
- External selection control (buttons to select/clear)
- Pre-selected rows on load
- Sync selection state with external UI

**shadcn/ui Reference:** N/A (Blazor binding pattern)

**Code Example:**
```razor
<div class="flex gap-2 mb-4">
    <Button Size="ButtonSize.Sm" OnClick="SelectHighValueOrders">Select High Value Orders</Button>
    <Button Size="ButtonSize.Sm" Variant="ButtonVariant.Outline" OnClick="ClearSelection">Clear Selection</Button>
</div>

<Grid Items="@orders" 
      SelectionMode="GridSelectionMode.Multiple" 
      @bind-SelectedItems="selectedOrders">
    <Columns>
        <GridColumn Field="Id" Header="Order ID" />
        <GridColumn Field="Customer" Header="Customer" />
        <GridColumn Field="Amount" Header="Amount" />
        <GridColumn Field="Status" Header="Status" />
    </Columns>
</Grid>

@code {
    private IReadOnlyCollection<Order> selectedOrders = Array.Empty<Order>();
    
    private void SelectHighValueOrders()
    {
        selectedOrders = orders.Where(o => o.Amount > 5000).ToList();
    }
    
    private void ClearSelection()
    {
        selectedOrders = Array.Empty<Order>();
    }
}
```

---

### Tab 4: Sorting & Filtering (4 demos)

#### 13. grid-sorting-default
**Name:** Column Sorting  
**Description:** Click column headers to sort ascending/descending/none.  
**Key Features:**
- Single column sorting
- Click header to toggle sort direction
- Sort indicator icons (up/down arrows)
- Multi-column sorting (shift+click)

**shadcn/ui Reference:** Similar to [Data Table Sorting](https://ui.shadcn.com/docs/components/data-table#sorting)

**Code Example:**
```razor
<Grid Items="@orders">
    <Columns>
        <GridColumn Field="Id" Header="Order ID" Sortable="true" />
        <GridColumn Field="Customer" Header="Customer" Sortable="true" />
        <GridColumn Field="Status" Header="Status" Sortable="true" />
        <GridColumn Field="Amount" Header="Amount" Sortable="true" />
        <GridColumn Field="OrderDate" Header="Order Date" Sortable="true" />
    </Columns>
</Grid>
```

---

#### 14. grid-sorting-controlled
**Name:** Controlled Sorting State  
**Description:** External sort controls with @bind-SortModel state management.  
**Key Features:**
- @bind-SortModel two-way binding
- External sort controls (dropdown or buttons)
- Programmatic sort on load
- Display current sort state

**shadcn/ui Reference:** N/A (Advanced pattern)

**Code Example:**
```razor
<div class="flex gap-2 mb-4">
    <Button Size="ButtonSize.Sm" OnClick="() => SortBy(nameof(Order.Amount), GridSortDirection.Descending)">
        Sort by Amount (High to Low)
    </Button>
    <Button Size="ButtonSize.Sm" OnClick="() => SortBy(nameof(Order.OrderDate), GridSortDirection.Descending)">
        Sort by Date (Newest First)
    </Button>
    <Button Size="ButtonSize.Sm" Variant="ButtonVariant.Outline" OnClick="ClearSort">Clear Sort</Button>
</div>

<Grid Items="@orders" InitialState="@gridState" OnStateChanged="HandleStateChanged">
    <Columns>
        <GridColumn Field="Id" Header="Order ID" Sortable="true" />
        <GridColumn Field="Customer" Header="Customer" Sortable="true" />
        <GridColumn Field="Amount" Header="Amount" Sortable="true" />
        <GridColumn Field="OrderDate" Header="Order Date" Sortable="true" />
    </Columns>
</Grid>

@code {
    private GridState gridState = new();
    
    private void SortBy(string field, GridSortDirection direction)
    {
        gridState.SortDescriptors.Clear();
        gridState.SortDescriptors.Add(new GridSortDescriptor
        {
            Field = field,
            Direction = direction,
            Order = 0
        });
    }
    
    private void ClearSort()
    {
        gridState.SortDescriptors.Clear();
    }
    
    private void HandleStateChanged(GridState newState)
    {
        gridState = newState;
    }
}
```

---

#### 15. grid-filtering-text
**Name:** Column Filtering  
**Description:** Text filter inputs in column headers for client-side filtering.  
**Key Features:**
- Text filter per column
- Filter icon in header
- Contains operator (default)
- Clear filter button

**shadcn/ui Reference:** Similar to [Data Table Filtering](https://ui.shadcn.com/docs/components/data-table#filtering)

**Code Example:**
```razor
<Grid Items="@orders">
    <Columns>
        <GridColumn Field="Id" Header="Order ID" Sortable="true" Filterable="true" />
        <GridColumn Field="Customer" Header="Customer" Sortable="true" Filterable="true" />
        <GridColumn Field="Status" Header="Status" Sortable="true" Filterable="true" />
        <GridColumn Field="Amount" Header="Amount" Sortable="true" Filterable="true" />
    </Columns>
</Grid>
```

---

#### 16. grid-filtering-controlled
**Name:** Controlled Filtering State  
**Description:** External filter controls with @bind-FilterModel state management.  
**Key Features:**
- @bind-FilterModel two-way binding
- Custom filter UI (select, date range, etc.)
- Programmatic filter on load
- Display active filters count

**shadcn/ui Reference:** N/A (Advanced pattern)

**Code Example:**
```razor
<div class="flex gap-2 mb-4">
    <Select @bind-Value="statusFilter">
        <SelectTrigger Class="w-[180px]">
            <SelectValue Placeholder="Filter by Status" />
        </SelectTrigger>
        <SelectContent>
            <SelectItem Value="all">All Statuses</SelectItem>
            <SelectItem Value="pending">Pending</SelectItem>
            <SelectItem Value="processing">Processing</SelectItem>
            <SelectItem Value="shipped">Shipped</SelectItem>
            <SelectItem Value="delivered">Delivered</SelectItem>
        </SelectContent>
    </Select>
    <Button Size="ButtonSize.Sm" Variant="ButtonVariant.Outline" OnClick="ClearFilters">Clear Filters</Button>
</div>

<Grid Items="@orders" InitialState="@gridState" OnStateChanged="HandleStateChanged">
    <Columns>
        <GridColumn Field="Id" Header="Order ID" />
        <GridColumn Field="Customer" Header="Customer" />
        <GridColumn Field="Status" Header="Status" />
        <GridColumn Field="Amount" Header="Amount" />
    </Columns>
</Grid>

@code {
    private GridState gridState = new();
    private string statusFilter = "all";
    
    protected override void OnParametersSet()
    {
        ApplyStatusFilter();
    }
    
    private void ApplyStatusFilter()
    {
        gridState.FilterDescriptors.Clear();
        if (statusFilter != "all")
        {
            gridState.FilterDescriptors.Add(new GridFilterDescriptor
            {
                Field = nameof(Order.Status),
                Operator = GridFilterOperator.Equals,
                Value = statusFilter
            });
        }
    }
}
```

---

### Tab 5: State (2 demos)

#### 17. grid-state-save-restore
**Name:** State Persistence  
**Description:** Save grid state (sort, filter, column order/width) to localStorage and restore on page load.  
**Key Features:**
- localStorage persistence
- Save button to persist state
- Auto-restore on page load
- Reset to defaults button
- Persists: sort, filter, page size, column visibility/width/order

**shadcn/ui Reference:** N/A (Advanced pattern)

**Code Example:**
```razor
<div class="flex gap-2 mb-4">
    <Button Size="ButtonSize.Sm" OnClick="SaveState">Save State</Button>
    <Button Size="ButtonSize.Sm" Variant="ButtonVariant.Outline" OnClick="ResetState">Reset to Default</Button>
    @if (stateSaved)
    {
        <span class="text-sm text-muted-foreground self-center">State saved!</span>
    }
</div>

<Grid Items="@orders" InitialState="@gridState" OnStateChanged="HandleStateChanged">
    <Columns>
        <GridColumn Field="Id" Header="Order ID" Sortable="true" Filterable="true" />
        <GridColumn Field="Customer" Header="Customer" Sortable="true" Filterable="true" />
        <GridColumn Field="Status" Header="Status" Sortable="true" Filterable="true" />
        <GridColumn Field="Amount" Header="Amount" Sortable="true" Filterable="true" />
        <GridColumn Field="OrderDate" Header="Order Date" Sortable="true" Filterable="true" />
    </Columns>
</Grid>

@code {
    [Inject] private IJSRuntime JS { get; set; } = null!;
    private GridState gridState = new();
    private bool stateSaved = false;
    
    protected override async Task OnInitializedAsync()
    {
        await LoadState();
    }
    
    private async Task LoadState()
    {
        var json = await JS.InvokeAsync<string>("localStorage.getItem", "gridState");
        if (!string.IsNullOrEmpty(json))
        {
            gridState = JsonSerializer.Deserialize<GridState>(json) ?? new GridState();
        }
    }
    
    private async Task SaveState()
    {
        var json = JsonSerializer.Serialize(gridState);
        await JS.InvokeVoidAsync("localStorage.setItem", "gridState", json);
        stateSaved = true;
        StateHasChanged();
        await Task.Delay(2000);
        stateSaved = false;
        StateHasChanged();
    }
    
    private async Task ResetState()
    {
        gridState = new GridState();
        await JS.InvokeVoidAsync("localStorage.removeItem", "gridState");
    }
    
    private void HandleStateChanged(GridState newState)
    {
        gridState = newState;
    }
}
```

---

#### 18. grid-state-serialization
**Name:** State Export/Import  
**Description:** Export grid state as JSON file and import from uploaded JSON.  
**Key Features:**
- Export state to JSON file download
- Import state from JSON file upload
- State validation on import
- Display current state JSON (formatted)

**shadcn/ui Reference:** N/A (Advanced pattern)

**Code Example:**
```razor
<div class="flex gap-2 mb-4">
    <Button Size="ButtonSize.Sm" OnClick="ExportState">
        <Icon Name="download" Size="16" Class="mr-2" />
        Export State
    </Button>
    <InputFile OnChange="ImportState" accept=".json" class="hidden" @ref="fileInput" />
    <Button Size="ButtonSize.Sm" Variant="ButtonVariant.Outline" OnClick="() => fileInput?.Element?.Value?.Click()">
        <Icon Name="upload" Size="16" Class="mr-2" />
        Import State
    </Button>
</div>

<Grid Items="@orders" InitialState="@gridState" OnStateChanged="HandleStateChanged">
    <Columns>
        <GridColumn Field="Id" Header="Order ID" Sortable="true" Filterable="true" />
        <GridColumn Field="Customer" Header="Customer" Sortable="true" Filterable="true" />
        <GridColumn Field="Amount" Header="Amount" Sortable="true" Filterable="true" />
    </Columns>
</Grid>

<details class="mt-4">
    <summary class="cursor-pointer text-sm font-medium">Current State JSON</summary>
    <pre class="mt-2 p-4 bg-muted rounded text-xs overflow-auto">@stateJson</pre>
</details>

@code {
    [Inject] private IJSRuntime JS { get; set; } = null!;
    private GridState gridState = new();
    private string stateJson = "{}";
    private InputFile? fileInput;
    
    private void HandleStateChanged(GridState newState)
    {
        gridState = newState;
        stateJson = JsonSerializer.Serialize(gridState, new JsonSerializerOptions { WriteIndented = true });
    }
    
    private async Task ExportState()
    {
        var json = JsonSerializer.Serialize(gridState, new JsonSerializerOptions { WriteIndented = true });
        var bytes = Encoding.UTF8.GetBytes(json);
        var base64 = Convert.ToBase64String(bytes);
        await JS.InvokeVoidAsync("downloadFile", "grid-state.json", base64);
    }
    
    private async Task ImportState(InputFileChangeEventArgs e)
    {
        var file = e.File;
        using var stream = file.OpenReadStream();
        using var reader = new StreamReader(stream);
        var json = await reader.ReadToEndAsync();
        
        try
        {
            gridState = JsonSerializer.Deserialize<GridState>(json) ?? new GridState();
            stateJson = json;
        }
        catch
        {
            // Show error toast
        }
    }
}
```

---

### Tab 6: Advanced (2 demos)

#### 19. grid-flex-columns
**Name:** Flexible Column Sizing  
**Description:** Responsive columns using flex sizing instead of fixed widths.  
**Key Features:**
- Flex-based column sizing
- Responsive layout (adapts to container width)
- MinWidth constraints
- Columns resize proportionally

**shadcn/ui Reference:** N/A (AG Grid specific feature)

**Code Example:**
```razor
<Grid Items="@orders">
    <Columns>
        <GridColumn Field="Id" Header="Order ID" Width="100px" />
        <GridColumn Field="Customer" Header="Customer" MinWidth="150px" /> @* Flex: auto *@
        <GridColumn Field="Status" Header="Status" Width="120px" />
        <GridColumn Field="Amount" Header="Amount" Width="120px" />
        <GridColumn Field="OrderDate" Header="Order Date" MinWidth="150px" /> @* Flex: auto *@
        <GridColumn Field="ShipTo" Header="Ship To" MinWidth="150px" /> @* Flex: auto *@
    </Columns>
</Grid>
```

---

#### 20. grid-theme-custom
**Name:** Custom Theme Variables  
**Description:** Override grid theme CSS variables for custom styling.  
**Key Features:**
- CSS variable overrides
- Custom colors (borders, backgrounds, headers)
- Custom density (row height, padding)
- Demonstrates theme customization without changing component code

**shadcn/ui Reference:** Similar to [Theming](https://ui.shadcn.com/docs/theming) approach

**Code Example:**
```razor
<div style="--grid-border: hsl(280 60% 50%); --grid-header-bg: hsl(280 60% 95%); --grid-row-hover: hsl(280 60% 98%);">
    <Grid Items="@orders" Theme="GridTheme.Bordered" Density="GridDensity.Compact">
        <Columns>
            <GridColumn Field="Id" Header="Order ID" Sortable="true" />
            <GridColumn Field="Customer" Header="Customer" Sortable="true" />
            <GridColumn Field="Status" Header="Status" Sortable="true" />
            <GridColumn Field="Amount" Header="Amount" Sortable="true" />
        </Columns>
    </Grid>
</div>

<style>
    /* Custom CSS variables for this demo */
    :root {
        --grid-border: hsl(var(--border));
        --grid-header-bg: hsl(var(--muted));
        --grid-row-hover: hsl(var(--muted) / 0.5);
        --grid-row-height-compact: 32px;
    }
</style>
```

---

## Demo Data Reference

All demos use the `Order` class and `GridDemoData.GenerateOrders()` method defined in `demo/BlazorUI.Demo/Data/GridDemoData.cs`:

```csharp
public class Order
{
    public int Id { get; set; }
    public string Customer { get; set; }
    public OrderStatus Status { get; set; }
    public decimal Amount { get; set; }
    public DateTime OrderDate { get; set; }
    public string ShipTo { get; set; }
}

public enum OrderStatus
{
    Pending, Processing, Shipped, Delivered, Cancelled
}
```

## Implementation Notes

1. **Progressive Disclosure:** Demos are ordered from simple to complex within each tab
2. **Consistent Data:** All demos use the same `Order` schema for familiarity
3. **Real-World Scenarios:** Demos simulate actual use cases (order management system)
4. **shadcn/ui Alignment:** Demos reference equivalent shadcn/ui patterns where applicable
5. **Code Quality:** All code examples are production-ready, not pseudocode

## Success Criteria

- [ ] All 20 demos implemented and functional
- [ ] Tab navigation works correctly
- [ ] Each demo has clear description and key features
- [ ] Code examples are syntactically correct
- [ ] Demos progressively increase in complexity
- [ ] shadcn/ui references are accurate
- [ ] Demo data schema is consistent across all examples
