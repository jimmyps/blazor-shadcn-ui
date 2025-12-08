using BlazorUI.Components.Utilities;
using Microsoft.AspNetCore.Components;

namespace BlazorUI.Components.Typography;

/// <summary>
/// A typography component for semantic text styling.
/// </summary>
/// <remarks>
/// <para>
/// The Typography component provides consistent text styling across the application
/// following the shadcn/ui design system.
/// </para>
/// <para>
/// Features:
/// - Multiple semantic variants (H1-H4, P, Blockquote, Code, etc.)
/// - Consistent typography scale
/// - Accessible semantic HTML
/// - Dark mode compatible via CSS variables
/// </para>
/// </remarks>
/// <example>
/// <code>
/// &lt;Typography Variant="TypographyVariant.H1"&gt;Main Heading&lt;/Typography&gt;
/// &lt;Typography Variant="TypographyVariant.P"&gt;Body text&lt;/Typography&gt;
/// &lt;Typography Variant="TypographyVariant.Muted"&gt;Secondary text&lt;/Typography&gt;
/// </code>
/// </example>
public partial class Typography : ComponentBase
{
    /// <summary>
    /// Gets or sets the typography variant.
    /// </summary>
    [Parameter]
    public TypographyVariant Variant { get; set; } = TypographyVariant.P;

    /// <summary>
    /// Gets or sets additional CSS classes to apply to the typography element.
    /// </summary>
    [Parameter]
    public string? Class { get; set; }

    /// <summary>
    /// Gets or sets the content to be rendered inside the typography element.
    /// </summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    /// <summary>
    /// Gets the computed CSS classes for the typography element.
    /// </summary>
    private string CssClass => ClassNames.cn(
        Variant switch
        {
            TypographyVariant.H1 => "scroll-m-20 text-4xl font-extrabold tracking-tight lg:text-5xl",
            TypographyVariant.H2 => "scroll-m-20 border-b pb-2 text-3xl font-semibold tracking-tight first:mt-0",
            TypographyVariant.H3 => "scroll-m-20 text-2xl font-semibold tracking-tight",
            TypographyVariant.H4 => "scroll-m-20 text-xl font-semibold tracking-tight",
            TypographyVariant.P => "leading-7 [&:not(:first-child)]:mt-6",
            TypographyVariant.Blockquote => "mt-6 border-l-2 pl-6 italic",
            TypographyVariant.InlineCode => "relative rounded bg-muted px-[0.3rem] py-[0.2rem] font-mono text-sm font-semibold",
            TypographyVariant.Lead => "text-xl text-muted-foreground",
            TypographyVariant.Large => "text-lg font-semibold",
            TypographyVariant.Small => "text-sm font-medium leading-none",
            TypographyVariant.Muted => "text-sm text-muted-foreground",
            _ => "leading-7 [&:not(:first-child)]:mt-6"
        },
        Class
    );
}
