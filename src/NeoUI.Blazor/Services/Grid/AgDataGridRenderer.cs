using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Text.Json;

namespace NeoUI.Blazor.Services.Grid;

/// <summary>
/// AG DataGrid renderer implementation.
/// Wraps AG Grid Community Edition with Blazor interop.
/// AG DataGrid is automatically loaded from CDN when first used.
/// </summary>
/// <typeparam name="TItem">The type of items in the grid.</typeparam>
public class AgDataGridRenderer<TItem> : IDataGridRenderer<TItem>, IDataGridRendererCapabilities
{
    private readonly IJSRuntime _jsRuntime;
    private readonly IDataGridDataGridTemplateRenderer? _templateRenderer;
    private IJSObjectReference? _jsModule;
    private IJSObjectReference? _gridInstance;
    private DotNetObjectReference<AgDataGridRenderer<TItem>>? _dotNetRef;
    private DataGridDefinition<TItem>? _currentDefinition;
    private readonly Dictionary<string, RenderFragment<TItem>> _templates = new();
    private readonly Dictionary<string, RenderFragment> _headerTemplates = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="AgDataGridRenderer{TItem}"/> class.
    /// </summary>
    /// <param name="jsRuntime">The JavaScript runtime for interop.</param>
    /// <param name="templateRenderer">Optional template renderer for cell templates.</param>
    public AgDataGridRenderer(IJSRuntime jsRuntime, IDataGridDataGridTemplateRenderer? templateRenderer = null)
    {
        _jsRuntime = jsRuntime;
        _templateRenderer = templateRenderer;
    }

    /// <inheritdoc/>
    public async Task InitializeAsync(ElementReference element, DataGridDefinition<TItem> definition)
    {
        Console.WriteLine("[AgDataGridRenderer] InitializeAsync called");
        
        _jsModule = await _jsRuntime.InvokeAsync<IJSObjectReference>(
            "import", "./_content/NeoUI.Blazor/js/grid/aggrid-renderer.js");
        Console.WriteLine("[AgDataGridRenderer] JS module imported");

        _dotNetRef = DotNetObjectReference.Create(this);
        _currentDefinition = definition;

        // Store cell templates for later rendering
        _templates.Clear();
        _headerTemplates.Clear();
        Console.WriteLine($"[AgDataGridRenderer] Storing templates for {definition.Columns.Count} columns:");
        foreach (var column in definition.Columns)
        {
            if (column.CellTemplate != null)
            {
                _templates[column.Id] = column.CellTemplate;
                Console.WriteLine($"[AgDataGridRenderer]   - Stored CELL template for column ID: '{column.Id}'");
            }
            if (column.HeaderTemplate != null)
            {
                _headerTemplates[column.Id] = column.HeaderTemplate;
                Console.WriteLine($"[AgDataGridRenderer]   - Stored HEADER template for column ID: '{column.Id}'");
            }
        }
        Console.WriteLine($"[AgDataGridRenderer] Total templates stored: {_templates.Count} cell, {_headerTemplates.Count} header");

        var config = BuildAgGridConfig(definition);
        Console.WriteLine($"[AgDataGridRenderer] Config built with {definition.Columns.Count} columns");
        
        // Verify element is valid before passing to JS
        // The ElementReference should be serialized properly by Blazor's JS interop
        try
        {
            // AG DataGrid will be auto-loaded from CDN by the JavaScript module
            _gridInstance = await _jsModule.InvokeAsync<IJSObjectReference>(
                "createGrid", element, config, _dotNetRef);
            Console.WriteLine("[AgDataGridRenderer] DataGrid instance created successfully");
            
            // ✅ Apply initial state if provided
            if (definition.State != null)
            {
                Console.WriteLine("[AgDataGridRenderer] Applying initial state");
                await UpdateStateAsync(definition.State);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[AgDataGridRenderer] Failed to create grid: {ex.Message}");
            Console.WriteLine($"[AgDataGridRenderer] Stack trace: {ex.StackTrace}");
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task UpdateDataAsync(IEnumerable<TItem> data)
    {
        Console.WriteLine($"[AgDataGridRenderer] UpdateDataAsync called with {data?.Count() ?? 0} items");
        
        if (_gridInstance != null)
        {
            // Convert to list and enhance with formatted values
            var dataList = data?.ToList() ?? new List<TItem>();
            Console.WriteLine($"[AgDataGridRenderer] Setting row data: {dataList.Count} rows");
            
            // Enhance data with formatted properties based on column definitions
            var enhancedData = EnhanceDataWithFormatting(dataList);
            
            if (enhancedData.Any())
            {
                // Log first item to verify data structure
                var firstItem = enhancedData.First();
                // Console.WriteLine($"[AgDataGridRenderer] First item keys: {string.Join(", ", ((IDictionary<string, object?>)firstItem).Keys)}");
            }
            
            await _gridInstance.InvokeVoidAsync("setRowData", enhancedData);
            Console.WriteLine("[AgDataGridRenderer] Row data set successfully");
        }
        else
        {
            Console.WriteLine("[AgDataGridRenderer] DataGrid instance is null, cannot set data");
        }
    }
    
    /// <inheritdoc/>
    public async Task ApplyTransactionAsync(DataGridTransaction<TItem> transaction)
    {
        if (_gridInstance == null)
        {
            Console.WriteLine("[AgDataGridRenderer] DataGrid instance is null, cannot apply transaction");
            return;
        }
        
        if (!transaction.HasChanges)
        {
            Console.WriteLine("[AgDataGridRenderer] Transaction has no changes, skipping");
            return;
        }
        
        Console.WriteLine($"[AgDataGridRenderer] Applying transaction: +{transaction.Add?.Count ?? 0} -{transaction.Remove?.Count ?? 0} ~{transaction.Update?.Count ?? 0}");
        
        // Enhance data with formatting before sending to AG DataGrid
        var transactionData = new
        {
            add = transaction.Add != null ? EnhanceDataWithFormatting(transaction.Add) : null,
            remove = transaction.Remove != null ? EnhanceDataWithFormatting(transaction.Remove) : null,
            update = transaction.Update != null ? EnhanceDataWithFormatting(transaction.Update) : null
        };
        
        await _gridInstance.InvokeVoidAsync("applyTransaction", transactionData);
        Console.WriteLine("[AgDataGridRenderer] Transaction applied successfully");
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
                        Console.WriteLine($"[AgDataGridRenderer] Failed to format field '{column.Field}' with format '{column.DataFormatString}': {ex.Message}");
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
    public async Task UpdateStateAsync(DataGridState state)
    {
        if (_gridInstance != null)
        {
            await _gridInstance.InvokeVoidAsync("applyState", state);
        }
    }

    /// <inheritdoc/>
    public async Task<DataGridState> GetStateAsync()
    {
        if (_gridInstance != null)
        {
            return await _gridInstance.InvokeAsync<DataGridState>("getState");
        }
        return new DataGridState();
    }

    /// <inheritdoc/>
    public async Task UpdateThemeAsync(DataGridTheme theme, Dictionary<string, object>? themeParams)
    {
        if (_gridInstance == null)
        {
            Console.WriteLine("[AgDataGridRenderer] Cannot update theme - grid instance is null");
            return;
        }
        
        Console.WriteLine($"[AgDataGridRenderer] Updating theme to {theme}");
        
        var themeUpdate = new
        {
            theme = theme.ToString(),
            themeParams = themeParams ?? new Dictionary<string, object>()
        };
        
        await _gridInstance.InvokeVoidAsync("setGridOptions", themeUpdate);
        Console.WriteLine("[AgDataGridRenderer] Theme update completed");
    }

    /// <summary>
    /// Refreshes the server-side cache, causing AG DataGrid to re-fetch data with current filters/sorts.
    /// Only applicable for server-side row models.
    /// </summary>
    public async Task RefreshServerSideCacheAsync()
    {
        if (_gridInstance == null)
        {
            Console.WriteLine("[AgDataGridRenderer] Cannot refresh server-side cache - grid instance is null");
            return;
        }
        
        Console.WriteLine("[AgDataGridRenderer] Refreshing server-side cache");
        await _gridInstance.InvokeVoidAsync("refreshServerSideStore");
        Console.WriteLine("[AgDataGridRenderer] Server-side cache refresh completed");
    }

    /// <summary>
    /// Called by JavaScript when grid state changes (sorting, filtering, selection).
    /// </summary>
    /// <param name="state">The new grid state.</param>
    [JSInvokable]
    public async Task OnDataGridStateChanged(DataGridState state)
    {
        if (_currentDefinition?.OnStateChanged.HasDelegate == true)
        {
            await _currentDefinition.OnStateChanged.InvokeAsync(state);
        }
    }

    /// <summary>
    /// Called by JavaScript when server-side data is requested.
    /// This method handles server-side row model data requests from AG Grid.
    /// </summary>
    /// <param name="requestJson">The data request parameters as a JSON element.</param>
    /// <returns>The data response with items and row count.</returns>
    [JSInvokable("OnDataRequested")]
    public async Task<object> OnDataRequested(JsonElement requestJson)
    {
        if (_currentDefinition?.ServerDataRequestHandler == null)
        {
            Console.WriteLine("[AgDataGridRenderer] No ServerDataRequestHandler configured for server-side row model");
            // Return empty response
            return new
            {
                items = Array.Empty<object>(),
                totalCount = 0
            };
        }
        
        try
        {
            // Deserialize JS request to DataGridDataRequest
            var request = JsonSerializer.Deserialize<DataGridDataRequest<TItem>>(
                requestJson.GetRawText(),
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            );
            
            if (request == null)
            {
                throw new InvalidOperationException("Failed to deserialize DataGridDataRequest");
            }
            
            Console.WriteLine($"[AgDataGridRenderer] Server data request: StartIndex={request.StartIndex}, Count={request.Count}");
            
            // Call the developer's callback
            var response = await _currentDefinition.ServerDataRequestHandler(request);
            
            // Response should be DataGridDataResponse<TItem>
            return response;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[AgDataGridRenderer] Server data request failed: {ex.Message}");
            throw;
        }
    }

    /// <summary>
    /// Called by JavaScript when selection changes.
    /// ✅ CRITICAL: Resolves deserialized items back to original instances from Items collection.
    /// AG DataGrid sends JSON-deserialized objects (new instances), but we need to return the ORIGINAL
    /// instances from the data source so that developer code like `collection.Remove(item)` works.
    /// </summary>
    /// <param name="selectedItems">The selected items as JSON elements (deserialized - NEW instances).</param>
    [JSInvokable]
    public async Task OnSelectionChanged(List<JsonElement> selectedItems)
    {
        // Use case-insensitive deserialization to handle camelCase (JS) to PascalCase (C#) conversion
        var jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true  // Critical: Maps camelCase JSON to PascalCase C# properties
        };
        
        // Deserialize JSON elements to typed items (these are NEW instances, not originals!)
        var deserializedItems = selectedItems
            .Select(json => JsonSerializer.Deserialize<TItem>(json.GetRawText(), jsonOptions))
            .Where(item => item != null)
            .Cast<TItem>()
            .ToList();
        
        // ✅ CRITICAL FIX: Resolve deserialized items back to ORIGINAL instances from Items collection
        // This ensures that developer code like `collection.Remove(selectedItem)` works correctly
        // because the items in SelectedItems will be the same references as in the source collection
        var originalItems = ResolveToOriginalInstances(deserializedItems);
        
        var readOnlyItems = originalItems.AsReadOnly();
        
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
    /// Resolves deserialized items (new instances from JSON) back to the original instances
    /// from the Items collection using ID-based matching.
    /// This is CRITICAL for enabling natural C# collection operations like Remove(item).
    /// </summary>
    /// <param name="deserializedItems">Items deserialized from JSON (new instances).</param>
    /// <returns>Original instances from the Items collection.</returns>
    private List<TItem> ResolveToOriginalInstances(List<TItem> deserializedItems)
    {
        if (deserializedItems.Count == 0)
        {
            return new List<TItem>();
        }
        
        // Check if the DataGrid provided a callback to resolve items by IDs
        if (_currentDefinition?.ResolveItemsByIds == null)
        {
            Console.WriteLine("[AgDataGridRenderer] WARNING: ResolveItemsByIds callback not provided. " +
                            "Returning deserialized items (this may break .Remove() operations).");
            return deserializedItems;
        }
        
        // Get the IdField property name (defaults to "Id")
        var idField = _currentDefinition.IdField ?? "Id";
        var itemType = typeof(TItem);
        var idProperty = itemType.GetProperty(idField);
        
        if (idProperty == null)
        {
            Console.WriteLine($"[AgDataGridRenderer] WARNING: IdField '{idField}' not found on type '{itemType.Name}'. " +
                            "Returning deserialized items (this may break .Remove() operations).");
            return deserializedItems;
        }
        
        try
        {
            // Extract IDs from deserialized items
            var ids = deserializedItems
                .Select(item => idProperty.GetValue(item))
                .Where(id => id != null)
                .ToList();
            
            if (ids.Count == 0)
            {
                Console.WriteLine("[AgDataGridRenderer] WARNING: No valid IDs found in deserialized items");
                return deserializedItems;
            }
            
            // ✅ Use the callback to resolve IDs back to original instances
            var originalItems = _currentDefinition.ResolveItemsByIds(ids!).ToList();
            
            Console.WriteLine($"[AgDataGridRenderer] Resolved {originalItems.Count}/{deserializedItems.Count} items to original instances");
            return originalItems;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[AgDataGridRenderer] ERROR resolving items to original instances: {ex.Message}");
            Console.WriteLine($"[AgDataGridRenderer] Stack trace: {ex.StackTrace}");
            return deserializedItems;  // Fallback
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
            Console.WriteLine($"[AgDataGridRenderer] Template renderer is null for column '{templateId}'");
            return null; // Fall back to field-based rendering
        }

        // Check if template exists for this column
        if (!_templates.TryGetValue(templateId, out var template))
        {
            Console.WriteLine($"[AgDataGridRenderer] No template found for column '{templateId}'");
            Console.WriteLine($"[AgDataGridRenderer] Available template IDs: {string.Join(", ", _templates.Keys.Select(k => $"'{k}'"))}");
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
            Console.WriteLine($"[AgDataGridRenderer] Deserializing data for template '{templateId}': {rawJson}");
            
            // Deserialize JSON to typed item
            var item = JsonSerializer.Deserialize<TItem>(rawJson, jsonOptions);
            if (item == null)
            {
                Console.WriteLine($"[AgDataGridRenderer] Deserialization returned null for column '{templateId}'");
                return null;
            }
            
            Console.WriteLine($"[AgDataGridRenderer] Successfully deserialized item of type {item.GetType().Name}");
            
            // Render template to HTML string
            var html = await _templateRenderer.RenderToStringAsync(template, item);
            Console.WriteLine($"[AgDataGridRenderer] Template rendered successfully for column '{templateId}', HTML length: {html?.Length ?? 0}");
            return html;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[AgDataGridRenderer] Template rendering failed for column '{templateId}': {ex.Message}");
            Console.WriteLine($"[AgDataGridRenderer] Stack trace: {ex.StackTrace}");
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
            Console.WriteLine($"[AgDataGridRenderer] Header template rendering failed for column '{templateId}': {ex.Message}");
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
        Console.WriteLine($"[AgDataGridRenderer] HandleCellAction called: action='{action}'");
        
        if (_currentDefinition?.Metadata == null)
        {
            Console.WriteLine("[AgDataGridRenderer] No metadata available for action handling");
            return;
        }

        // Check if action handler is registered in metadata
        var actionKey = $"CellAction_{action}";
        if (!_currentDefinition.Metadata.TryGetValue(actionKey, out var handler))
        {
            Console.WriteLine($"[AgDataGridRenderer] No handler registered for action '{action}'");
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
                Console.WriteLine($"[AgDataGridRenderer] Failed to deserialize data for action '{action}'");
                return;
            }

            Console.WriteLine($"[AgDataGridRenderer] Invoking action handler for '{action}'");
            
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
                Console.WriteLine($"[AgDataGridRenderer] Handler for '{action}' is not a valid type");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[AgDataGridRenderer] Action handler failed for '{action}': {ex.Message}");
            Console.WriteLine($"[AgDataGridRenderer] Stack trace: {ex.StackTrace}");
        }
    }

    private object BuildAgGridConfig(DataGridDefinition<TItem> definition)
    {
        // Convert DataGridDefinition to AG DataGrid column defs and options
        var columnDefs = definition.Columns.Select(col => new
        {
            // For template-only columns (no Field), use colId instead of field
            // This allows AG DataGrid to render the column without binding to data
            colId = col.Id,
            // Only set field if it exists - template-only columns don't need a field
            field = !string.IsNullOrEmpty(col.Field) ? ToCamelCase(col.Field) : (string?)null,
            headerName = col.Header,
            sortable = col.Sortable && !string.IsNullOrEmpty(col.Field), // Can't sort without a field
            filter = col.Filterable && !string.IsNullOrEmpty(col.Field) ? col.AgGridFilterType ?? "agTextColumnFilter" : (string?)null,
            width = ParseWidth(col.Width),
            minWidth = ParseWidth(col.MinWidth),
            maxWidth = ParseWidth(col.MaxWidth),
            pinned = col.Pinned == DataDataGridColumnPinPosition.Left ? "left" :
                     col.Pinned == DataDataGridColumnPinPosition.Right ? "right" : null,
            resizable = col.AllowResize,
            editable = col.CellEditTemplate != null,
            // Template handling - will use HtmlRenderer when available
            cellRenderer = col.CellTemplate != null && _templateRenderer != null ? "templateRenderer" : null,
            cellRendererParams = col.CellTemplate != null && _templateRenderer != null ? new { templateId = col.Id } : null,
            // Header template handling
            headerComponent = col.HeaderTemplate != null && _templateRenderer != null ? "headerDataGridTemplateRenderer" : null,
            headerComponentParams = col.HeaderTemplate != null && _templateRenderer != null ? new { templateId = col.Id } : null,
            // Value formatting - use simple formatter that reads {field}_formatted property
            valueFormatter = !string.IsNullOrEmpty(col.DataFormatString) && col.CellTemplate == null ? "formattedValueFormatter" : null,
            // ValueSelector mapped to valueGetter
            valueGetter = col.ValueSelector != null ? "valueGetter" : null,
            // ✅ Suppress header menus for controlled filtering scenarios
            suppressMenu = definition.SuppressHeaderMenus,
            suppressHeaderMenuButton = definition.SuppressHeaderMenus,
            suppressHeaderFilterButton = definition.SuppressHeaderMenus,

            // ? Store field type info for debugging
            __fieldType = col.FieldType?.Name
        }).ToArray();

        Console.WriteLine($"[AgDataGridRenderer] Column defs built:");
        foreach (var col in columnDefs)
        {
            Console.WriteLine($"  - colId: '{col.colId}', field: '{col.field ?? "(none)"}', headerName: '{col.headerName}', filter: '{col.filter ?? "(none)"}', fieldType: '{col.__fieldType ?? "(none)"}'");
        }

        return new
        {
            columnDefs,
            rowSelection = definition.SelectionMode == DataGridSelectionMode.Multiple ? "multiple" :
                          definition.SelectionMode == DataGridSelectionMode.Single ? "single" : null,
            pagination = definition.PagingMode != DataGridPagingMode.None,
            paginationPageSize = definition.PageSize,
            // ✅ FIX: Use RowModelType property instead of PagingMode
            // RowModelType is explicitly set by DataGrid component for server-side data scenarios
            rowModelType = definition.RowModelType ?? "clientSide",
            // Enable row selection checkbox for multiple selection
            rowMultiSelectWithClick = definition.SelectionMode == DataGridSelectionMode.Multiple,
            suppressRowClickSelection = definition.SelectionMode == DataGridSelectionMode.Multiple,
            // ✅ AG DataGrid v32.2+: ID field for stable row identification
            // Used by getRowId function in JavaScript for selection persistence
            idField = ToCamelCase(definition.IdField),
            // Theme and theme parameters
            theme = definition.Theme.ToString(),
            themeParams = definition.ThemeParams
        };
    }

    /// <summary>
    /// Gets AG DataGrid filter parameters for better UX based on filter type.
    /// </summary>
    private object? GetFilterParams(string? filterType)
    {
        if (string.IsNullOrEmpty(filterType))
            return null;

        return filterType switch
        {
            "agNumberColumnFilter" => new
            {
                buttons = new[] { "reset", "apply" },
                closeOnApply = true
            },
            "agDateColumnFilter" => new
            {
                buttons = new[] { "reset", "apply" },
                closeOnApply = true,
                // Comparator will be set in JavaScript if needed
            },
            "agTextColumnFilter" => new
            {
                buttons = new[] { "reset", "apply" },
                closeOnApply = true
            },
            "agSetColumnFilter" => new
            {
                buttons = new[] { "reset", "apply" },
                closeOnApply = true
            },
            _ => null
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
        
        return null; // Let AG DataGrid handle percentages and other units via CSS
    }

    // IDataGridRendererCapabilities implementation
    
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
        ? new[] { "Cell templates (IDataGridDataGridTemplateRenderer not registered)" }
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
