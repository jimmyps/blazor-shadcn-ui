using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;
using System.Linq.Expressions;
using System.Text;

namespace NeoUI.Blazor;

/// <summary>
/// A multi-select component that allows users to select multiple options from a searchable dropdown.
/// </summary>
/// <typeparam name="TItem">The type of items in the multiselect list.</typeparam>
public partial class MultiSelect<TItem> : ComponentBase, IAsyncDisposable
{
    /// <summary>
    /// Gets or sets the JavaScript runtime for interop operations.
    /// </summary>
    [Inject]
    private IJSRuntime JSRuntime { get; set; } = default!;

    [Inject]
    private ILogger<MultiSelect<TItem>> Logger { get; set; } = default!;

    private static readonly Action<ILogger, string, Exception?> LogJsSetupFailed =
        LoggerMessage.Define<string>(LogLevel.Warning, new EventId(1, nameof(LogJsSetupFailed)),
            "MultiSelect JS setup failed: {Message}");

    private FieldIdentifier _fieldIdentifier;
    private EditContext? _editContext;
    private IJSObjectReference? _multiSelectModule;
    private DotNetObjectReference<MultiSelect<TItem>>? _dotNetRef;
    private ElementReference _searchInputRef;
    private bool _jsSetupDone = false;
    private bool _focusDone = false;

    // ShouldRender tracking fields
    private IEnumerable<TItem>? _lastItems;
    private IEnumerable<string>? _lastValues;
    private bool _lastIsOpen;
    private string _lastSearchQuery = string.Empty;
    private bool _lastDisabled;
    private bool _lastIsLoading;

    private ElementReference _listboxScrollRef;
    private IJSObjectReference? _elementUtilsModule;

    // Cached event handlers to avoid allocations on every render
    private readonly Dictionary<string, Func<Task>> _toggleHandlerCache = new();
    private readonly Dictionary<string, Func<Task>> _removeHandlerCache = new();

    // Persists display text for values across item list changes (e.g. async paging)
    private readonly Dictionary<string, string> _displayTextCache = new();

    // Cached CSS class strings to avoid recomputation on every render
    private string? _cachedTriggerCssClass;
    private string? _lastPopoverWidth;
    private string? _lastClass;
    private bool _lastCachedIsOpen;
    private StyleVariant _lastStyleVariant;

    /// <summary>
    /// Gets or sets the cascaded EditContext from a parent EditForm.
    /// </summary>
    [CascadingParameter]
    private EditContext? CascadedEditContext { get; set; }

    [CascadingParameter(Name = "StyleVariant")]
    private StyleVariant _styleVariant { get; set; } = StyleVariant.Default;

    /// <summary>
    /// Gets or sets the collection of items to display in the multiselect.
    /// </summary>
    [Parameter, EditorRequired]
    public IEnumerable<TItem> Items { get; set; } = Enumerable.Empty<TItem>();

    /// <summary>
    /// Gets or sets the currently selected values.
    /// </summary>
    [Parameter]
    public IEnumerable<string>? Values { get; set; }

    /// <summary>
    /// Gets or sets the callback that is invoked when the selected values change.
    /// </summary>
    [Parameter]
    public EventCallback<IEnumerable<string>?> ValuesChanged { get; set; }

    /// <summary>
    /// Gets or sets the function to extract the value from an item.
    /// </summary>
    [Parameter, EditorRequired]
    public Func<TItem, string> ValueSelector { get; set; } = default!;

    /// <summary>
    /// Gets or sets the function to extract the display text from an item.
    /// </summary>
    [Parameter, EditorRequired]
    public Func<TItem, string> DisplaySelector { get; set; } = default!;

    /// <summary>
    /// Gets or sets the placeholder text shown when no items are selected.
    /// When null, falls back to the localizer value for "MultiSelect.Placeholder".
    /// </summary>
    [Parameter]
    public string? Placeholder { get; set; }

    /// <summary>
    /// Gets or sets the placeholder text shown in the search input.
    /// When null, falls back to the localizer value for "MultiSelect.SearchPlaceholder".
    /// </summary>
    [Parameter]
    public string? SearchPlaceholder { get; set; }

    /// <summary>
    /// Gets or sets the message displayed when no items match the search.
    /// When null, falls back to the localizer value for "MultiSelect.EmptyMessage".
    /// </summary>
    [Parameter]
    public string? EmptyMessage { get; set; }

    /// <summary>
    /// Gets or sets the label for the Select All option.
    /// When null, falls back to the localizer value for "MultiSelect.SelectAll".
    /// </summary>
    [Parameter]
    public string? SelectAllLabel { get; set; }

    /// <summary>
    /// Gets or sets whether to show the Select All option.
    /// </summary>
    [Parameter]
    public bool ShowSelectAll { get; set; } = true;

    /// <summary>
    /// Gets or sets the label for the Clear button.
    /// When null, falls back to the localizer value for "MultiSelect.Clear".
    /// </summary>
    [Parameter]
    public string? ClearLabel { get; set; }

    /// <summary>
    /// Gets or sets the label for the Close button.
    /// When null, falls back to the localizer value for "MultiSelect.Close".
    /// </summary>
    [Parameter]
    public string? CloseLabel { get; set; }

    [Inject]
    private ILocalizer Localizer { get; set; } = default!;

    private string EffectivePlaceholder => Placeholder ?? Localizer["MultiSelect.Placeholder"];
    private string EffectiveSearchPlaceholder => SearchPlaceholder ?? Localizer["MultiSelect.SearchPlaceholder"];
    private string EffectiveEmptyMessage => EmptyMessage ?? Localizer["MultiSelect.EmptyMessage"];
    private string EffectiveSelectAllLabel => SelectAllLabel ?? Localizer["MultiSelect.SelectAll"];
    private string EffectiveClearLabel => ClearLabel ?? Localizer["MultiSelect.Clear"];
    private string EffectiveCloseLabel => CloseLabel ?? Localizer["MultiSelect.Close"];

    /// <summary>
    /// Gets or sets additional CSS classes to apply to the multiselect container.
    /// </summary>
    [Parameter]
    public string? Class { get; set; }

    /// <summary>
    /// Gets or sets whether the multiselect is disabled.
    /// </summary>
    [Parameter]
    public bool Disabled { get; set; }

    /// <summary>
    /// Gets or sets the maximum number of tags to display before showing "+N more".
    /// </summary>
    [Parameter]
    public int MaxDisplayTags { get; set; } = 3;

    /// <summary>
    /// Gets or sets the width of the popover content.
    /// Ignored when MatchTriggerWidth is true.
    /// </summary>
    [Parameter]
    public string PopoverWidth { get; set; } = "w-[300px]";

    /// <summary>
    /// Gets or sets whether to match the dropdown width to the trigger element width.
    /// When true, PopoverWidth is ignored.
    /// </summary>
    [Parameter]
    public bool MatchTriggerWidth { get; set; } = false;

    /// <summary>
    /// Gets or sets whether clicking outside the dropdown should close it.
    /// When true, the dropdown will close and the search query will be cleared.
    /// Default is true.
    /// </summary>
    [Parameter]
    public bool AutoClose { get; set; } = true;

    /// <summary>
    /// Gets or sets the callback invoked when the search query changes.
    /// Use for server-side filtering or loading external data. When set, the component
    /// bypasses its internal text filter and trusts the consumer to control the displayed items.
    /// </summary>
    [Parameter]
    public EventCallback<string> SearchQueryChanged { get; set; }

    /// <summary>
    /// Gets or sets the callback invoked when the user scrolls near the bottom of the dropdown list.
    /// Use this to load additional items for infinite scroll scenarios.
    /// </summary>
    [Parameter]
    public EventCallback OnLoadMore { get; set; }

    /// <summary>
    /// Gets or sets whether additional items are currently being loaded.
    /// When true, a loading spinner is shown at the bottom of the list and
    /// <see cref="OnLoadMore"/> is suppressed until loading completes.
    /// </summary>
    [Parameter]
    public bool IsLoading { get; set; }

    /// <summary>
    /// Gets or sets a message displayed at the bottom of the list when all items have been loaded.
    /// Only shown when <see cref="IsLoading"/> is false. Set to <c>null</c> or empty to hide.
    /// </summary>
    [Parameter]
    public string? EndOfListMessage { get; set; }

    /// <summary>
    /// Gets or sets an expression that identifies the bound values.
    /// Used for form validation integration.
    /// </summary>
    [Parameter]
    public Expression<Func<IEnumerable<string>?>>? ValuesExpression { get; set; }

    /// <summary>
    /// Tracks whether the popover is currently open.
    /// </summary>
    private bool _isOpen { get; set; } = false;

    /// <summary>
    /// Tracks the current search query for filtering.
    /// </summary>
    private string _searchQuery = string.Empty;

    /// <summary>
    /// Gets a unique identifier for this multiselect instance.
    /// </summary>
    private string Id { get; set; } = $"multiselect-{Guid.NewGuid():N}";

    /// <summary>
    /// Gets the list of currently selected values.
    /// </summary>
    private List<string> SelectedValues => Values?.ToList() ?? new List<string>();

    /// <summary>
    /// Gets the number of selected items that exceed MaxDisplayTags.
    /// </summary>
    private int OverflowCount => Math.Max(0, SelectedValues.Count - MaxDisplayTags);

    /// <summary>
    /// Gets whether any items are selected.
    /// </summary>
    private bool HasSelectedItems => SelectedValues.Count > 0;

    /// <summary>
    /// Gets the filtered items based on current search query.
    /// </summary>
    private IEnumerable<TItem> FilteredItems => GetFilteredItems();

    /// <summary>
    /// Gets whether there are any items visible after filtering.
    /// </summary>
    private bool HasFilteredItems => FilteredItems.Any();

    /// <summary>
    /// Gets the items that match the current search query.
    /// When <see cref="SearchQueryChanged"/> has a delegate, bypasses internal filtering
    /// (the consumer controls the items externally).
    /// </summary>
    private IEnumerable<TItem> GetFilteredItems()
    {
        // When external filtering is active, trust the consumer's item list
        if (SearchQueryChanged.HasDelegate)
        {
            return Items;
        }

        if (string.IsNullOrWhiteSpace(_searchQuery))
        {
            return Items;
        }

        return Items.Where(item =>
            DisplaySelector(item).Contains(_searchQuery, StringComparison.OrdinalIgnoreCase));
    }

    /// <summary>
    /// Gets whether the multiselect is in an invalid state (for validation).
    /// </summary>
    private bool IsInvalid
    {
        get
        {
            if (_editContext != null && ValuesExpression != null && _fieldIdentifier.FieldName != null)
            {
                return _editContext.GetValidationMessages(_fieldIdentifier).Any();
            }
            return false;
        }
    }

    /// <summary>
    /// Validates required parameters.
    /// </summary>
    protected override void OnParametersSet()
    {
        base.OnParametersSet();

        if (ValueSelector == null)
        {
            throw new InvalidOperationException(
                $"{nameof(MultiSelect<TItem>)}: {nameof(ValueSelector)} parameter is required.");
        }

        if (DisplaySelector == null)
        {
            throw new InvalidOperationException(
                $"{nameof(MultiSelect<TItem>)}: {nameof(DisplaySelector)} parameter is required.");
        }

        // Filter out null items for safety
        Items = Items?.Where(item => item != null) ?? Enumerable.Empty<TItem>();

        // Clear handler caches when Items reference changes to avoid stale delegates
        if (!ReferenceEquals(_lastItems, Items))
        {
            _toggleHandlerCache.Clear();
        }

        // Initialize EditContext integration if available
        if (CascadedEditContext != null && ValuesExpression != null)
        {
            _editContext = CascadedEditContext;
            _fieldIdentifier = FieldIdentifier.Create(ValuesExpression);
        }
    }

    /// <inheritdoc/>
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        // Setup JS when popover opens
        if (_isOpen && !_jsSetupDone)
        {
            _jsSetupDone = true;

            try
            {
                _multiSelectModule = await JSRuntime.InvokeAsync<IJSObjectReference>(
                    "import", "./_content/NeoUI.Blazor/js/multiselect.js");

                _dotNetRef = DotNetObjectReference.Create(this);

                await _multiSelectModule.InvokeVoidAsync(
                    "setupMultiSelectInput",
                    _searchInputRef,
                    _dotNetRef,
                    $"{Id}-search",
                    $"{Id}-listbox");
            }
            catch (Exception ex)
            {
                LogJsSetupFailed(Logger, ex.Message, ex);
            }
        }
        // Cleanup JS when popover closes
        else if (!_isOpen && _jsSetupDone)
        {
            await CleanupJsAsync();
        }
    }

    /// <summary>
    /// Cleans up JavaScript resources when the popover closes.
    /// </summary>
    private async Task CleanupJsAsync()
    {
        if (_multiSelectModule != null)
        {
            try
            {
                await _multiSelectModule.InvokeVoidAsync("removeMultiSelectInput", $"{Id}-search");
            }
            catch
            {
                // Module may already be disposed
            }
        }
        _jsSetupDone = false;
        _focusDone = false;
    }

    /// <summary>
    /// Handles the open state change of the popover.
    /// Resets focus tracking when the popover closes.
    /// </summary>
    /// <param name="isOpen">Whether the popover is now open.</param>
    private void HandleOpenChanged(bool isOpen)
    {
        _isOpen = isOpen;
        if (!isOpen)
        {
            _focusDone = false; // Reset for next open
        }
    }

    /// <summary>
    /// Opens the dropdown.
    /// </summary>
    private void Open()
    {
        if (Disabled) return;
        _isOpen = true;
    }

    /// <summary>
    /// Closes the dropdown.
    /// </summary>
    private async Task Close()
    {
        _isOpen = false;
        _searchQuery = string.Empty;

        // Notify consumer so they can reload the default dataset for the next open
        if (SearchQueryChanged.HasDelegate)
        {
            await SearchQueryChanged.InvokeAsync(string.Empty);
        }
    }

    /// <summary>
    /// Gets the click-outside event handler based on AutoClose setting.
    /// Returns an empty EventCallback when AutoClose is false.
    /// </summary>
    private EventCallback GetClickOutsideHandler()
    {
        return AutoClose
            ? EventCallback.Factory.Create(this, HandleClickOutside)
            : default;
    }

    /// <summary>
    /// Handles click-outside events when AutoClose is enabled.
    /// </summary>
    private async Task HandleClickOutside() => await Close();

    /// <summary>
    /// Handles the popover content ready event to focus the search input.
    /// This is called when the popover is fully positioned and visible.
    /// </summary>
    private async Task HandleContentReady()
    {
        // Guard against multiple calls per open
        if (_focusDone) return;
        _focusDone = true;

        try
        {
            // Small delay to let browser finish processing DOM changes
            await Task.Delay(50);
            await _searchInputRef.FocusAsync();
        }
        catch
        {
            // Ignore focus errors
        }
    }

    /// <summary>
    /// Handles search input changes.
    /// </summary>
    private async Task HandleSearchInput(ChangeEventArgs args)
    {
        _searchQuery = args.Value?.ToString() ?? string.Empty;
        if (SearchQueryChanged.HasDelegate)
        {
            await SearchQueryChanged.InvokeAsync(_searchQuery);
        }
    }

    /// <summary>
    /// Handles keyboard events on the search input.
    /// </summary>
    private async Task HandleSearchKeyDown(KeyboardEventArgs args)
    {
        switch (args.Key)
        {
            case "Enter":
                await HandleEnter();
                break;
            case "Escape":
                await HandleEscape();
                break;
        }
    }

    /// <summary>
    /// Handles item toggle (selection/deselection).
    /// </summary>
    private async Task HandleToggle(TItem item)
    {
        var itemValue = ValueSelector(item);
        var currentValues = SelectedValues;

        if (currentValues.Contains(itemValue))
        {
            currentValues.Remove(itemValue);
            _displayTextCache.Remove(itemValue);
        }
        else
        {
            currentValues.Add(itemValue);
            // Cache display text so it survives Items list changes during async filtering
            _displayTextCache[itemValue] = DisplaySelector(item);
        }

        await UpdateValues(currentValues);
    }

    /// <summary>
    /// Gets a cached toggle handler for the specified item to avoid delegate allocations.
    /// </summary>
    private Func<Task> GetToggleHandler(TItem item)
    {
        var key = ValueSelector(item);
        if (!_toggleHandlerCache.TryGetValue(key, out var handler))
        {
            handler = () => HandleToggle(item);
            _toggleHandlerCache[key] = handler;
        }
        return handler;
    }

    /// <summary>
    /// Gets a cached remove handler for the specified value to avoid delegate allocations.
    /// </summary>
    private Func<Task> GetRemoveHandler(string value)
    {
        if (!_removeHandlerCache.TryGetValue(value, out var handler))
        {
            handler = () => RemoveValue(value);
            _removeHandlerCache[value] = handler;
        }
        return handler;
    }

    /// <summary>
    /// Handles Select All toggle. Only affects filtered/visible items.
    /// </summary>
    private async Task HandleSelectAllToggle()
    {
        var filteredItems = FilteredItems.ToList();
        var filteredValues = filteredItems.Select(ValueSelector).ToList();
        var currentValues = SelectedValues;

        // Check if all FILTERED items are selected
        var allFilteredSelected = filteredValues.All(v => currentValues.Contains(v));

        if (allFilteredSelected)
        {
            // Deselect only the filtered items
            currentValues.RemoveAll(v => filteredValues.Contains(v));
            foreach (var v in filteredValues)
            {
                _displayTextCache.Remove(v);
            }
        }
        else
        {
            // Select all filtered items (add to existing selection)
            foreach (var item in filteredItems.Where(i => !currentValues.Contains(ValueSelector(i))))
            {
                var v = ValueSelector(item);
                currentValues.Add(v);
                _displayTextCache[v] = DisplaySelector(item);
            }
        }

        await UpdateValues(currentValues);
    }

    /// <summary>
    /// Removes a value from the selection.
    /// </summary>
    private async Task RemoveValue(string value)
    {
        var currentValues = SelectedValues;
        currentValues.Remove(value);
        _displayTextCache.Remove(value);
        await UpdateValues(currentValues);
    }

    /// <summary>
    /// Clears all selections.
    /// </summary>
    private async Task ClearAll()
    {
        _displayTextCache.Clear();
        await UpdateValues(new List<string>());
    }

    /// <summary>
    /// Updates the selected values and notifies parent.
    /// </summary>
    private async Task UpdateValues(List<string> values)
    {
        Values = values.Count > 0 ? values : null;
        await ValuesChanged.InvokeAsync(Values);

        // Notify EditContext of field change for validation
        if (_editContext != null && ValuesExpression != null && _fieldIdentifier.FieldName != null)
        {
            _editContext.NotifyFieldChanged(_fieldIdentifier);
        }
    }

    /// <summary>
    /// Checks if an item is currently selected.
    /// </summary>
    private bool IsSelected(TItem item)
    {
        return SelectedValues.Contains(ValueSelector(item));
    }

    /// <summary>
    /// Gets the Select All state based on filtered items.
    /// </summary>
    private SelectAllState GetSelectAllState()
    {
        var filteredValues = FilteredItems.Select(ValueSelector).ToHashSet();
        var selectedFilteredCount = SelectedValues.Count(v => filteredValues.Contains(v));

        if (selectedFilteredCount == 0) return SelectAllState.None;
        if (selectedFilteredCount == filteredValues.Count) return SelectAllState.All;
        return SelectAllState.Indeterminate;
    }

    /// <summary>
    /// Gets the display text for a value.
    /// Checks the current Items list first; falls back to the display text cache so that
    /// badge labels remain correct even when the item has been paged out of Items.
    /// </summary>
    private string GetDisplayText(string value)
    {
        var item = Items.FirstOrDefault(i => ValueSelector(i) == value);
        if (item != null)
        {
            var displayText = DisplaySelector(item);
            _displayTextCache[value] = displayText;
            return displayText;
        }
        return _displayTextCache.TryGetValue(value, out var cached) ? cached : value;
    }

    // JSInvokable callbacks for keyboard navigation

    /// <summary>
    /// Called from JavaScript when Space key is pressed on an item.
    /// </summary>
    [JSInvokable]
    public void HandleSpace()
    {
        // Space toggles checkbox - item click already handled by JS
        // No additional action needed, dropdown stays open
    }

    /// <summary>
    /// Called from JavaScript when Enter key is pressed to close the dropdown.
    /// </summary>
    [JSInvokable]
    public async Task HandleEnter()
    {
        // Enter closes the dropdown after item toggle
        await Close();
        StateHasChanged();
    }

    /// <summary>
    /// Called from JavaScript when Escape key is pressed to close the dropdown.
    /// </summary>
    [JSInvokable]
    public async Task HandleEscape()
    {
        // Escape closes the dropdown
        await Close();
        StateHasChanged();
    }

    /// <summary>
    /// Handles listbox scroll events for infinite scroll.
    /// Triggers <see cref="OnLoadMore"/> when the user scrolls near the bottom.
    /// </summary>
    private async Task HandleListboxScrollAsync()
    {
        if (!OnLoadMore.HasDelegate || IsLoading)
        {
            return;
        }

        // Suppress infinite scroll during active search when using client-side filter.
        // Allow it through only when SearchQueryChanged is set (server-side filter + paginated results).
        if (!string.IsNullOrEmpty(_searchQuery) && !SearchQueryChanged.HasDelegate)
        {
            return;
        }

        _elementUtilsModule ??= await JSRuntime.InvokeAsync<IJSObjectReference>(
            "import", "./_content/NeoUI.Blazor.Primitives/js/primitives/element-utils.js");

        try
        {
            var nearBottom = await _elementUtilsModule.InvokeAsync<bool>("isNearBottom", _listboxScrollRef, 80.0);
            if (nearBottom)
            {
                await OnLoadMore.InvokeAsync();
            }
        }
        catch (Exception ex) when (ex is JSDisconnectedException or TaskCanceledException or ObjectDisposedException)
        {
            // Expected during circuit disconnect or disposal
        }
    }

    /// <inheritdoc/>
    public async ValueTask DisposeAsync()
    {
        GC.SuppressFinalize(this);
        await CleanupJsAsync();

        if (_multiSelectModule != null)
        {
            try
            {
                await _multiSelectModule.DisposeAsync();
            }
            catch
            {
                // Module may already be disposed
            }
            _multiSelectModule = null;
        }

        if (_elementUtilsModule != null)
        {
            try
            {
                await _elementUtilsModule.DisposeAsync();
            }
            catch
            {
                // Module may already be disposed
            }
            _elementUtilsModule = null;
        }

        _dotNetRef?.Dispose();
        _dotNetRef = null;
    }

    /// <summary>
    /// Determines whether the component should re-render based on tracked state changes.
    /// This optimization reduces unnecessary render cycles.
    /// </summary>
    protected override bool ShouldRender()
    {
        var itemsChanged = !ReferenceEquals(_lastItems, Items);
        var valuesChanged = !ReferenceEquals(_lastValues, Values);
        var openChanged = _lastIsOpen != _isOpen;
        var searchChanged = _lastSearchQuery != _searchQuery;
        var disabledChanged = _lastDisabled != Disabled;
        var loadingChanged = _lastIsLoading != IsLoading;

        if (itemsChanged || valuesChanged || openChanged || searchChanged || disabledChanged || loadingChanged)
        {
            _lastItems = Items;
            _lastValues = Values;
            _lastIsOpen = _isOpen;
            _lastSearchQuery = _searchQuery;
            _lastDisabled = Disabled;
            _lastIsLoading = IsLoading;
            return true;
        }

        return false;
    }

    /// <summary>
    /// Gets the CSS class for the multiselect container.
    /// </summary>
    private string ContainerClass => "relative";

    /// <summary>
    /// Gets the CSS class for the trigger button.
    /// Uses caching to avoid recomputation on every render.
    /// </summary>
    private string TriggerCssClass
    {
        get
        {
            // Return cached value if inputs haven't changed
            if (_cachedTriggerCssClass != null &&
                _lastPopoverWidth == PopoverWidth &&
                _lastClass == Class &&
                _lastCachedIsOpen == _isOpen &&
                _lastStyleVariant == _styleVariant)
            {
                return _cachedTriggerCssClass;
            }

            _cachedTriggerCssClass = ClassNames.cn(
                "inline-flex items-center justify-between rounded-md text-sm",
                "ring-offset-background",
                "transition-[color,box-shadow]",
                "focus:outline-none focus:ring-2 focus:ring-ring/50",
                "focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-ring/50",
                "disabled:opacity-50 disabled:pointer-events-none disabled:hover:bg-background",
                "border border-input bg-background hover:bg-accent hover:text-accent-foreground",
                _isOpen ? "ring-2 ring-ring/50" : "",
                "min-h-9 px-3 py-1.5",
                PopoverWidth,
                _styleVariant.GetClasses("SelectTrigger.Root"),
                Class);

            _lastPopoverWidth = PopoverWidth;
            _lastClass = Class;
            _lastCachedIsOpen = _isOpen;
            _lastStyleVariant = _styleVariant;

            return _cachedTriggerCssClass;
        }
    }

    /// <summary>
    /// Gets the CSS class for the tag remove button.
    /// </summary>
    private string TagRemoveButtonCssClass =>
        "ml-0.5 rounded-full outline-none hover:bg-secondary-foreground/20 focus:ring-1 focus:ring-ring";

    /// <summary>
    /// Gets the CSS class for the dropdown item.
    /// </summary>
    private string ItemCssClass => ClassNames.cn(
        "relative flex cursor-pointer select-none items-center gap-2 rounded-sm px-2 py-1.5 text-sm outline-none",
        "data-[focused=true]:bg-accent data-[focused=true]:text-accent-foreground",
        "data-[disabled=true]:pointer-events-none data-[disabled=true]:opacity-50",
        _styleVariant.GetClasses("SelectItem.Root")
    );

    /// <summary>
    /// Gets the CSS class for the checkbox.
    /// Uses data-state attribute from Checkbox primitive for checked/unchecked/indeterminate styling.
    /// </summary>
    private string CheckboxCssClass => ClassNames.cn(
        "h-4 w-4 shrink-0 rounded-sm border border-primary ring-offset-background flex items-center justify-center",
        "focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-ring focus-visible:ring-offset-2",
        "disabled:cursor-not-allowed disabled:opacity-50",
        "data-[state=checked]:bg-primary data-[state=checked]:text-primary-foreground",
        "data-[state=indeterminate]:bg-primary data-[state=indeterminate]:text-primary-foreground",
        "data-[state=unchecked]:bg-background",
        _styleVariant.GetClasses("Checkbox.Root")
    );
}

/// <summary>
/// Represents the state of the Select All checkbox.
/// </summary>
public enum SelectAllState
{
    /// <summary>No items are selected.</summary>
    None,
    /// <summary>Some items are selected.</summary>
    Indeterminate,
    /// <summary>All items are selected.</summary>
    All
}
