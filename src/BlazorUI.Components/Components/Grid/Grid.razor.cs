using BlazorUI.Components.Utilities;
using Microsoft.AspNetCore.Components;

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
    private bool _initialized = false;

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
    public GridPagingMode PagingMode { get; set; } = GridPagingMode.Client;

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
    /// Gets or sets the visual theme for the grid.
    /// </summary>
    [Parameter]
    public GridTheme Theme { get; set; } = GridTheme.Default;

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
    /// Gets or sets inline styles to apply to the grid container.
    /// </summary>
    [Parameter]
    public string? Style { get; set; }

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
        GetDensityClass()
    );

    protected override void OnInitialized()
    {
        // Validate PageSize
        if (PageSize <= 0)
        {
            throw new ArgumentException("PageSize must be greater than 0.", nameof(PageSize));
        }
        
        // BuildGridDefinition will be called in OnAfterRenderAsync after child columns have registered
    }

    protected override async Task OnParametersSetAsync()
    {
        // Update grid data when Items parameter changes
        if (_initialized)
        {
            // TODO: Integrate with IGridRenderer when available
            // await _gridRenderer.UpdateDataAsync(Items);
        }
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender && !_initialized)
        {
            BuildGridDefinition();
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
        // TODO: Integrate with IGridRenderer when available (Milestone 3)
        // await _gridRenderer.InitializeAsync(_gridContainer, _gridDefinition);
        // await _gridRenderer.UpdateDataAsync(Items);
        await Task.CompletedTask;
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
                // TODO: Integrate with IGridRenderer when available
                // await _gridRenderer.DisposeAsync();
            }
            catch
            {
                // Ignore disposal errors
            }
        }

        await Task.CompletedTask;
    }
}
