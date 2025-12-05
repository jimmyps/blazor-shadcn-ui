using BlazorUI.Components.Utilities;
using Microsoft.AspNetCore.Components;

namespace BlazorUI.Components.Breadcrumb;

/// <summary>
/// A list container for breadcrumb items.
/// </summary>
/// <remarks>
/// BreadcrumbList wraps the breadcrumb items in an ordered list (ol)
/// with proper flexbox styling for horizontal layout.
/// </remarks>
public partial class BreadcrumbList : ComponentBase
{
    /// <summary>
    /// Gets or sets additional CSS classes to apply to the list.
    /// </summary>
    [Parameter]
    public string? Class { get; set; }

    /// <summary>
    /// Gets or sets the content to be rendered inside the list.
    /// </summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    /// <summary>
    /// Gets the computed CSS classes for the ol element.
    /// </summary>
    private string CssClass => ClassNames.cn(
        "flex flex-wrap items-center gap-1.5 break-words text-sm text-muted-foreground sm:gap-2.5",
        Class
    );
}
