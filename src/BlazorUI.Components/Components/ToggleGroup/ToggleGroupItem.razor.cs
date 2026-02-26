using BlazorUI.Components.Utilities;
using Microsoft.AspNetCore.Components;

namespace BlazorUI.Components.ToggleGroup;

/// <summary>
/// An item component for a toggle group.
/// </summary>
public partial class ToggleGroupItem : ComponentBase
{
    [CascadingParameter]
    private ToggleGroup? ParentGroup { get; set; }

    /// <summary>
    /// Gets or sets the value of this toggle item.
    /// </summary>
    [Parameter]
    public string Value { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets whether this item is disabled.
    /// </summary>
    [Parameter]
    public bool Disabled { get; set; }

    /// <summary>
    /// Gets or sets additional CSS classes to apply to the toggle item.
    /// </summary>
    [Parameter]
    public string? Class { get; set; }

    /// <summary>
    /// Gets or sets the content to be rendered inside the toggle item.
    /// </summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    private bool IsSelected => ParentGroup?.IsItemSelected(Value) ?? false;

    private async Task OnClickAsync()
    {
        if (!Disabled && ParentGroup != null)
        {
            await ParentGroup.OnItemSelectedAsync(Value);
        }
    }

    private string CssClass => ClassNames.cn(
        "inline-flex items-center justify-center whitespace-nowrap rounded-md px-3 text-sm font-medium ring-offset-background transition-all",
        "focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-ring focus-visible:ring-offset-2",
        "disabled:pointer-events-none disabled:opacity-50",
        "hover:bg-muted hover:text-muted-foreground",
        "h-10",
        IsSelected ? "bg-accent text-accent-foreground" : "bg-transparent",
        Class
    );
}
