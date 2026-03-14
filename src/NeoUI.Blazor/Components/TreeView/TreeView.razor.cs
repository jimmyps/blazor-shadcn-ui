using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace NeoUI.Blazor;

/// <summary>
/// A tree view component for hierarchical data, supporting single/multi selection,
/// checkboxes, keyboard navigation, and drag-and-drop reordering.
/// </summary>
/// <typeparam name="TItem">The type of data items in the tree.</typeparam>
/// <example>
/// <code>
/// &lt;TreeView Items="@_nodes" ValueField="n => n.Id" TextField="n => n.Name"
///           ChildrenProperty="n => n.Children"
///           SelectionMode="TreeSelectionMode.Single"
///           @bind-SelectedValue="_selected" /&gt;
/// </code>
/// </example>
public partial class TreeView<TItem> : ComponentBase, IAsyncDisposable
{
    private readonly HashSet<string> _expandedInternal = new();
    private readonly string _instanceId = Guid.NewGuid().ToString("N")[..8];
    private ElementReference _treeRef;
    private IJSObjectReference? _jsModule;
    private DotNetObjectReference<TreeView<TItem>>? _dotNetRef;
    private bool _jsDragInitialized;
    private bool _disposed;

    // Internal state for LoadChildrenAsync
    private readonly Dictionary<string, List<TItem>> _fetchedChildren = new();
    private readonly HashSet<string> _internalLoadingNodes = new();
    private readonly HashSet<string> _internalErrorNodes   = new();
    private readonly HashSet<string> _attemptedNodes       = new();

    [Inject] private IJSRuntime JSRuntime { get; set; } = null!;

    // ── Data-driven parameters ────────────────────────────────────────

    /// <summary>Gets or sets the data source for data-driven mode.</summary>
    [Parameter] public IEnumerable<TItem>? Items { get; set; }

    /// <summary>Gets or sets a function to extract child items from a parent (nested mode).</summary>
    [Parameter] public Func<TItem, IEnumerable<TItem>?>? ChildrenProperty { get; set; }

    /// <summary>Gets or sets a function to extract the parent ID from an item (flat mode).</summary>
    [Parameter] public Func<TItem, string?>? ParentField { get; set; }

    /// <summary>Gets or sets a function to extract display text from an item.</summary>
    [Parameter] public Func<TItem, string>? TextField { get; set; }

    /// <summary>Gets or sets a function to extract the unique value/ID from an item.</summary>
    [Parameter] public Func<TItem, string>? ValueField { get; set; }

    /// <summary>Gets or sets a function to extract an icon name from an item.</summary>
    [Parameter] public Func<TItem, string?>? IconField { get; set; }

    /// <summary>Gets or sets a function to determine if an item has children (for lazy load).</summary>
    [Parameter] public Func<TItem, bool>? HasChildrenField { get; set; }

    // ── Selection ────────────────────────────────────────────────────

    /// <summary>Gets or sets the selected value in single selection mode.</summary>
    [Parameter] public string? SelectedValue { get; set; }
    [Parameter] public EventCallback<string?> SelectedValueChanged { get; set; }

    /// <summary>Gets or sets the selected values in multiple selection mode.</summary>
    [Parameter] public HashSet<string>? SelectedValues { get; set; }
    [Parameter] public EventCallback<HashSet<string>> SelectedValuesChanged { get; set; }

    /// <summary>Gets or sets the selection mode.</summary>
    [Parameter] public TreeSelectionMode SelectionMode { get; set; } = TreeSelectionMode.None;

    // ── Expand/Collapse ───────────────────────────────────────────────

    /// <summary>Gets or sets the set of expanded node values (controlled mode).</summary>
    [Parameter] public HashSet<string>? ExpandedValues { get; set; }
    [Parameter] public EventCallback<HashSet<string>> ExpandedValuesChanged { get; set; }

    /// <summary>Gets or sets whether to expand all nodes on first render.</summary>
    [Parameter] public bool DefaultExpandAll { get; set; }

    /// <summary>Gets or sets the depth to auto-expand on first render (1 = root children only).</summary>
    [Parameter] public int? DefaultExpandDepth { get; set; }

    // ── Checkboxes ───────────────────────────────────────────────────

    /// <summary>Gets or sets whether nodes display checkboxes.</summary>
    [Parameter] public bool Checkable { get; set; }

    /// <summary>Gets or sets the checked values.</summary>
    [Parameter] public HashSet<string>? CheckedValues { get; set; }
    [Parameter] public EventCallback<HashSet<string>> CheckedValuesChanged { get; set; }

    /// <summary>
    /// When true, checking a parent node checks all its descendants and vice-versa.
    /// Ancestor nodes automatically show an indeterminate state when only some descendants are checked.
    /// </summary>
    [Parameter] public bool PropagateChecks { get; set; }

    // ── Features ─────────────────────────────────────────────────────

    /// <summary>Gets or sets whether to show vertical connecting lines between sibling nodes.</summary>
    [Parameter] public bool ShowLines { get; set; }

    /// <summary>Gets or sets whether nodes can be dragged to reorder.</summary>
    [Parameter] public bool Draggable { get; set; }

    /// <summary>Optional predicate to restrict which nodes can be dragged. Receives the item; return true to allow.</summary>
    [Parameter] public Func<TItem, bool>? AllowDrag { get; set; }

    /// <summary>Search text used to filter and highlight nodes. Nodes whose label doesn't match are hidden.</summary>
    [Parameter] public string? SearchText { get; set; }

    /// <summary>Set of node values currently loading children asynchronously.</summary>
    [Parameter] public HashSet<string> LoadingNodes { get; set; } = new();

    /// <summary>Set of node values that failed to load children.</summary>
    [Parameter] public HashSet<string> ErrorNodes { get; set; } = new();

    /// <summary>Callback invoked when the user clicks Retry on a failed node.</summary>
    [Parameter] public EventCallback<string> OnRetryLoad { get; set; }

    /// <summary>
    /// When set, the tree calls this function on first expand of each node and manages
    /// loading/error state internally. Throw any exception to put the node into error state.
    /// </summary>
    [Parameter] public Func<TItem, Task<IEnumerable<TItem>>>? LoadChildrenAsync { get; set; }

    /// <summary>Callback invoked when a node is dropped. Receives (sourceValue, targetValue, position).</summary>
    [Parameter] public EventCallback<(string Source, string Target, string Position)> OnNodeDrop { get; set; }

    /// <summary>Callback invoked when a node is clicked.</summary>
    [Parameter] public EventCallback<string> OnNodeClick { get; set; }

    /// <summary>Callback invoked when a node is expanded.</summary>
    [Parameter] public EventCallback<string> OnNodeExpand { get; set; }

    /// <summary>Callback invoked when a node is collapsed.</summary>
    [Parameter] public EventCallback<string> OnNodeCollapse { get; set; }

    // ── Layout ────────────────────────────────────────────────────────

    /// <summary>Declarative child content (TreeItem elements).</summary>
    [Parameter] public RenderFragment? ChildContent { get; set; }

    /// <summary>ARIA label for the tree container.</summary>
    [Parameter] public string? AriaLabel { get; set; }

    /// <summary>Gets or sets additional CSS classes.</summary>
    [Parameter] public string? Class { get; set; }

    [Parameter(CaptureUnmatchedValues = true)]
    public Dictionary<string, object>? AdditionalAttributes { get; set; }

    // ── Internal state ───────────────────────────────────────────────

    internal HashSet<string> EffectiveExpandedValues =>
        ExpandedValues ?? _expandedInternal;

    // Merge external + internal loading/error sets so both APIs work together
    internal HashSet<string> MergedLoadingNodes =>
        LoadChildrenAsync is not null && _internalLoadingNodes.Count > 0
            ? [.. LoadingNodes, .. _internalLoadingNodes]
            : LoadingNodes;

    internal HashSet<string> MergedErrorNodes =>
        LoadChildrenAsync is not null && _internalErrorNodes.Count > 0
            ? [.. ErrorNodes, .. _internalErrorNodes]
            : ErrorNodes;

    // ── Lifecycle ────────────────────────────────────────────────────

    protected override void OnParametersSet()
    {
        if (DefaultExpandAll && ExpandedValues is null && _expandedInternal.Count == 0)
            ExpandAll();
        else if (DefaultExpandDepth.HasValue && ExpandedValues is null && _expandedInternal.Count == 0)
            ExpandToDepth(DefaultExpandDepth.Value);
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender && Draggable)
            await InitDragDrop();
    }

    // ── Public API ───────────────────────────────────────────────────

    internal async Task HandleNodeClick(string value)
    {
        await OnNodeClick.InvokeAsync(value);
        if (SelectionMode == TreeSelectionMode.Single)
        {
            SelectedValue = value;
            await SelectedValueChanged.InvokeAsync(value);
        }
        else if (SelectionMode == TreeSelectionMode.Multiple)
        {
            var set = SelectedValues ?? new HashSet<string>();
            if (!set.Add(value)) set.Remove(value);
            SelectedValues = set;
            await SelectedValuesChanged.InvokeAsync(set);
        }
        StateHasChanged();
    }

    internal async Task HandleNodeToggle(string value, bool expanding)
    {
        var set = ExpandedValues ?? _expandedInternal;
        if (expanding)
        {
            set.Add(value);
            await OnNodeExpand.InvokeAsync(value);

            // Trigger internal async fetch if not yet loaded/loading
            if (LoadChildrenAsync is not null
                && !_fetchedChildren.ContainsKey(value)
                && !_internalLoadingNodes.Contains(value)
                && !_internalErrorNodes.Contains(value))
            {
                await FetchChildrenInternalAsync(value);
                return; // StateHasChanged already called inside
            }
        }
        else
        {
            set.Remove(value);
            await OnNodeCollapse.InvokeAsync(value);
        }
        if (ExpandedValues is not null)
            await ExpandedValuesChanged.InvokeAsync(set);
        StateHasChanged();
    }

    private async Task FetchChildrenInternalAsync(string value)
    {
        _attemptedNodes.Add(value);
        _internalLoadingNodes.Add(value);
        StateHasChanged();

        try
        {
            var item = FindItem(value);
            if (item is not null && LoadChildrenAsync is not null)
            {
                var children = await LoadChildrenAsync(item);
                _fetchedChildren[value] = children.ToList();
            }
            _internalLoadingNodes.Remove(value);
        }
        catch
        {
            _internalLoadingNodes.Remove(value);
            _internalErrorNodes.Add(value);
        }

        if (ExpandedValues is not null)
            await ExpandedValuesChanged.InvokeAsync(ExpandedValues ?? _expandedInternal);
        StateHasChanged();
    }

    /// <summary>
    /// When <see cref="LoadChildrenAsync"/> is set, clears the error state and re-fetches children.
    /// Otherwise fires <see cref="OnRetryLoad"/> as usual.
    /// </summary>
    internal async Task HandleRetryInternal(string value)
    {
        if (LoadChildrenAsync is not null && _internalErrorNodes.Contains(value))
        {
            _internalErrorNodes.Remove(value);
            _attemptedNodes.Remove(value);
            await FetchChildrenInternalAsync(value);
        }
        else if (OnRetryLoad.HasDelegate)
        {
            await OnRetryLoad.InvokeAsync(value);
        }
    }

    internal bool IsExpanded(string value) => EffectiveExpandedValues.Contains(value);
    internal bool IsSelected(string value) =>
        SelectionMode == TreeSelectionMode.Single
            ? SelectedValue == value
            : SelectedValues?.Contains(value) == true;

    /// <summary>Returns the effective check state for a node, accounting for PropagateChecks.</summary>
    internal CheckStateKind GetCheckState(string value)
    {
        if (CheckedValues is null) return CheckStateKind.Unchecked;

        if (!PropagateChecks)
            return CheckedValues.Contains(value) ? CheckStateKind.Checked : CheckStateKind.Unchecked;

        var item = FindItem(value);
        if (item is null)
            return CheckedValues.Contains(value) ? CheckStateKind.Checked : CheckStateKind.Unchecked;

        var children = GetChildren(item).ToList();
        if (children.Count == 0)
            return CheckedValues.Contains(value) ? CheckStateKind.Checked : CheckStateKind.Unchecked;

        var checkedCount = children.Count(c => GetCheckState(ValueField!(c)) == CheckStateKind.Checked);
        if (checkedCount == children.Count) return CheckStateKind.Checked;
        if (checkedCount > 0 || children.Any(c => GetCheckState(ValueField!(c)) == CheckStateKind.Indeterminate))
            return CheckStateKind.Indeterminate;
        return CheckStateKind.Unchecked;
    }

    internal async Task HandleNodeCheck(string value, bool isChecked)
    {
        var set = CheckedValues ?? new HashSet<string>();

        if (isChecked) set.Add(value);
        else set.Remove(value);

        if (PropagateChecks)
        {
            var item = FindItem(value);
            if (item is not null)
            {
                foreach (var desc in GetAllDescendantValues(item))
                {
                    if (isChecked) set.Add(desc);
                    else set.Remove(desc);
                }
            }
        }

        CheckedValues ??= set;
        await CheckedValuesChanged.InvokeAsync(set);
        StateHasChanged();
    }

    private IEnumerable<string> GetAllDescendantValues(TItem item)
    {
        foreach (var child in GetChildren(item))
        {
            yield return ValueField!(child);
            foreach (var desc in GetAllDescendantValues(child))
                yield return desc;
        }
    }

    // ── JS drag-and-drop ─────────────────────────────────────────────

    [JSInvokable]
    public async Task JsOnNodeDrop(string source, string target, string position)
    {
        if (!_disposed)
            await OnNodeDrop.InvokeAsync((source, target, position));
    }

    private async Task InitDragDrop()
    {
        try
        {
            _jsModule = await JSRuntime.InvokeAsync<IJSObjectReference>(
                "import", "./_content/NeoUI.Blazor/js/tree-view.js");
            _dotNetRef = DotNetObjectReference.Create(this);
            await _jsModule.InvokeVoidAsync("initDragDrop", _treeRef, _dotNetRef, _instanceId);
            _jsDragInitialized = true;
        }
        catch (Exception ex) when (ex is JSDisconnectedException or TaskCanceledException or ObjectDisposedException or InvalidOperationException)
        { _ = ex; }
    }

    // ── Data helpers ─────────────────────────────────────────────────

    internal bool IsFilteredOut(TItem item)
    {
        if (string.IsNullOrWhiteSpace(SearchText) || TextField is null) return false;
        var label = TextField(item);
        if (label.Contains(SearchText, StringComparison.OrdinalIgnoreCase)) return false;
        return !HasMatchingDescendant(item);
    }

    private bool HasMatchingDescendant(TItem item)
    {
        foreach (var child in GetChildren(item))
        {
            var label = TextField?.Invoke(child) ?? string.Empty;
            if (label.Contains(SearchText!, StringComparison.OrdinalIgnoreCase)) return true;
            if (HasMatchingDescendant(child)) return true;
        }
        return false;
    }

    internal IEnumerable<TItem> GetRootItems()
    {
        if (Items is null) return [];
        if (ChildrenProperty is not null) return Items;
        if (ParentField is not null)
            return Items.Where(i => string.IsNullOrEmpty(ParentField(i)));
        return Items;
    }

    internal IEnumerable<TItem> GetChildren(TItem item)
    {
        if (ValueField is not null && _fetchedChildren.TryGetValue(ValueField(item), out var fetched))
            return fetched;
        if (ChildrenProperty is not null)
            return ChildrenProperty(item) ?? [];
        if (ParentField is not null && ValueField is not null)
        {
            var parentId = ValueField(item);
            return Items?.Where(i => ParentField(i) == parentId) ?? [];
        }
        return [];
    }

    internal bool GetHasChildren(TItem item)
    {
        if (HasChildrenField is not null) return HasChildrenField(item);
        if (ValueField is not null && _fetchedChildren.TryGetValue(ValueField(item), out var fetched))
            return fetched.Count > 0;
        if (LoadChildrenAsync is not null) return true; // assume expandable until proven otherwise
        return GetChildren(item).Any();
    }

    private TItem? FindItem(string value)
    {
        if (Items is null || ValueField is null) return default;
        return FindItemRecursive(Items, value);
    }

    private TItem? FindItemRecursive(IEnumerable<TItem> items, string value)
    {
        foreach (var item in items)
        {
            if (ValueField!(item) == value) return item;
            var found = FindItemRecursive(GetChildren(item), value);
            if (found is not null) return found;
        }
        return default;
    }

    private void ExpandAll()
    {
        if (Items is null || ValueField is null) return;
        foreach (var item in GetRootItems())
            ExpandRecursive(item);
    }

    private void ExpandRecursive(TItem item)
    {
        _expandedInternal.Add(ValueField!(item));
        foreach (var child in GetChildren(item))
            ExpandRecursive(child);
    }

    private void ExpandToDepth(int maxDepth)
    {
        if (Items is null || ValueField is null) return;
        foreach (var item in GetRootItems())
            ExpandToDepthRecursive(item, 0, maxDepth);
    }

    private void ExpandToDepthRecursive(TItem item, int currentDepth, int maxDepth)
    {
        if (currentDepth >= maxDepth) return;
        _expandedInternal.Add(ValueField!(item));
        foreach (var child in GetChildren(item))
            ExpandToDepthRecursive(child, currentDepth + 1, maxDepth);
    }

    // ── CSS ───────────────────────────────────────────────────────────

    private string CssClass => ClassNames.cn("w-full text-sm", Class);

    // ── IAsyncDisposable ─────────────────────────────────────────────

    public async ValueTask DisposeAsync()
    {
        _disposed = true;
        GC.SuppressFinalize(this);
        if (_jsModule is not null && _jsDragInitialized)
        {
            try
            {
                await _jsModule.InvokeVoidAsync("disposeDragDrop", _instanceId);
                await _jsModule.DisposeAsync();
            }
            catch (Exception ex) when (ex is JSDisconnectedException or TaskCanceledException or ObjectDisposedException or InvalidOperationException) { _ = ex; }
        }
        _dotNetRef?.Dispose();
    }
}
