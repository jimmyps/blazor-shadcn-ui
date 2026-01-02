using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using BlazorUI.Components.Grid;
using System.Text.Json;

namespace BlazorUI.Components.Services.Grid;

/// <summary>
/// AG Grid renderer implementation.
/// Wraps AG Grid Community Edition with Blazor interop.
/// AG Grid is automatically loaded from CDN when first used.
/// </summary>
/// <typeparam name="TItem">The type of items in the grid.</typeparam>
public class AgGridRenderer<TItem> : IGridRenderer<TItem>, IGridRendererCapabilities
{
    private readonly IJSRuntime _jsRuntime;
    private readonly ITemplateRenderer? _templateRenderer;
    private IJSObjectReference? _jsModule;
    private IJSObjectReference? _gridInstance;
    private DotNetObjectReference<AgGridRenderer<TItem>>? _dotNetRef;
    private GridDefinition<TItem>? _currentDefinition;
    private readonly Dictionary<string, RenderFragment<TItem>> _templates = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="AgGridRenderer{TItem}"/> class.
    /// </summary>
    /// <param name="jsRuntime">The JavaScript runtime for interop.</param>
    /// <param name="templateRenderer">Optional template renderer for cell templates.</param>
    public AgGridRenderer(IJSRuntime jsRuntime, ITemplateRenderer? templateRenderer = null)
    {
        _jsRuntime = jsRuntime;
        _templateRenderer = templateRenderer;
    }

    /// <inheritdoc/>
    public async Task InitializeAsync(ElementReference element, GridDefinition<TItem> definition)
    {
        Console.WriteLine("[AgGridRenderer] InitializeAsync called");
        
        _jsModule = await _jsRuntime.InvokeAsync<IJSObjectReference>(
            "import", "./_content/BlazorUI.Components/js/aggrid-renderer.js");
        Console.WriteLine("[AgGridRenderer] JS module imported");

        _dotNetRef = DotNetObjectReference.Create(this);
        _currentDefinition = definition;

        // Store templates for later rendering
        _templates.Clear();
        foreach (var column in definition.Columns)
        {
            if (column.CellTemplate != null)
            {
                _templates[column.Id] = column.CellTemplate;
            }
        }

        var config = BuildAgGridConfig(definition);
        Console.WriteLine($"[AgGridRenderer] Config built with {definition.Columns.Count} columns");
        
        // Verify element is valid before passing to JS
        // The ElementReference should be serialized properly by Blazor's JS interop
        try
        {
            // AG Grid will be auto-loaded from CDN by the JavaScript module
            _gridInstance = await _jsModule.InvokeAsync<IJSObjectReference>(
                "createGrid", element, config, _dotNetRef);
            Console.WriteLine("[AgGridRenderer] Grid instance created successfully");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[AgGridRenderer] Failed to create grid: {ex.Message}");
            Console.WriteLine($"[AgGridRenderer] Stack trace: {ex.StackTrace}");
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task UpdateDataAsync(IEnumerable<TItem> data)
    {
        Console.WriteLine($"[AgGridRenderer] UpdateDataAsync called with {data?.Count() ?? 0} items");
        
        if (_gridInstance != null)
        {
            // Convert to list to ensure it's serializable
            var dataList = data?.ToList() ?? new List<TItem>();
            Console.WriteLine($"[AgGridRenderer] Setting row data: {dataList.Count} rows");
            
            if (dataList.Any())
            {
                // Log first item to verify data structure
                var firstItem = dataList.First();
                Console.WriteLine($"[AgGridRenderer] First item type: {firstItem?.GetType().Name}");
                Console.WriteLine($"[AgGridRenderer] First item JSON: {System.Text.Json.JsonSerializer.Serialize(firstItem)}");
            }
            
            await _gridInstance.InvokeVoidAsync("setRowData", dataList);
            Console.WriteLine("[AgGridRenderer] Row data set successfully");
        }
        else
        {
            Console.WriteLine("[AgGridRenderer] Grid instance is null, cannot set data");
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
        if (_currentDefinition?.OnStateChanged.HasDelegate == true)
        {
            await _currentDefinition.OnStateChanged.InvokeAsync(state);
        }
    }

    /// <summary>
    /// Called by JavaScript when server-side data is requested.
    /// </summary>
    /// <param name="request">The data request parameters.</param>
    /// <returns>The data response with items and counts.</returns>
    [JSInvokable]
    public async Task<GridDataResponse<object>> OnDataRequested(GridDataRequest<TItem> request)
    {
        // TODO: Server-side data loading needs redesign. 
        // EventCallback<T> doesn't return values - need Func<> or state update pattern
        if (_currentDefinition?.OnDataRequest.HasDelegate == true)
        {
            await _currentDefinition.OnDataRequest.InvokeAsync(request);
        }
        
        // Return empty response - proper implementation needs API redesign
        return new GridDataResponse<object>
        {
            Items = Array.Empty<object>(),
            TotalCount = 0,
            FilteredCount = 0
        };
    }

    /// <summary>
    /// Called by JavaScript when selection changes.
    /// </summary>
    /// <param name="selectedItems">The selected items as JSON elements.</param>
    [JSInvokable]
    public async Task OnSelectionChanged(List<JsonElement> selectedItems)
    {
        if (_currentDefinition?.OnSelectionChanged.HasDelegate == true)
        {
            // Deserialize JSON elements to typed items
            var typedItems = selectedItems
                .Select(json => JsonSerializer.Deserialize<TItem>(json.GetRawText()))
                .Where(item => item != null)
                .Cast<TItem>()
                .ToList();
            
            await _currentDefinition.OnSelectionChanged.InvokeAsync(typedItems.AsReadOnly());
        }
    }

    /// <summary>
    /// Called by JavaScript to render a cell template.
    /// Returns the HTML string for the template.
    /// </summary>
    /// <param name="templateId">The column ID containing the template.</param>
    /// <param name="data">The row data object as JSON element.</param>
    /// <returns>HTML string representing the rendered template.</returns>
    [JSInvokable]
    public async Task<string?> RenderCellTemplate(string templateId, JsonElement data)
    {
        // Check if template renderer is available
        if (_templateRenderer == null)
        {
            return null; // Fall back to field-based rendering
        }

        // Check if template exists for this column
        if (!_templates.TryGetValue(templateId, out var template))
        {
            return null; // Fall back to field-based rendering
        }

        try
        {
            // Deserialize JSON to typed item
            var item = JsonSerializer.Deserialize<TItem>(data.GetRawText());
            if (item == null)
            {
                return null;
            }
            
            // Render template to HTML string
            var html = await _templateRenderer.RenderToStringAsync(template, item);
            return html;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[AgGridRenderer] Template rendering failed for column '{templateId}': {ex.Message}");
            return null; // Fall back to field-based rendering
        }
    }

    private object BuildAgGridConfig(GridDefinition<TItem> definition)
    {
        // Convert GridDefinition to AG Grid column defs and options
        var columnDefs = definition.Columns.Select(col => new
        {
            // Convert field name to camelCase to match JSON serialization
            // C# property "Id" becomes JSON "id"
            field = ToCamelCase(col.Field),
            headerName = col.Header,
            sortable = col.Sortable,
            filter = col.Filterable,
            width = ParseWidth(col.Width),
            minWidth = ParseWidth(col.MinWidth),
            maxWidth = ParseWidth(col.MaxWidth),
            pinned = col.Pinned == GridColumnPinPosition.Left ? "left" :
                     col.Pinned == GridColumnPinPosition.Right ? "right" : null,
            resizable = col.AllowResize,
            editable = col.CellEditTemplate != null,
            // Template handling - will use HtmlRenderer when available
            cellRenderer = col.CellTemplate != null && _templateRenderer != null ? "templateRenderer" : null,
            cellRendererParams = col.CellTemplate != null && _templateRenderer != null ? new { templateId = col.Id } : null,
            // ValueSelector mapped to valueGetter
            valueGetter = col.ValueSelector != null ? "valueGetter" : null
        }).ToArray();

        Console.WriteLine($"[AgGridRenderer] Column defs built:");
        foreach (var col in columnDefs)
        {
            Console.WriteLine($"  - field: '{col.field}', headerName: '{col.headerName}'");
        }

        return new
        {
            columnDefs,
            rowSelection = definition.SelectionMode == GridSelectionMode.Multiple ? "multiple" :
                          definition.SelectionMode == GridSelectionMode.Single ? "single" : null,
            pagination = definition.PagingMode != GridPagingMode.None,
            paginationPageSize = definition.PageSize,
            rowModelType = definition.PagingMode == GridPagingMode.Server ? "serverSide" :
                          definition.PagingMode == GridPagingMode.InfiniteScroll ? "infinite" : "clientSide",
            // Enable row selection checkbox for multiple selection
            rowMultiSelectWithClick = definition.SelectionMode == GridSelectionMode.Multiple,
            suppressRowClickSelection = definition.SelectionMode == GridSelectionMode.Multiple
        };
    }

    private string? ToCamelCase(string? str)
    {
        if (string.IsNullOrEmpty(str) || str.Length == 0)
            return str;

        // Convert first character to lowercase
        return char.ToLowerInvariant(str[0]) + str.Substring(1);
    }

    private int? ParseWidth(string? width)
    {
        if (string.IsNullOrEmpty(width)) return null;
        
        // Only parse pixel values for AG Grid
        // Percentage and other CSS units will be handled by AG Grid's column sizing
        if (width.EndsWith("px", StringComparison.OrdinalIgnoreCase) && 
            int.TryParse(width[..^2], out var px))
        {
            return px;
        }
        
        // If it's just a number, assume pixels
        if (int.TryParse(width, out var numericWidth))
        {
            return numericWidth;
        }
        
        return null; // Let AG Grid handle percentages and other units via CSS
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
    public string[] UnsupportedFeatures => _templateRenderer == null 
        ? new[] { "Cell templates (ITemplateRenderer not registered)" }
        : Array.Empty<string>();

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
        _templates.Clear();
    }
}
