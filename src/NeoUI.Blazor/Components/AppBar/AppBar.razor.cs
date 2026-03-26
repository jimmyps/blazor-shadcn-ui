using Microsoft.AspNetCore.Components;

namespace NeoUI.Blazor;

/// <summary>
/// A mobile-style application top bar with centered title, optional back button, and right actions.
/// </summary>
/// <remarks>
/// <para>
/// AppBar provides the standard mobile navigation header pattern — the same pattern used by
/// iOS UINavigationBar, Android TopAppBar, and .NET MAUI NavigationPage.
/// It is intentionally distinct from <see cref="ResponsiveNav"/>, which is designed for
/// full-width desktop/responsive horizontal navigation menus.
/// </para>
/// <para>
/// Designed for use in:
/// - Blazor mobile-first web apps
/// - .NET MAUI Blazor Hybrid apps (BlazorWebView)
/// - Detail / sub-screens in any app where a back navigation is needed
/// </para>
/// <para>
/// Features:
/// - Centered title (string or custom <see cref="TitleContent"/> render fragment)
/// - Back button (shown automatically when <see cref="OnBack"/> is provided)
/// - Right-side action slot (<see cref="RightContent"/> render fragment)
/// - Transparent mode for placement over hero images
/// - Configurable height and border
/// </para>
/// </remarks>
/// <example>
/// <code>
/// &lt;!-- Basic back + title --&gt;
/// &lt;AppBar Title="Product Detail" OnBack="NavigateBack" /&gt;
///
/// &lt;!-- With right action (cart icon with badge) --&gt;
/// &lt;AppBar Title="Menu" OnBack="NavigateBack"&gt;
///     &lt;RightContent&gt;
///         &lt;NotificationBadge Count="@cartCount"&gt;
///             &lt;Button Variant="ButtonVariant.Ghost" Size="ButtonSize.Icon"&gt;
///                 &lt;LucideIcon Name="shopping-cart" Size="20" /&gt;
///             &lt;/Button&gt;
///         &lt;/NotificationBadge&gt;
///     &lt;/RightContent&gt;
/// &lt;/AppBar&gt;
///
/// &lt;!-- Transparent mode over hero --&gt;
/// &lt;AppBar Transparent="true" OnBack="NavigateBack" /&gt;
/// </code>
/// </example>
public partial class AppBar : ComponentBase
{
    /// <summary>
    /// Gets or sets the title text displayed in the center of the bar.
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
    /// Gets or sets the callback fired when the back button is pressed.
    /// The back button is only rendered when this parameter is provided.
    /// </summary>
    [Parameter]
    public EventCallback OnBack { get; set; }

    /// <summary>
    /// Gets or sets the accessible label for the back button. Defaults to "Go back".
    /// </summary>
    [Parameter]
    public string BackLabel { get; set; } = "Go back";

    /// <summary>
    /// Gets or sets the right-side action slot (e.g. icon buttons, avatar, share icon).
    /// </summary>
    [Parameter]
    public RenderFragment? RightContent { get; set; }

    /// <summary>
    /// Gets or sets whether the AppBar has a transparent background.
    /// Use when placing the AppBar over a hero image or full-bleed banner.
    /// Defaults to false.
    /// </summary>
    [Parameter]
    public bool Transparent { get; set; }

    /// <summary>
    /// Gets or sets whether to show the bottom border. Defaults to true.
    /// Automatically hidden when <see cref="Transparent"/> is true.
    /// </summary>
    [Parameter]
    public bool ShowBorder { get; set; } = true;

    /// <summary>
    /// Gets or sets additional CSS classes on the header element.
    /// </summary>
    [Parameter]
    public string? Class { get; set; }

    private string CssClass => ClassNames.cn(
        "relative flex items-center h-[60px] px-3",
        Transparent
            ? "bg-transparent"
            : "bg-background",
        ShowBorder && !Transparent
            ? "border-b border-border"
            : string.Empty,
        Class
    );

    private string TitleCssClass => ClassNames.cn(
        "flex-1 text-center text-[17px] font-semibold text-foreground truncate",
        // Indent title text to avoid overlap with back/right buttons
        OnBack.HasDelegate ? "px-10" : "px-2"
    );
}
