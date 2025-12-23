using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using BlazorUI.Components.Grid;

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

    /// <summary>
    /// Initializes a new instance of the <see cref="AgGridRenderer"/> class.
    /// </summary>
    /// <param name="jsRuntime">The JavaScript runtime for interop.</param>
    public AgGridRenderer(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    /// <inheritdoc/>
    public async Task InitializeAsync<TItem>(ElementReference element, GridDefinition<TItem> definition)
    {
        _jsModule = await _jsRuntime.InvokeAsync<IJSObjectReference>(
            "import", "./_content/BlazorUI.Components/js/aggrid-renderer.js");

        _dotNetRef = DotNetObjectReference.Create(this);

        var config = BuildAgGridConfig(definition);
        
        // TODO: Uncomment when AG Grid Community library is installed
        // _gridInstance = await _jsModule.InvokeAsync<IJSObjectReference>(
        //     "createGrid", element, config, _dotNetRef);
        
        await Task.CompletedTask;
    }

    /// <inheritdoc/>
    public async Task UpdateDataAsync<TItem>(IEnumerable<TItem> data)
    {
        if (_gridInstance != null)
        {
            await _gridInstance.InvokeVoidAsync("setRowData", data);
        }
    }

    /// <inheritdoc/>
    public async Task UpdateStateAsync(GridState state)
    {
        if (_gridInstance != null)
        {
            await _gridInstance.InvokeVoidAsync("applyState", state);
        }
    }

    /// <inheritdoc/>
    public async Task<GridState> GetStateAsync()
    {
        if (_gridInstance != null)
        {
            return await _gridInstance.InvokeAsync<GridState>("getState");
        }
        return new GridState();
    }

    /// <summary>
    /// Called by JavaScript when grid state changes (sorting, filtering, selection).
    /// </summary>
    /// <param name="state">The new grid state.</param>
    [JSInvokable]
    public async Task OnGridStateChanged(GridState state)
    {
        // Called from JS when grid state changes
        // TODO: Invoke OnStateChanged callback from GridDefinition
        await Task.CompletedTask;
    }

    /// <summary>
    /// Called by JavaScript when server-side data is requested.
    /// </summary>
    /// <param name="request">The data request parameters.</param>
    /// <returns>The data response with items and counts.</returns>
    [JSInvokable]
    public async Task<GridDataResponse<object>> OnDataRequested(GridDataRequest<object> request)
    {
        // Called from JS for server-side data loading
        // TODO: Invoke OnDataRequest callback from GridDefinition
        await Task.CompletedTask;
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

    // IGridRendererCapabilities implementation
    
    /// <inheritdoc/>
    public bool SupportsVirtualization => true;
    
    /// <inheritdoc/>
    public bool SupportsColumnPinning => true;
    
    /// <inheritdoc/>
    public bool SupportsColumnReordering => true;
    
    /// <inheritdoc/>
    public bool SupportsColumnResizing => true;
    
    /// <inheritdoc/>
    public bool SupportsInfiniteScroll => true;
    
    /// <inheritdoc/>
    public bool SupportsServerSidePaging => true;
    
    /// <inheritdoc/>
    public bool SupportsExport => true;
    
    /// <inheritdoc/>
    public string[] UnsupportedFeatures => Array.Empty<string>();

    /// <inheritdoc/>
    public async ValueTask DisposeAsync()
    {
        if (_gridInstance != null)
        {
            try
            {
                await _gridInstance.InvokeVoidAsync("destroy");
                await _gridInstance.DisposeAsync();
            }
            catch
            {
                // Ignore errors during disposal
            }
        }

        if (_jsModule != null)
        {
            try
            {
                await _jsModule.DisposeAsync();
            }
            catch
            {
                // Ignore errors during disposal
            }
        }

        _dotNetRef?.Dispose();
    }
}
