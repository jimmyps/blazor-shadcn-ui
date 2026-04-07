namespace NeoUI.Blazor;

/// <summary>
/// Visual style variants that control component character (radius, density, spacing).
/// </summary>
public enum StyleVariant
{
    /// <summary>Default style — standard NeoUI look.</summary>
    Default,

    /// <summary>Vega — professional, general-purpose balanced style.</summary>
    Vega,

    /// <summary>Nova — compact dashboard/admin-dense style.</summary>
    Nova,

    /// <summary>Maia — spacious, consumer-friendly rounded style.</summary>
    Maia,

    /// <summary>Lyra — sharp/boxy developer and tooling style (no radius).</summary>
    Lyra,

    /// <summary>Mira — ultra-dense style for data-heavy tables.</summary>
    Mira,

    /// <summary>Luma — glassmorphism, modern SaaS aesthetic with soft blur overlays.</summary>
    Luma
}
