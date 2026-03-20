# Grid Component

A powerful, enterprise-grade data grid component powered by AG DataGrid with full Blazor integration, template support, and shadcn/ui theming.

## Features

- ✅ **Powered by AG Grid Community** - Industry-leading grid library with excellent performance
- ✅ **Blazor Template Support** - Use Blazor `RenderFragment` for cells and headers
- ✅ **Cell Actions** - Auto-discovery of grid actions via `[DataGridAction]` attribute
- ✅ **Full Theme Integration** - Seamless shadcn/ui theme integration with CSS variables
- ✅ **Sorting & Filtering** - Built-in sorting and filtering capabilities
- ✅ **Pagination** - Client-side and server-side pagination support
- ✅ **Row Selection** - Single and multiple row selection modes
- ✅ **Column Features** - Pinning, resizing, reordering, and visibility control
- ✅ **Virtualization** - Efficient rendering of large datasets
- ✅ **Responsive** - Mobile-friendly with adaptive layouts

## Installation

The DataGrid component is included in the `NeoUI.Blazor` package and uses AG Grid Community Edition (loaded automatically from CDN).

## Basic Usage

```razor
<DataGrid Items="@orders" ActionHost="this">
    <Columns>
        <DataGridColumn Field="Id" Header="Order ID" Sortable="true" Width="100px" />
        <DataGridColumn Field="CustomerName" Header="Customer" Sortable="true" />
        <DataGridColumn Field="OrderDate" Header="Date" Sortable="true" />
        <DataGridColumn Field="Total" Header="Total" Sortable="true" />
    </Columns>
</DataGrid>

@code {
    private List<Order> orders = new();
}
```

## Column Configuration

### Basic Column Properties

```razor
<DataGridColumn 
    Id="custom-id"                    // Unique column identifier
    Field="PropertyName"              // Property to bind to
    Header="Display Name"             // Column header text
    Sortable="true"                   // Enable sorting
    Filterable="true"                 // Enable filtering
    Width="150px"                     // Fixed width
    MinWidth="100px"                  // Minimum width
    MaxWidth="300px"                  // Maximum width
    Pinned="DataDataGridColumnPinPosition.Left"  // Pin column
    AllowResize="true"                // Allow column resizing
    AllowReorder="true"               // Allow column reordering
    IsVisible="true"                  // Column visibility
    DataFormatString="C"              // Format string (e.g., "C", "N2", "P")
/>
```

### Data Formatting

Use `DataFormatString` for simple formatting without templates:

```razor
<!-- Currency formatting -->
<DataGridColumn Field="Price" Header="Price" DataFormatString="C" />
<!-- Output: $1,234.56 -->

<!-- Number with 2 decimals -->
<DataGridColumn Field="Quantity" Header="Qty" DataFormatString="N2" />
<!-- Output: 1,234.56 -->

<!-- Number with no decimals (integer) -->
<DataGridColumn Field="Count" Header="Count" DataFormatString="N0" />
<!-- Output: 1,234 -->

<!-- Percentage with 2 decimals -->
<DataGridColumn Field="Rate" Header="Rate" DataFormatString="P2" />
<!-- Output: 45.67% -->

<!-- Short date -->
<DataGridColumn Field="OrderDate" Header="Date" DataFormatString="d" />
<!-- Output: 12/31/2024 -->

<!-- Long date -->
<DataGridColumn Field="OrderDate" Header="Date" DataFormatString="D" />
<!-- Output: Tuesday, December 31, 2024 -->

<!-- Short time -->
<DataGridColumn Field="CreatedAt" Header="Time" DataFormatString="t" />
<!-- Output: 3:45 PM -->

<!-- Composite format strings also supported -->
<DataGridColumn Field="Price" Header="Price" DataFormatString="{0:C}" />
<DataGridColumn Field="Discount" Header="Discount" DataFormatString="{0:P1}" />
```

**How It Works:**
- Formatting happens in C# using .NET's `IFormattable.ToString(format, culture)`
- Formatted values are added as `{field}_formatted` properties to the data
- AG DataGrid displays the formatted value while preserving the original for sorting/filtering
- Supports **ALL** .NET format strings (standard and custom)
- Respects current culture settings

**Supported Format Strings:**
- **All standard .NET format strings**: `C`, `N`, `P`, `F`, `E`, `G`, `D`, `X`, etc.
- **Custom format strings**: `"0.00"`, `"#,##0"`, `"MM/dd/yyyy"`, etc.
- **Composite formats**: `"{0:C}"`, `"{0:N2}"`, etc.
- **Culture-aware**: Automatically uses `CultureInfo.CurrentCulture`

> **Note**: `DataFormatString` is only applied when `CellTemplate` is not specified. If you need custom formatting beyond standard format strings, use a `CellTemplate` instead.

> **Performance**: Formatting happens once when data is loaded, not on every render. The formatted strings are cached in the data object.

### Column Pinning

```razor
<DataGridColumn Field="Id" Header="ID" Pinned="DataDataGridColumnPinPosition.Left" />
<DataGridColumn Field="Name" Header="Name" />
<DataGridColumn Field="Actions" Header="Actions" Pinned="DataDataGridColumnPinPosition.Right" />
```

## Cell Templates

Use Blazor `RenderFragment` to create custom cell content:

```razor
<DataGridColumn Field="Status" Header="Status">
    <CellTemplate Context="order">
        <Badge Variant="@(order.Status == "Active" ? BadgeVariant.Default : BadgeVariant.Secondary)">
            @order.Status
        </Badge>
    </CellTemplate>
</DataGridColumn>

<DataGridColumn Field="Actions" Header="Actions">
    <CellTemplate Context="order">
        <div class="flex gap-2">
            <Button Variant="ButtonVariant.Ghost" Size="ButtonSize.Icon" 
                    data-action="Edit">
                <LucideIcon Name="pencil" Size="16" />
            </Button>
            <Button Variant="ButtonVariant.Ghost" Size="ButtonSize.Icon" 
                    data-action="Delete">
                <LucideIcon Name="trash-2" Size="16" />
            </Button>
        </div>
    </CellTemplate>
</DataGridColumn>
```

## Header Templates

Customize column headers with Blazor templates:

```razor
<DataGridColumn Field="Name" Header="Customer Name">
    <HeaderTemplate>
        <div class="flex items-center gap-2">
            <LucideIcon Name="user" Size="16" />
            <span>Customer</span>
        </div>
    </HeaderTemplate>
</DataGridColumn>
```

## Cell Actions

Use the `[DataGridAction]` attribute to automatically wire up actions from your component:

```razor
<DataGrid Items="@orders" ActionHost="this">
    <Columns>
        <DataGridColumn Field="Actions" Header="Actions">
            <CellTemplate Context="order">
                <Button data-action="Edit">Edit</Button>
                <Button data-action="Delete">Delete</Button>
            </CellTemplate>
        </DataGridColumn>
    </Columns>
</DataGrid>

@code {
    private List<Order> orders = new();

    [DataGridAction]
    private async Task Edit(Order order)
    {
        // Handle edit action
        await ShowEditDialog(order);
    }

    [DataGridAction(Name = "Delete")]
    private async Task HandleDelete(Order order)
    {
        // Handle delete action
        await DeleteOrder(order);
    }
}
```

### How Cell Actions Work

1. Add `data-action="ActionName"` to any element in your cell template
2. Mark a method in your component with `[DataGridAction]`
3. Set `ActionHost="this"` on the DataGrid component
4. Actions are auto-discovered and wired up automatically

The method signature must be:
- `void MethodName(TItem item)` for synchronous actions
- `Task MethodName(TItem item)` for async actions

## Selection

### Single Selection

```razor
<DataGrid Items="@orders" SelectionMode="DataGridSelectionMode.Single">
    <Columns>
        <!-- columns -->
    </Columns>
</DataGrid>
```

### Multiple Selection

```razor
<DataGrid Items="@orders" SelectionMode="DataGridSelectionMode.Multiple">
    <Columns>
        <!-- columns -->
    </Columns>
</DataGrid>
```

## Pagination

### Client-Side Pagination

```razor
<DataGrid Items="@orders" 
      PagingMode="DataGridPagingMode.Client" 
      PageSize="25">
    <Columns>
        <!-- columns -->
    </Columns>
</DataGrid>
```

### Page Size Options

The grid automatically provides page size options: 10, 25, 50, 100.

## Theming

The DataGrid component integrates with shadcn/ui themes using CSS variables.

### Available Themes

```razor
<!-- Shadcn (default) - Custom theme based on Quartz with shadcn colors -->
<DataGrid Theme="DataGridTheme.Shadcn" Items="@orders">
    <Columns><!-- columns --></Columns>
</DataGrid>

<!-- Alpine - Clean, modern theme -->
<DataGrid Theme="DataGridTheme.Alpine" Items="@orders">
    <Columns><!-- columns --></Columns>
</DataGrid>

<!-- Balham - Professional business theme -->
<DataGrid Theme="DataGridTheme.Balham" Items="@orders">
    <Columns><!-- columns --></Columns>
</DataGrid>

<!-- Quartz - Modern, minimal theme -->
<DataGrid Theme="DataGridTheme.Quartz" Items="@orders">
    <Columns><!-- columns --></Columns>
</DataGrid>

<!-- Material - Google Material Design theme -->
<DataGrid Theme="DataGridTheme.Material" Items="@orders">
    <Columns><!-- columns --></Columns>
</DataGrid>
```

### Visual Styles

Apply visual style modifiers to any theme:

```razor
<!-- Default style -->
<DataGrid VisualStyle="DataGridStyle.Default" Items="@orders">
    <Columns><!-- columns --></Columns>
</DataGrid>

<!-- Striped rows (zebra striping) -->
<DataGrid VisualStyle="DataGridStyle.Striped" Items="@orders">
    <Columns><!-- columns --></Columns>
</DataGrid>

<!-- Bordered cells -->
<DataGrid VisualStyle="DataGridStyle.Bordered" Items="@orders">
    <Columns><!-- columns --></Columns>
</DataGrid>

<!-- Minimal (no borders) -->
<DataGrid VisualStyle="DataGridStyle.Minimal" Items="@orders">
    <Columns><!-- columns --></Columns>
</DataGrid>
```

### Density Options

Control spacing and row height:

```razor
<!-- Compact (36px rows, default) -->
<DataGrid Density="DataGridDensity.Compact" Items="@orders">
    <Columns><!-- columns --></Columns>
</DataGrid>

<!-- Medium (42px rows) -->
<DataGrid Density="DataGridDensity.Medium" Items="@orders">
    <Columns><!-- columns --></Columns>
</DataGrid>

<!-- Spacious (56px rows) -->
<DataGrid Density="DataGridDensity.Spacious" Items="@orders">
    <Columns><!-- columns --></Columns>
</DataGrid>
```

### Custom Theme Parameters

Fine-tune the theme with custom parameters:

```razor
<DataGrid Items="@orders">
    <DataDataGridThemeParameters 
        AccentColor="var(--primary)"
        BackgroundColor="var(--background)"
        ForegroundColor="var(--foreground)"
        BorderColor="var(--border)"
        HeaderBackgroundColor="var(--muted)"
        RowHeight="48"
        FontSize="14"
    />
    <Columns>
        <!-- columns -->
    </Columns>
</DataGrid>
```

## Sizing

### Fixed Height

```razor
<DataGrid Items="@orders" Height="500px">
    <Columns><!-- columns --></Columns>
</DataGrid>
```

### Responsive Height

```razor
<DataGrid Items="@orders" Height="100%" Width="100%">
    <Columns><!-- columns --></Columns>
</DataGrid>
```

## Loading and Initialization States

The DataGrid component provides two separate state management concepts:

### Initialization State

Shown automatically during first-time setup (downloading scripts, building grid options):

```razor
<DataGrid Items="@orders">
    <InitializingTemplate>
        <div class="flex items-center justify-center h-64">
            <Spinner Size="SpinnerSize.Large" />
            <span class="ml-2">Setting up grid...</span>
        </div>
    </InitializingTemplate>
    <Columns>
        <!-- columns -->
    </Columns>
</DataGrid>
```

The initialization template is shown only once when the grid is first being set up. Once initialized, it never shows again.

### Loading State (Data Operations)

Show a loading indicator while data is being fetched or refreshed:

```razor
<DataGrid Items="@orders" IsLoading="@isLoading">
    <LoadingTemplate>
        <div class="flex items-center justify-center h-64">
            <Spinner Size="SpinnerSize.Large" />
            <span class="ml-2">Loading data...</span>
        </div>
    </LoadingTemplate>
    <Columns>
        <!-- columns -->
    </Columns>
</DataGrid>

@code {
    private bool isLoading = true;

    protected override async Task OnInitializedAsync()
    {
        await LoadData();
    }

    private async Task LoadData()
    {
        isLoading = true;
        orders = await OrderService.GetOrdersAsync();
        isLoading = false;
    }
}
```

Use `IsLoading` to control the loading state when fetching or refreshing data. This is independent of the initialization state.

**Key Differences:**
- **InitializingTemplate**: Shown during first-time grid setup (internal, automatic)
- **LoadingTemplate**: Shown when `IsLoading=true` (external, controlled by you)

## Empty State

Customize the message when there's no data:

```razor
<DataGrid Items="@orders">
    <Columns>
        <!-- columns -->
    </Columns>
    <!-- Empty state is shown automatically when Items is empty -->
</DataGrid>
```

## Advanced Features

### Value Selector

Use a custom function to extract cell values:

```razor
<DataGridColumn Header="Full Name">
    <CellTemplate Context="customer">
        @customer.FirstName @customer.LastName
    </CellTemplate>
</DataGridColumn>
```

### CSS Classes

Apply custom CSS classes to cells and headers:

```razor
<DataGridColumn Field="Status" 
            Header="Status"
            CellClass="font-semibold"
            HeaderClass="bg-muted">
    <CellTemplate Context="order">
        <span class="@GetStatusClass(order.Status)">
            @order.Status
        </span>
    </CellTemplate>
</DataGridColumn>
```

## Complete Example

```razor
@page "/orders"
@using NeoUI.Blazor

<div class="container py-6">
    <h1 class="text-3xl font-bold mb-6">Orders</h1>

    <DataGrid Items="@orders" 
          ActionHost="this"
          SelectionMode="DataGridSelectionMode.Multiple"
          Theme="DataGridTheme.Shadcn"
          VisualStyle="DataGridStyle.Striped"
          Density="DataGridDensity.Medium"
          PagingMode="DataGridPagingMode.Client"
          PageSize="25"
          Height="600px"
          IsLoading="@isLoading">
        
        <DataDataGridThemeParameters 
            RowHeight="48"
            BorderRadius="6"
        />

        <Columns>
            <DataGridColumn Field="Id" 
                       Header="Order #" 
                       Width="100px" 
                       Pinned="DataDataGridColumnPinPosition.Left"
                       Sortable="true" />

            <DataGridColumn Field="CustomerName" 
                       Header="Customer" 
                       Sortable="true"
                       Filterable="true" />

            <DataGridColumn Field="OrderDate" 
                       Header="Date" 
                       Sortable="true">
                <CellTemplate Context="order">
                    @order.OrderDate.ToString("MMM dd, yyyy")
                </CellTemplate>
            </DataGridColumn>

            <DataGridColumn Field="Status" 
                       Header="Status" 
                       Sortable="true">
                <CellTemplate Context="order">
                    <Badge Variant="@GetStatusVariant(order.Status)">
                        @order.Status
                    </Badge>
                </CellTemplate>
            </DataGridColumn>

            <DataGridColumn Field="Total" 
                       Header="Total" 
                       Sortable="true">
                <CellTemplate Context="order">
                    @order.Total.ToString("C")
                </CellTemplate>
            </DataGridColumn>

            <DataGridColumn Header="Actions" 
                       Width="120px" 
                       Pinned="DataDataGridColumnPinPosition.Right">
                <CellTemplate Context="order">
                    <div class="flex gap-2">
                        <Button Variant="ButtonVariant.Ghost" 
                               Size="ButtonSize.Icon"
                               data-action="Edit">
                            <LucideIcon Name="pencil" Size="16" />
                        </Button>
                        <Button Variant="ButtonVariant.Ghost" 
                               Size="ButtonSize.Icon"
                               data-action="Delete">
                            <LucideIcon Name="trash-2" Size="16" />
                        </Button>
                    </div>
                </CellTemplate>
            </DataGridColumn>
        </Columns>

        <LoadingTemplate>
            <div class="flex items-center justify-center h-96">
                <Spinner Size="SpinnerSize.Large" />
            </div>
        </LoadingTemplate>
    </DataGrid>
</div>

@code {
    private List<Order> orders = new();
    private bool isLoading = true;

    protected override async Task OnInitializedAsync()
    {
        await LoadOrders();
    }

    private async Task LoadOrders()
    {
        isLoading = true;
        orders = await OrderService.GetOrdersAsync();
        isLoading = false;
    }

    [DataGridAction]
    private async Task Edit(Order order)
    {
        NavigationManager.NavigateTo($"/orders/{order.Id}/edit");
    }

    [DataGridAction]
    private async Task Delete(Order order)
    {
        var confirmed = await DialogService.ConfirmAsync(
            "Delete Order",
            $"Are you sure you want to delete order #{order.Id}?"
        );

        if (confirmed)
        {
            await OrderService.DeleteAsync(order.Id);
            await LoadOrders();
            await ToastService.ShowSuccessAsync("Order deleted successfully");
        }
    }

    private BadgeVariant GetStatusVariant(string status) => status switch
    {
        "Completed" => BadgeVariant.Default,
        "Processing" => BadgeVariant.Secondary,
        "Cancelled" => BadgeVariant.Destructive,
        _ => BadgeVariant.Outline
    };
}
```

## API Reference

### Grid Component

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Items` | `IEnumerable<TItem>` | Required | The data items to display |
| `ActionHost` | `object?` | `null` | Host component for auto-discovering grid actions |
| `SelectionMode` | `DataGridSelectionMode` | `None` | Row selection mode (None, Single, Multiple) |
| `PagingMode` | `DataGridPagingMode` | `None` | Pagination mode (None, Client, Server, InfiniteScroll) |
| `PageSize` | `int` | `25` | Number of items per page |
| `Theme` | `DataGridTheme` | `Shadcn` | AG DataGrid theme (Shadcn, Alpine, Balham, Material, Quartz) |
| `VisualStyle` | `DataGridStyle` | `Default` | Visual style modifier (Default, Striped, Bordered, Minimal) |
| `Density` | `DataGridDensity` | `Medium` | Spacing density (Compact, Medium, Spacious) |
| `Height` | `string?` | `"300px"` | DataGrid height (e.g., "500px", "100%") |
| `Width` | `string?` | `"100%"` | DataGrid width (e.g., "800px", "100%") |
| `IsLoading` | `bool` | `false` | Show loading state for data operations |
| `Class` | `string?` | `null` | Additional CSS classes |
| `Columns` | `RenderFragment` | Required | Column definitions |
| `LoadingTemplate` | `RenderFragment?` | `null` | Custom template for data loading state |
| `InitializingTemplate` | `RenderFragment?` | `null` | Custom template for first-time grid initialization |

### DataGridColumn Component

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Id` | `string?` | Auto-generated | Unique column identifier |
| `Field` | `string?` | `null` | Property name to bind to |
| `Header` | `string` | Required | Column header text |
| `Sortable` | `bool` | `false` | Enable sorting |
| `Filterable` | `bool` | `false` | Enable filtering |
| `Width` | `string?` | `null` | Column width (e.g., "150px") |
| `MinWidth` | `string?` | `null` | Minimum width |
| `MaxWidth` | `string?` | `null` | Maximum width |
| `Pinned` | `DataDataGridColumnPinPosition` | `None` | Pin column (None, Left, Right) |
| `AllowResize` | `bool` | `true` | Allow column resizing |
| `AllowReorder` | `bool` | `true` | Allow column reordering |
| `IsVisible` | `bool` | `true` | Column visibility |
| `CellTemplate` | `RenderFragment<TItem>?` | `null` | Custom cell template |
| `HeaderTemplate` | `RenderFragment?` | `null` | Custom header template |
| `DataFormatString` | `string?` | `null` | Format string for cell values (e.g., "C", "N2", "P") |
| `CellClass` | `string?` | `null` | CSS class for cells |
| `HeaderClass` | `string?` | `null` | CSS class for header |

### DataGridActionAttribute

```csharp
[AttributeUsage(AttributeTargets.Method)]
public class DataGridActionAttribute : Attribute
{
    public string? Name { get; set; }
}
```

## Dependencies

- **AG Grid Community** (v32.3.3) - Loaded automatically from CDN
- **IDataGridDataGridTemplateRenderer** - Required for cell and header templates (auto-registered)

## Browser Support

- Chrome (latest)
- Firefox (latest)
- Safari (latest)
- Edge (latest)

## Performance Considerations

- ✅ Virtualized rendering for large datasets
- ✅ Efficient DOM updates
- ✅ Optimized for 10,000+ rows
- ✅ Lazy loading support
- ✅ Server-side pagination for very large datasets

## Accessibility

- ✅ Full keyboard navigation
- ✅ Screen reader support
- ✅ ARIA attributes
- ✅ Focus management

## Related Components

- [DataTable](../DataTable/README.md) - Styled data table with built-in features
- [Table Primitive](../../Primitives/Table/README.md) - Headless table primitive

## Examples

See the demo application for more examples:
- Basic grid with sorting and filtering
- Cell templates with badges and icons
- DataGrid actions with auto-discovery
- Custom themes and styling
- Loading and empty states

## License

MIT

---

> **Localization**: UI strings (Loading, No Results Found, Initializing) use `ILocalizer`. Override `DataGrid.Loading`, `DataGrid.NoResultsFound`, `DataGrid.InitializingGrid` keys in your `ILocalizer` implementation.
