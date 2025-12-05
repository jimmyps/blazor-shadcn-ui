using BlazorUI.Components.Utilities;
using Microsoft.AspNetCore.Components;

namespace BlazorUI.Components.Breadcrumb;

/// <summary>
/// A breadcrumb component for displaying hierarchical navigation.
/// </summary>
/// <remarks>
/// <para>
/// The Breadcrumb component provides a navigation aid showing the user's
/// current location within a site's hierarchy. It follows the shadcn/ui
/// design system and accessibility best practices.
/// </para>
/// <para>
/// Features:
/// - Semantic HTML with nav element and aria-label
/// - Support for custom separators
/// - Accessible with proper ARIA attributes
/// - RTL (Right-to-Left) support
/// - Dark mode compatible via CSS variables
/// </para>
/// </remarks>
/// <example>
/// <code>
/// &lt;Breadcrumb&gt;
///     &lt;BreadcrumbList&gt;
///         &lt;BreadcrumbItem&gt;
///             &lt;BreadcrumbLink Href="/"&gt;Home&lt;/BreadcrumbLink&gt;
///         &lt;/BreadcrumbItem&gt;
///         &lt;BreadcrumbSeparator /&gt;
///         &lt;BreadcrumbItem&gt;
///             &lt;BreadcrumbPage&gt;Current Page&lt;/BreadcrumbPage&gt;
///         &lt;/BreadcrumbItem&gt;
///     &lt;/BreadcrumbList&gt;
/// &lt;/Breadcrumb&gt;
/// </code>
/// </example>
public partial class Breadcrumb : ComponentBase
{
    /// <summary>
    /// Gets or sets additional CSS classes to apply to the breadcrumb nav element.
    /// </summary>
    [Parameter]
    public string? Class { get; set; }

    /// <summary>
    /// Gets or sets the content to be rendered inside the breadcrumb.
    /// </summary>
    /// <remarks>
    /// Should contain a BreadcrumbList with BreadcrumbItem, BreadcrumbLink,
    /// BreadcrumbPage, and BreadcrumbSeparator components.
    /// </remarks>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    /// <summary>
    /// Gets the computed CSS classes for the nav element.
    /// </summary>
    private string CssClass => ClassNames.cn(Class);
}
