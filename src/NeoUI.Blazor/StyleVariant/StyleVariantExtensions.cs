using NeoUI.Blazor.Services;

namespace NeoUI.Blazor;

/// <summary>
/// Extension methods for StyleVariant to enable fluent class lookups at component render time.
/// Usage: _styleVariant.GetClasses("Button.Root")
/// </summary>
public static class StyleVariantExtensions
{
    /// <summary>
    /// Returns the Tailwind class override string for the given component key,
    /// or null if this variant defines no override for that key.
    /// Null is safe to pass directly to ClassNames.cn — it is ignored.
    /// </summary>
    public static string? GetClasses(this StyleVariant variant, string key)
        => StyleVariantClasses.Get(variant, key);
}
