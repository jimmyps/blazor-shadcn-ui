using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
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
    private static readonly Action<ILogger, string, Exception?> LogCreateGridFailed =
        LoggerMessage.Define<string>(LogLevel.Error, new EventId(1, nameof(LogCreateGridFailed)),
            "[AgDataGridRenderer] Failed to create grid: {Message}");

    private static readonly Action<ILogger, Exception?> LogGridInstanceNullData =
        LoggerMessage.Define(LogLevel.Warning, new EventId(2, nameof(LogGridInstanceNullData)),
            "[AgDataGridRenderer] DataGrid instance is null, cannot set data");

    private static readonly Action<ILogger, Exception?> LogGridInstanceNullTransaction =
        LoggerMessage.Define(LogLevel.Warning, new EventId(3, nameof(LogGridInstanceNullTransaction)),
            "[AgDataGridRenderer] DataGrid instance is null, cannot apply transaction");

    private static readonly Action<ILogger, string, string, Exception?> LogFormatFieldFailed =
        LoggerMessage.Define<string, string>(LogLevel.Warning, new EventId(4, nameof(LogFormatFieldFailed)),
            "[AgDataGridRenderer] Failed to format field '{Field}' with format '{Format}': {Message}");

    private static readonly Action<ILogger, Exception?> LogGridInstanceNullTheme =
        LoggerMessage.Define(LogLevel.Warning, new EventId(5, nameof(LogGridInstanceNullTheme)),
            "[AgDataGridRenderer] Cannot update theme - grid instance is null");

    private static readonly Action<ILogger, Exception?> LogGridInstanceNullCache =
        LoggerMessage.Define(LogLevel.Warning, new EventId(6, nameof(LogGridInstanceNullCache)),
            "[AgDataGridRenderer] Cannot refresh server-side cache - grid instance is null");

    private static readonly Action<ILogger, Exception?> LogGridInstanceNullFetch =
        LoggerMessage.Define(LogLevel.Warning, new EventId(7, nameof(LogGridInstanceNullFetch)),
            "[AgDataGridRenderer] Cannot trigger BlazorServerSide fetch - grid instance is null");

    private static readonly Action<ILogger, Exception?> LogGridInstanceNullPage =
        LoggerMessage.Define(LogLevel.Warning, new EventId(8, nameof(LogGridInstanceNullPage)),
            "[AgDataGridRenderer] Cannot set Blazor page - grid instance is null");

    private static readonly Action<ILogger, string, Exception?> LogBlazorServerSideFetchFailed =
        LoggerMessage.Define<string>(LogLevel.Error, new EventId(9, nameof(LogBlazorServerSideFetchFailed)),
            "[AgDataGridRenderer] BlazorServerSide fetch failed: {Message}");

    private static readonly Action<ILogger, Exception?> LogNoServerDataRequestHandler =
        LoggerMessage.Define(LogLevel.Warning, new EventId(10, nameof(LogNoServerDataRequestHandler)),
            "[AgDataGridRenderer] No ServerDataRequestHandler configured for server-side row model");

    private static readonly Action<ILogger, string, Exception?> LogServerDataRequestFailed =
        LoggerMessage.Define<string>(LogLevel.Error, new EventId(11, nameof(LogServerDataRequestFailed)),
            "[AgDataGridRenderer] Server data request failed: {Message}");

    private static readonly Action<ILogger, Exception?> LogResolveItemsByIdsNotProvided =
        LoggerMessage.Define(LogLevel.Warning, new EventId(12, nameof(LogResolveItemsByIdsNotProvided)),
            "[AgDataGridRenderer] WARNING: ResolveItemsByIds callback not provided. Returning deserialized items (this may break .Remove() operations).");

    private static readonly Action<ILogger, string, string, Exception?> LogIdFieldNotFound =
        LoggerMessage.Define<string, string>(LogLevel.Warning, new EventId(13, nameof(LogIdFieldNotFound)),
            "[AgDataGridRenderer] WARNING: IdField '{IdField}' not found on type '{TypeName}'. Returning deserialized items (this may break .Remove() operations).");

    private static readonly Action<ILogger, Exception?> LogNoValidIds =
        LoggerMessage.Define(LogLevel.Warning, new EventId(14, nameof(LogNoValidIds)),
            "[AgDataGridRenderer] WARNING: No valid IDs found in deserialized items");

    private static readonly Action<ILogger, string, Exception?> LogErrorResolvingItems =
        LoggerMessage.Define<string>(LogLevel.Error, new EventId(15, nameof(LogErrorResolvingItems)),
            "[AgDataGridRenderer] ERROR resolving items to original instances: {Message}");

    private static readonly Action<ILogger, string, Exception?> LogTemplateRendererNull =
        LoggerMessage.Define<string>(LogLevel.Warning, new EventId(16, nameof(LogTemplateRendererNull)),
            "[AgDataGridRenderer] Template renderer is null for column '{TemplateId}'");

    private static readonly Action<ILogger, string, Exception?> LogTemplateNotFound =
        LoggerMessage.Define<string>(LogLevel.Warning, new EventId(17, nameof(LogTemplateNotFound)),
            "[AgDataGridRenderer] No template found for column '{TemplateId}'");

    private static readonly Action<ILogger, string, Exception?> LogDeserializationReturnedNull =
        LoggerMessage.Define<string>(LogLevel.Warning, new EventId(18, nameof(LogDeserializationReturnedNull)),
            "[AgDataGridRenderer] Deserialization returned null for column '{TemplateId}'");

    private static readonly Action<ILogger, string, string, Exception?> LogTemplateRenderingFailed =
        LoggerMessage.Define<string, string>(LogLevel.Error, new EventId(19, nameof(LogTemplateRenderingFailed)),
            "[AgDataGridRenderer] Template rendering failed for column '{TemplateId}': {Message}");

    private static readonly Action<ILogger, string, string, Exception?> LogHeaderTemplateRenderingFailed =
        LoggerMessage.Define<string, string>(LogLevel.Error, new EventId(20, nameof(LogHeaderTemplateRenderingFailed)),
            "[AgDataGridRenderer] Header template rendering failed for column '{TemplateId}': {Message}");

    private static readonly Action<ILogger, Exception?> LogNoMetadataForAction =
        LoggerMessage.Define(LogLevel.Warning, new EventId(21, nameof(LogNoMetadataForAction)),
            "[AgDataGridRenderer] No metadata available for action handling");

    private static readonly Action<ILogger, string, Exception?> LogNoHandlerForAction =
        LoggerMessage.Define<string>(LogLevel.Warning, new EventId(22, nameof(LogNoHandlerForAction)),
            "[AgDataGridRenderer] No handler registered for action '{Action}'");

    private static readonly Action<ILogger, string, Exception?> LogFailedToDeserializeAction =
        LoggerMessage.Define<string>(LogLevel.Warning, new EventId(23, nameof(LogFailedToDeserializeAction)),
            "[AgDataGridRenderer] Failed to deserialize data for action '{Action}'");

    private static readonly Action<ILogger, string, Exception?> LogHandlerInvalidType =
        LoggerMessage.Define<string>(LogLevel.Warning, new EventId(24, nameof(LogHandlerInvalidType)),
            "[AgDataGridRenderer] Handler for '{Action}' is not a valid type");

    private static readonly Action<ILogger, string, string, Exception?> LogActionHandlerFailed =
        LoggerMessage.Define<string, string>(LogLevel.Error, new EventId(25, nameof(LogActionHandlerFailed)),
            "[AgDataGridRenderer] Action handler failed for '{Action}': {Message}");

    private readonly IJSRuntime _jsRuntime;
    private readonly ILogger<AgDataGridRenderer<TItem>> _logger;
    private readonly IDataGridDataGridTemplateRenderer? _templateRenderer;
    private IJSObjectReference? _jsModule;
    private IJSObjectReference? _gridInstance;
    private DotNetObjectReference<AgDataGridRenderer<TItem>>? _dotNetRef;
    private DataGridDefinition<TItem>? _currentDefinition;
    private Action? _onGridReady;
    private readonly Dictionary<string, RenderFragment<TItem>> _templates = new();
    private readonly Dictionary<string, RenderFragment> _headerTemplates = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="AgDataGridRenderer{TItem}"/> class.
    /// </summary>
    /// <param name="jsRuntime">The JavaScript runtime for interop.</param>
    /// <param name="templateRenderer">Optional template renderer for cell templates.</param>
    /// <param name="logger">The logger instance.</param>
    public AgDataGridRenderer(IJSRuntime jsRuntime, IDataGridDataGridTemplateRenderer? templateRenderer = null, ILogger<AgDataGridRenderer<TItem>>? logger = null)
    {
        _jsRuntime = jsRuntime;
        _templateRenderer = templateRenderer;
        _logger = logger ?? Microsoft.Extensions.Logging.Abstractions.NullLogger<AgDataGridRenderer<TItem>>.Instance;
    }

    /// <inheritdoc/>
    public async Task InitializeAsync(ElementReference element, DataGridDefinition<TItem> definition)
    {
        _jsModule = await _jsRuntime.InvokeAsync<IJSObjectReference>(
            "import", "./_content/NeoUI.Blazor/js/grid/aggrid-renderer.js");

        _dotNetRef = DotNetObjectReference.Create(this);
        _currentDefinition = definition;
        _onGridReady = definition.OnGridReady;

        // Store cell templates for later rendering
        _templates.Clear();
        _headerTemplates.Clear();
        foreach (var column in definition.Columns)
        {
            if (column.CellTemplate != null)
            {
                _templates[column.Id] = column.CellTemplate;
            }
            if (column.HeaderTemplate != null)
            {
                _headerTemplates[column.Id] = column.HeaderTemplate;
            }
        }

        var config = BuildAgGridConfig(definition);

        // Verify element is valid before passing to JS
        // The ElementReference should be serialized properly by Blazor's JS interop
        try
        {
            // AG DataGrid will be auto-loaded from CDN by the JavaScript module
            _gridInstance = await _jsModule.InvokeAsync<IJSObjectReference>(
                "createGrid", element, config, _dotNetRef);

            // ✅ Apply initial state if provided
            if (definition.State != null)
            {
                await UpdateStateAsync(definition.State);
            }
        }
        catch (Exception ex)
        {
            LogCreateGridFailed(_logger, ex.Message, ex);
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task UpdateDataAsync(IEnumerable<TItem> data)
    {
        if (_gridInstance != null)
        {
            var dataList = data?.ToList() ?? new List<TItem>();
            var enhancedData = EnhanceDataWithFormatting(dataList);
            await _gridInstance.InvokeVoidAsync("setRowData", enhancedData);
        }
        else
        {
            LogGridInstanceNullData(_logger, null);
        }
    }
    
    /// <inheritdoc/>
    public async Task ApplyTransactionAsync(DataGridTransaction<TItem> transaction)
    {
        if (_gridInstance == null)
        {
            LogGridInstanceNullTransaction(_logger, null);
            return;
        }

        if (!transaction.HasChanges)
        {
            return;
        }

        var transactionData = new
        {
            add = transaction.Add != null ? EnhanceDataWithFormatting(transaction.Add) : null,
            remove = transaction.Remove != null ? EnhanceDataWithFormatting(transaction.Remove) : null,
            update = transaction.Update != null ? EnhanceDataWithFormatting(transaction.Update) : null
        };

        await _gridInstance.InvokeVoidAsync("applyTransaction", transactionData);
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
                        LogFormatFieldFailed(_logger, column.Field!, column.DataFormatString!, ex);
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
            LogGridInstanceNullTheme(_logger, null);
            return;
        }

        var themeUpdate = new
        {
            theme = theme.ToString(),
            themeParams = themeParams ?? new Dictionary<string, object>()
        };

        await _gridInstance.InvokeVoidAsync("setGridOptions", themeUpdate);
    }

    /// <summary>
    /// Refreshes the server-side cache, causing AG DataGrid to re-fetch data with current filters/sorts.
    /// Only applicable for server-side row models.
    /// </summary>
    public async Task RefreshServerSideCacheAsync()
    {
        if (_gridInstance == null)
        {
            LogGridInstanceNullCache(_logger, null);
            return;
        }

        await _gridInstance.InvokeVoidAsync("refreshServerSideStore");
    }

    /// <summary>
    /// Triggers a BlazorServerSide data fetch by notifying JavaScript.
    /// Only applicable for BlazorServerSide row model.
    /// </summary>
    public async Task TriggerBlazorServerSideFetchAsync()
    {
        if (_gridInstance == null)
        {
            LogGridInstanceNullFetch(_logger, null);
            return;
        }

        await _gridInstance.InvokeVoidAsync("triggerBlazorServerSideFetch");
    }

    /// <inheritdoc/>
    public async Task SetBlazorPageAsync(int page, int pageSize)
    {
        if (_gridInstance == null)
        {
            LogGridInstanceNullPage(_logger, null);
            return;
        }

        await _gridInstance.InvokeVoidAsync("setBlazorPage", page, pageSize);
    }

    /// <inheritdoc/>
    public async Task AutoSizeColumnsAsync(bool skipHeader = false)
    {
        if (_gridInstance == null)
            return;

        await _gridInstance.InvokeVoidAsync("autoSizeColumns", skipHeader);
    }

    /// <summary>
    /// Called by JavaScript when the grid has been fully initialized and rendered.
    /// Triggers the OnGridReady callback on the DataGrid component.
    /// </summary>
    [JSInvokable]
    public Task OnGridReadyInternal()
    {
        _onGridReady?.Invoke();
        return Task.CompletedTask;
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
    /// Called by JavaScript for BlazorServerSide mode.
    /// Single round trip: receives grid state, fetches data, returns serialisable response.
    /// JavaScript applies setGridOption('rowData', items) locally — no second interop call needed.
    /// </summary>
    /// <param name="state">The current grid state from JavaScript.</param>
    /// <returns>An object with items, totalCount, pageNumber, and pageSize.</returns>
    [JSInvokable("OnStateChangedAndFetchData")]
    public async Task<object?> OnStateChangedAndFetchData(DataGridState state)
    {
        // 1. Notify state observers (same as existing OnDataGridStateChanged)
        if (_currentDefinition?.OnStateChanged.HasDelegate == true)
            await _currentDefinition.OnStateChanged.InvokeAsync(state);

        // 2. Fetch data
        if (_currentDefinition?.BlazorServerSideFetchHandler == null)
            return null;

        try
        {
            var response = await _currentDefinition.BlazorServerSideFetchHandler(state);
            var enhancedItems = EnhanceDataWithFormatting(response.Items?.ToList() ?? []);
            var selectedIds = _currentDefinition.GetSelectedIdsForRestore?.Invoke() ?? Array.Empty<string>();

            return new
            {
                items      = enhancedItems,
                totalCount = response.TotalCount,
                pageNumber = state.PageNumber,
                pageSize   = state.PageSize,
                selectedRowIds = selectedIds
            };
        }
        catch (Exception ex)
        {
            LogBlazorServerSideFetchFailed(_logger, ex.Message, ex);
            return new { items = Array.Empty<object>(), totalCount = 0, pageNumber = state.PageNumber, pageSize = state.PageSize };
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
            LogNoServerDataRequestHandler(_logger, null);
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

            // Call the developer's callback
            var response = await _currentDefinition.ServerDataRequestHandler(request);

            // Response should be DataGridDataResponse<TItem>
            return response;
        }
        catch (Exception ex)
        {
            LogServerDataRequestFailed(_logger, ex.Message, ex);
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
            LogResolveItemsByIdsNotProvided(_logger, null);
            return deserializedItems;
        }

        // Get the IdField property name (defaults to "Id")
        var idField = _currentDefinition.IdField ?? "Id";
        var itemType = typeof(TItem);
        var idProperty = itemType.GetProperty(idField);

        if (idProperty == null)
        {
            LogIdFieldNotFound(_logger, idField, itemType.Name, null);
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
                LogNoValidIds(_logger, null);
                return deserializedItems;
            }

            // ✅ Use the callback to resolve IDs back to original instances
            var originalItems = _currentDefinition.ResolveItemsByIds(ids!).ToList();

            // Fallback: in BlazorServerSide mode Items is empty, so resolution returns 0 items.
            // Deserialized instances are structurally equivalent (records support value equality).
            if (originalItems.Count == 0 && deserializedItems.Count > 0)
            {
                return deserializedItems;
            }

            return originalItems;
        }
        catch (Exception ex)
        {
            LogErrorResolvingItems(_logger, ex.Message, ex);
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
            LogTemplateRendererNull(_logger, templateId, null);
            return null; // Fall back to field-based rendering
        }

        // Check if template exists for this column
        if (!_templates.TryGetValue(templateId, out var template))
        {
            LogTemplateNotFound(_logger, templateId, null);
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

            // Deserialize JSON to typed item
            var item = JsonSerializer.Deserialize<TItem>(rawJson, jsonOptions);
            if (item == null)
            {
                LogDeserializationReturnedNull(_logger, templateId, null);
                return null;
            }

            // Render template to HTML string
            var html = await _templateRenderer.RenderToStringAsync(template, item);
            return html;
        }
        catch (Exception ex)
        {
            LogTemplateRenderingFailed(_logger, templateId, ex.Message, ex);
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
            LogHeaderTemplateRenderingFailed(_logger, templateId, ex.Message, ex);
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
        if (_currentDefinition?.Metadata == null)
        {
            LogNoMetadataForAction(_logger, null);
            return;
        }

        // Check if action handler is registered in metadata
        var actionKey = $"CellAction_{action}";
        if (!_currentDefinition.Metadata.TryGetValue(actionKey, out var handler))
        {
            LogNoHandlerForAction(_logger, action, null);
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
                LogFailedToDeserializeAction(_logger, action, null);
                return;
            }

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
                LogHandlerInvalidType(_logger, action, null);
            }
        }
        catch (Exception ex)
        {
            LogActionHandlerFailed(_logger, action, ex.Message, ex);
        }
    }

    private object BuildAgGridConfig(DataGridDefinition<TItem> definition)
    {
        // Convert DataGridDefinition to AG DataGrid column defs and options
        var columnDefs = definition.Columns.Select(col =>
        {
            // Flex priority:
            //   1. Explicit Flex on column  → use that value, clear width
            //   2. FillWidth=true + no Width → auto flex: 1 to fill available space
            //   3. Otherwise                 → no flex, use width as-is
            var flexValue = col.Flex.HasValue       ? col.Flex.Value
                          : definition.FillWidth && string.IsNullOrEmpty(col.Width) ? 1
                          : (int?)null;

            // Width is ignored when flex is active (AG Grid behaviour)
            var widthValue = flexValue.HasValue ? (int?)null : ParseWidth(col.Width);

            return new
            {
                // For template-only columns (no Field), use colId instead of field
                // This allows AG DataGrid to render the column without binding to data
                colId = col.Id,
                // Only set field if it exists - template-only columns don't need a field
                field = !string.IsNullOrEmpty(col.Field) ? ToCamelCase(col.Field) : (string?)null,
                headerName = col.Header,
                sortable = col.Sortable && !string.IsNullOrEmpty(col.Field), // Can't sort without a field
                filter = col.Filterable && !string.IsNullOrEmpty(col.Field) ? col.AgGridFilterType ?? "agTextColumnFilter" : (string?)null,
                flex = flexValue,
                width = widthValue,
                minWidth = ParseWidth(col.MinWidth),
                maxWidth = ParseWidth(col.MaxWidth),
                pinned = col.Pinned == DataGridColumnPinPosition.Left ? "left" :
                         col.Pinned == DataGridColumnPinPosition.Right ? "right" : null,
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
            };
        }).ToArray();

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
            themeParams = definition.ThemeParams,
            // BlazorServerSide flag: uses clientSide row model with C# orchestration
            blazorServerSide = definition.RowModelType == "clientSide" && definition.BlazorServerSideFetchHandler != null,
            // Auto-size columns to content after each data load
            autoSizeColumns = definition.AutoSizeColumns
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
