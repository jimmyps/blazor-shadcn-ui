using Microsoft.AspNetCore.Components;

namespace NeoUI.Blazor;

/// <summary>
/// A mobile bottom navigation bar for primary app navigation.
/// </summary>
/// <remarks>
/// <para>
/// BottomNav is a mobile-first navigation component that presents top-level
/// destinations at the bottom of the screen, following established mobile UX patterns
/// (iOS tab bar, Android bottom navigation, .NET MAUI Shell tabs).
/// </para>
/// <para>
/// This component is designed to work equally well in:
/// - Blazor WebAssembly / Blazor Server mobile-first web apps
/// - .NET MAUI Blazor Hybrid apps (via BlazorWebView)
/// - Progressive Web Apps (PWA)
/// </para>
/// <para>
/// Features:
/// - 2–5 tab items with icon + label
/// - Active state highlighting via <see cref="ActiveTab"/> binding
/// - Respects iOS/Android safe-area-inset-bottom automatically
/// - Notification badge support per item (see <see cref="BottomNavItem"/>)
/// - Accessible: role="navigation", aria-current on active item
/// </para>
/// </remarks>
/// <example>
/// <code>
/// &lt;BottomNav @bind-ActiveTab="activeTab"&gt;
///     &lt;BottomNavItem Value="home"    Icon="house"          Label="Home" /&gt;
///     &lt;BottomNavItem Value="search"  Icon="search"         Label="Search" /&gt;
///     &lt;BottomNavItem Value="orders"  Icon="clipboard-list" Label="Orders" BadgeCount="3" /&gt;
///     &lt;BottomNavItem Value="account" Icon="user"           Label="Account" /&gt;
/// &lt;/BottomNav&gt;
/// </code>
/// </example>
public partial class BottomNav : ComponentBase
{
    /// <summary>
    /// Gets or sets the value of the currently active tab.
    /// </summary>
    [Parameter]
    public string? ActiveTab { get; set; }

    /// <summary>
    /// Fires when the active tab changes.
    /// </summary>
    [Parameter]
    public EventCallback<string?> ActiveTabChanged { get; set; }

    /// <summary>
    /// Gets or sets the <see cref="BottomNavItem"/> children.
    /// </summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    /// <summary>
    /// Gets or sets additional CSS classes.
    /// </summary>
    [Parameter]
    public string? Class { get; set; }

    /// <summary>
    /// Gets or sets the accessible label for the nav landmark.
    /// Defaults to "Main navigation".
    /// </summary>
    [Parameter]
    public string AriaLabel { get; set; } = "Main navigation";

    /// <summary>
    /// Gets or sets whether the nav is fixed to the bottom of the viewport.
    /// Set to false when the BottomNav is scoped inside a container
    /// (e.g. a .app-shell div) rather than the full viewport.
    /// Defaults to true.
    /// </summary>
    [Parameter]
    public bool Fixed { get; set; } = true;

    private string CssClass => ClassNames.cn(
        "flex items-stretch bg-background border-t border-border",
        // Safe area padding for iOS notch / Android nav bar
        "pb-[env(safe-area-inset-bottom,0px)]",
        Fixed ? "fixed bottom-0 left-0 right-0 z-50" : "w-full",
        "shadow-[0_-1px_12px_rgba(0,0,0,0.08)]",
        Class
    );

    internal async Task SetActiveTabAsync(string? value)
    {
        if (ActiveTab != value)
        {
            ActiveTab = value;
            await ActiveTabChanged.InvokeAsync(value);
        }
    }

    internal bool IsActive(string? value) =>
        !string.IsNullOrEmpty(value) && value == ActiveTab;
}
