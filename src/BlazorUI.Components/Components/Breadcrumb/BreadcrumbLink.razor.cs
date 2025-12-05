using BlazorUI.Components.Utilities;
using Microsoft.AspNetCore.Components;

namespace BlazorUI.Components.Breadcrumb;

/// <summary>
/// A link component within a breadcrumb trail.
/// </summary>
/// <remarks>
/// BreadcrumbLink renders a clickable anchor element for navigation
/// to parent pages in the hierarchy.
/// </remarks>
public partial class BreadcrumbLink : ComponentBase
{
    /// <summary>
    /// Gets or sets the URL that the link navigates to.
    /// </summary>
    [Parameter]
    public string? Href { get; set; }

    /// <summary>
    /// Gets or sets additional CSS classes to apply to the link.
    /// </summary>
    [Parameter]
    public string? Class { get; set; }

    /// <summary>
    /// Gets or sets the content to be rendered inside the link.
    /// </summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    /// <summary>
    /// Gets or sets additional attributes to apply to the anchor element.
    /// </summary>
    [Parameter(CaptureUnmatchedValues = true)]
    public Dictionary<string, object>? AdditionalAttributes { get; set; }

    /// <summary>
    /// Gets the computed CSS classes for the anchor element.
    /// </summary>
    private string CssClass => ClassNames.cn(
        "transition-colors hover:text-foreground",
        Class
    );
}
