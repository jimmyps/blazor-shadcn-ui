using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace NeoUI.Blazor;

/// <summary>
/// A split button that combines a primary action button with a dropdown menu for secondary actions.
/// </summary>
/// <example>
/// <code>
/// &lt;SplitButton OnClick="Save" Variant="ButtonVariant.Default"&gt;
///     Save
///     &lt;DropdownContent&gt;
///         &lt;SplitButtonItem OnClick="SaveAs"&gt;Save As…&lt;/SplitButtonItem&gt;
///         &lt;SplitButtonSeparator /&gt;
///         &lt;SplitButtonItem OnClick="Discard"&gt;Discard&lt;/SplitButtonItem&gt;
///     &lt;/DropdownContent&gt;
/// &lt;/SplitButton&gt;
/// </code>
/// </example>
public partial class SplitButton : ComponentBase
{
    /// <summary>Gets or sets the primary button label content.</summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    /// <summary>Gets or sets the dropdown menu items content.</summary>
    [Parameter]
    public RenderFragment? DropdownContent { get; set; }

    /// <summary>Gets or sets the visual variant.</summary>
    [Parameter]
    public ButtonVariant Variant { get; set; } = ButtonVariant.Default;

    /// <summary>Gets or sets the size.</summary>
    [Parameter]
    public ButtonSize Size { get; set; } = ButtonSize.Default;

    /// <summary>Gets or sets whether the button is disabled.</summary>
    [Parameter]
    public bool Disabled { get; set; }

    /// <summary>Gets or sets the primary button click callback.</summary>
    [Parameter]
    public EventCallback<MouseEventArgs> OnClick { get; set; }

    /// <summary>Gets or sets an icon to render inside the primary button.</summary>
    [Parameter]
    public RenderFragment? Icon { get; set; }

    /// <summary>Gets or sets the icon position inside the primary button.</summary>
    [Parameter]
    public IconPosition IconPosition { get; set; } = IconPosition.Start;

    /// <summary>Gets or sets the aria-label for the primary button.</summary>
    [Parameter]
    public string? AriaLabel { get; set; }

    /// <summary>Gets or sets the aria-label for the dropdown toggle button.</summary>
    [Parameter]
    public string? DropdownAriaLabel { get; set; }

    /// <summary>Gets or sets additional CSS classes.</summary>
    [Parameter]
    public string? Class { get; set; }

    /// <summary>Captures any additional HTML attributes.</summary>
    [Parameter(CaptureUnmatchedValues = true)]
    public Dictionary<string, object>? AdditionalAttributes { get; set; }

    private string CssClass => ClassNames.cn("inline-flex", Class);

    private bool _isOpen;

    private string? OpenActiveClass => _isOpen ? Variant switch
    {
        ButtonVariant.Default     => "!bg-primary/90",
        ButtonVariant.Destructive => "!bg-destructive/90",
        ButtonVariant.Outline     => "!bg-accent !text-accent-foreground",
        ButtonVariant.Secondary   => "!bg-secondary/80",
        ButtonVariant.Ghost       => "!bg-accent !text-accent-foreground",
        _                         => null
    } : null;

    private static string PrimaryButtonClass => "!rounded-r-none border-r-0 focus-visible:z-10";

    private string DropdownButtonClass => ClassNames.cn(
        "!rounded-l-none !px-2 focus-visible:z-10",
        Variant == ButtonVariant.Outline
            ? "border-l"
            : "border-l border-l-primary-foreground/20"
    );

    private ButtonSize DropdownButtonSize => Size switch
    {
        ButtonSize.Small => ButtonSize.Small,
        ButtonSize.Large => ButtonSize.Large,
        _                => ButtonSize.Default
    };
}
