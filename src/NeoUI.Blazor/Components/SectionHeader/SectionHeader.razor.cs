using Microsoft.AspNetCore.Components;

namespace NeoUI.Blazor;

/// <summary>
/// A section heading with an optional "view all" link and separator line.
/// </summary>
/// <remarks>
/// <para>
/// SectionHeader is a recurring layout primitive in mobile and content-heavy UIs:
/// a bold section title on the left with an optional chevron-right "See all" action on
/// the right, followed by a thin separator. It is used multiple times per screen in
/// typical e-commerce, news, and dashboard layouts.
/// </para>
/// <para>
/// While trivially composable from <c>Typography</c> + <c>Button</c> + <c>Separator</c>,
/// having it as a component eliminates boilerplate, enforces consistent spacing/typography,
/// and makes the intent explicit.
/// </para>
/// </remarks>
/// <example>
/// <code>
/// &lt;!-- Basic section header --&gt;
/// &lt;SectionHeader Title="Today's Promo" OnViewAll="NavigateToPromo" /&gt;
///
/// &lt;!-- With custom view-all text --&gt;
/// &lt;SectionHeader Title="Best Sellers" OnViewAll="NavigateToBestSellers" ViewAllText="See all" /&gt;
///
/// &lt;!-- No view-all link, no separator --&gt;
/// &lt;SectionHeader Title="Order Summary" ShowSeparator="false" /&gt;
///
/// &lt;!-- Custom title render fragment --&gt;
/// &lt;SectionHeader&gt;
///     &lt;TitleContent&gt;
///         &lt;span&gt;Flash Sale &lt;Badge Variant="BadgeVariant.Destructive"&gt;Ends 23:59&lt;/Badge&gt;&lt;/span&gt;
///     &lt;/TitleContent&gt;
/// &lt;/SectionHeader&gt;
/// </code>
/// </example>
public partial class SectionHeader : ComponentBase
{
    /// <summary>
    /// Gets or sets the section title text.
    /// Ignored when <see cref="TitleContent"/> is provided.
    /// </summary>
    [Parameter]
    public string? Title { get; set; }

    /// <summary>
    /// Gets or sets a custom title render fragment. Takes precedence over <see cref="Title"/>.
    /// </summary>
    [Parameter]
    public RenderFragment? TitleContent { get; set; }

    /// <summary>
    /// Gets or sets the callback fired when the "view all" button is clicked.
    /// The button is only rendered when this parameter is provided.
    /// </summary>
    [Parameter]
    public EventCallback OnViewAll { get; set; }

    /// <summary>
    /// Gets or sets the text shown in the "view all" button.
    /// When empty (default), only the chevron icon is shown.
    /// </summary>
    [Parameter]
    public string? ViewAllText { get; set; }

    /// <summary>
    /// Gets or sets the accessible label for the "view all" button.
    /// Defaults to "View all".
    /// </summary>
    [Parameter]
    public string ViewAllLabel { get; set; } = "View all";

    /// <summary>
    /// Gets or sets whether a <see cref="Separator"/> line is shown below the title row.
    /// Defaults to true.
    /// </summary>
    [Parameter]
    public bool ShowSeparator { get; set; } = true;

    /// <summary>
    /// Gets or sets additional CSS classes on the outer wrapper.
    /// </summary>
    [Parameter]
    public string? Class { get; set; }

    private string CssClass => ClassNames.cn("flex flex-col", Class);

    private string TitleCssClass => "text-[17px] font-semibold text-foreground";
}
