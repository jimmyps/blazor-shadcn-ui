using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace NeoUI.Blazor;

/// <summary>
/// Internal component for recursively rendering data-driven tree items with support for
/// search highlighting, node filtering, per-node loading/error states, and indeterminate checkboxes.
/// Used by <see cref="TreeView{TItem}"/> in data-driven mode.
/// </summary>
public partial class TreeItemNode<TItem> : ComponentBase
{
    [CascadingParameter]
    public TreeView<TItem>? ParentTree { get; set; }

    [Parameter, EditorRequired]
    public TItem Item { get; set; } = default!;

    [Parameter, EditorRequired]
    public Func<TItem, string> TextField { get; set; } = null!;

    [Parameter, EditorRequired]
    public Func<TItem, string> ValueField { get; set; } = null!;

    [Parameter]
    public Func<TItem, string?>? IconField { get; set; }

    [Parameter, EditorRequired]
    public Func<TItem, IEnumerable<TItem>> ChildrenAccessor { get; set; } = null!;

    [Parameter, EditorRequired]
    public Func<TItem, bool> HasChildrenAccessor { get; set; } = null!;

    /// <summary>Predicate that returns true when a node should be hidden (used for search filtering).</summary>
    [Parameter, EditorRequired]
    public Func<TItem, bool> IsFilteredOut { get; set; } = null!;

    /// <summary>Text to highlight within node labels during search.</summary>
    [Parameter]
    public string? SearchText { get; set; }

    [Parameter]
    public bool ShowIcons { get; set; } = true;

    [Parameter]
    public bool Checkable { get; set; }

    [Parameter]
    public bool Draggable { get; set; }

    [Parameter]
    public Func<TItem, bool>? AllowDrag { get; set; }

    [Parameter]
    public int Depth { get; set; }

    /// <summary>Set of node values currently in a loading state (async lazy load).</summary>
    [Parameter]
    public HashSet<string> LoadingNodes { get; set; } = new();

    /// <summary>Set of node values that failed to load children.</summary>
    [Parameter]
    public HashSet<string> ErrorNodes { get; set; } = new();

    [Parameter]
    public EventCallback<string> OnRetryLoad { get; set; }

    [Parameter]
    public bool ShowLines { get; set; }

    private ElementReference _checkboxRef; // kept for potential future JS use

    // ── Computed ─────────────────────────────────────────────────────────

    private string NodeValue  => ValueField(Item);
    private string LabelValue => TextField(Item);
    private string? IconValue => IconField?.Invoke(Item);

    private bool HasChildren  => HasChildrenAccessor(Item);
    private bool IsLoading    => LoadingNodes.Contains(NodeValue);
    private bool HasError     => ErrorNodes.Contains(NodeValue);
    private bool IsExpanded   => ParentTree?.IsExpanded(NodeValue) ?? false;
    private bool IsSelected   => ParentTree?.IsSelected(NodeValue) ?? false;
    private bool IsDraggable  => Draggable && (AllowDrag == null || AllowDrag(Item));

    private CheckStateKind CheckState =>
        ParentTree?.GetCheckState(NodeValue) ?? CheckStateKind.Unchecked;

    private bool IsChecked       => CheckState == CheckStateKind.Checked;
    private bool IsIndeterminate => CheckState == CheckStateKind.Indeterminate;

    private IEnumerable<TItem> Children => ChildrenAccessor(Item);

    // ── Handlers ─────────────────────────────────────────────────────────

    private async Task HandleClick(MouseEventArgs _)
    {
        if (ParentTree is not null)
            await ParentTree.HandleNodeClick(NodeValue);
        if (HasChildren)
            await ParentTree!.HandleNodeToggle(NodeValue, !IsExpanded);
    }

    private async Task HandleToggle(MouseEventArgs _)
    {
        if (ParentTree is not null && HasChildren)
            await ParentTree.HandleNodeToggle(NodeValue, !IsExpanded);
    }

    private async Task HandleCheckClick()
    {
        if (ParentTree is null) return;
        // Indeterminate → checked; checked → unchecked; unchecked → checked
        var newChecked = CheckState != CheckStateKind.Checked;
        await ParentTree.HandleNodeCheck(NodeValue, newChecked);
    }

    private async Task HandleRetry()
    {
        if (OnRetryLoad.HasDelegate)
            await OnRetryLoad.InvokeAsync(NodeValue);
    }

    // ── CSS ──────────────────────────────────────────────────────────────

    private string NodeCssClass => ClassNames.cn(
        "flex items-center gap-0.5 rounded-md px-1 py-1 cursor-pointer select-none",
        "hover:bg-accent hover:text-accent-foreground transition-colors",
        IsSelected ? "bg-accent text-accent-foreground" : null
    );

    private string ChevronCssClass => ClassNames.cn(
        "transition-transform duration-150",
        IsExpanded ? "rotate-90" : null
    );

    private string ChevronWrapCssClass =>
        "rounded-sm hover:bg-accent/80";

    private string CheckboxCssClass => ClassNames.cn(
        "h-3.5 w-3.5 rounded-sm border cursor-pointer flex items-center justify-center transition-colors",
        IsChecked || IsIndeterminate
            ? "bg-primary border-primary text-primary-foreground"
            : "border-border hover:border-primary bg-background"
    );

    // ── Search highlight ─────────────────────────────────────────────────

    private RenderFragment HighlightedLabel => builder =>
    {
        var text = LabelValue;
        if (string.IsNullOrEmpty(SearchText))
        {
            builder.OpenElement(0, "span");
            builder.AddAttribute(1, "class", "truncate flex-1");
            builder.AddContent(2, text);
            builder.CloseElement();
            return;
        }

        var index = text.IndexOf(SearchText, StringComparison.OrdinalIgnoreCase);
        if (index < 0)
        {
            builder.OpenElement(0, "span");
            builder.AddAttribute(1, "class", "truncate flex-1");
            builder.AddContent(2, text);
            builder.CloseElement();
            return;
        }

        builder.OpenElement(0, "span");
        builder.AddAttribute(1, "class", "truncate flex-1");

        if (index > 0)
            builder.AddContent(2, text[..index]);

        builder.OpenElement(3, "mark");
        builder.AddAttribute(4, "class", "bg-yellow-200 dark:bg-yellow-900/50 rounded-sm");
        builder.AddContent(5, text.Substring(index, SearchText.Length));
        builder.CloseElement();

        if (index + SearchText.Length < text.Length)
            builder.AddContent(6, text[(index + SearchText.Length)..]);

        builder.CloseElement();
    };
}
