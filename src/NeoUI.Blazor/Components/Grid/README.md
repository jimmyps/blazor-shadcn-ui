# Grid Component

A powerful, enterprise-grade data grid component powered by AG Grid with full Blazor integration, template support, and shadcn/ui theming.

## Features

- ✅ **Powered by AG Grid Community** - Industry-leading grid library with excellent performance
- ✅ **Blazor Template Support** - Use Blazor `RenderFragment` for cells and headers
- ✅ **Cell Actions** - Auto-discovery of grid actions via `[GridAction]` attribute
- ✅ **Full Theme Integration** - Seamless shadcn/ui theme integration with CSS variables
- ✅ **Sorting & Filtering** - Built-in sorting and filtering capabilities
- ✅ **Pagination** - Client-side and server-side pagination support
- ✅ **Row Selection** - Single and multiple row selection modes
- ✅ **Column Features** - Pinning, resizing, reordering, and visibility control
- ✅ **Virtualization** - Efficient rendering of large datasets
- ✅ **Responsive** - Mobile-friendly with adaptive layouts

## Installation

The Grid component is included in the `BlazorUI.Components` package and uses AG Grid Community Edition (loaded automatically from CDN).

## Basic Usage

```razor
<Grid Items="@orders" ActionHost="this">
    <Columns>
        <GridColumn Field="Id" Header="Order ID" Sortable="true" Width="100px" />
        <GridColumn Field="CustomerName" Header="Customer" Sortable="true" />
        <GridColumn Field="OrderDate" Header="Date" Sortable="true" />
        <GridColumn Field="Total" Header="Total" Sortable="true" />
    </Columns>
</Grid>

@code {
    private List<Order> orders = new();
}
```

## Column Configuration

### Basic Column Properties

```razor
<GridColumn 
    Id="custom-id"                    // Unique column identifier
    Field="PropertyName"              // Property to bind to
    Header="Display Name"             // Column header text
    Sortable="true"                   // Enable sorting
    Filterable="true"                 // Enable filtering
    Width="150px"                     // Fixed width
    MinWidth="100px"                  // Minimum width
    MaxWidth="300px"                  // Maximum width
    Pinned="GridColumnPinPosition.Left"  // Pin column
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
<GridColumn Field="Price" Header="Price" DataFormatString="C" />
<!-- Output: $1,234.56 -->

<!-- Number with 2 decimals -->
<GridColumn Field="Quantity" Header="Qty" DataFormatString="N2" />
<!-- Output: 1,234.56 -->

<!-- Number with no decimals (integer) -->
<GridColumn Field="Count" Header="Count" DataFormatString="N0" />
<!-- Output: 1,234 -->

<!-- Percentage with 2 decimals -->
<GridColumn Field="Rate" Header="Rate" DataFormatString="P2" />
<!-- Output: 45.67% -->

<!-- Short date -->
<GridColumn Field="OrderDate" Header="Date" DataFormatString="d" />
<!-- Output: 12/31/2024 -->

<!-- Long date -->
<GridColumn Field="OrderDate" Header="Date" DataFormatString="D" />
<!-- Output: Tuesday, December 31, 2024 -->

<!-- Short time -->
<GridColumn Field="CreatedAt" Header="Time" DataFormatString="t" />
<!-- Output: 3:45 PM -->

<!-- Composite format strings also supported -->
<GridColumn Field="Price" Header="Price" DataFormatString="{0:C}" />
<GridColumn Field="Discount" Header="Discount" DataFormatString="{0:P1}" />
```

**How It Works:**
- Formatting happens in C# using .NET's `IFormattable.ToString(format, culture)`
- Formatted values are added as `{field}_formatted` properties to the data
- AG Grid displays the formatted value while preserving the original for sorting/filtering
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
<GridColumn Field="Id" Header="ID" Pinned="GridColumnPinPosition.Left" />
<GridColumn Field="Name" Header="Name" />
<GridColumn Field="Actions" Header="Actions" Pinned="GridColumnPinPosition.Right" />
```

## Cell Templates

Use Blazor `RenderFragment` to create custom cell content:

```razor
<GridColumn Field="Status" Header="Status">
    <CellTemplate Context="order">
        <Badge Variant="@(order.Status == "Active" ? BadgeVariant.Default : BadgeVariant.Secondary)">
            @order.Status
        </Badge>
    </CellTemplate>
</GridColumn>

<GridColumn Field="Actions" Header="Actions">
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
</GridColumn>
```

## Header Templates

Customize column headers with Blazor templates:

```razor
<GridColumn Field="Name" Header="Customer Name">
    <HeaderTemplate>
        <div class="flex items-center gap-2">
            <LucideIcon Name="user" Size="16" />
            <span>Customer</span>
        </div>
    </HeaderTemplate>
</GridColumn>
```

## Cell Actions

Use the `[GridAction]` attribute to automatically wire up actions from your component:

```razor
<Grid Items="@orders" ActionHost="this">
    <Columns>
        <GridColumn Field="Actions" Header="Actions">
            <CellTemplate Context="order">
                <Button data-action="Edit">Edit</Button>
                <Button data-action="Delete">Delete</Button>
            </CellTemplate>
        </GridColumn>
    </Columns>
</Grid>

@code {
    private List<Order> orders = new();

    [GridAction]
    private async Task Edit(Order order)
    {
        // Handle edit action
        await ShowEditDialog(order);
    }

    [GridAction(Name = "Delete")]
    private async Task HandleDelete(Order order)
    {
        // Handle delete action
        await DeleteOrder(order);
    }
}
```

### How Cell Actions Work

1. Add `data-action="ActionName"` to any element in your cell template
2. Mark a method in your component with `[GridAction]`
3. Set `ActionHost="this"` on the Grid component
4. Actions are auto-discovered and wired up automatically

The method signature must be:
- `void MethodName(TItem item)` for synchronous actions
- `Task MethodName(TItem item)` for async actions

## Selection

### Single Selection

```razor
<Grid Items="@orders" SelectionMode="GridSelectionMode.Single">
    <Columns>
        <!-- columns -->
    </Columns>
</Grid>
```

### Multiple Selection

```razor
<Grid Items="@orders" SelectionMode="GridSelectionMode.Multiple">
    <Columns>
        <!-- columns -->
    </Columns>
</Grid>
```

## Pagination

### Client-Side Pagination

```razor
<Grid Items="@orders" 
      PagingMode="GridPagingMode.Client" 
      PageSize="25">
    <Columns>
        <!-- columns -->
    </Columns>
</Grid>
```

### Page Size Options

The grid automatically provides page size options: 10, 25, 50, 100.

## Theming

The Grid component integrates with shadcn/ui themes using CSS variables.

### Available Themes

```razor
<!-- Shadcn (default) - Custom theme based on Quartz with shadcn colors -->
<Grid Theme="GridTheme.Shadcn" Items="@orders">
    <Columns><!-- columns --></Columns>
</Grid>

<!-- Alpine - Clean, modern theme -->
<Grid Theme="GridTheme.Alpine" Items="@orders">
    <Columns><!-- columns --></Columns>
</Grid>

<!-- Balham - Professional business theme -->
<Grid Theme="GridTheme.Balham" Items="@orders">
    <Columns><!-- columns --></Columns>
</Grid>

<!-- Quartz - Modern, minimal theme -->
<Grid Theme="GridTheme.Quartz" Items="@orders">
    <Columns><!-- columns --></Columns>
</Grid>

<!-- Material - Google Material Design theme -->
<Grid Theme="GridTheme.Material" Items="@orders">
    <Columns><!-- columns --></Columns>
</Grid>
```

### Visual Styles

Apply visual style modifiers to any theme:

```razor
<!-- Default style -->
<Grid VisualStyle="GridStyle.Default" Items="@orders">
    <Columns><!-- columns --></Columns>
</Grid>

<!-- Striped rows (zebra striping) -->
<Grid VisualStyle="GridStyle.Striped" Items="@orders">
    <Columns><!-- columns --></Columns>
</Grid>

<!-- Bordered cells -->
<Grid VisualStyle="GridStyle.Bordered" Items="@orders">
    <Columns><!-- columns --></Columns>
</Grid>

<!-- Minimal (no borders) -->
<Grid VisualStyle="GridStyle.Minimal" Items="@orders">
    <Columns><!-- columns --></Columns>
</Grid>
```

### Density Options

Control spacing and row height:

```razor
<!-- Compact (28px rows) -->
<Grid Density="GridDensity.Compact" Items="@orders">
    <Columns><!-- columns --></Columns>
</Grid>

<!-- Comfortable (42px rows, default) -->
<Grid Density="GridDensity.Comfortable" Items="@orders">
    <Columns><!-- columns --></Columns>
</Grid>

<!-- Spacious (56px rows) -->
<Grid Density="GridDensity.Spacious" Items="@orders">
    <Columns><!-- columns --></Columns>
</Grid>
```

### Custom Theme Parameters

Fine-tune the theme with custom parameters:

```razor
<Grid Items="@orders">
    <GridThemeParameters 
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
</Grid>
```

## Sizing

### Fixed Height

```razor
<Grid Items="@orders" Height="500px">
    <Columns><!-- columns --></Columns>
</Grid>
```

### Responsive Height

```razor
<Grid Items="@orders" Height="100%" Width="100%">
    <Columns><!-- columns --></Columns>
</Grid>
```

## Loading State

Show a loading indicator while data is being fetched:

```razor
<Grid Items="@orders" IsLoading="@isLoading">
    <LoadingTemplate>
        <div class="flex items-center justify-center h-64">
            <Spinner Size="SpinnerSize.Large" />
            <span class="ml-2">Loading data...</span>
        </div>
    </LoadingTemplate>
    <Columns>
        <!-- columns -->
    </Columns>
</Grid>
```

## Empty State

Customize the message when there's no data:

```razor
<Grid Items="@orders">
    <Columns>
        <!-- columns -->
    </Columns>
    <!-- Empty state is shown automatically when Items is empty -->
</Grid>
```

## Advanced Features

### Value Selector

Use a custom function to extract cell values:

```razor
<GridColumn Header="Full Name">
    <CellTemplate Context="customer">
        @customer.FirstName @customer.LastName
    </CellTemplate>
</GridColumn>
```

### CSS Classes

Apply custom CSS classes to cells and headers:

```razor
<GridColumn Field="Status" 
            Header="Status"
            CellClass="font-semibold"
            HeaderClass="bg-muted">
    <CellTemplate Context="order">
        <span class="@GetStatusClass(order.Status)">
            @order.Status
        </span>
    </CellTemplate>
</GridColumn>
```

## Complete Example

```razor
@page "/orders"
@using BlazorUI.Components.Grid

<div class="container py-6">
    <h1 class="text-3xl font-bold mb-6">Orders</h1>

    <Grid Items="@orders" 
          ActionHost="this"
          SelectionMode="GridSelectionMode.Multiple"
          Theme="GridTheme.Shadcn"
          VisualStyle="GridStyle.Striped"
          Density="GridDensity.Comfortable"
          PagingMode="GridPagingMode.Client"
          PageSize="25"
          Height="600px"
          IsLoading="@isLoading">
        
        <GridThemeParameters 
            RowHeight="48"
            BorderRadius="6"
        />

        <Columns>
            <GridColumn Field="Id" 
                       Header="Order #" 
                       Width="100px" 
                       Pinned="GridColumnPinPosition.Left"
                       Sortable="true" />

            <GridColumn Field="CustomerName" 
                       Header="Customer" 
                       Sortable="true"
                       Filterable="true" />

            <GridColumn Field="OrderDate" 
                       Header="Date" 
                       Sortable="true">
                <CellTemplate Context="order">
                    @order.OrderDate.ToString("MMM dd, yyyy")
                </CellTemplate>
            </GridColumn>

            <GridColumn Field="Status" 
                       Header="Status" 
                       Sortable="true">
                <CellTemplate Context="order">
                    <Badge Variant="@GetStatusVariant(order.Status)">
                        @order.Status
                    </Badge>
                </CellTemplate>
            </GridColumn>

            <GridColumn Field="Total" 
                       Header="Total" 
                       Sortable="true">
                <CellTemplate Context="order">
                    @order.Total.ToString("C")
                </CellTemplate>
            </GridColumn>

            <GridColumn Header="Actions" 
                       Width="120px" 
                       Pinned="GridColumnPinPosition.Right">
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
            </GridColumn>
        </Columns>

        <LoadingTemplate>
            <div class="flex items-center justify-center h-96">
                <Spinner Size="SpinnerSize.Large" />
            </div>
        </LoadingTemplate>
    </Grid>
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

    [GridAction]
    private async Task Edit(Order order)
    {
        NavigationManager.NavigateTo($"/orders/{order.Id}/edit");
    }

    [GridAction]
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
| `SelectionMode` | `GridSelectionMode` | `None` | Row selection mode (None, Single, Multiple) |
| `PagingMode` | `GridPagingMode` | `None` | Pagination mode (None, Client, Server, InfiniteScroll) |
| `PageSize` | `int` | `25` | Number of items per page |
| `Theme` | `GridTheme` | `Shadcn` | AG Grid theme (Shadcn, Alpine, Balham, Material, Quartz) |
| `VisualStyle` | `GridStyle` | `Default` | Visual style modifier (Default, Striped, Bordered, Minimal) |
| `Density` | `GridDensity` | `Comfortable` | Spacing density (Compact, Comfortable, Spacious) |
| `Height` | `string?` | `"300px"` | Grid height (e.g., "500px", "100%") |
| `Width` | `string?` | `"100%"` | Grid width (e.g., "800px", "100%") |
| `IsLoading` | `bool` | `false` | Show loading state |
| `Class` | `string?` | `null` | Additional CSS classes |
| `Columns` | `RenderFragment` | Required | Column definitions |
| `LoadingTemplate` | `RenderFragment?` | `null` | Custom loading template |

### GridColumn Component

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
| `Pinned` | `GridColumnPinPosition` | `None` | Pin column (None, Left, Right) |
| `AllowResize` | `bool` | `true` | Allow column resizing |
| `AllowReorder` | `bool` | `true` | Allow column reordering |
| `IsVisible` | `bool` | `true` | Column visibility |
| `CellTemplate` | `RenderFragment<TItem>?` | `null` | Custom cell template |
| `HeaderTemplate` | `RenderFragment?` | `null` | Custom header template |
| `DataFormatString` | `string?` | `null` | Format string for cell values (e.g., "C", "N2", "P") |
| `CellClass` | `string?` | `null` | CSS class for cells |
| `HeaderClass` | `string?` | `null` | CSS class for header |

### GridActionAttribute

```csharp
[AttributeUsage(AttributeTargets.Method)]
public class GridActionAttribute : Attribute
{
    public string? Name { get; set; }
}
```

## Dependencies

- **AG Grid Community** (v32.3.3) - Loaded automatically from CDN
- **ITemplateRenderer** - Required for cell and header templates (auto-registered)

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
- Grid actions with auto-discovery
- Custom themes and styling
- Loading and empty states

## License

MIT
