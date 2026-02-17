using Microsoft.AspNetCore.Components.Routing;

namespace BlazorBlueprint.Components;

/// <summary>
/// Defines a navigation item for use with <see cref="BbResponsiveNavItems"/>.
/// Allows defining navigation links once and rendering them in both desktop and mobile contexts.
/// </summary>
public class NavItemDefinition
{
    /// <summary>
    /// The display text for the navigation item.
    /// </summary>
    public required string Text { get; init; }

    /// <summary>
    /// The URL to navigate to. If null, this item acts as a group header with <see cref="Children"/>.
    /// </summary>
    public string? Href { get; init; }

    /// <summary>
    /// Child navigation items. On desktop, renders as a dropdown panel.
    /// On mobile, renders as an indented list under a category header.
    /// </summary>
    public IReadOnlyList<NavItemDefinition>? Children { get; init; }

    /// <summary>
    /// Optional description text shown in the desktop dropdown content.
    /// </summary>
    public string? Description { get; init; }

    /// <summary>
    /// How to match the URL for active state highlighting. Default is <see cref="NavLinkMatch.Prefix"/>.
    /// </summary>
    public NavLinkMatch Match { get; init; } = NavLinkMatch.Prefix;
}
