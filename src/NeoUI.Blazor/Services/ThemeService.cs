using Microsoft.JSInterop;

namespace NeoUI.Blazor.Services;

/// <summary>
/// Base color options for the theme system.
/// </summary>
public enum BaseColor
{
    /// <summary>Zinc color scheme — cool gray with blue undertones.</summary>
    Zinc,

    /// <summary>Slate color scheme — blue-gray with subtle blue tones.</summary>
    Slate,

    /// <summary>Gray color scheme — neutral gray without color bias.</summary>
    Gray,

    /// <summary>Neutral color scheme — true neutral gray.</summary>
    Neutral,

    /// <summary>Stone color scheme — warm gray with brown undertones.</summary>
    Stone,

    /// <summary>Luma color scheme — vibrant tinted neutral (flagship modern SaaS look).</summary>
    Luma,

    /// <summary>Mist color scheme — cool blue-gray.</summary>
    Mist,

    /// <summary>Mauve color scheme — warm purple-gray.</summary>
    Mauve,

    /// <summary>Taupe color scheme — warm brownish-gray.</summary>
    Taupe,

    /// <summary>Olive color scheme — muted green-gray.</summary>
    Olive
}

/// <summary>
/// Primary color options for the theme system.
/// </summary>
public enum PrimaryColor
{
    /// <summary>Default theme color — uses the base color.</summary>
    Default,

    /// <summary>Red primary color.</summary>
    Red,

    /// <summary>Rose primary color — pink-red.</summary>
    Rose,

    /// <summary>Orange primary color.</summary>
    Orange,

    /// <summary>Amber primary color — golden orange.</summary>
    Amber,

    /// <summary>Yellow primary color.</summary>
    Yellow,

    /// <summary>Lime primary color — yellow-green.</summary>
    Lime,

    /// <summary>Green primary color.</summary>
    Green,

    /// <summary>Emerald primary color — blue-green.</summary>
    Emerald,

    /// <summary>Teal primary color — cyan-green.</summary>
    Teal,

    /// <summary>Cyan primary color — blue-green.</summary>
    Cyan,

    /// <summary>Sky primary color — light blue.</summary>
    Sky,

    /// <summary>Blue primary color.</summary>
    Blue,

    /// <summary>Indigo primary color — deep blue.</summary>
    Indigo,

    /// <summary>Violet primary color — blue-purple.</summary>
    Violet,

    /// <summary>Purple primary color.</summary>
    Purple,

    /// <summary>Fuchsia primary color — bright purple-pink.</summary>
    Fuchsia,

    /// <summary>Pink primary color.</summary>
    Pink
}

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
    Mira
}

/// <summary>
/// Named radius presets for consistent corner rounding across the app.
/// </summary>
public enum RadiusPreset
{
    /// <summary>No rounding — square corners (--radius: 0rem).</summary>
    None,

    /// <summary>Subtle rounding (--radius: 0.25rem).</summary>
    Small,

    /// <summary>Medium rounding — default (--radius: 0.625rem).</summary>
    Medium,

    /// <summary>Generous rounding (--radius: 1rem).</summary>
    Large,

    /// <summary>Pill-shaped elements (--radius: 9999px).</summary>
    Full
}

/// <summary>
/// Named font presets for consistent typography across the app.
/// </summary>
public enum FontPreset
{
    /// <summary>System default fonts — no override applied.</summary>
    System,

    /// <summary>Inter — versatile, highly legible sans-serif.</summary>
    Inter,

    /// <summary>Geist — modern developer-friendly sans-serif by Vercel.</summary>
    Geist,

    /// <summary>Cal Sans — geometric display heading paired with Inter body.</summary>
    CalSans,

    /// <summary>DM Sans — friendly, modern geometric sans-serif.</summary>
    DmSans,

    /// <summary>Plus Jakarta Sans — contemporary, warm humanist sans-serif.</summary>
    PlusJakarta
}

/// <summary>
/// A portable, serializable record encoding all theme dimensions as a named preset.
/// </summary>
/// <param name="Name">Human-readable name for the preset.</param>
/// <param name="BaseColor">Base (neutral) color scheme.</param>
/// <param name="PrimaryColor">Primary accent color.</param>
/// <param name="StyleVariant">Visual style variant (radius + density character).</param>
/// <param name="RadiusPreset">Explicit radius override. <see cref="RadiusPreset.Medium"/> means no override.</param>
/// <param name="FontPreset">Font preset. <see cref="FontPreset.System"/> means no override.</param>
/// <param name="IsDarkMode">Whether dark mode is active.</param>
public record ThemePreset(
    string        Name,
    BaseColor     BaseColor     = BaseColor.Zinc,
    PrimaryColor  PrimaryColor  = PrimaryColor.Default,
    StyleVariant  StyleVariant  = StyleVariant.Default,
    RadiusPreset  RadiusPreset  = RadiusPreset.Medium,
    FontPreset    FontPreset    = FontPreset.System,
    bool          IsDarkMode    = false)
{
    /// <summary>Clean, neutral default — the classic NeoUI look.</summary>
    public static readonly ThemePreset Default = new("Default");

    /// <summary>Luma + Vega — modern SaaS, vibrant tinted neutrals with balanced style.</summary>
    public static readonly ThemePreset Luma = new("Luma",
        BaseColor: BaseColor.Luma,
        StyleVariant: StyleVariant.Vega,
        FontPreset: FontPreset.Inter);

    /// <summary>Nova — compact dashboard preset with Zinc base.</summary>
    public static readonly ThemePreset Nova = new("Nova",
        StyleVariant: StyleVariant.Nova,
        FontPreset: FontPreset.Geist);

    /// <summary>Maia — spacious consumer UI with generous rounding and warm neutrals.</summary>
    public static readonly ThemePreset Maia = new("Maia",
        BaseColor: BaseColor.Mauve,
        StyleVariant: StyleVariant.Maia,
        FontPreset: FontPreset.PlusJakarta);

    /// <summary>Lyra — sharp developer/tooling preset with no rounding.</summary>
    public static readonly ThemePreset Lyra = new("Lyra",
        StyleVariant: StyleVariant.Lyra,
        FontPreset: FontPreset.Geist);
}

/// <summary>
/// Service for managing theme state including dark mode, base colors, primary colors,
/// style variants, radius presets, and font presets.
/// Handles toggling between light and dark themes with localStorage persistence.
/// </summary>
public class ThemeService
{
    private readonly IJSRuntime _jsRuntime;
    private bool _isDarkMode;
    private BaseColor _baseColor = BaseColor.Zinc;
    private PrimaryColor _primaryColor = PrimaryColor.Default;
    private StyleVariant _styleVariant = StyleVariant.Default;
    private RadiusPreset _radiusPreset = RadiusPreset.Medium;
    private FontPreset _fontPreset = FontPreset.System;
    private bool _isInitialized;

    /// <summary>Event raised when the theme changes.</summary>
    public event Action? OnThemeChanged;

    /// <summary>Gets whether dark mode is currently enabled.</summary>
    public bool IsDarkMode => _isDarkMode;

    /// <summary>Gets the current base color.</summary>
    public BaseColor CurrentBaseColor => _baseColor;

    /// <summary>Gets the current primary color.</summary>
    public PrimaryColor CurrentPrimaryColor => _primaryColor;

    /// <summary>Gets the current visual style variant.</summary>
    public StyleVariant CurrentStyleVariant => _styleVariant;

    /// <summary>Gets the current radius preset.</summary>
    public RadiusPreset CurrentRadiusPreset => _radiusPreset;

    /// <summary>Gets the current font preset.</summary>
    public FontPreset CurrentFontPreset => _fontPreset;

    /// <summary>
    /// Initializes a new instance of the <see cref="ThemeService"/> class.
    /// </summary>
    /// <param name="jsRuntime">The JavaScript runtime for interop operations.</param>
    public ThemeService(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    /// <summary>
    /// Initializes the theme service by loading saved preferences from localStorage.
    /// Should be called once during application startup.
    /// </summary>
    public async Task InitializeAsync()
    {
        if (_isInitialized) return;

        try
        {
            var savedTheme        = await _jsRuntime.InvokeAsync<string?>("localStorage.getItem", "theme");
            var savedBaseColor    = await _jsRuntime.InvokeAsync<string?>("localStorage.getItem", "baseColor");
            var savedPrimaryColor = await _jsRuntime.InvokeAsync<string?>("localStorage.getItem", "primaryColor");
            var savedStyle        = await _jsRuntime.InvokeAsync<string?>("localStorage.getItem", "styleVariant");
            var savedRadius       = await _jsRuntime.InvokeAsync<string?>("localStorage.getItem", "radiusPreset");
            var savedFont         = await _jsRuntime.InvokeAsync<string?>("localStorage.getItem", "fontPreset");

            _isDarkMode = !string.IsNullOrEmpty(savedTheme)
                ? savedTheme == "dark"
                : await _jsRuntime.InvokeAsync<bool>("theme.isDark");

            if (!string.IsNullOrEmpty(savedBaseColor) && Enum.TryParse<BaseColor>(savedBaseColor, true, out var baseColor))
                _baseColor = baseColor;

            if (!string.IsNullOrEmpty(savedPrimaryColor) && Enum.TryParse<PrimaryColor>(savedPrimaryColor, true, out var primaryColor))
                _primaryColor = primaryColor;

            if (!string.IsNullOrEmpty(savedStyle) && Enum.TryParse<StyleVariant>(savedStyle, true, out var styleVariant))
                _styleVariant = styleVariant;

            if (!string.IsNullOrEmpty(savedRadius) && Enum.TryParse<RadiusPreset>(savedRadius, true, out var radiusPreset))
                _radiusPreset = radiusPreset;

            if (!string.IsNullOrEmpty(savedFont) && Enum.TryParse<FontPreset>(savedFont, true, out var fontPreset))
                _fontPreset = fontPreset;

            await ApplyThemeAsync();

            _isInitialized = true;
        }
        catch
        {
            _isDarkMode    = false;
            _baseColor     = BaseColor.Zinc;
            _primaryColor  = PrimaryColor.Default;
            _styleVariant  = StyleVariant.Default;
            _radiusPreset  = RadiusPreset.Medium;
            _fontPreset    = FontPreset.System;

            try { await ApplyThemeAsync(); } catch { }

            _isInitialized = true;
        }
    }

    /// <summary>Toggles between light and dark mode.</summary>
    public async Task ToggleThemeAsync()
    {
        _isDarkMode = !_isDarkMode;
        await ApplyThemeAsync();
        await SaveAsync("theme", _isDarkMode ? "dark" : "light");
        OnThemeChanged?.Invoke();
    }

    /// <summary>Sets the theme to a specific mode.</summary>
    public async Task SetThemeAsync(bool isDark)
    {
        if (_isDarkMode == isDark) return;
        _isDarkMode = isDark;
        await ApplyThemeAsync();
        await SaveAsync("theme", _isDarkMode ? "dark" : "light");
        OnThemeChanged?.Invoke();
    }

    /// <summary>Sets the base color of the theme.</summary>
    public async Task SetBaseColorAsync(BaseColor baseColor)
    {
        if (_baseColor == baseColor) return;
        _baseColor = baseColor;
        await ApplyThemeAsync();
        await SaveAsync("baseColor", _baseColor.ToString());
        OnThemeChanged?.Invoke();
    }

    /// <summary>Sets the primary color of the theme.</summary>
    public async Task SetPrimaryColorAsync(PrimaryColor primaryColor)
    {
        if (_primaryColor == primaryColor) return;
        _primaryColor = primaryColor;
        await ApplyThemeAsync();
        await SaveAsync("primaryColor", _primaryColor.ToString());
        OnThemeChanged?.Invoke();
    }

    /// <summary>Sets the visual style variant.</summary>
    public async Task SetStyleVariantAsync(StyleVariant styleVariant)
    {
        if (_styleVariant == styleVariant) return;
        _styleVariant = styleVariant;
        await ApplyThemeAsync();
        await SaveAsync("styleVariant", _styleVariant.ToString());
        OnThemeChanged?.Invoke();
    }

    /// <summary>Sets the radius preset.</summary>
    public async Task SetRadiusPresetAsync(RadiusPreset radiusPreset)
    {
        if (_radiusPreset == radiusPreset) return;
        _radiusPreset = radiusPreset;
        await ApplyThemeAsync();
        await SaveAsync("radiusPreset", _radiusPreset.ToString());
        OnThemeChanged?.Invoke();
    }

    /// <summary>Sets the font preset.</summary>
    public async Task SetFontPresetAsync(FontPreset fontPreset)
    {
        if (_fontPreset == fontPreset) return;
        _fontPreset = fontPreset;
        await ApplyThemeAsync();
        await SaveAsync("fontPreset", _fontPreset.ToString());
        OnThemeChanged?.Invoke();
    }

    /// <summary>
    /// Sets the complete theme (base color, primary color, and dark mode).
    /// Existing overloads remain unchanged for backward compatibility.
    /// </summary>
    public async Task SetThemeAsync(BaseColor baseColor, PrimaryColor primaryColor, bool isDark)
    {
        _baseColor    = baseColor;
        _primaryColor = primaryColor;
        _isDarkMode   = isDark;

        await ApplyThemeAsync();
        await SaveAsync("theme", _isDarkMode ? "dark" : "light");
        await SaveAsync("baseColor", _baseColor.ToString());
        await SaveAsync("primaryColor", _primaryColor.ToString());

        OnThemeChanged?.Invoke();
    }

    /// <summary>
    /// Applies a named preset, updating all theme dimensions simultaneously.
    /// </summary>
    public async Task ApplyPresetAsync(ThemePreset preset)
    {
        _baseColor    = preset.BaseColor;
        _primaryColor = preset.PrimaryColor;
        _styleVariant = preset.StyleVariant;
        _radiusPreset = preset.RadiusPreset;
        _fontPreset   = preset.FontPreset;
        _isDarkMode   = preset.IsDarkMode;

        await ApplyThemeAsync();
        await SaveAsync("theme",        _isDarkMode ? "dark" : "light");
        await SaveAsync("baseColor",    _baseColor.ToString());
        await SaveAsync("primaryColor", _primaryColor.ToString());
        await SaveAsync("styleVariant", _styleVariant.ToString());
        await SaveAsync("radiusPreset", _radiusPreset.ToString());
        await SaveAsync("fontPreset",   _fontPreset.ToString());

        OnThemeChanged?.Invoke();
    }

    // ──────────────────────────────────────────────────────────────────────────
    // Internal helpers
    // ──────────────────────────────────────────────────────────────────────────

    private async Task ApplyThemeAsync()
    {
        try
        {
            await _jsRuntime.InvokeVoidAsync("theme.apply", new
            {
                @base   = $"base-{_baseColor.ToString().ToLowerInvariant()}",
                primary = _primaryColor == PrimaryColor.Default
                              ? ""
                              : $"primary-{_primaryColor.ToString().ToLowerInvariant()}",
                style   = _styleVariant == StyleVariant.Default
                              ? ""
                              : $"style-{_styleVariant.ToString().ToLowerInvariant()}",
                radius  = _radiusPreset == RadiusPreset.Medium
                              ? ""
                              : $"radius-{_radiusPreset.ToString().ToLowerInvariant()}",
                font    = _fontPreset == FontPreset.System
                              ? ""
                              : $"font-{_fontPreset.ToString().ToLowerInvariant()}",
                dark    = _isDarkMode
            });
        }
        catch
        {
            // Ignore errors during SSR
        }
    }

    private async Task SaveAsync(string key, string value)
    {
        try
        {
            await _jsRuntime.InvokeVoidAsync("localStorage.setItem", key, value);
        }
        catch
        {
            // Ignore errors if localStorage is not available
        }
    }
}

