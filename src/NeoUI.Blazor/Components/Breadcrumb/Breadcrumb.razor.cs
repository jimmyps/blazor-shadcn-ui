using Microsoft.AspNetCore.Components;

namespace NeoUI.Blazor;

/// <summary>
/// A breadcrumb component that displays page hierarchy for navigation.
/// </summary>
/// <remarks>
/// <para>
/// The Breadcrumb component provides navigational hierarchy with semantic markup
/// for accessibility. It follows the shadcn/ui design system.
/// </para>
/// <para>
/// Features:
/// - Semantic HTML with nav and aria-label for screen readers
/// - Composable with BreadcrumbList, BreadcrumbItem, BreadcrumbLink, BreadcrumbSeparator
/// - Ellipsis support for long paths
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
///             &lt;BreadcrumbLink Href="/products"&gt;Products&lt;/BreadcrumbLink&gt;
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
    /// Gets or sets the aria-label for the breadcrumb navigation.
    /// </summary>
    /// <remarks>
    /// Provides accessible label for screen readers.
    /// When null, falls back to the localizer value for "Breadcrumb.Breadcrumb".
    /// </remarks>
    [Parameter]
    public string? AriaLabel { get; set; }

    [Inject]
    private ILocalizer Localizer { get; set; } = default!;

    private string EffectiveAriaLabel => AriaLabel ?? Localizer["Breadcrumb.Breadcrumb"];

    /// <summary>
    /// Gets or sets additional CSS classes to apply to the breadcrumb.
    /// </summary>
    [Parameter]
    public string? Class { get; set; }

    /// <summary>
    /// Gets or sets the content to be rendered inside the breadcrumb.
    /// </summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    /// <summary>
    /// Gets the computed CSS classes for the breadcrumb element.
    /// </summary>
    private string CssClass => ClassNames.cn(Class);
}
