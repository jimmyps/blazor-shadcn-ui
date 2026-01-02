using BlazorUI.Components.Services.Grid;
using BlazorUI.Components.Utilities;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;

namespace BlazorUI.Components.Grid;

/// <summary>
/// A renderer-agnostic data grid component with support for sorting, filtering,
/// pagination, virtualization, and state persistence.
/// </summary>
/// <typeparam name="TItem">The type of items in the grid.</typeparam>
public partial class Grid<TItem> : ComponentBase, IAsyncDisposable
{
    private ElementReference _gridContainer;
    private GridDefinition<TItem> _gridDefinition = new();
    private List<GridColumn<TItem>> _columns = new();
    private IGridRenderer<TItem>? _gridRenderer;
    private bool _initialized = false;
    private bool _columnsRegistered = false;

    [Inject]
    private IServiceProvider ServiceProvider { get; set; } = default!;

    /// <summary>
    /// Gets or sets the collection of items to display in the grid.
    /// </summary>
    [Parameter]
    public IEnumerable<TItem> Items { get; set; } = Array.Empty<TItem>();

    /// <summary>
    /// Gets or sets the selection mode for the grid.
    /// </summary>
    [Parameter]
    public GridSelectionMode SelectionMode { get; set; } = GridSelectionMode.None;

    /// <summary>
    /// Gets or sets the paging mode for the grid.
    /// </summary>
    [Parameter]
    public GridPagingMode PagingMode { get; set; } = GridPagingMode.None;

    /// <summary>
    /// Gets or sets the number of items per page.
    /// Must be greater than 0. Default is 25.
    /// </summary>
    [Parameter]
    public int PageSize { get; set; } = 25;

    /// <summary>
    /// Gets or sets the virtualization mode for the grid.
    /// </summary>
    [Parameter]
    public GridVirtualizationMode VirtualizationMode { get; set; } = GridVirtualizationMode.Auto;

    /// <summary>
    /// Gets or sets the AG Grid theme to use (Alpine, Balham, Material, Quartz).
    /// Default is Alpine.
    /// </summary>
    [Parameter]
    public GridTheme Theme { get; set; } = GridTheme.Alpine;

    /// <summary>
    /// Gets or sets the visual style modifiers for the grid (Default, Striped, Bordered, Minimal).
    /// These modifiers work with any AG Grid theme.
    /// </summary>
    [Parameter]
    public GridStyle VisualStyle { get; set; } = GridStyle.Default;

    /// <summary>
    /// Gets or sets the spacing density for the grid.
    /// </summary>
    [Parameter]
    public GridDensity Density { get; set; } = GridDensity.Comfortable;

    /// <summary>
    /// Gets or sets the initial state of the grid.
    /// </summary>
    [Parameter]
    public GridState? InitialState { get; set; }

    /// <summary>
    /// Gets or sets whether the grid is in a loading state.
    /// </summary>
    [Parameter]
    public bool IsLoading { get; set; }

    /// <summary>
    /// Gets or sets the child content containing GridColumn definitions.
    /// </summary>
    [Parameter]
    public RenderFragment? Columns { get; set; }

    /// <summary>
    /// Gets or sets the template to display while the grid is loading.
    /// </summary>
    [Parameter]
    public RenderFragment? LoadingTemplate { get; set; }

    /// <summary>
    /// Gets or sets additional CSS classes to apply to the grid container.
    /// </summary>
    [Parameter]
    public string? Class { get; set; }

    /// <summary>
    /// Gets or sets the height of the grid.
    /// Can be a fixed value (e.g., "500px") or a percentage (e.g., "100%").
    /// If not specified, defaults to "300px".
    /// </summary>
    [Parameter]
    public string? Height { get; set; }

    /// <summary>
    /// Gets or sets the width of the grid.
    /// Can be a fixed value (e.g., "800px") or a percentage (e.g., "100%").
    /// Defaults to "100%".
    /// </summary>
    [Parameter]
    public string? Width { get; set; }

    /// <summary>
    /// Gets or sets inline styles to apply to the grid container.
    /// </summary>
    [Parameter]
    public string? InlineStyle { get; set; }

    /// <summary>
    /// Gets or sets the localization key prefix for grid text resources.
    /// </summary>
    [Parameter]
    public string? LocalizationKeyPrefix { get; set; }

    /// <summary>
    /// Gets or sets the callback invoked when the grid state changes.
    /// </summary>
    [Parameter]
    public EventCallback<GridState> OnStateChanged { get; set; }

    /// <summary>
    /// Gets or sets the callback invoked when server-side data is requested.
    /// </summary>
    [Parameter]
    public EventCallback<GridDataRequest<TItem>> OnDataRequest { get; set; }

    /// <summary>
    /// Gets or sets the callback invoked when the selection changes.
    /// </summary>
    [Parameter]
    public EventCallback<IReadOnlyCollection<TItem>> OnSelectionChanged { get; set; }

    /// <summary>
    /// Gets or sets the selected items in the grid.
    /// </summary>
    [Parameter]
    public IReadOnlyCollection<TItem> SelectedItems { get; set; } = Array.Empty<TItem>();

    /// <summary>
    /// Gets or sets the callback invoked when the selected items change (for two-way binding).
    /// </summary>
    [Parameter]
    public EventCallback<IReadOnlyCollection<TItem>> SelectedItemsChanged { get; set; }

    private string ContainerCssClass => ClassNames.cn(
        "grid-container w-full",
        Class
    );

    private string GridCssClass => ClassNames.cn(
        "grid-content",
        GetThemeClass(),
        GetStyleClass(),
        GetDensityClass()
    );

    private string GetGridCssClass()
    {
        // Combine AG Grid theme with our custom style modifiers
        return ClassNames.cn(
            "grid-content",
            GetThemeClass(),  // AG Grid's base theme (Alpine, Balham, etc.)
            GetStyleClass(),  // Our style modifiers (Striped, Bordered, etc.)
            GetDensityClass() // Our density modifiers
        );
    }

    protected override void OnInitialized()
    {
        // Validate PageSize
        if (PageSize <= 0)
        {
            throw new ArgumentException("PageSize must be greater than 0.", nameof(PageSize));
        }
        
        // Resolve generic grid renderer
        _gridRenderer = ServiceProvider.GetRequiredService<IGridRenderer<TItem>>();
    }

    protected override async Task OnParametersSetAsync()
    {
        // Update grid data when Items parameter changes
        if (_initialized && _gridRenderer != null)
        {
            await _gridRenderer.UpdateDataAsync(Items);
        }
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            // First render: columns haven't registered yet
            // Force a second render to allow child GridColumn components to register
            if (!_columnsRegistered)
            {
                _columnsRegistered = true;
                StateHasChanged();
                return;
            }
        }
        
        // Second render (or later): columns are now registered
        if (!_initialized && _columnsRegistered && _gridRenderer != null)
        {
            if (_columns.Count == 0)
            {
                Console.WriteLine("[Grid] No columns registered - grid not initialized");
                return;
            }
            
            if (IsLoading)
            {
                Console.WriteLine("[Grid] Grid is loading - delaying initialization");
                return;
            }
            
            Console.WriteLine($"[Grid] Initializing with {_columns.Count} columns");
            BuildGridDefinition();
            
            // Small delay to ensure DOM element is fully ready
            await Task.Delay(100);
            
            try
            {
                await InitializeGridAsync();
                _initialized = true;
                Console.WriteLine("[Grid] Initialization complete");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Grid] Failed to initialize: {ex.Message}");
                Console.WriteLine($"[Grid] Stack trace: {ex.StackTrace}");
                throw;
            }
        }
    }

    internal void RegisterColumn(GridColumn<TItem> column)
    {
        _columns.Add(column);
        Console.WriteLine($"[Grid] Column registered: {column.Header} (Total: {_columns.Count})");
    }

    private void BuildGridDefinition()
    {
        _gridDefinition.Columns = _columns.Select(c => c.ToDefinition()).ToList();
        _gridDefinition.SelectionMode = SelectionMode;
        _gridDefinition.PagingMode = PagingMode;
        _gridDefinition.VirtualizationMode = VirtualizationMode;
        _gridDefinition.Theme = Theme;                      // AG Grid theme (Alpine, Balham, etc.)
        _gridDefinition.VisualStyle = VisualStyle;          // Visual modifiers (Striped, Bordered, etc.)
        _gridDefinition.Density = Density;
        _gridDefinition.PageSize = PageSize;
        _gridDefinition.InitialState = InitialState;
        _gridDefinition.OnStateChanged = OnStateChanged;
        _gridDefinition.OnDataRequest = OnDataRequest;
        _gridDefinition.OnSelectionChanged = OnSelectionChanged;
        _gridDefinition.Class = Class;
        _gridDefinition.InlineStyle = InlineStyle;          // CSS inline style
        _gridDefinition.LocalizationKeyPrefix = LocalizationKeyPrefix;
    }

    private async Task InitializeGridAsync()
    {
        if (_gridRenderer != null)
        {
            await _gridRenderer.InitializeAsync(_gridContainer, _gridDefinition);
            await _gridRenderer.UpdateDataAsync(Items);
        }
    }

    private string GetThemeClass()
    {
        return Theme switch
        {
            GridTheme.Alpine => "ag-theme-alpine",
            GridTheme.Balham => "ag-theme-balham",
            GridTheme.Material => "ag-theme-material",
            GridTheme.Quartz => "ag-theme-quartz",
            _ => "ag-theme-alpine"
        };
    }

    private string GetStyleClass()
    {
        return VisualStyle switch
        {
            GridStyle.Striped => "grid-striped",
            GridStyle.Bordered => "grid-bordered",
            GridStyle.Minimal => "grid-minimal",
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

    private string GetGridContainerStyle()
    {
        // Build inline styles for the grid container
        // AG Grid requires explicit height to render properly
        var styles = new List<string>();

        // Add height - default to 300px if not specified
        var height = !string.IsNullOrEmpty(Height) ? Height : "300px";
        styles.Add($"height: {height}");

        // Add width - default to 100% if not specified
        var width = !string.IsNullOrEmpty(Width) ? Width : "100%";
        styles.Add($"width: {width}");

        return string.Join("; ", styles);
    }

    public async ValueTask DisposeAsync()
    {
        if (_initialized && _gridRenderer != null)
        {
            try
            {
                await _gridRenderer.DisposeAsync();
            }
            catch
            {
                // Ignore disposal errors
            }
        }
    }
}
