using Microsoft.AspNetCore.Components;

namespace BlazorBlueprint.Primitives.Services;

/// <summary>
/// Represents a registered portal with its content, category, and insertion order.
/// Insertion order ensures parent portals render before children within the same category.
/// </summary>
internal sealed class PortalEntry
{
    /// <summary>
    /// The render fragment content of the portal.
    /// </summary>
    public required RenderFragment Content { get; set; }

    /// <summary>
    /// The category this portal belongs to.
    /// </summary>
    public required PortalCategory Category { get; init; }

    /// <summary>
    /// Monotonically increasing order value assigned at registration time.
    /// Used to maintain insertion order when iterating portals in a category.
    /// </summary>
    public required long Order { get; init; }
}
