using BlazorUI.Components.Utilities;
using Microsoft.AspNetCore.Components;

namespace BlazorUI.Components.Empty;

/// <summary>
/// A description component for an empty state.
/// </summary>
public partial class EmptyDescription : ComponentBase
{
    /// <summary>
    /// Gets or sets additional CSS classes to apply to the empty description.
    /// </summary>
    [Parameter]
    public string? Class { get; set; }

    /// <summary>
    /// Gets or sets the child content to render inside the empty description.
    /// </summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    private string CssClass => ClassNames.cn(
        "mb-4 mt-2 text-sm text-muted-foreground",
        Class
    );
}
