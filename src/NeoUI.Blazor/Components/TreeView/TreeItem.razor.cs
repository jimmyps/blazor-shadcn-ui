using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace NeoUI.Blazor;

/// <summary>
/// Renders a single node inside a TreeView — supports nesting, checkboxes, icons, and drag-and-drop.
/// </summary>
/// <typeparam name="TItem">The type of data items in the tree.</typeparam>
public partial class TreeItem<TItem> : ComponentBase
{
    [CascadingParameter]
    public TreeView<TItem>? ParentTree { get; set; }

    // ── Item data ────────────────────────────────────────────────────

    /// <summary>The data item this node represents.</summary>
    [Parameter] public TItem Item { get; set; } = default!;

    /// <summary>Accessor to extract display text from the item.</summary>
    [Parameter] public Func<TItem, string>? TextField { get; set; }

    /// <summary>Accessor to extract the unique value/ID from the item.</summary>
    [Parameter] public Func<TItem, string>? ValueField { get; set; }

    /// <summary>Accessor to extract an icon name from the item.</summary>
    [Parameter] public Func<TItem, string?>? IconField { get; set; }

    /// <summary>Accessor to get child items.</summary>
    [Parameter] public Func<TItem, IEnumerable<TItem>>? ChildrenAccessor { get; set; }

    /// <summary>Accessor to determine if an item has children.</summary>
    [Parameter] public Func<TItem, bool>? HasChildrenAccessor { get; set; }

    /// <summary>Nesting depth (0 = root).</summary>
    [Parameter] public int Depth { get; set; }

    /// <summary>Whether to show vertical connection lines.</summary>
    [Parameter] public bool ShowLines { get; set; }

    /// <summary>Whether to show checkboxes.</summary>
    [Parameter] public bool Checkable { get; set; }

    /// <summary>Whether drag-and-drop is enabled.</summary>
    [Parameter] public bool Draggable { get; set; }

    // ── Computed ────────────────────────────────────────────────────

    private string  ItemValue   => ValueField?.Invoke(Item) ?? string.Empty;
    private string  Label       => TextField?.Invoke(Item) ?? string.Empty;
    private string? IconName    => IconField?.Invoke(Item);
    private bool    IsExpanded  => ParentTree?.IsExpanded(ItemValue) ?? false;
    private bool    IsSelected  => ParentTree?.IsSelected(ItemValue) ?? false;
    private bool    IsChecked   => ParentTree?.CheckedValues?.Contains(ItemValue) ?? false;

    private IEnumerable<TItem> Children    => ChildrenAccessor?.Invoke(Item) ?? [];
    private bool               HasChildren => HasChildrenAccessor?.Invoke(Item) ?? Children.Any();

    // ── Handlers ────────────────────────────────────────────────────

    private async Task HandleClick(MouseEventArgs _)
    {
        if (ParentTree is not null)
            await ParentTree.HandleNodeClick(ItemValue);
        if (HasChildren)
            await ParentTree!.HandleNodeToggle(ItemValue, !IsExpanded);
    }

    private async Task HandleToggle(MouseEventArgs _)
    {
        if (ParentTree is not null && HasChildren)
            await ParentTree.HandleNodeToggle(ItemValue, !IsExpanded);
    }

    private async Task HandleCheck(ChangeEventArgs e)
    {
        if (ParentTree?.CheckedValues is null) return;
        var isChecked = e.Value is true;
        if (isChecked)
            ParentTree.CheckedValues.Add(ItemValue);
        else
            ParentTree.CheckedValues.Remove(ItemValue);
        await ParentTree.CheckedValuesChanged.InvokeAsync(ParentTree.CheckedValues);
        StateHasChanged();
    }

    // ── CSS ──────────────────────────────────────────────────────────

    private string ItemCssClass => ClassNames.cn(
        "w-full",
        ShowLines && Depth > 0 ? "border-l border-border ml-3.5" : null
    );

    private string RowCssClass => ClassNames.cn(
        "flex items-center gap-0.5 rounded-md px-1 py-1 cursor-pointer select-none",
        "hover:bg-accent hover:text-accent-foreground transition-colors",
        IsSelected ? "bg-accent text-accent-foreground" : null
    );

    private string ToggleCssClass => "flex items-center justify-center w-4 h-4 shrink-0";
    private string IndentClass    => "w-4 shrink-0";

    private string ChildGroupClass => ClassNames.cn(
        "pl-4",
        ShowLines ? "border-l border-border ml-3.5" : null
    );
}
