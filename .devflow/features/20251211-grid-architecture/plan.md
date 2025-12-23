# Technical Implementation Plan: Blazor-native Data Grid Architecture

**Feature:** 20251211-grid-architecture
**Status:** Planning
**Architect:** GitHub Copilot
**Created:** 2025-12-11

## 1. Architecture Overview

### Grid System Architecture

The implementation follows a four-layer architecture inspired by the charting components pattern:

```
┌─────────────────────────────────────────────────┐
│         PUBLIC COMPONENTS LAYER                  │
│  Grid<TItem>, GridColumn<TItem>                 │
│  - Blazor-native, renderer-agnostic            │
│  - RenderFragment<TItem> templates             │
│  - Type inference with CascadingTypeParameter  │
│  - No AG Grid/JS concepts                      │
└─────────────────────────────────────────────────┘
                    ↓
┌─────────────────────────────────────────────────┐
│         INTERNAL OBJECT MODEL LAYER              │
│  GridDefinition<TItem>                          │
│  GridColumnDefinition<TItem>                    │
│  - Internal representation of grid config      │
│  - Template references and metadata            │
│  - Passed to renderers for processing          │
└─────────────────────────────────────────────────┘
                    ↓
┌─────────────────────────────────────────────────┐
│         STATE & DATA LAYER                       │
│  GridState, GridDataRequest, GridDataResponse  │
│  - JSON-serializable DTOs                      │
│  - Sort/filter/page descriptors                │
│  - Column state (visibility, width, pinning)   │
└─────────────────────────────────────────────────┘
                    ↓
┌─────────────────────────────────────────────────┐
│         RENDERER ABSTRACTION LAYER               │
│  IGridRenderer                                   │
│  IGridRendererCapabilities                      │
│  IGridExportService                             │
│  - Renderer-agnostic interfaces                │
└─────────────────────────────────────────────────┘
                    ↓
┌─────────────────────────────────────────────────┐
│         RENDERER IMPLEMENTATIONS                 │
│  AgGridRenderer (v1)                            │
│  NativeBlazorRenderer (future)                  │
│  - AG Grid JS interop                          │
│  - Template HTML generation                    │
│  - State synchronization                       │
└─────────────────────────────────────────────────┘
```

### Alignment with BlazorUI Architecture

- **Follows charting pattern**: Styled components + renderer abstraction + service layer
- **Consistent with DataTable**: Similar parameter naming, selection modes, event patterns
- **Integrates with primitives**: Uses Table primitive concepts where applicable
- **Shadcn theming**: CSS classes and variables for visual consistency
- **Service injection**: Renderer registered via DI, injectable into components

## 2. Module Structure

### File Organization

```
src/BlazorUI.Components/
├── Components/
│   └── Grid/
│       ├── Grid.razor                      # Main grid component
│       ├── Grid.razor.cs                   # Grid code-behind
│       ├── GridColumn.razor                # Column definition component
│       ├── GridColumn.razor.cs             # Column code-behind
│       ├── GridSelectionMode.cs            # Enum: None, Single, Multiple
│       ├── GridPagingMode.cs               # Enum: None, Client, Server, InfiniteScroll
│       ├── GridVirtualizationMode.cs       # Enum: Auto, None, RowOnly, RowAndColumn
│       ├── GridTheme.cs                    # Enum: Default, Striped, Bordered, Minimal
│       ├── GridDensity.cs                  # Enum: Comfortable, Compact, Spacious
│       ├── GridColumnPinPosition.cs        # Enum: None, Left, Right
│       ├── GridSortDirection.cs            # Enum: None, Ascending, Descending
│       ├── GridFilterOperator.cs           # Enum: Equals, Contains, etc.
│       ├── GridState.cs                    # State DTO
│       ├── GridColumnState.cs              # Column state DTO
│       ├── GridSortDescriptor.cs           # Sort descriptor DTO
│       ├── GridFilterDescriptor.cs         # Filter descriptor DTO
│       ├── GridDataRequest.cs              # Data request DTO
│       ├── GridDataResponse.cs             # Data response DTO
│       ├── GridDefinition.cs               # Internal grid model
│       └── GridColumnDefinition.cs         # Internal column model
├── Services/
│   └── Grid/
│       ├── IGridRenderer.cs                # Renderer interface
│       ├── IGridRendererCapabilities.cs    # Capabilities interface
│       ├── IGridExportService.cs           # Export service interface
│       ├── AgGridRenderer.cs               # AG Grid renderer implementation
│       ├── GridRendererFactory.cs          # Factory for creating renderers
│       └── CsvExportService.cs             # CSV export implementation
└── wwwroot/
    └── js/
        └── aggrid-renderer.js              # AG Grid JavaScript interop
```

## 3. Technical Approach

### Phase 1: Core Object Model (Week 1)

#### Enums and Basic Types

All enums use `[JsonConverter(typeof(JsonStringEnumConverter))]` for JSON serialization.

**GridSelectionMode.cs:**
```csharp
namespace BlazorUI.Components.Grid;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum GridSelectionMode
{
    None,
    Single,
    Multiple
}
```

**GridPagingMode.cs:**
```csharp
namespace BlazorUI.Components.Grid;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum GridPagingMode
{
    None,      // All data displayed, no pagination
    Client,    // All data loaded, paginated client-side
    Server,    // Data fetched page-by-page from server
    InfiniteScroll  // Incremental loading as user scrolls
}
```

**GridVirtualizationMode.cs:**
```csharp
namespace BlazorUI.Components.Grid;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum GridVirtualizationMode
{
    Auto,           // Renderer decides based on data size
    None,           // No virtualization
    RowOnly,        // Row virtualization only
    RowAndColumn    // Both row and column virtualization
}
```

**GridColumnPinPosition.cs:**
```csharp
namespace BlazorUI.Components.Grid;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum GridColumnPinPosition
{
    None,   // Not pinned
    Left,   // Pinned to left side
    Right   // Pinned to right side
}
```

**GridSortDirection.cs:**
```csharp
namespace BlazorUI.Components.Grid;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum GridSortDirection
{
    None,
    Ascending,
    Descending
}
```

**GridFilterOperator.cs:**
```csharp
namespace BlazorUI.Components.Grid;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum GridFilterOperator
{
    Equals,
    NotEquals,
    Contains,
    NotContains,
    StartsWith,
    EndsWith,
    LessThan,
    LessThanOrEqual,
    GreaterThan,
    GreaterThanOrEqual,
    IsEmpty,
    IsNotEmpty
}
```

**GridTheme.cs:**
```csharp
namespace BlazorUI.Components.Grid;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum GridTheme
{
    Default,   // Standard borders and hover states
    Striped,   // Alternating row backgrounds
    Bordered,  // Bordered cells with vertical dividers
    Minimal    // Minimal borders, subtle styling
}
```

**GridDensity.cs:**
```csharp
namespace BlazorUI.Components.Grid;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum GridDensity
{
    Comfortable,  // Standard padding (default)
    Compact,      // Reduced padding for more rows
    Spacious      // Increased padding for readability
}
```

#### State DTOs

**GridSortDescriptor.cs:**
```csharp
namespace BlazorUI.Components.Grid;

public class GridSortDescriptor
{
    [JsonPropertyName("field")]
    public string Field { get; set; } = string.Empty;

    [JsonPropertyName("direction")]
    public GridSortDirection Direction { get; set; } = GridSortDirection.None;

    [JsonPropertyName("order")]
    public int Order { get; set; }
}
```

**GridFilterDescriptor.cs:**
```csharp
namespace BlazorUI.Components.Grid;

public class GridFilterDescriptor
{
    [JsonPropertyName("field")]
    public string Field { get; set; } = string.Empty;

    [JsonPropertyName("operator")]
    public GridFilterOperator Operator { get; set; } = GridFilterOperator.Contains;

    [JsonPropertyName("value")]
    public object? Value { get; set; }

    [JsonPropertyName("caseSensitive")]
    public bool CaseSensitive { get; set; }
}
```

**GridColumnState.cs:**
```csharp
namespace BlazorUI.Components.Grid;

public class GridColumnState
{
    [JsonPropertyName("field")]
    public string Field { get; set; } = string.Empty;

    [JsonPropertyName("visible")]
    public bool Visible { get; set; } = true;

    [JsonPropertyName("width")]
    public string? Width { get; set; }

    [JsonPropertyName("pinned")]
    public GridColumnPinPosition Pinned { get; set; } = GridColumnPinPosition.None;

    [JsonPropertyName("order")]
    public int Order { get; set; }
}
```

**GridState.cs:**
```csharp
namespace BlazorUI.Components.Grid;

public class GridState
{
    [JsonPropertyName("sortDescriptors")]
    public List<GridSortDescriptor> SortDescriptors { get; set; } = new();

    [JsonPropertyName("filterDescriptors")]
    public List<GridFilterDescriptor> FilterDescriptors { get; set; } = new();

    [JsonPropertyName("pageNumber")]
    public int PageNumber { get; set; } = 1;

    [JsonPropertyName("pageSize")]
    public int PageSize { get; set; } = 25;

    [JsonPropertyName("columnStates")]
    public List<GridColumnState> ColumnStates { get; set; } = new();

    [JsonPropertyName("selectedRowIds")]
    public List<object> SelectedRowIds { get; set; } = new();
}
```

**GridDataRequest.cs:**
```csharp
namespace BlazorUI.Components.Grid;

public class GridDataRequest<TItem>
{
    [JsonPropertyName("startIndex")]
    public int StartIndex { get; set; }

    [JsonPropertyName("count")]
    public int Count { get; set; }

    [JsonPropertyName("sortDescriptors")]
    public List<GridSortDescriptor> SortDescriptors { get; set; } = new();

    [JsonPropertyName("filterDescriptors")]
    public List<GridFilterDescriptor> FilterDescriptors { get; set; } = new();

    [JsonPropertyName("pageNumber")]
    public int PageNumber { get; set; } = 1;

    [JsonPropertyName("pageSize")]
    public int PageSize { get; set; } = 25;

    [JsonPropertyName("customParameters")]
    public Dictionary<string, object?> CustomParameters { get; set; } = new();
}
```

**GridDataResponse.cs:**
```csharp
namespace BlazorUI.Components.Grid;

public class GridDataResponse<TItem>
{
    [JsonPropertyName("items")]
    public IEnumerable<TItem> Items { get; set; } = Array.Empty<TItem>();

    [JsonPropertyName("totalCount")]
    public int TotalCount { get; set; }

    [JsonPropertyName("filteredCount")]
    public int? FilteredCount { get; set; }
}
```

#### Internal Object Model

**GridColumnDefinition.cs:**
```csharp
namespace BlazorUI.Components.Grid;

internal class GridColumnDefinition<TItem>
{
    public string Id { get; set; } = string.Empty;
    public string? Field { get; set; }
    public string Header { get; set; } = string.Empty;
    public bool Sortable { get; set; }
    public bool Filterable { get; set; }
    public string? Width { get; set; }
    public string? MinWidth { get; set; }
    public string? MaxWidth { get; set; }
    public GridColumnPinPosition Pinned { get; set; } = GridColumnPinPosition.None;
    public bool AllowResize { get; set; } = true;
    public bool AllowReorder { get; set; } = true;
    public bool IsVisible { get; set; } = true;
    
    // Templates
    public RenderFragment<TItem>? CellTemplate { get; set; }
    public RenderFragment? HeaderTemplate { get; set; }
    public RenderFragment? FilterTemplate { get; set; }
    public RenderFragment<TItem>? CellEditTemplate { get; set; }
    
    // Value extraction
    public Func<TItem, object?>? ValueSelector { get; set; }
    
    // CSS classes
    public string? CellClass { get; set; }
    public string? HeaderClass { get; set; }
    
    // Metadata for renderer
    public Dictionary<string, object?> Metadata { get; set; } = new();
}
```

**GridDefinition.cs:**
```csharp
namespace BlazorUI.Components.Grid;

internal class GridDefinition<TItem>
{
    public List<GridColumnDefinition<TItem>> Columns { get; set; } = new();
    public GridSelectionMode SelectionMode { get; set; } = GridSelectionMode.None;
    public GridPagingMode PagingMode { get; set; } = GridPagingMode.Client;
    public GridVirtualizationMode VirtualizationMode { get; set; } = GridVirtualizationMode.Auto;
    public GridTheme Theme { get; set; } = GridTheme.Default;
    public GridDensity Density { get; set; } = GridDensity.Comfortable;
    public int PageSize { get; set; } = 25;
    public GridState? InitialState { get; set; }
    
    // Callbacks
    public EventCallback<GridState> OnStateChanged { get; set; }
    public EventCallback<GridDataRequest<TItem>> OnDataRequest { get; set; }
    public EventCallback<IReadOnlyCollection<TItem>> OnSelectionChanged { get; set; }
    
    // CSS
    public string? Class { get; set; }
    public string? Style { get; set; }
    
    // Localization
    public string? LocalizationKeyPrefix { get; set; }
    
    // Metadata for renderer
    public Dictionary<string, object?> Metadata { get; set; } = new();
}
```

### Phase 2: Grid Components (Week 2)

#### GridColumn Component

**GridColumn.razor:**
```razor
@namespace BlazorUI.Components.Grid
@typeparam TItem

@* This component has no render output - it registers itself with parent Grid *@
```

**GridColumn.razor.cs:**
```csharp
using Microsoft.AspNetCore.Components;

namespace BlazorUI.Components.Grid;

/// <summary>
/// Defines a column in a Grid component.
/// </summary>
/// <typeparam name="TItem">The type of items in the grid.</typeparam>
public partial class GridColumn<TItem> : ComponentBase
{
    [Parameter] public string? Id { get; set; }
    [Parameter] public string? Field { get; set; }
    [Parameter, EditorRequired] public string Header { get; set; } = string.Empty;
    [Parameter] public bool Sortable { get; set; }
    [Parameter] public bool Filterable { get; set; }
    [Parameter] public string? Width { get; set; }
    [Parameter] public string? MinWidth { get; set; }
    [Parameter] public string? MaxWidth { get; set; }
    [Parameter] public GridColumnPinPosition Pinned { get; set; } = GridColumnPinPosition.None;
    [Parameter] public bool AllowResize { get; set; } = true;
    [Parameter] public bool AllowReorder { get; set; } = true;
    [Parameter] public bool IsVisible { get; set; } = true;
    
    [Parameter] public RenderFragment<TItem>? CellTemplate { get; set; }
    [Parameter] public RenderFragment? HeaderTemplate { get; set; }
    [Parameter] public RenderFragment? FilterTemplate { get; set; }
    [Parameter] public RenderFragment<TItem>? CellEditTemplate { get; set; }
    
    [Parameter] public Func<TItem, object?>? ValueSelector { get; set; }
    [Parameter] public string? CellClass { get; set; }
    [Parameter] public string? HeaderClass { get; set; }
    
    [CascadingParameter] internal Grid<TItem>? ParentGrid { get; set; }
    
    protected override void OnInitialized()
    {
        if (ParentGrid == null)
        {
            throw new InvalidOperationException(
                $"{nameof(GridColumn<TItem>)} must be placed inside a {nameof(Grid<TItem>)} component.");
        }
        
        ParentGrid.RegisterColumn(this);
    }
    
    internal GridColumnDefinition<TItem> ToDefinition()
    {
        return new GridColumnDefinition<TItem>
        {
            Id = Id ?? Field ?? Header.ToLowerInvariant().Replace(" ", "-"),
            Field = Field,
            Header = Header,
            Sortable = Sortable,
            Filterable = Filterable,
            Width = Width,
            MinWidth = MinWidth,
            MaxWidth = MaxWidth,
            Pinned = Pinned,
            AllowResize = AllowResize,
            AllowReorder = AllowReorder,
            IsVisible = IsVisible,
            CellTemplate = CellTemplate,
            HeaderTemplate = HeaderTemplate,
            FilterTemplate = FilterTemplate,
            CellEditTemplate = CellEditTemplate,
            ValueSelector = ValueSelector,
            CellClass = CellClass,
            HeaderClass = HeaderClass
        };
    }
}
```

#### Grid Component

**Grid.razor:**
```razor
@namespace BlazorUI.Components.Grid
@typeparam TItem
@using BlazorUI.Components.Utilities
@inject IGridRenderer GridRenderer

<CascadingValue Value="this" IsFixed="true">
    <div class="@ContainerCssClass" style="@Style">
        @if (IsLoading)
        {
            @if (LoadingTemplate != null)
            {
                @LoadingTemplate
            }
            else
            {
                <div class="flex items-center justify-center p-8 text-muted-foreground">
                    <div class="text-sm">Loading...</div>
                </div>
            }
        }
        else if (_gridDefinition.Columns.Count == 0)
        {
            <div class="text-sm text-muted-foreground p-4">
                No columns defined. Add GridColumn components inside the Columns parameter.
            </div>
        }
        else
        {
            <div @ref="_gridContainer" class="@GridCssClass"></div>
        }
    </div>
    
    @* Capture column definitions *@
    @Columns
</CascadingValue>
```

**Grid.razor.cs:**
```csharp
using BlazorUI.Components.Services.Grid;
using BlazorUI.Components.Utilities;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace BlazorUI.Components.Grid;

/// <summary>
/// A renderer-agnostic data grid component with support for sorting, filtering,
/// pagination, virtualization, and state persistence.
/// </summary>
/// <typeparam name="TItem">The type of items in the grid.</typeparam>
public partial class Grid<TItem> : ComponentBase, IAsyncDisposable
{
    [Inject] private IGridRenderer GridRenderer { get; set; } = null!;
    
    private ElementReference _gridContainer;
    private GridDefinition<TItem> _gridDefinition = new();
    private List<GridColumn<TItem>> _columns = new();
    private bool _initialized = false;
    
    [Parameter] public IEnumerable<TItem> Items { get; set; } = Array.Empty<TItem>();
    [Parameter] public GridSelectionMode SelectionMode { get; set; } = GridSelectionMode.None;
    [Parameter] public GridPagingMode PagingMode { get; set; } = GridPagingMode.Client;
    [Parameter] public int PageSize { get; set; } = 25;
    [Parameter] public GridVirtualizationMode VirtualizationMode { get; set; } = GridVirtualizationMode.Auto;
    [Parameter] public GridTheme Theme { get; set; } = GridTheme.Default;
    [Parameter] public GridDensity Density { get; set; } = GridDensity.Comfortable;
    [Parameter] public GridState? InitialState { get; set; }
    [Parameter] public bool IsLoading { get; set; }
    
    [Parameter] public RenderFragment? Columns { get; set; }
    [Parameter] public RenderFragment? LoadingTemplate { get; set; }
    [Parameter] public string? Class { get; set; }
    [Parameter] public string? Style { get; set; }
    [Parameter] public string? LocalizationKeyPrefix { get; set; }
    
    [Parameter] public EventCallback<GridState> OnStateChanged { get; set; }
    [Parameter] public EventCallback<GridDataRequest<TItem>> OnDataRequest { get; set; }
    [Parameter] public EventCallback<IReadOnlyCollection<TItem>> OnSelectionChanged { get; set; }
    
    [Parameter] public IReadOnlyCollection<TItem> SelectedItems { get; set; } = Array.Empty<TItem>();
    [Parameter] public EventCallback<IReadOnlyCollection<TItem>> SelectedItemsChanged { get; set; }
    
    private string ContainerCssClass => ClassNames.cn(
        "grid-container w-full",
        Class
    );
    
    private string GridCssClass => ClassNames.cn(
        "grid-content",
        GetThemeClass(),
        GetDensityClass()
    );
    
    protected override void OnInitialized()
    {
        BuildGridDefinition();
    }
    
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender && !_initialized)
        {
            await InitializeGridAsync();
            _initialized = true;
        }
    }
    
    internal void RegisterColumn(GridColumn<TItem> column)
    {
        _columns.Add(column);
    }
    
    private void BuildGridDefinition()
    {
        _gridDefinition.Columns = _columns.Select(c => c.ToDefinition()).ToList();
        _gridDefinition.SelectionMode = SelectionMode;
        _gridDefinition.PagingMode = PagingMode;
        _gridDefinition.VirtualizationMode = VirtualizationMode;
        _gridDefinition.Theme = Theme;
        _gridDefinition.Density = Density;
        _gridDefinition.PageSize = PageSize;
        _gridDefinition.InitialState = InitialState;
        _gridDefinition.OnStateChanged = OnStateChanged;
        _gridDefinition.OnDataRequest = OnDataRequest;
        _gridDefinition.OnSelectionChanged = OnSelectionChanged;
        _gridDefinition.Class = Class;
        _gridDefinition.Style = Style;
        _gridDefinition.LocalizationKeyPrefix = LocalizationKeyPrefix;
    }
    
    private async Task InitializeGridAsync()
    {
        await GridRenderer.InitializeAsync(_gridContainer, _gridDefinition);
        await GridRenderer.UpdateDataAsync(Items);
    }
    
    private string GetThemeClass()
    {
        return Theme switch
        {
            GridTheme.Striped => "grid-striped",
            GridTheme.Bordered => "grid-bordered",
            GridTheme.Minimal => "grid-minimal",
            _ => "grid-default"
        };
    }
    
    private string GetDensityClass()
    {
        return Density switch
        {
            GridDensity.Compact => "grid-compact",
            GridDensity.Spacious => "grid-spacious",
            _ => "grid-comfortable"
        };
    }
    
    public async ValueTask DisposeAsync()
    {
        if (_initialized)
        {
            try
            {
                await GridRenderer.DisposeAsync();
            }
            catch
            {
                // Ignore disposal errors
            }
        }
    }
}
```

### Demo Data Schema (Defined Upfront)

**Lesson from Charts:** Chart demos redefined data schema 3 times mid-PR. Grid defines schema before implementation.

**File:** `demo/BlazorUI.Demo/Data/GridDemoData.cs`

```csharp
namespace BlazorUI.Demo.Data;

public class Order
{
    public int Id { get; set; }
    public string Customer { get; set; } = "";
    public OrderStatus Status { get; set; }
    public decimal Amount { get; set; }
    public DateTime OrderDate { get; set; }
    public string ShipTo { get; set; } = "";
}

public enum OrderStatus
{
    Pending,
    Processing,
    Shipped,
    Delivered,
    Cancelled
}

public static class GridDemoData
{
    public static List<Order> GenerateOrders(int count = 1000)
    {
        var statuses = new[] { OrderStatus.Pending, OrderStatus.Processing, OrderStatus.Shipped, OrderStatus.Delivered, OrderStatus.Cancelled };
        var customers = new[] { "Acme Corp", "TechStart Inc", "GlobalTrade LLC", "FastShip Co", "MegaMart" };
        var cities = new[] { "New York", "Los Angeles", "Chicago", "Houston", "Phoenix" };
        var random = new Random(42); // Fixed seed for reproducibility
        
        return Enumerable.Range(1, count)
            .Select(i => new Order
            {
                Id = i,
                Customer = customers[random.Next(customers.Length)],
                Status = statuses[random.Next(statuses.Length)],
                Amount = Math.Round((decimal)(random.NextDouble() * 10000), 2),
                OrderDate = DateTime.Now.AddDays(-random.Next(365)),
                ShipTo = cities[random.Next(cities.Length)]
            })
            .ToList();
    }
}
```

**Usage in Demos:**
- Small demos (grid-default, grid-cell-template): 100 rows
- Pagination demos: 1000 rows
- Performance test (grid-height with virtualization): 10,000 rows

### Phase 3: Renderer Abstraction (Week 2)

**IGridRenderer.cs:**
```csharp
namespace BlazorUI.Components.Services.Grid;

/// <summary>
/// Interface for grid rendering implementations.
/// Abstracts the underlying grid technology (AG Grid, native Blazor, etc.).
/// </summary>
public interface IGridRenderer : IAsyncDisposable
{
    Task InitializeAsync<TItem>(ElementReference element, GridDefinition<TItem> definition);
    Task UpdateDataAsync<TItem>(IEnumerable<TItem> data);
    Task UpdateStateAsync(GridState state);
    Task<GridState> GetStateAsync();
}
```

**IGridRendererCapabilities.cs:**
```csharp
namespace BlazorUI.Components.Services.Grid;

/// <summary>
/// Describes the capabilities of a grid renderer.
/// Used for diagnostics and feature detection.
/// </summary>
public interface IGridRendererCapabilities
{
    bool SupportsVirtualization { get; }
    bool SupportsColumnPinning { get; }
    bool SupportsColumnReordering { get; }
    bool SupportsColumnResizing { get; }
    bool SupportsInfiniteScroll { get; }
    bool SupportsServerSidePaging { get; }
    bool SupportsExport { get; }
    string[] UnsupportedFeatures { get; }
}
```

**Usage Example:**

```csharp
// In Grid.razor.cs
protected override async Task OnInitializedAsync()
{
    if (GridRenderer is IGridRendererCapabilities capabilities)
    {
        if (Columns.Any(c => c.Pinned != GridColumnPinPosition.None) && !capabilities.SupportsColumnPinning)
        {
            Logger.LogWarning("Current renderer ({RendererType}) does not support column pinning. Pinned columns will be ignored.",
                GridRenderer.GetType().Name);
        }
        
        if (VirtualizationMode != GridVirtualizationMode.None && !capabilities.SupportsVirtualization)
        {
            Logger.LogWarning("Current renderer ({RendererType}) does not support virtualization. Performance may degrade with large datasets.",
                GridRenderer.GetType().Name);
        }
        
        if (capabilities.UnsupportedFeatures.Any())
        {
            Logger.LogInformation("Renderer capabilities: {SupportedFeatures}. Unsupported: {UnsupportedFeatures}",
                GetSupportedFeaturesList(capabilities),
                string.Join(", ", capabilities.UnsupportedFeatures));
        }
    }
}
```

**IGridExportService.cs:**
```csharp
namespace BlazorUI.Components.Services.Grid;

/// <summary>
/// Service for exporting grid data to various formats.
/// </summary>
public interface IGridExportService
{
    Task<byte[]> ExportToCsvAsync<TItem>(GridDefinition<TItem> definition, IEnumerable<TItem> data);
    Task<byte[]> ExportToExcelAsync<TItem>(GridDefinition<TItem> definition, IEnumerable<TItem> data);
}
```

### Phase 4: AG Grid Renderer (Weeks 3-4)

**AgGridRenderer.cs:**
```csharp
namespace BlazorUI.Components.Services.Grid;

/// <summary>
/// AG Grid renderer implementation.
/// Wraps AG Grid Community Edition with Blazor interop.
/// </summary>
public class AgGridRenderer : IGridRenderer, IGridRendererCapabilities
{
    private readonly IJSRuntime _jsRuntime;
    private IJSObjectReference? _jsModule;
    private IJSObjectReference? _gridInstance;
    private DotNetObjectReference<AgGridRenderer>? _dotNetRef;
    
    public AgGridRenderer(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }
    
    public async Task InitializeAsync<TItem>(ElementReference element, GridDefinition<TItem> definition)
    {
        _jsModule = await _jsRuntime.InvokeAsync<IJSObjectReference>(
            "import", "./_content/BlazorUI.Components/js/aggrid-renderer.js");
        
        _dotNetRef = DotNetObjectReference.Create(this);
        
        var config = BuildAgGridConfig(definition);
        _gridInstance = await _jsModule.InvokeAsync<IJSObjectReference>(
            "createGrid", element, config, _dotNetRef);
    }
    
    public async Task UpdateDataAsync<TItem>(IEnumerable<TItem> data)
    {
        if (_gridInstance != null)
        {
            await _gridInstance.InvokeVoidAsync("setRowData", data);
        }
    }
    
    public async Task UpdateStateAsync(GridState state)
    {
        if (_gridInstance != null)
        {
            await _gridInstance.InvokeVoidAsync("applyState", state);
        }
    }
    
    public async Task<GridState> GetStateAsync()
    {
        if (_gridInstance != null)
        {
            return await _gridInstance.InvokeAsync<GridState>("getState");
        }
        return new GridState();
    }
    
    [JSInvokable]
    public async Task OnGridStateChanged(GridState state)
    {
        // Called from JS when grid state changes
        // Invoke OnStateChanged callback
    }
    
    [JSInvokable]
    public async Task<GridDataResponse<object>> OnDataRequested(GridDataRequest<object> request)
    {
        // Called from JS for server-side data loading
        // Invoke OnDataRequest callback
        return new GridDataResponse<object>();
    }
    
    private object BuildAgGridConfig<TItem>(GridDefinition<TItem> definition)
    {
        // Convert GridDefinition to AG Grid column defs and options
        var columnDefs = definition.Columns.Select(col => new
        {
            field = col.Field,
            headerName = col.Header,
            sortable = col.Sortable,
            filter = col.Filterable,
            width = ParseWidth(col.Width),
            minWidth = ParseWidth(col.MinWidth),
            maxWidth = ParseWidth(col.MaxWidth),
            pinned = col.Pinned == GridColumnPinPosition.Left ? "left" :
                     col.Pinned == GridColumnPinPosition.Right ? "right" : null,
            resizable = col.AllowResize,
            // Template handling - precompile or use cell renderer
            cellRenderer = col.CellTemplate != null ? "templateRenderer" : null,
            cellRendererParams = col.CellTemplate != null ? new { templateId = col.Id } : null
        }).ToArray();
        
        return new
        {
            columnDefs,
            rowSelection = definition.SelectionMode == GridSelectionMode.Multiple ? "multiple" :
                          definition.SelectionMode == GridSelectionMode.Single ? "single" : null,
            pagination = definition.PagingMode != GridPagingMode.None,
            paginationPageSize = definition.PageSize,
            rowModelType = definition.PagingMode == GridPagingMode.Server ? "serverSide" :
                          definition.PagingMode == GridPagingMode.InfiniteScroll ? "infinite" : "clientSide"
        };
    }
    
    private int? ParseWidth(string? width)
    {
        if (string.IsNullOrEmpty(width)) return null;
        if (width.EndsWith("px") && int.TryParse(width[..^2], out var px)) return px;
        return null;
    }
    
    // IGridRendererCapabilities
    public bool SupportsVirtualization => true;
    public bool SupportsColumnPinning => true;
    public bool SupportsColumnReordering => true;
    public bool SupportsColumnResizing => true;
    public bool SupportsInfiniteScroll => true;
    public bool SupportsServerSidePaging => true;
    public bool SupportsExport => true;
    public string[] UnsupportedFeatures => Array.Empty<string>();
    
    public async ValueTask DisposeAsync()
    {
        if (_gridInstance != null)
        {
            await _gridInstance.InvokeVoidAsync("destroy");
            await _gridInstance.DisposeAsync();
        }
        
        if (_jsModule != null)
        {
            await _jsModule.DisposeAsync();
        }
        
        _dotNetRef?.Dispose();
    }
}
```

**aggrid-renderer.js:**
```javascript
// AG Grid JavaScript interop module
import 'ag-grid-community';

export function createGrid(element, config, dotNetRef) {
    const gridOptions = {
        ...config,
        onGridReady: (params) => {
            console.log('AG Grid ready');
        },
        onSortChanged: async () => {
            const state = getState(gridOptions.api);
            await dotNetRef.invokeMethodAsync('OnGridStateChanged', state);
        },
        onFilterChanged: async () => {
            const state = getState(gridOptions.api);
            await dotNetRef.invokeMethodAsync('OnGridStateChanged', state);
        },
        onSelectionChanged: async () => {
            const state = getState(gridOptions.api);
            await dotNetRef.invokeMethodAsync('OnGridStateChanged', state);
        },
        // Server-side row model datasource
        serverSideDatasource: config.rowModelType === 'serverSide' ? {
            getRows: async (params) => {
                const request = buildDataRequest(params);
                const response = await dotNetRef.invokeMethodAsync('OnDataRequested', request);
                params.success({
                    rowData: response.items,
                    rowCount: response.totalCount
                });
            }
        } : undefined
    };
    
    const grid = agGrid.createGrid(element, gridOptions);
    
    return {
        setRowData: (data) => {
            gridOptions.api.setRowData(data);
        },
        applyState: (state) => {
            // Apply sort, filter, column state
            if (state.sortDescriptors) {
                const sortModel = state.sortDescriptors.map(s => ({
                    colId: s.field,
                    sort: s.direction === 'Ascending' ? 'asc' : 'desc'
                }));
                gridOptions.api.applyColumnState({ state: sortModel });
            }
        },
        getState: () => {
            return getState(gridOptions.api);
        },
        destroy: () => {
            grid.destroy();
        }
    };
}

function getState(api) {
    const sortModel = api.getColumnState().filter(c => c.sort);
    const filterModel = api.getFilterModel();
    
    return {
        sortDescriptors: sortModel.map(s => ({
            field: s.colId,
            direction: s.sort === 'asc' ? 'Ascending' : 'Descending',
            order: s.sortIndex || 0
        })),
        filterDescriptors: Object.keys(filterModel).map(field => ({
            field,
            operator: 'Contains',
            value: filterModel[field].filter
        })),
        // ... additional state
    };
}

function buildDataRequest(params) {
    return {
        startIndex: params.startRow,
        count: params.endRow - params.startRow,
        sortDescriptors: params.sortModel ? params.sortModel.map(s => ({
            field: s.colId,
            direction: s.sort === 'asc' ? 'Ascending' : 'Descending'
        })) : [],
        filterDescriptors: [], // Build from params.filterModel
        pageNumber: Math.floor(params.startRow / (params.endRow - params.startRow)) + 1,
        pageSize: params.endRow - params.startRow
    };
}
```

#### Task 4.4: Implement Template Precompilation

**Acceptance Criteria:**

- [ ] CellTemplate renders to HTML string using HtmlRenderer service
- [ ] Custom BlazorDomRenderer JavaScript class created (extends ICellRendererComp)
- [ ] Renderer receives HTML via cellRendererParams and creates DOM element
- [ ] DOM element created with `document.createElement('div')` and `innerHTML`
- [ ] getGui() returns DOM element (not string)
- [ ] refresh() method updates DOM element for data changes
- [ ] Template context includes full TItem data
- [ ] Handles null/empty templates gracefully
- [ ] Performance acceptable for 1000+ rows (pre-render on init)
- [ ] No interactive Blazor components (documented limitation for v1)

#### BlazorDomRenderer Implementation Example

```javascript
// wwwroot/js/blazor-grid-renderers.js
class BlazorDomRenderer {
  init(params) {
    this.eGui = document.createElement('div');
    if (params.html) {
      this.eGui.innerHTML = params.html; // Safe: HTML from Blazor is sanitized
    }
  }
  
  getGui() {
    return this.eGui; // Return DOM element, not string
  }
  
  refresh(params) {
    if (params.html) {
      this.eGui.innerHTML = params.html;
      return true; // Refreshed successfully
    }
    return false;
  }
  
  destroy() {
    this.eGui = null;
  }
}

// Register with AG Grid
gridOptions.components = {
  blazorDomRenderer: BlazorDomRenderer
};
```

### Phase 5: Export Service (Week 4)

**CsvExportService.cs:**
```csharp
namespace BlazorUI.Components.Services.Grid;

public class CsvExportService : IGridExportService
{
    public Task<byte[]> ExportToCsvAsync<TItem>(GridDefinition<TItem> definition, IEnumerable<TItem> data)
    {
        var sb = new StringBuilder();
        
        // Header row
        var headers = definition.Columns
            .Where(c => c.IsVisible)
            .Select(c => EscapeCsvValue(c.Header));
        sb.AppendLine(string.Join(",", headers));
        
        // Data rows
        foreach (var item in data)
        {
            var values = definition.Columns
                .Where(c => c.IsVisible)
                .Select(c =>
                {
                    var value = c.ValueSelector != null
                        ? c.ValueSelector(item)
                        : c.Field != null ? GetPropertyValue(item, c.Field) : null;
                    return EscapeCsvValue(value?.ToString());
                });
            sb.AppendLine(string.Join(",", values));
        }
        
        return Task.FromResult(Encoding.UTF8.GetBytes(sb.ToString()));
    }
    
    public Task<byte[]> ExportToExcelAsync<TItem>(GridDefinition<TItem> definition, IEnumerable<TItem> data)
    {
        throw new NotImplementedException("Excel export requires additional package.");
    }
    
    private string EscapeCsvValue(string? value)
    {
        if (string.IsNullOrEmpty(value)) return string.Empty;
        if (value.Contains(",") || value.Contains("\"") || value.Contains("\n"))
        {
            return $"\"{value.Replace("\"", "\"\"")}\"";
        }
        return value;
    }
    
    private object? GetPropertyValue(object obj, string propertyName)
    {
        return obj.GetType().GetProperty(propertyName)?.GetValue(obj);
    }
}
```

## 4. Testing Strategy

### Unit Tests
- GridState serialization/deserialization
- GridDataRequest/Response DTOs
- CSV export formatting
- Column definition building

### Component Tests (bUnit)
- Grid renders with columns
- Column registration
- Template rendering
- State binding

### Integration Tests
- AG Grid initialization
- State synchronization
- Data loading pipeline
- Export functionality

## 5. Implementation Timeline

**Week 1:** Enums, DTOs, object model
**Week 2:** Grid and GridColumn components, renderer interfaces
**Week 3:** AG Grid renderer, JS interop
**Week 4:** Export service, state persistence, testing
**Week 5:** Documentation, examples, polish

## 6. Dependencies and Configuration

### NuGet Packages
- System.Text.Json (included)
- (Optional) ClosedXML for Excel export

### JavaScript Libraries
- AG Grid Community (~200KB, MIT license)
- Loaded via CDN or bundled

### Service Registration
```csharp
builder.Services.AddScoped<IGridRenderer, AgGridRenderer>();
builder.Services.AddScoped<IGridExportService, CsvExportService>();
```

## 7. Migration from DataTable

Existing DataTable component will continue to work. Grid provides:
- More advanced features
- Renderer abstraction
- Better performance for large datasets

Migration path:
- `DataTable<T>` → `Grid<T>`
- `DataTableColumn<T, TValue>` → `GridColumn<T>`
- `DataTableSelectionMode` → `GridSelectionMode`
- Same parameter names where applicable

## 8. Success Criteria

- All acceptance criteria from spec.md met
- AG Grid renderer fully functional
- State persistence works
- Server-side paging works
- CSV export works
- No AG Grid types in public API
- Documentation complete

## Appendix A: AG Grid DTOs (Complete Reference)

### AGGridOption.cs

```csharp
using System.Text.Json.Serialization;

namespace BlazorUI.Components.Grid.Internal;

/// <summary>
/// Root AG Grid option - 1:1 match to AG Grid's GridOptions interface.
/// See: https://www.ag-grid.com/javascript-data-grid/grid-options/
/// </summary>
public sealed class AGGridOption
{
    [JsonPropertyName("columnDefs")]
    public List<AGColumnDef>? ColumnDefs { get; set; }
    
    [JsonPropertyName("rowData")]
    public object? RowData { get; set; }
    
    [JsonPropertyName("pagination")]
    public bool? Pagination { get; set; }
    
    [JsonPropertyName("paginationPageSize")]
    public int? PaginationPageSize { get; set; }
    
    [JsonPropertyName("rowSelection")]
    public string? RowSelection { get; set; } // "single" | "multiple"
    
    [JsonPropertyName("suppressRowClickSelection")]
    public bool? SuppressRowClickSelection { get; set; }
    
    [JsonPropertyName("domLayout")]
    public string? DomLayout { get; set; } // "normal" | "autoHeight" | "print"
    
    [JsonPropertyName("defaultColDef")]
    public AGColumnDef? DefaultColDef { get; set; }
    
    [JsonPropertyName("rowModelType")]
    public string? RowModelType { get; set; } // "clientSide" | "serverSide" | "infinite"
    
    [JsonPropertyName("theme")]
    public string? Theme { get; set; } // "ag-theme-quartz"
    
    [JsonPropertyName("components")]
    public Dictionary<string, object>? Components { get; set; }
    
    [JsonExtensionData]
    public Dictionary<string, JsonElement>? ExtensionData { get; set; }
}

public sealed class AGColumnDef
{
    [JsonPropertyName("field")]
    public string? Field { get; set; }
    
    [JsonPropertyName("headerName")]
    public string? HeaderName { get; set; }
    
    [JsonPropertyName("colId")]
    public string? ColId { get; set; }
    
    [JsonPropertyName("sortable")]
    public bool? Sortable { get; set; }
    
    [JsonPropertyName("resizable")]
    public bool? Resizable { get; set; }
    
    [JsonPropertyName("filter")]
    public object? Filter { get; set; } // bool | "agTextColumnFilter" | AGFilterConfig
    
    [JsonPropertyName("pinned")]
    public string? Pinned { get; set; } // "left" | "right" | null
    
    [JsonPropertyName("width")]
    public int? Width { get; set; }
    
    [JsonPropertyName("minWidth")]
    public int? MinWidth { get; set; }
    
    [JsonPropertyName("maxWidth")]
    public int? MaxWidth { get; set; }
    
    [JsonPropertyName("flex")]
    public int? Flex { get; set; }
    
    [JsonPropertyName("cellRenderer")]
    public string? CellRenderer { get; set; } // "blazorDomRenderer" for templates
    
    [JsonPropertyName("cellRendererParams")]
    public object? CellRendererParams { get; set; } // { html: "..." }
    
    [JsonPropertyName("hide")]
    public bool? Hide { get; set; }
    
    [JsonExtensionData]
    public Dictionary<string, JsonElement>? ExtensionData { get; set; }
}
```

**Key Design Decisions:**
- Uses `[JsonPropertyName]` with camelCase to match AG Grid JSON exactly
- `object?` type for polymorphic properties (filter, cellRendererParams)
- `[JsonExtensionData]` for future AG Grid options not yet mapped
- Sealed classes to prevent inheritance (DTOs should be data-only)

## Approval

This plan requires approval before task breakdown and implementation.

**Approved by:** _Pending_  
**Date:** _Pending_
