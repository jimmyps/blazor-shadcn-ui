using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.Web.Virtualization;
using NeoUI.Blazor.Primitives;

namespace NeoUI.Blazor;

/// <summary>
/// Displays collections in switchable list/grid layouts with optional grouping,
/// built-in pagination, toolbar search and sort (via DataViewColumn), selection,
/// loading and empty states.
/// </summary>
/// <typeparam name="TItem">The type of data items to display.</typeparam>
public partial class DataView<TItem> : ComponentBase
{
    // ── Internal column metadata ────────────────────────────────────────

    private sealed class ColumnMetadata
    {
        public string Id       { get; init; } = "";
        public string Header   { get; init; } = "";
        public Func<TItem, object?>? Property { get; init; }
        public bool Sortable   { get; init; }
        public bool Filterable { get; init; }
    }

    // ── Internal state ──────────────────────────────────────────────────

    private DataViewLayout             _layout;
    private readonly HashSet<object>   _selectedKeys    = new();
    private readonly KeyboardNavigator _navigator       = new();
    private readonly string            _instanceId      = Guid.NewGuid().ToString("N")[..8];
    private int                        _focusedIndex    = -1;
    private bool                       _shouldPreventDefault;
    private readonly PaginationState   _paginationState = new();
    private bool                       _paginationInit;
    private readonly List<ColumnMetadata> _columns      = new();
    private string                     _searchValue     = "";
    private string                     _sortColumnId    = "";
    private SortDirection              _sortDirection   = SortDirection.None;
    private Virtualize<TItem>?         _virtualizeRef;

    // ── Parameters ──────────────────────────────────────────────────────

    /// <summary>Data source.</summary>
    [Parameter] public IEnumerable<TItem>? Items { get; set; }

    /// <summary>
    /// Server-side data provider for infinite-scroll virtualization.
    /// When set, <see cref="Items"/> is ignored, pagination is hidden, and the list
    /// renders via <c>&lt;Virtualize&gt;</c>.  Only list layout is supported in this mode.
    /// The delegate receives a <see cref="ItemsProviderRequest"/> with
    /// <c>StartIndex</c> and <c>Count</c> and must return an
    /// <see cref="ItemsProviderResult{TItem}"/> containing the slice and total count.
    /// </summary>
    [Parameter] public ItemsProviderDelegate<TItem>? ItemsProvider { get; set; }

    /// <summary>
    /// When <c>true</c> and <see cref="Items"/> is set, renders the list via
    /// <c>&lt;Virtualize Items="@SortedItems"&gt;</c> instead of a plain loop.
    /// All filtering and sorting still run client-side; only DOM nodes outside
    /// the viewport are removed.  Pagination is hidden in this mode.
    /// Has no effect when <see cref="ItemsProvider"/> is also set.
    /// </summary>
    [Parameter] public bool Virtualize { get; set; }

    /// <summary>
    /// Approximate height of a single item in pixels used by the virtualizer.
    /// Defaults to 72 (list row height). Adjust to match your <c>ListTemplate</c> height.
    /// Only relevant when <see cref="Virtualize"/> is <c>true</c> or <see cref="ItemsProvider"/> is set.
    /// </summary>
    [Parameter] public float ItemHeight { get; set; } = 72f;

    /// <summary>
    /// CSS height of the virtualized scroll container, e.g. <c>"500px"</c> or
    /// <c>"calc(100vh - 200px)"</c>.  Required when <see cref="Virtualize"/> is
    /// <c>true</c> or <see cref="ItemsProvider"/> is set so the browser knows the
    /// viewport size.  Defaults to <c>"500px"</c>.
    /// </summary>
    [Parameter] public string Height { get; set; } = "500px";

    /// <summary>
    /// Number of extra items to render beyond the visible viewport.
    /// Maps to <see cref="Microsoft.AspNetCore.Components.Web.Virtualization.Virtualize{TItem}.OverscanCount"/>.
    /// Higher values reduce blank flicker during fast scrolling at the cost of extra DOM nodes.
    /// Only relevant when <see cref="Virtualize"/> is <c>true</c> or <see cref="ItemsProvider"/> is set.
    /// </summary>
    [Parameter] public int VirtualizeOverscanCount { get; set; } = 3;

    /// <summary>True when the component operates in server-side infinite-scroll mode.</summary>
    private bool IsServerMode => ItemsProvider is not null;

    /// <summary>True when any form of virtualized rendering is active (client or server).</summary>
    private bool IsVirtualized => IsServerMode || Virtualize;

    /// <summary>Initial layout mode (List or Grid).</summary>
    [Parameter] public DataViewLayout Layout { get; set; } = DataViewLayout.List;

    /// <summary>Initial items per page. 0 (default) shows all items and hides the pagination bar.</summary>
    [Parameter] public int PageSize { get; set; } = 0;

    /// <summary>Available page-size options shown in the pagination size selector.</summary>
    [Parameter] public int[] PageSizes { get; set; } = [10, 25, 50, 100];

    /// <summary>Function to extract a unique key per item for diffing.</summary>
    [Parameter] public Func<TItem, object>? ItemKey { get; set; }

    /// <summary>Shows a loading spinner when true.</summary>
    [Parameter] public bool Loading { get; set; }

    /// <summary>Text shown in the loading state.</summary>
    [Parameter] public string LoadingText { get; set; } = "Loading…";

    /// <summary>Text shown when Items is empty and EmptyTemplate is null.</summary>
    [Parameter] public string EmptyText { get; set; } = "No items to display.";

    /// <summary>Whether to render the pagination bar (requires PageSize &gt; 0).</summary>
    [Parameter] public bool ShowPagination { get; set; } = true;

    /// <summary>None, Single, or Multiple item selection.</summary>
    [Parameter] public DataViewSelectionMode SelectionMode { get; set; } = DataViewSelectionMode.None;

    /// <summary>Selected item in Single mode (two-way bindable).</summary>
    [Parameter] public TItem? SelectedItem { get; set; }

    /// <summary>Fires when the selected item changes in Single mode.</summary>
    [Parameter] public EventCallback<TItem?> SelectedItemChanged { get; set; }

    /// <summary>Selected items in Multiple mode (two-way bindable).</summary>
    [Parameter] public IReadOnlyList<TItem>? SelectedItems { get; set; }

    /// <summary>Fires when the selected items collection changes in Multiple mode.</summary>
    [Parameter] public EventCallback<IReadOnlyList<TItem>> SelectedItemsChanged { get; set; }

    /// <summary>Check indicator style: CircleCheck (default), Check, or None.</summary>
    [Parameter] public DataViewCheckVariant CheckVariant { get; set; } = DataViewCheckVariant.CircleCheck;

    /// <summary>Column count in Grid layout (1–6).</summary>
    [Parameter] public int GridColumns { get; set; } = 3;

    /// <summary>
    /// Minimum tile width for auto-fill columns in Grid layout.
    /// Accepts any CSS length string (e.g. <c>"160px"</c>, <c>"10rem"</c>) or a bare Tailwind
    /// spacing key (e.g. <c>"40"</c> = 10 rem). Raw CSS values are automatically wrapped in
    /// Tailwind's arbitrary-value brackets so <c>"160px"</c> emits <c>grid-auto-fill-[160px]</c>.
    /// When set, the number of columns grows automatically to fill the container width.
    /// Overrides <see cref="GridColumns"/> when set. <c>null</c> or empty = disabled.
    /// </summary>
    [Parameter] public string? GridColumnMinWidth { get; set; }

    /// <summary>Child content — used to place <see cref="DataViewListTemplate{TItem}"/> and <see cref="DataViewGridTemplate{TItem}"/> sub-components.</summary>
    [Parameter] public RenderFragment? ChildContent { get; set; }

    /// <summary>Shared template used for both layouts when layout-specific templates are not set.</summary>
    [Parameter] public RenderFragment<TItem>? ItemTemplate { get; set; }

    /// <summary>
    /// List-specific item template. Provide with <see cref="GridTemplate"/> to enable the layout toggle.
    /// </summary>
    [Parameter] public RenderFragment<TItem>? ListTemplate { get; set; }

    /// <summary>
    /// Grid-specific item template. Provide with <see cref="ListTemplate"/> to enable the layout toggle.
    /// </summary>
    [Parameter] public RenderFragment<TItem>? GridTemplate { get; set; }

    /// <summary>Custom empty-state content.</summary>
    [Parameter] public RenderFragment? EmptyTemplate { get; set; }

    /// <summary>Optional header rendered above the toolbar and items.</summary>
    [Parameter] public RenderFragment? Header { get; set; }

    /// <summary>Whether the toolbar is visible.</summary>
    [Parameter] public bool ShowToolbar { get; set; } = true;

    /// <summary>Custom content rendered on the left of the toolbar (after built-in search/sort).</summary>
    [Parameter] public RenderFragment? ToolbarActions { get; set; }

    /// <summary>DataViewColumn child declarations that enable built-in search and sort.</summary>
    [Parameter] public RenderFragment? Fields { get; set; }

    /// <summary>Function to extract the group key from each item. When set, items are visually grouped.</summary>
    [Parameter] public Func<TItem, object>? GroupBy { get; set; }

    /// <summary>Custom render template for group headers. Falls back to the group key's ToString().</summary>
    [Parameter] public RenderFragment<object>? GroupHeaderTemplate { get; set; }

    /// <summary>Additional CSS classes applied to the root element.</summary>
    [Parameter] public string? Class { get; set; }

    /// <summary>Captures any additional HTML attributes.</summary>
    [Parameter(CaptureUnmatchedValues = true)] public Dictionary<string, object>? AdditionalAttributes { get; set; }

    // ── Data pipeline ────────────────────────────────────────────────────

    private IReadOnlyList<TItem> AllItems => Items?.ToList() ?? [];

    private IReadOnlyList<TItem> FilteredItems
    {
        get
        {
            if (string.IsNullOrWhiteSpace(_searchValue)) return AllItems;
            var filterable = _columns.Where(c => c.Filterable && c.Property is not null).ToList();
            if (filterable.Count == 0) return AllItems;
            return AllItems
                .Where(item => filterable.Any(col =>
                    col.Property!(item)?.ToString()
                        ?.Contains(_searchValue, StringComparison.OrdinalIgnoreCase) == true))
                .ToList();
        }
    }

    private IReadOnlyList<TItem> SortedItems
    {
        get
        {
            var items = FilteredItems;
            if (string.IsNullOrEmpty(_sortColumnId) || _sortDirection == SortDirection.None)
                return items;
            var col = _columns.FirstOrDefault(c => c.Id == _sortColumnId);
            if (col?.Property is null) return items;
            return _sortDirection == SortDirection.Ascending
                ? (IReadOnlyList<TItem>)items.OrderBy(col.Property).ToList()
                : items.OrderByDescending(col.Property).ToList();
        }
    }

    private IReadOnlyList<TItem> PagedItems
    {
        get
        {
            var items = SortedItems;
            if (_paginationState.PageSize <= 0) return items;
            return (IReadOnlyList<TItem>)items
                .Skip(_paginationState.StartIndex)
                .Take(_paginationState.PageSize)
                .ToList();
        }
    }

    private bool IsPaginated      => PageSize > 0;
    private bool HasItems         => AllItems.Count > 0;
    private bool HasFilteredItems => SortedItems.Count > 0;

    // ── Toolbar flags ────────────────────────────────────────────────────

    private bool CanToggleLayout => ListTemplate is not null && GridTemplate is not null;
    private bool HasSearch       => _columns.Any(c => c.Filterable);
    private bool HasSort         => _columns.Any(c => c.Sortable);
    private bool ShowToolbarRow  => ShowToolbar &&
        (CanToggleLayout || ToolbarActions is not null || HasSearch || HasSort);

    // ── Template registration (sub-component API) ────────────────────────

    internal void SetGridTemplate(RenderFragment<TItem>? template)
    {
        GridTemplate = template;
        StateHasChanged();
    }

    internal void SetListTemplate(RenderFragment<TItem>? template)
    {
        ListTemplate = template;
        StateHasChanged();
    }

    // ── Column registration ───────────────────────────────────────────────

    internal void RegisterColumn(DataViewColumn<TItem> col)
    {
        _columns.Add(new ColumnMetadata
        {
            Id        = col.EffectiveId,
            Header    = col.Header,
            Property  = col.Property is not null ? item => col.Property(item) : null,
            Sortable  = col.Sortable,
            Filterable = col.Filterable
        });
        StateHasChanged();
    }

    // ── Layout ───────────────────────────────────────────────────────────

    private void SetLayout(DataViewLayout layout)
    {
        if (_layout == layout) return;
        _layout = layout;
        StateHasChanged();
    }

    // ── Search & Sort ─────────────────────────────────────────────────────

    private void HandleSearchInput(ChangeEventArgs e)
    {
        _searchValue = e.Value?.ToString() ?? "";
        _paginationState.CurrentPage = 1;
        _focusedIndex = -1;
        SyncPaginationTotal();
        if (IsServerMode && _virtualizeRef is not null)
            InvokeAsync(_virtualizeRef.RefreshDataAsync);
        StateHasChanged();
    }

    private void SetSort(string columnId)
    {
        if (_sortColumnId == columnId)
        {
            _sortDirection = _sortDirection switch
            {
                SortDirection.Ascending  => SortDirection.Descending,
                SortDirection.Descending => SortDirection.None,
                _                        => SortDirection.Ascending
            };
            if (_sortDirection == SortDirection.None)
                _sortColumnId = "";
        }
        else
        {
            _sortColumnId  = columnId;
            _sortDirection = SortDirection.Ascending;
        }

        _paginationState.CurrentPage = 1;
        _focusedIndex = -1;
        SyncPaginationTotal();
        if (IsServerMode && _virtualizeRef is not null)
            InvokeAsync(_virtualizeRef.RefreshDataAsync);
        StateHasChanged();
    }

    private string GetSortIconName(string colId) =>
        _sortColumnId != colId ? "arrow-up-down" :
        _sortDirection == SortDirection.Ascending ? "arrow-up" : "arrow-down";

    private void SyncPaginationTotal() =>
        _paginationState.TotalItems = SortedItems.Count;

    // ── Pagination handlers ───────────────────────────────────────────────

    private Task HandlePageChanged(int _)
    {
        _focusedIndex = -1;
        StateHasChanged();
        return Task.CompletedTask;
    }

    private Task HandlePageSizeChanged(int _)
    {
        _focusedIndex = -1;
        StateHasChanged();
        return Task.CompletedTask;
    }

    // ── Selection ────────────────────────────────────────────────────────

    private object GetItemKey(TItem item) => ItemKey?.Invoke(item) ?? (object)item!;

    private bool IsSelected(TItem item) =>
        SelectionMode != DataViewSelectionMode.None && _selectedKeys.Contains(GetItemKey(item));

    private async Task HandleItemClick(TItem item, int index)
    {
        if (SelectionMode == DataViewSelectionMode.None) return;

        _focusedIndex = index;
        var key = GetItemKey(item);

        if (SelectionMode == DataViewSelectionMode.Single)
        {
            if (_selectedKeys.Contains(key))
            {
                _selectedKeys.Clear();
                SelectedItem = default;
                await SelectedItemChanged.InvokeAsync(default);
            }
            else
            {
                _selectedKeys.Clear();
                _selectedKeys.Add(key);
                SelectedItem = item;
                await SelectedItemChanged.InvokeAsync(item);
            }
        }
        else
        {
            if (!_selectedKeys.Add(key))
                _selectedKeys.Remove(key);

            var list = (IReadOnlyList<TItem>)AllItems
                .Where(i => _selectedKeys.Contains(GetItemKey(i)))
                .ToList();
            SelectedItems = list;
            await SelectedItemsChanged.InvokeAsync(list);
        }

        StateHasChanged();
    }

    // ── Keyboard navigation ───────────────────────────────────────────────

    private async Task HandleKeyDown(KeyboardEventArgs e)
    {
        if (SelectionMode == DataViewSelectionMode.None) return;
        if (PagedItems.Count == 0) return;

        _shouldPreventDefault = e.Key is "ArrowUp" or "ArrowDown" or "ArrowLeft" or "ArrowRight"
            or "Home" or "End" or " ";

        var orientation = _layout == DataViewLayout.Grid
            ? NeoUI.Blazor.Primitives.Orientation.Both
            : NeoUI.Blazor.Primitives.Orientation.Vertical;
        var offset = _navigator.HandleArrowNavigation(e, new NeoUI.Blazor.Primitives.NavigationOptions
        {
            Orientation = orientation,
            Loop = false
        });

        if (offset.HasValue)
        {
            _focusedIndex = _navigator.GetNextIndex(_focusedIndex, offset.Value, PagedItems.Count, loop: false);
            StateHasChanged();
            return;
        }

        if ((e.Key == " " || e.Key == "Enter") && _focusedIndex >= 0 && _focusedIndex < PagedItems.Count)
            await HandleItemClick(PagedItems[_focusedIndex], _focusedIndex);
    }

    private void HandleContainerFocus()
    {
        if (_focusedIndex >= 0) return;
        if (PagedItems.Count == 0) return;

        if (SelectionMode == DataViewSelectionMode.Single && SelectedItem is not null)
        {
            var key = GetItemKey(SelectedItem);
            var idx = PagedItems.ToList().FindIndex(i => GetItemKey(i).Equals(key));
            _focusedIndex = idx >= 0 ? idx : 0;
        }
        else
        {
            _focusedIndex = 0;
        }

        StateHasChanged();
    }

    private void HandleContainerBlur()
    {
        _focusedIndex = -1;
        StateHasChanged();
    }

    // ── CSS & icon helpers ────────────────────────────────────────────────

    private string GetListItemClass(TItem item, int index) => ClassNames.cn(
        "flex items-center transition-all outline-none border-2 border-transparent ",
        SelectionMode != DataViewSelectionMode.None ? "cursor-pointer select-none" : null,
        index < 0 ? "border-b border-border" : null,
        IsSelected(item) ? "bg-accent/50" : "hover:bg-muted/30",
        _focusedIndex == index && index >= 0 ? "border-2 border-primary rounded-sm" : null);

    private string GetGridItemClass(TItem item, int index) => ClassNames.cn(
        "relative cursor-pointer select-none rounded-lg transition-all outline-none border-2 border-transparent",
        IsSelected(item) ? "ring-2 ring-primary" : "hover:ring-1 hover:ring-border",
        _focusedIndex == index ? "border-2 border-primary" : null);

    private string GetCheckIconName =>
        CheckVariant == DataViewCheckVariant.Check ? "check" : "circle-check";

    private string GetCheckClass(TItem item, bool isGrid = false) =>
        IsSelected(item) ? "text-primary" :
        CheckVariant == DataViewCheckVariant.Check ? "invisible" :
        isGrid ? "text-muted-foreground/25" : "text-muted-foreground/20";

    private string ItemContainerId(int index) => $"dv-{_instanceId}-{index}";

    private string? ActiveDescendantId =>
        SelectionMode != DataViewSelectionMode.None && _focusedIndex >= 0
            ? ItemContainerId(_focusedIndex)
            : null;

    private string ContainerCssClass => ClassNames.cn("w-full", Class);

    private string GridCssClass => ClassNames.cn(
        "grid gap-4 focus:outline-none",
        !string.IsNullOrWhiteSpace(GridColumnMinWidth)
            ? BuildGridAutoFillClass(GridColumnMinWidth!)
            : GridColumns switch
            {
                1 => "grid-cols-1",
                2 => "grid-cols-1 sm:grid-cols-2",
                4 => "grid-cols-1 sm:grid-cols-2 lg:grid-cols-4",
                5 => "grid-cols-1 sm:grid-cols-2 lg:grid-cols-5",
                6 => "grid-cols-2 sm:grid-cols-3 lg:grid-cols-6",
                _ => "grid-cols-1 sm:grid-cols-2 lg:grid-cols-3"
            });

    private static string ListCssClass => "flex flex-col divide-y divide-border outline-none";

    /// <summary>
    /// Converts a CSS length or Tailwind spacing key into a <c>grid-auto-fill-*</c> class.
    /// Raw CSS values (e.g. "160px", "10rem") are wrapped in Tailwind arbitrary-value
    /// brackets; spacing keys (e.g. "40") and already-bracketed values pass through unchanged.
    /// </summary>
    private static string BuildGridAutoFillClass(string value)
    {
        var v = value.Trim();
        // Tailwind spacing key (pure integer) or already uses bracket notation → pass through
        if (int.TryParse(v, out _) || v.StartsWith('['))
            return $"grid-auto-fill-{v}";
        // CSS length (e.g. "160px", "10rem", "50%") → wrap in brackets
        return $"grid-auto-fill-[{v}]";
    }

    // ── Lifecycle ─────────────────────────────────────────────────────────

    protected override void OnParametersSet()
    {
        if (_layout == default) _layout = Layout;

        if (!_paginationInit && PageSize > 0)
        {
            _paginationInit = true;
            _paginationState.PageSize = PageSize;
        }

        _selectedKeys.Clear();
        switch (SelectionMode)
        {
            case DataViewSelectionMode.Single when SelectedItem is not null:
                _selectedKeys.Add(GetItemKey(SelectedItem));
                break;
            case DataViewSelectionMode.Multiple when SelectedItems is not null:
                foreach (var i in SelectedItems)
                    _selectedKeys.Add(GetItemKey(i));
                break;
        }

        SyncPaginationTotal();
    }
}
