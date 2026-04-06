using NeoUI.Blazor.Services;

namespace NeoUI.Blazor;

/// <summary>
/// Central registrymapping StyleVariant values to per-component Tailwind class overrides.
/// Each entry is keyed by "ComponentName.Part" (e.g. "Button.Root", "Card.Header").
/// Returns null when no override is defined — ClassNames.cn ignores null, so unregistered
/// components and the Default variant are zero-cost no-ops.
/// </summary>
public static class StyleVariantClasses
{
    private static readonly IReadOnlyDictionary<StyleVariant, IReadOnlyDictionary<string, string>> Registry =
        new Dictionary<StyleVariant, IReadOnlyDictionary<string, string>>
        {
            [StyleVariant.Default] = DefaultClasses.All,
            [StyleVariant.Vega]    = VegaClasses.All,
            [StyleVariant.Nova]    = NovaClasses.All,
            [StyleVariant.Maia]    = MaiaClasses.All,
            [StyleVariant.Lyra]    = LyraClasses.All,
            [StyleVariant.Mira]    = MiraClasses.All,
            [StyleVariant.Luma]    = LumaClasses.All,
        };

    public static string? Get(StyleVariant variant, string key)
        => Registry.TryGetValue(variant, out var dict) && dict.TryGetValue(key, out var cls)
            ? cls
            : null;
}
