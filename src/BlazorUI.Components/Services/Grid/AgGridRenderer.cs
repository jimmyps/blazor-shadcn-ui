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
    private readonly Dictionary<string, RenderFragment> _headerTemplates = new();

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
            "import", "./_content/BlazorUI.Components/js/grid/aggrid-renderer.js");
        Console.WriteLine("[AgGridRenderer] JS module imported");

        _dotNetRef = DotNetObjectReference.Create(this);
        _currentDefinition = definition;

        // Store cell templates for later rendering
        _templates.Clear();
        _headerTemplates.Clear();
        Console.WriteLine($"[AgGridRenderer] Storing templates for {definition.Columns.Count} columns:");
        foreach (var column in definition.Columns)
        {
            if (column.CellTemplate != null)
            {
                _templates[column.Id] = column.CellTemplate;
                Console.WriteLine($"[AgGridRenderer]   - Stored CELL template for column ID: '{column.Id}'");
            }
            if (column.HeaderTemplate != null)
            {
                _headerTemplates[column.Id] = column.HeaderTemplate;
                Console.WriteLine($"[AgGridRenderer]   - Stored HEADER template for column ID: '{column.Id}'");
            }
        }
        Console.WriteLine($"[AgGridRenderer] Total templates stored: {_templates.Count} cell, {_headerTemplates.Count} header");

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
            // Convert to list and enhance with formatted values
            var dataList = data?.ToList() ?? new List<TItem>();
            Console.WriteLine($"[AgGridRenderer] Setting row data: {dataList.Count} rows");
            
            // Enhance data with formatted properties based on column definitions
            var enhancedData = EnhanceDataWithFormatting(dataList);
            
            if (enhancedData.Any())
            {
                // Log first item to verify data structure
                var firstItem = enhancedData.First();
                // Console.WriteLine($"[AgGridRenderer] First item keys: {string.Join(", ", ((IDictionary<string, object?>)firstItem).Keys)}");
            }
            
            await _gridInstance.InvokeVoidAsync("setRowData", enhancedData);
            Console.WriteLine("[AgGridRenderer] Row data set successfully");
        }
        else
        {
            Console.WriteLine("[AgGridRenderer] Grid instance is null, cannot set data");
        }
    }
    
    /// <inheritdoc/>
    public async Task ApplyTransactionAsync(GridTransaction<TItem> transaction)
    {
        if (_gridInstance == null)
        {
            Console.WriteLine("[AgGridRenderer] Grid instance is null, cannot apply transaction");
            return;
        }
        
        if (!transaction.HasChanges)
        {
            Console.WriteLine("[AgGridRenderer] Transaction has no changes, skipping");
            return;
        }
        
        Console.WriteLine($"[AgGridRenderer] Applying transaction: +{transaction.Add?.Count ?? 0} -{transaction.Remove?.Count ?? 0} ~{transaction.Update?.Count ?? 0}");
        
        // Enhance data with formatting before sending to AG Grid
        var transactionData = new
        {
            add = transaction.Add != null ? EnhanceDataWithFormatting(transaction.Add) : null,
            remove = transaction.Remove != null ? EnhanceDataWithFormatting(transaction.Remove) : null,
            update = transaction.Update != null ? EnhanceDataWithFormatting(transaction.Update) : null
        };
        
        await _gridInstance.InvokeVoidAsync("applyTransaction", transactionData);
        Console.WriteLine("[AgGridRenderer] Transaction applied successfully");
    }

    /// <summary>
    /// Enhances data objects with formatted string properties for columns with DataFormatString.
    /// Adds {field}_formatted properties alongside original values.
    /// </summary>
    private List<object> EnhanceDataWithFormatting(List<TItem> data)
    {
        // If no columns have DataFormatString, return original data
        var columnsWithFormatting = _currentDefinition?.Columns
            .Where(c => !string.IsNullOrEmpty(c.DataFormatString) && !string.IsNullOrEmpty(c.Field))
            .ToList();
        
        if (columnsWithFormatting == null || !columnsWithFormatting.Any())
        {
            return data.Cast<object>().ToList();
        }

        var enhancedData = new List<object>();
        var itemType = typeof(TItem);
        var properties = itemType.GetProperties();
        
        foreach (var item in data)
        {
            var dict = new Dictionary<string, object?>();
            
            // Copy all properties from the original item
            foreach (var prop in properties)
            {
                var value = prop.GetValue(item);
                var fieldName = ToCamelCase(prop.Name);
                dict[fieldName] = value;
            }
            
            // Add formatted properties for columns with DataFormatString
            foreach (var column in columnsWithFormatting)
            {
                var fieldName = ToCamelCase(column.Field!);
                var formattedFieldName = $"{fieldName}_formatted";
                
                // Get the property by name
                var property = properties.FirstOrDefault(p => 
                    string.Equals(p.Name, column.Field, StringComparison.OrdinalIgnoreCase));
                
                if (property != null)
                {
                    try
                    {
                        var rawValue = property.GetValue(item);
                        if (rawValue != null)
                        {
                            // Use .NET formatting to create the formatted string
                            var formattedValue = FormatValue(rawValue, column.DataFormatString!);
                            dict[formattedFieldName] = formattedValue;
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"[AgGridRenderer] Failed to format field '{column.Field}' with format '{column.DataFormatString}': {ex.Message}");
                    }
                }
            }
            
            enhancedData.Add(dict);
        }
        
        return enhancedData;
    }

    /// <summary>
    /// Formats a value using .NET format strings.
    /// </summary>
    private string FormatValue(object value, string formatString)
    {
        // Remove composite format wrapper if present (e.g., "{0:C}" -> "C")
        var cleanFormat = formatString.Replace("{0:", "").TrimEnd('}');
        
        // Handle IFormattable types (numbers, dates, etc.)
        if (value is IFormattable formattable)
        {
            return formattable.ToString(cleanFormat, System.Globalization.CultureInfo.CurrentCulture);
        }
        
        // Fallback to string.Format for composite formats
        if (formatString.Contains("{0"))
        {
            return string.Format(formatString, value);
        }
        
        // Last resort: ToString
        return value.ToString() ?? string.Empty;
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

    /// <inheritdoc/>
    public async Task UpdateThemeAsync(GridTheme theme, Dictionary<string, object>? themeParams)
    {
        if (_gridInstance == null)
        {
            Console.WriteLine("[AgGridRenderer] Cannot update theme - grid instance is null");
            return;
        }
        
        Console.WriteLine($"[AgGridRenderer] Updating theme to {theme}");
        
        var themeUpdate = new
        {
            theme = theme.ToString(),
            themeParams = themeParams ?? new Dictionary<string, object>()
        };
        
        await _gridInstance.InvokeVoidAsync("setGridOptions", themeUpdate);
        Console.WriteLine("[AgGridRenderer] Theme update completed");
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
        // Deserialize JSON elements to typed items
        var typedItems = selectedItems
            .Select(json => JsonSerializer.Deserialize<TItem>(json.GetRawText()))
            .Where(item => item != null)
            .Cast<TItem>()
            .ToList();
        
        var readOnlyItems = typedItems.AsReadOnly();
        
        // Invoke both callbacks for proper two-way binding support
        if (_currentDefinition?.OnSelectionChanged.HasDelegate == true)
        {
            await _currentDefinition.OnSelectionChanged.InvokeAsync(readOnlyItems);
        }
        
        // CRITICAL: Also invoke SelectedItemsChanged for @bind-SelectedItems support
        if (_currentDefinition?.SelectedItemsChanged.HasDelegate == true)
        {
            await _currentDefinition.SelectedItemsChanged.InvokeAsync(readOnlyItems);
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
            Console.WriteLine($"[AgGridRenderer] Template renderer is null for column '{templateId}'");
            return null; // Fall back to field-based rendering
        }

        // Check if template exists for this column
        if (!_templates.TryGetValue(templateId, out var template))
        {
            Console.WriteLine($"[AgGridRenderer] No template found for column '{templateId}'");
            Console.WriteLine($"[AgGridRenderer] Available template IDs: {string.Join(", ", _templates.Keys.Select(k => $"'{k}'"))}");
            return null; // Fall back to field-based rendering
        }

        try
        {
            // Use camelCase naming policy to match Blazor's JS interop serialization
            var jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                PropertyNameCaseInsensitive = true
            };
            
            var rawJson = data.GetRawText();
            Console.WriteLine($"[AgGridRenderer] Deserializing data for template '{templateId}': {rawJson}");
            
            // Deserialize JSON to typed item
            var item = JsonSerializer.Deserialize<TItem>(rawJson, jsonOptions);
            if (item == null)
            {
                Console.WriteLine($"[AgGridRenderer] Deserialization returned null for column '{templateId}'");
                return null;
            }
            
            Console.WriteLine($"[AgGridRenderer] Successfully deserialized item of type {item.GetType().Name}");
            
            // Render template to HTML string
            var html = await _templateRenderer.RenderToStringAsync(template, item);
            Console.WriteLine($"[AgGridRenderer] Template rendered successfully for column '{templateId}', HTML length: {html?.Length ?? 0}");
            return html;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[AgGridRenderer] Template rendering failed for column '{templateId}': {ex.Message}");
            Console.WriteLine($"[AgGridRenderer] Stack trace: {ex.StackTrace}");
            return null; // Fall back to field-based rendering
        }
    }

    /// <summary>
    /// Called by JavaScript to render a header template.
    /// Returns the HTML string for the header template.
    /// </summary>
    /// <param name="templateId">The column ID containing the template.</param>
    /// <returns>HTML string representing the rendered header template.</returns>
    [JSInvokable]
    public async Task<string?> RenderHeaderTemplate(string templateId)
    {
        // Check if template renderer is available
        if (_templateRenderer == null)
        {
            return null; // Fall back to headerName
        }

        // Check if template exists for this column
        if (!_headerTemplates.TryGetValue(templateId, out var template))
        {
            return null; // Fall back to headerName
        }

        try
        {
            // Render header template to HTML string (no context needed)
            var html = await _templateRenderer.RenderToStringAsync(template);
            return html;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[AgGridRenderer] Header template rendering failed for column '{templateId}': {ex.Message}");
            return null; // Fall back to headerName
        }
    }

    /// <summary>
    /// Called by JavaScript when a cell action is triggered (via data-action attribute).
    /// </summary>
    /// <param name="action">The action name (method name to invoke).</param>
    /// <param name="data">The row data as JSON element.</param>
    [JSInvokable]
    public async Task HandleCellAction(string action, JsonElement data)
    {
        Console.WriteLine($"[AgGridRenderer] HandleCellAction called: action='{action}'");
        
        if (_currentDefinition?.Metadata == null)
        {
            Console.WriteLine("[AgGridRenderer] No metadata available for action handling");
            return;
        }

        // Check if action handler is registered in metadata
        var actionKey = $"CellAction_{action}";
        if (!_currentDefinition.Metadata.TryGetValue(actionKey, out var handler))
        {
            Console.WriteLine($"[AgGridRenderer] No handler registered for action '{action}'");
            return;
        }

        try
        {
            // Deserialize row data
            var jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                PropertyNameCaseInsensitive = true
            };
            
            var item = JsonSerializer.Deserialize<TItem>(data.GetRawText(), jsonOptions);
            if (item == null)
            {
                Console.WriteLine($"[AgGridRenderer] Failed to deserialize data for action '{action}'");
                return;
            }

            Console.WriteLine($"[AgGridRenderer] Invoking action handler for '{action}'");
            
            // Invoke the action handler
            if (handler is Func<TItem, Task> asyncFunc)
            {
                await asyncFunc(item);
            }
            else if (handler is Action<TItem> syncAction)
            {
                syncAction(item);
            }
            else
            {
                Console.WriteLine($"[AgGridRenderer] Handler for '{action}' is not a valid type");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[AgGridRenderer] Action handler failed for '{action}': {ex.Message}");
            Console.WriteLine($"[AgGridRenderer] Stack trace: {ex.StackTrace}");
        }
    }

    private object BuildAgGridConfig(GridDefinition<TItem> definition)
    {
        // Convert GridDefinition to AG Grid column defs and options
        var columnDefs = definition.Columns.Select(col => new
        {
            // For template-only columns (no Field), use colId instead of field
            // This allows AG Grid to render the column without binding to data
            colId = col.Id,
            // Only set field if it exists - template-only columns don't need a field
            field = !string.IsNullOrEmpty(col.Field) ? ToCamelCase(col.Field) : (string?)null,
            headerName = col.Header,
            sortable = col.Sortable && !string.IsNullOrEmpty(col.Field), // Can't sort without a field
            filter = col.Filterable && !string.IsNullOrEmpty(col.Field), // Can't filter without a field
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
            // Header template handling
            headerComponent = col.HeaderTemplate != null && _templateRenderer != null ? "headerTemplateRenderer" : null,
            headerComponentParams = col.HeaderTemplate != null && _templateRenderer != null ? new { templateId = col.Id } : null,
            // Value formatting - use simple formatter that reads {field}_formatted property
            valueFormatter = !string.IsNullOrEmpty(col.DataFormatString) && col.CellTemplate == null ? "formattedValueFormatter" : null,
            // ValueSelector mapped to valueGetter
            valueGetter = col.ValueSelector != null ? "valueGetter" : null
        }).ToArray();

        Console.WriteLine($"[AgGridRenderer] Column defs built:");
        foreach (var col in columnDefs)
        {
            Console.WriteLine($"  - colId: '{col.colId}', field: '{col.field ?? "(none)"}', headerName: '{col.headerName}', cellRenderer: '{col.cellRenderer ?? "(none)"}'");
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
            suppressRowClickSelection = definition.SelectionMode == GridSelectionMode.Multiple,
            // ✅ AG Grid v32.2+: ID field for stable row identification
            // Used by getRowId function in JavaScript for selection persistence
            idField = ToCamelCase(definition.IdField),
            // Theme and theme parameters
            theme = definition.Theme.ToString(),
            themeParams = definition.ThemeParams
        };
    }

    private string? ToCamelCase(string? str)
    {
        if (string.IsNullOrEmpty(str) || str.Length == 0)
            return str;

        // Don't modify if already camelCase or all lowercase
        if (char.IsLower(str[0]))
            return str;

        // Convert PascalCase to camelCase
        // Handle single character
        if (str.Length == 1)
            return char.ToLowerInvariant(str[0]).ToString();

        // Standard conversion: first char to lowercase
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
