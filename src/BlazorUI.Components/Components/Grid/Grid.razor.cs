using BlazorUI.Components.Services.Grid;
using BlazorUI.Components.Utilities;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using System.Collections.Specialized;

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
    private bool _actionsRegistered = false;
    private GridThemeParameters? _themeParameters;
    private IReadOnlyCollection<TItem> _previousSelectedItems = Array.Empty<TItem>();
    private bool _isUpdatingSelectionFromGrid = false;
    
    // Theme tracking for runtime updates
    private GridTheme _previousTheme;
    private GridDensity _previousDensity;
    private GridStyle _previousVisualStyle;
    
    // State tracking for mutation detection
    private int _previousStateHash;
    
    // Observable Collection support
    private IEnumerable<TItem> _currentItems = Array.Empty<TItem>();
    private IDisposable? _collectionSubscription;
    private readonly List<NotifyCollectionChangedEventArgs> _pendingChanges = new();
    private CancellationTokenSource? _batchCts;
    private int _previousItemsHash = 0;
    private Dictionary<object, TItem>? _previousItemsById; // Track items by ID for delta detection

    [Inject]
    private IServiceProvider ServiceProvider { get; set; } = default!;

    /// <summary>
    /// Gets or sets the host component that contains grid action methods.
    /// Set this to 'this' to enable auto-discovery of methods marked with [GridAction].
    /// </summary>
    [Parameter]
    public object? ActionHost { get; set; }

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
    /// Gets or sets the property name to use as the unique row identifier.
    /// This is critical for row selection persistence across data updates and pagination.
    /// Examples: "Id" (C# convention), "ProductId", "OrderId", "id" (JavaScript convention), "_id" (MongoDB).
    /// Default is "Id".
    /// </summary>
    [Parameter]
    public string IdField { get; set; } = "Id";

    /// <summary>
    /// Gets or sets the virtualization mode for the grid.
    /// </summary>
    [Parameter]
    public GridVirtualizationMode VirtualizationMode { get; set; } = GridVirtualizationMode.Auto;

    /// <summary>
    /// Gets or sets the AG Grid theme to use (Shadcn, Alpine, Balham, Material, Quartz).
    /// Default is Shadcn.
    /// </summary>
    [Parameter]
    public GridTheme Theme { get; set; } = GridTheme.Shadcn;

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
    /// Gets or sets whether to suppress the header menus (filter/column menu).
    /// When true, columns will not show the menu icon even if filterable/sortable.
    /// This is useful for controlled filtering scenarios where you provide external filter UI.
    /// Default is false.
    /// </summary>
    [Parameter]
    public bool SuppressHeaderMenus { get; set; } = false;

    /// <summary>
    /// Gets or sets the current state of the grid.
    /// Supports two-way binding via @bind-State for automatic state synchronization.
    /// </summary>
    [Parameter]
    public GridState? State { get; set; }
    
    /// <summary>
    /// Gets or sets the callback invoked when the grid state changes.
    /// Used for two-way binding support (@bind-State).
    /// </summary>
    [Parameter]
    public EventCallback<GridState> StateChanged { get; set; }

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
    /// Gets or sets the row model type for the grid.
    /// Default is ClientSide. Use ServerSide for server-side data fetching with sorting/filtering/pagination.
    /// </summary>
    [Parameter]
    public GridRowModelType RowModelType { get; set; } = GridRowModelType.ClientSide;

    /// <summary>
    /// Gets or sets the callback invoked when server-side data is requested.
    /// Required when RowModelType is ServerSide or Infinite.
    /// This callback receives a GridDataRequest and should return a GridDataResponse.
    /// </summary>
    [Parameter]
    public Func<GridDataRequest<TItem>, Task<GridDataResponse<TItem>>>? OnServerDataRequest { get; set; }
    
    /// <summary>
    /// Gets or sets the callback invoked when server-side data is requested (legacy EventCallback version).
    /// For new code, use OnServerDataRequest (Func) instead.
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
        "grid-container w-full h-full",
        Class
    );

    private string GetGridCssClass()
    {
        // Combine AG Grid theme with our custom style modifiers
        return ClassNames.cn(
            "grid-content"
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
        if (!_initialized || _gridRenderer == null)
            return;

        // Detect theme changes and apply them without grid recreation
        bool themeChanged = _previousTheme != Theme;
        bool densityChanged = _previousDensity != Density;
        bool styleChanged = _previousVisualStyle != VisualStyle;

        if (themeChanged || densityChanged || styleChanged)
        {
            _previousTheme = Theme;
            _previousDensity = Density;
            _previousVisualStyle = VisualStyle;
            
            var mergedParams = GetMergedThemeParams();
            await _gridRenderer.UpdateThemeAsync(Theme, mergedParams);
        }
        
        // ✅ Detect State mutations using hash-based change detection
        // This enables controlled sort/filter scenarios with natural object mutations
        if (State != null)
        {
            var currentHash = ComputeStateHash(State);
            if (currentHash != _previousStateHash)
            {
                _previousStateHash = currentHash;
                await _gridRenderer.UpdateStateAsync(State);
            }
        }

        // Check if Items collection instance changed (reference comparison)
        bool collectionReplaced = !ReferenceEquals(_currentItems, Items);
        bool isObservable = Items is INotifyCollectionChanged;

        if (collectionReplaced)
        {
            // Unsubscribe from old collection
            UnsubscribeFromCollection();
            
            // Subscribe to new collection if it's observable
            _currentItems = Items;
            SubscribeToCollection();
            
            // Full refresh on collection replacement
            _previousItemsHash = Items?.Count() ?? 0;
            await _gridRenderer.UpdateDataAsync(Items);
        }
        else if (!isObservable)
        {
            // Collection reference is the same, but check count as fallback
            // (for non-observable collections like List<T> without INotifyCollectionChanged)
            int currentCount = Items?.Count() ?? 0;
            bool countChanged = currentCount != _previousItemsHash;
            
            if (countChanged)
            {
                _previousItemsHash = currentCount;
                await _gridRenderer.UpdateDataAsync(Items);
            }
        }
        
        // Sync selection state from parent to grid when SelectedItems changes
        // This is a UI STATE change, NOT a data change - no UpdateDataAsync needed!
        if (SelectionMode != GridSelectionMode.None && 
            SelectedItems != null && 
            !SelectedItems.SequenceEqual(_previousSelectedItems) &&
            !_isUpdatingSelectionFromGrid)
        {
            _previousSelectedItems = SelectedItems;
            await SyncSelectionToGrid();
        }
        else if (_isUpdatingSelectionFromGrid)
        {
            // Just update the tracking reference without syncing
            _previousSelectedItems = SelectedItems;
        }
    }
    
    /// <summary>
    /// Synchronizes the SelectedItems from the parent component to the grid's internal selection state.
    /// This ensures that programmatic changes to SelectedItems are reflected in the grid UI.
    /// </summary>
    private async Task SyncSelectionToGrid()
    {
        if (_gridRenderer == null || SelectedItems == null)
            return;
            
        try
        {
            // Build a GridState with the current selection
            var state = new GridState
            {
                SelectedRowIds = SelectedItems
                    .Select(item =>
                    {
                        // Extract the ID using reflection based on IdField
                        // Use case-insensitive lookup to handle both PascalCase (C#) and camelCase (JSON)
                        var idProperty = typeof(TItem).GetProperty(
                            IdField, 
                            BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase
                        );
                        
                        if (idProperty == null)
                        {
                            Console.WriteLine($"[Grid] WARNING: IdField '{IdField}' not found on type {typeof(TItem).Name}");
                            return (object)string.Empty;
                        }
                        
                        var idValue = idProperty.GetValue(item);
                        return (object)(idValue?.ToString() ?? string.Empty);
                    })
                    .Where(id => !string.IsNullOrEmpty((string)id))
                    .ToList()
            };
            
            // Apply the selection state to the grid
            await _gridRenderer.UpdateStateAsync(state);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Grid] ERROR: Failed to sync selection - {ex.Message}");
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
                Console.WriteLine("[Grid] ERROR: No columns registered");
                return;
            }
            
            if (IsLoading)
            {
                return;
            }
            
            BuildGridDefinition();
            
            // Auto-discover and register grid actions AFTER BuildGridDefinition
            // This ensures _gridDefinition.Metadata is initialized
            if (!_actionsRegistered && ActionHost != null)
            {
                AutoRegisterActions();
                _actionsRegistered = true;
            }
            
            // Small delay to ensure DOM element is fully ready
            await Task.Delay(100);
            
            try
            {
                await InitializeGridAsync();
                _initialized = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Grid] ERROR: Initialization failed - {ex.Message}");
                throw;
            }
        }
    }

    internal void RegisterColumn(GridColumn<TItem> column)
    {
        _columns.Add(column);
    }

    internal void RegisterThemeParameters(GridThemeParameters parameters)
    {
        _themeParameters = parameters;
    }
    
    /// <summary>
    /// Registers a cell action handler that can be invoked from templates via data-action attributes.
    /// </summary>
    /// <param name="actionName">The name of the action (used in data-action attribute).</param>
    /// <param name="handler">The handler to invoke when the action is triggered.</param>
    public void RegisterCellAction(string actionName, Action<TItem> handler)
    {
        var actionKey = $"CellAction_{actionName}";
        _gridDefinition.Metadata ??= new Dictionary<string, object?>();
        _gridDefinition.Metadata[actionKey] = handler;
    }
    
    /// <summary>
    /// Registers an async cell action handler that can be invoked from templates via data-action attributes.
    /// </summary>
    /// <param name="actionName">The name of the action (used in data-action attribute).</param>
    /// <param name="handler">The async handler to invoke when the action is triggered.</param>
    public void RegisterCellAction(string actionName, Func<TItem, Task> handler)
    {
        var actionKey = $"CellAction_{actionName}";
        _gridDefinition.Metadata ??= new Dictionary<string, object?>();
        _gridDefinition.Metadata[actionKey] = handler;
    }
    
    /// <summary>
    /// Auto-discovers and registers methods marked with [GridAction] attribute from the ActionHost.
    /// </summary>
    private void AutoRegisterActions()
    {
        if (ActionHost == null)
        {
            return;
        }
        
        var hostType = ActionHost.GetType();
        
        // Find all methods with [GridAction] attribute
        var methods = hostType.GetMethods(BindingFlags.Instance | 
                                         BindingFlags.Public | 
                                         BindingFlags.NonPublic)
            .Where(m => m.GetCustomAttribute<Attributes.GridActionAttribute>() != null);
        
        foreach (var method in methods)
        {
            var attr = method.GetCustomAttribute<Attributes.GridActionAttribute>()!;
            var actionName = attr.Name ?? method.Name;
            
            // Validate method signature
            var parameters = method.GetParameters();
            if (parameters.Length != 1 || !parameters[0].ParameterType.IsAssignableFrom(typeof(TItem)))
            {
                continue;
            }
            
            try
            {
                // Create delegate based on return type
                if (method.ReturnType == typeof(Task))
                {
                    var handler = (Func<TItem, Task>)Delegate.CreateDelegate(typeof(Func<TItem, Task>), ActionHost, method);
                    RegisterCellAction(actionName, handler);
                }
                else if (method.ReturnType == typeof(void))
                {
                    var handler = (Action<TItem>)Delegate.CreateDelegate(typeof(Action<TItem>), ActionHost, method);
                    RegisterCellAction(actionName, handler);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Grid] WARNING: Failed to register action '{actionName}' - {ex.Message}");
            }
        }
    }

    private Dictionary<string, object> GetThemeDefaults(GridTheme theme)
    {
        if (theme == GridTheme.Shadcn)
        {
            return new Dictionary<string, object>
            {
                { "accentColor", "var(--primary)" },
                { "backgroundColor", "var(--background)" },
                { "foregroundColor", "var(--foreground)" },
                { "borderColor", "var(--border)" },
                { "headerBackgroundColor", "var(--muted)" },
                { "headerForegroundColor", "var(--foreground)" },
                { "rowHoverColor", "color-mix(in srgb, var(--accent) 50%, transparent)" },
                { "selectedRowBackgroundColor", "color-mix(in srgb, var(--accent) 70%, transparent)" },
                { "invalidColor", "var(--destructive)" },
                { "fontFamily", "var(--font-sans)" },
                { "borderRadius", 4 },
                { "tooltipBackgroundColor", "var(--popover)" },
                { "tooltipTextColor", "var(--popover-foreground)" },
            };
        }
        return new Dictionary<string, object>();
    }

    private Dictionary<string, object> GetDensityPreset(GridDensity density)
    {
        return density switch
        {
            GridDensity.Compact => new Dictionary<string, object>
            {
                { "spacing", 4 },
                { "rowHeight", 28 },
                { "headerHeight", 32 },
                { "fontSize", 12 },
                { "iconSize", 14 },
                { "inputHeight", 28 },
            },
            GridDensity.Spacious => new Dictionary<string, object>
            {
                { "spacing", 12 },
                { "rowHeight", 56 },
                { "headerHeight", 64 },
                { "fontSize", 16 },
                { "iconSize", 20 },
                { "inputHeight", 40 },
            },
            GridDensity.Comfortable or _ => new Dictionary<string, object>
            {
                { "spacing", 8 },
                { "rowHeight", 42 },
                { "headerHeight", 48 },
                { "fontSize", 14 },
                { "iconSize", 16 },
                { "inputHeight", 32 },
            },
        };
    }

    private Dictionary<string, object> GetVisualStylePreset(GridStyle style)
    {
        return style switch
        {
            GridStyle.Striped => new Dictionary<string, object>
            {
                { "wrapperBorder", true },
                { "oddRowBackgroundColor", "color-mix(in srgb, var(--muted) 30%, transparent)" },
            },
            GridStyle.Bordered => new Dictionary<string, object>
            {
                { "wrapperBorder", true },
            },
            GridStyle.Minimal => new Dictionary<string, object>
            {
                { "wrapperBorder", false },
                { "borderWidth", 0 },
            },
            GridStyle.Default or _ => new Dictionary<string, object>
            {
                { "wrapperBorder", true },
            },
        };
    }

    private Dictionary<string, object> GetMergedThemeParams()
    {
        // Merge theme parameters with precedence: ThemeDefaults < Density < VisualStyle < GridThemeParameters
        var themeParams = GetThemeDefaults(Theme);
        
        // Apply density preset
        foreach (var kvp in GetDensityPreset(Density))
        {
            themeParams[kvp.Key] = kvp.Value;
        }
        
        // Apply visual style preset
        foreach (var kvp in GetVisualStylePreset(VisualStyle))
        {
            themeParams[kvp.Key] = kvp.Value;
        }
        
        // Apply user's GridThemeParameters (highest priority)
        if (_themeParameters != null)
        {
            foreach (var kvp in _themeParameters.ToDictionary())
            {
                themeParams[kvp.Key] = kvp.Value;
            }
        }
        
        return themeParams;
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
        _gridDefinition.IdField = IdField;                  // Row ID field for selection persistence
        _gridDefinition.State = State;
        _gridDefinition.SuppressHeaderMenus = SuppressHeaderMenus; // Hide filter/menu UI
        _gridDefinition.OnStateChanged = OnStateChanged;
        _gridDefinition.OnDataRequest = OnDataRequest;
        _gridDefinition.OnSelectionChanged = OnSelectionChanged;
        
        // Wrap SelectedItemsChanged to track when selection is coming from grid
        _gridDefinition.SelectedItemsChanged = EventCallback.Factory.Create<IReadOnlyCollection<TItem>>(
            this, 
            async (items) =>
            {
                // Set flag to indicate this selection change originated from the grid
                _isUpdatingSelectionFromGrid = true;
                try
                {
                    if (SelectedItemsChanged.HasDelegate)
                    {
                        await SelectedItemsChanged.InvokeAsync(items);
                    }
                }
                finally
                {
                    // Clear flag after a delay to ensure OnParametersSetAsync completes
                    await Task.Delay(10);
                    _isUpdatingSelectionFromGrid = false;
                }
            }
        );
        
        _gridDefinition.Class = Class;
        _gridDefinition.InlineStyle = InlineStyle;          // CSS inline style
        _gridDefinition.LocalizationKeyPrefix = LocalizationKeyPrefix;

        // Use the helper method to merge theme parameters
        _gridDefinition.ThemeParams = GetMergedThemeParams();
        
        // ✅ Server-side row model configuration
        if (RowModelType == GridRowModelType.ServerSide)
        {
            _gridDefinition.RowModelType = "serverSide";
            _gridDefinition.ServerDataRequestHandler = OnServerDataRequest;
        }
        else if (RowModelType == GridRowModelType.Infinite)
        {
            _gridDefinition.RowModelType = "infinite";
            _gridDefinition.ServerDataRequestHandler = OnServerDataRequest;
        }
        else
        {
            _gridDefinition.RowModelType = "clientSide";
        }
        
        // ✅ Provide a callback for the renderer to resolve IDs back to original instances
        _gridDefinition.ResolveItemsByIds = (ids) => ResolveItemsByIds(ids);
    }
    
    /// <summary>
    /// Resolves a collection of IDs to their corresponding original item instances.
    /// Used by the renderer to convert deserialized items back to original references.
    /// </summary>
    /// <param name="ids">The IDs to resolve.</param>
    /// <returns>Original item instances matching the provided IDs.</returns>
    private IEnumerable<TItem> ResolveItemsByIds(IEnumerable<object> ids)
    {
        if (!ids.Any())
        {
            return Enumerable.Empty<TItem>();
        }
        
        // Get the ID property
        var idProperty = typeof(TItem).GetProperty(
            IdField,
            System.Reflection.BindingFlags.Public | 
            System.Reflection.BindingFlags.Instance | 
            System.Reflection.BindingFlags.IgnoreCase
        );
        
        if (idProperty == null)
        {
            Console.WriteLine($"[Grid] WARNING: IdField '{IdField}' not found on type {typeof(TItem).Name}");
            return Enumerable.Empty<TItem>();
        }
        
        // Build lookup dictionary of items by ID
        var itemsById = Items
            .Select(item => new
            {
                Id = idProperty.GetValue(item),
                Item = item
            })
            .Where(x => x.Id != null)
            .ToDictionary(x => x.Id!, x => x.Item);
        
        // Resolve IDs to original items
        var resolvedItems = new List<TItem>();
        foreach (var id in ids)
        {
            if (itemsById.TryGetValue(id, out var item))
            {
                resolvedItems.Add(item);
            }
            else
            {
                Console.WriteLine($"[Grid] WARNING: Could not resolve ID '{id}' to an item");
            }
        }
        
        return resolvedItems;
    }

    private async Task InitializeGridAsync()
    {
        if (_gridRenderer != null)
        {
            await _gridRenderer.InitializeAsync(_gridContainer, _gridDefinition);
            
            // Initialize theme tracking
            _previousTheme = Theme;
            _previousDensity = Density;
            _previousVisualStyle = VisualStyle;
            
            // Set initial data, track count, and subscribe if observable
            _currentItems = Items;
            _previousItemsHash = Items?.Count() ?? 0;
            SubscribeToCollection();
            
            await _gridRenderer.UpdateDataAsync(Items);
        }
    }
    
    private void SubscribeToCollection()
    {
        if (_currentItems is INotifyCollectionChanged observable)
        {
            observable.CollectionChanged += HandleCollectionChanged;
            _collectionSubscription = new CollectionChangedSubscription(observable, HandleCollectionChanged);
        }
        
        // ✅ NEW: Subscribe to ItemsChanged for TrackedObservableCollection
        if (_currentItems is TrackedObservableCollection<TItem> tracked)
        {
            tracked.ItemsChanged += HandleItemsChanged;
        }
    }

    private void UnsubscribeFromCollection()
    {
        _collectionSubscription?.Dispose();
        _collectionSubscription = null;
        
        // Unsubscribe from TrackedObservableCollection
        if (_currentItems is TrackedObservableCollection<TItem> tracked)
        {
            tracked.ItemsChanged -= HandleItemsChanged;
        }
        
        // Cancel any pending batch operations
        _batchCts?.Cancel();
        _batchCts = null;
        _pendingChanges.Clear();
    }

    private void HandleCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        // Synchronous collection of changes - no await here!
        _pendingChanges.Add(e);
        
        // Special case: Reset means full refresh - do it immediately
        if (e.Action == NotifyCollectionChangedAction.Reset)
        {
            _batchCts?.Cancel();
            _pendingChanges.Clear();
            _ = InvokeAsync(async () =>
            {
                _previousItemsHash = Items?.Count() ?? 0;
                await _gridRenderer!.UpdateDataAsync(Items);
                StateHasChanged();
            });
            return;
        }
        
        // Debounce: batch multiple rapid changes together
        _batchCts?.Cancel();
        _batchCts = new CancellationTokenSource();
        var token = _batchCts.Token;
        
        // 100ms batching window
        _ = Task.Delay(100, token).ContinueWith(async _ =>
        {
            if (!token.IsCancellationRequested)
            {
                await InvokeAsync(async () =>
                {
                    await ApplyBatchedChangesAsync();
                    StateHasChanged();
                });
            }
        }, TaskScheduler.Default);
    }
    
    /// <summary>
    /// Handles ItemsChanged events from TrackedObservableCollection.
    /// Applies update transactions to AG Grid for modified items.
    /// </summary>
    private async void HandleItemsChanged(object? sender, ItemsChangedEventArgs<TItem> e)
    {
        if (_gridRenderer == null || !_initialized)
            return;
        
        if (e.ChangedItems == null || e.ChangedItems.Count == 0)
            return;
        
        // Apply update transaction to AG Grid
        await InvokeAsync(async () =>
        {
            await _gridRenderer.ApplyTransactionAsync(new GridTransaction<TItem>
            {
                Update = e.ChangedItems.ToList()
            });
            StateHasChanged();
        });
    }

    private async Task ApplyBatchedChangesAsync()
    {
        var changes = _pendingChanges.ToList();
        _pendingChanges.Clear();
        
        if (changes.Count == 0)
            return;
        
        // Aggregate all changes into a single transaction
        var adds = new List<TItem>();
        var removes = new List<TItem>();
        var updates = new List<TItem>();
        
        foreach (var change in changes)
        {
            switch (change.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    adds.AddRange(change.NewItems?.Cast<TItem>() ?? Enumerable.Empty<TItem>());
                    break;
                    
                case NotifyCollectionChangedAction.Remove:
                    removes.AddRange(change.OldItems?.Cast<TItem>() ?? Enumerable.Empty<TItem>());
                    break;
                    
                case NotifyCollectionChangedAction.Replace:
                    updates.AddRange(change.NewItems?.Cast<TItem>() ?? Enumerable.Empty<TItem>());
                    break;
                    
                case NotifyCollectionChangedAction.Move:
                    // Move is just a visual reorder - AG Grid handles this automatically
                    break;
            }
        }
        
        // Update count tracking
        _previousItemsHash = Items?.Count() ?? 0;
        
        // Single transaction with all batched changes
        if (adds.Any() || removes.Any() || updates.Any())
        {
            await ApplyTransactionAsync(adds, removes, updates);
        }
    }
    
    /// <summary>
    /// Extract IDs from items using the configured IdField.
    /// </summary>
    private IEnumerable<object> GetItemIds(IEnumerable<TItem> items)
    {
        var idProperty = typeof(TItem).GetProperty(
            IdField,
            BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase
        );
        
        if (idProperty == null)
        {
            Console.WriteLine($"[Grid] WARNING: IdField '{IdField}' not found on type {typeof(TItem).Name}");
            yield break;
        }
        
        foreach (var item in items)
        {
            var id = idProperty.GetValue(item);
            if (id != null)
            {
                yield return id;
            }
        }
    }
    
    /// <summary>
    /// Get items with their IDs for lookup.
    /// </summary>
    private IEnumerable<(object id, TItem item)> GetItemsWithIds(IEnumerable<TItem> items)
    {
        var idProperty = typeof(TItem).GetProperty(
            IdField,
            BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase
        );
        
        if (idProperty == null)
        {
            yield break;
        }
        
        foreach (var item in items)
        {
            var id = idProperty.GetValue(item);
            if (id != null)
            {
                yield return (id, item);
            }
        }
    }

    private async Task ApplyTransactionAsync(
        List<TItem> adds,
        List<TItem> removes,
        List<TItem> updates)
    {
        if (_gridRenderer == null)
            return;
        
        var transaction = new GridTransaction<TItem>
        {
            Add = adds.Any() ? adds : null,
            Remove = removes.Any() ? removes : null,
            Update = updates.Any() ? updates : null
        };
        
        await _gridRenderer.ApplyTransactionAsync(transaction);
    }
    
    /// <summary>
    /// Manually adds rows to the grid using transaction API.
    /// This is more efficient than replacing the entire Items collection.
    /// </summary>
    public async Task AddRowsAsync(params TItem[] rows)
    {
        if (_gridRenderer == null || !_initialized)
            throw new InvalidOperationException("Grid not initialized");
        
        await _gridRenderer.ApplyTransactionAsync(new GridTransaction<TItem>
        {
            Add = rows.ToList()
        });
        
        // Update count tracking
        _previousItemsHash = Items?.Count() ?? 0;
    }

    /// <summary>
    /// Manually updates rows in the grid using transaction API.
    /// This is more efficient than replacing the entire Items collection.
    /// </summary>
    public async Task UpdateRowsAsync(params TItem[] rows)
    {
        if (_gridRenderer == null || !_initialized)
            throw new InvalidOperationException("Grid not initialized");
        
        await _gridRenderer.ApplyTransactionAsync(new GridTransaction<TItem>
        {
            Update = rows.ToList()
        });
    }

    /// <summary>
    /// Manually removes rows from the grid using transaction API.
    /// This is more efficient than replacing the entire Items collection.
    /// </summary>
    public async Task RemoveRowsAsync(params TItem[] rows)
    {
        if (_gridRenderer == null || !_initialized)
            throw new InvalidOperationException("Grid not initialized");
        
        await _gridRenderer.ApplyTransactionAsync(new GridTransaction<TItem>
        {
            Remove = rows.ToList()
        });
        
        // Update count tracking
        _previousItemsHash = Items?.Count() ?? 0;
    }
    
    /// <summary>
    /// Manually refreshes the grid display to reflect changes in the underlying data.
    /// This forces a full grid refresh and is useful when:
    /// - Data properties have been modified in-place (without ObservableCollection events)
    /// - You want to ensure the grid reflects the current state of the data
    /// - Debugging or testing scenarios
    /// 
    /// For most scenarios, prefer using ObservableCollection or the transaction APIs
    /// (AddRowsAsync, UpdateRowsAsync, RemoveRowsAsync) which are more efficient.
    /// </summary>
    /// <example>
    /// <code>
    /// // Modify data in-place
    /// foreach (var product in products)
    /// {
    ///     product.Price *= 1.1m; // 10% price increase
    /// }
    /// 
    /// // Force grid to refresh and show updated prices
    /// await gridRef.RefreshAsync();
    /// </code>
    /// </example>
    public async Task RefreshAsync()
    {
        if (_gridRenderer == null || !_initialized)
            throw new InvalidOperationException("Grid not initialized");
        
        Console.WriteLine("[Grid] Manual refresh requested - reloading data");
        
        // Force full data reload by calling UpdateDataAsync
        // This ensures all in-place changes are reflected in the grid
        await _gridRenderer.UpdateDataAsync(Items);
    }
    
    /// <summary>
    /// Gets the current state of the grid from AG Grid.
    /// This returns the actual grid state, not a cached copy.
    /// Useful for persisting grid state to localStorage or a database.
    /// </summary>
    /// <returns>The current grid state including sort, filter, column configuration, and selection.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the grid is not initialized.</exception>
    public async Task<GridState> GetStateAsync()
    {
        if (_gridRenderer == null || !_initialized)
        {
            throw new InvalidOperationException("Grid not initialized. Ensure the grid has been rendered before calling GetStateAsync.");
        }
        
        return await _gridRenderer.GetStateAsync();
    }
    
    /// <summary>
    /// Computes a hash code for the given GridState to detect mutations.
    /// Uses HashCode struct for efficient, deterministic hashing of state properties.
    /// </summary>
    private static int ComputeStateHash(GridState state)
    {
        var hash = new HashCode();
        
        // Basic pagination state
        hash.Add(state.PageNumber);
        hash.Add(state.PageSize);
        
        // Sort descriptors
        foreach (var sort in state.SortDescriptors)
        {
            hash.Add(sort.Field);
            hash.Add(sort.Direction);
            hash.Add(sort.Order);
        }
        
        // Filter descriptors
        foreach (var filter in state.FilterDescriptors)
        {
            hash.Add(filter.Field);
            hash.Add(filter.Operator);
            hash.Add(filter.Value);
        }
        
        // Column states
        foreach (var col in state.ColumnStates)
        {
            hash.Add(col.Field);
            hash.Add(col.Visible);
            hash.Add(col.Width);
            hash.Add(col.Pinned);
            hash.Add(col.Order);
            hash.Add(col.Sort);
            hash.Add(col.SortIndex);
            hash.Add(col.AggFunc);
            hash.Add(col.RowGroup);
            hash.Add(col.RowGroupIndex);
            hash.Add(col.Pivot);
            hash.Add(col.PivotIndex);
            hash.Add(col.Flex);
        }
        
        // Selected row IDs
        foreach (var id in state.SelectedRowIds)
        {
            hash.Add(id);
        }
        
        // Row grouping
        foreach (var col in state.RowGroupColumns)
        {
            hash.Add(col);
        }
        
        // Pivot state
        hash.Add(state.PivotMode);
        foreach (var col in state.PivotColumns)
        {
            hash.Add(col);
        }
        
        // Focused cell
        if (state.FocusedCell != null)
        {
            hash.Add(state.FocusedCell.RowIndex);
            hash.Add(state.FocusedCell.ColumnId);
        }
        
        // Sidebar state
        if (state.SideBar != null)
        {
            hash.Add(state.SideBar.Visible);
            hash.Add(state.SideBar.ActivePanel);
        }
        
        // Scroll position
        if (state.Scroll != null)
        {
            hash.Add(state.Scroll.Top);
            hash.Add(state.Scroll.Left);
        }
        
        // Expanded row groups
        foreach (var group in state.ExpandedRowGroups)
        {
            hash.Add(group);
        }
        
        // Pinned rows
        foreach (var row in state.PinnedTopRows)
        {
            hash.Add(row);
        }
        
        foreach (var row in state.PinnedBottomRows)
        {
            hash.Add(row);
        }
        
        // Cell range selection
        foreach (var range in state.CellRangeSelection)
        {
            hash.Add(range.StartRow);
            hash.Add(range.EndRow);
            foreach (var col in range.Columns)
            {
                hash.Add(col);
            }
        }
        
        // Advanced filter model
        if (state.AdvancedFilterModel != null)
        {
            hash.Add(state.AdvancedFilterModel);
        }
        
        // Version
        hash.Add(state.Version);
        
        return hash.ToHashCode();
    }

    public async ValueTask DisposeAsync()
    {
        UnsubscribeFromCollection();
        
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
}

/// <summary>
/// Helper class for proper disposal of collection change subscriptions.
/// </summary>
internal sealed class CollectionChangedSubscription : IDisposable
{
    private readonly INotifyCollectionChanged _observable;
    private readonly NotifyCollectionChangedEventHandler _handler;

    public CollectionChangedSubscription(
        INotifyCollectionChanged observable,
        NotifyCollectionChangedEventHandler handler)
    {
        _observable = observable;
        _handler = handler;
    }

    public void Dispose()
    {
        _observable.CollectionChanged -= _handler;
    }
}
