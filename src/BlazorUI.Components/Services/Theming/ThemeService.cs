using Microsoft.JSInterop;

namespace BlazorUI.Components.Services.Theming;

/// <summary>
/// Base color options for the theme system.
/// </summary>
public enum BaseColor
{
    /// <summary>
    /// Zinc color scheme - cool gray with blue undertones.
    /// </summary>
    Zinc,

    /// <summary>
    /// Slate color scheme - blue-gray with subtle blue tones.
    /// </summary>
    Slate,

    /// <summary>
    /// Gray color scheme - neutral gray without color bias.
    /// </summary>
    Gray,

    /// <summary>
    /// Neutral color scheme - true neutral gray.
    /// </summary>
    Neutral,

    /// <summary>
    /// Stone color scheme - warm gray with brown undertones.
    /// </summary>
    Stone
}

/// <summary>
/// Primary color options for the theme system.
/// </summary>
public enum PrimaryColor
{
    /// <summary>
    /// Default theme color - uses the base color.
    /// </summary>
    Default,

    /// <summary>
    /// Red primary color.
    /// </summary>
    Red,

    /// <summary>
    /// Rose primary color - pink-red.
    /// </summary>
    Rose,

    /// <summary>
    /// Orange primary color.
    /// </summary>
    Orange,

    /// <summary>
    /// Amber primary color - golden orange.
    /// </summary>
    Amber,

    /// <summary>
    /// Yellow primary color.
    /// </summary>
    Yellow,

    /// <summary>
    /// Lime primary color - yellow-green.
    /// </summary>
    Lime,

    /// <summary>
    /// Green primary color.
    /// </summary>
    Green,

    /// <summary>
    /// Emerald primary color - blue-green.
    /// </summary>
    Emerald,

    /// <summary>
    /// Teal primary color - cyan-green.
    /// </summary>
    Teal,

    /// <summary>
    /// Cyan primary color - blue-green.
    /// </summary>
    Cyan,

    /// <summary>
    /// Sky primary color - light blue.
    /// </summary>
    Sky,

    /// <summary>
    /// Blue primary color.
    /// </summary>
    Blue,

    /// <summary>
    /// Indigo primary color - deep blue.
    /// </summary>
    Indigo,

    /// <summary>
    /// Violet primary color - blue-purple.
    /// </summary>
    Violet,

    /// <summary>
    /// Purple primary color.
    /// </summary>
    Purple,

    /// <summary>
    /// Fuchsia primary color - bright purple-pink.
    /// </summary>
    Fuchsia,

    /// <summary>
    /// Pink primary color.
    /// </summary>
    Pink
}

/// <summary>
/// Service for managing theme state including dark mode, base colors, and primary colors.
/// Handles toggling between light and dark themes with localStorage persistence.
/// </summary>
public class ThemeService
{
    private readonly IJSRuntime _jsRuntime;
    private bool _isDarkMode;
    private BaseColor _baseColor = BaseColor.Zinc;
    private PrimaryColor _primaryColor = PrimaryColor.Default;
    private bool _isInitialized;

    /// <summary>
    /// Event raised when the theme changes.
    /// </summary>
    public event Action? OnThemeChanged;

    /// <summary>
    /// Gets whether dark mode is currently enabled.
    /// </summary>
    public bool IsDarkMode => _isDarkMode;

    /// <summary>
    /// Gets the current base color.
    /// </summary>
    public BaseColor CurrentBaseColor => _baseColor;

    /// <summary>
    /// Gets the current primary color.
    /// </summary>
    public PrimaryColor CurrentPrimaryColor => _primaryColor;

    /// <summary>
    /// Initializes a new instance of the <see cref="ThemeService"/> class.
    /// </summary>
    /// <param name="jsRuntime">The JavaScript runtime for interop operations.</param>
    public ThemeService(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    /// <summary>
    /// Initializes the theme service by loading the saved preference from localStorage.
    /// Should be called once during application startup.
    /// </summary>
    public async Task InitializeAsync()
    {
        if (_isInitialized) return;

        try
        {
            // Try to load saved preference from localStorage
            var savedTheme = await _jsRuntime.InvokeAsync<string?>("localStorage.getItem", "theme");
            var savedBaseColor = await _jsRuntime.InvokeAsync<string?>("localStorage.getItem", "baseColor");
            var savedPrimaryColor = await _jsRuntime.InvokeAsync<string?>("localStorage.getItem", "primaryColor");

            // Determine dark mode state
            if (!string.IsNullOrEmpty(savedTheme))
            {
                // Use the explicitly saved theme preference
                _isDarkMode = savedTheme == "dark";
            }
            else
            {
                // No saved theme; align with the current DOM theme (set by App.razor using prefers-color-scheme)
                // This reads whether the <html> element currently has the "dark" class
                _isDarkMode = await _jsRuntime.InvokeAsync<bool>("theme.isDark");
            }
            
            // Parse base color
            if (!string.IsNullOrEmpty(savedBaseColor) && Enum.TryParse<BaseColor>(savedBaseColor, true, out var baseColor))
            {
                _baseColor = baseColor;
            }
            
            // Parse primary color
            if (!string.IsNullOrEmpty(savedPrimaryColor) && Enum.TryParse<PrimaryColor>(savedPrimaryColor, true, out var primaryColor))
            {
                _primaryColor = primaryColor;
            }

            await ApplyThemeAsync(_isDarkMode, _baseColor, _primaryColor);

            _isInitialized = true;
        }
        catch
        {
            // If localStorage is not available (SSR), use defaults
            _isDarkMode = false;
            _baseColor = BaseColor.Zinc;
            _primaryColor = PrimaryColor.Default;

            try
            {
                // Attempt to apply the default theme; ignore failures so we can retry later
                await ApplyThemeAsync(_isDarkMode, _baseColor, _primaryColor);
            }
            catch
            {
                // Swallow exceptions here; JS/localStorage may not yet be available (e.g., SSR or disabled)
            }

            _isInitialized = true;
        }
    }

    /// <summary>
    /// Toggles between light and dark mode.
    /// </summary>
    public async Task ToggleThemeAsync()
    {
        _isDarkMode = !_isDarkMode;
        await ApplyThemeAsync(_isDarkMode, _baseColor, _primaryColor);
        await SaveThemeAsync(_isDarkMode);

        OnThemeChanged?.Invoke();
    }

    /// <summary>
    /// Sets the theme to a specific mode.
    /// </summary>
    /// <param name="isDark">True for dark mode, false for light mode.</param>
    public async Task SetThemeAsync(bool isDark)
    {
        if (_isDarkMode == isDark) return;

        _isDarkMode = isDark;
        await ApplyThemeAsync(_isDarkMode, _baseColor, _primaryColor);
        await SaveThemeAsync(_isDarkMode);

        OnThemeChanged?.Invoke();
    }

    /// <summary>
    /// Sets the base color of the theme.
    /// </summary>
    /// <param name="baseColor">The base color to apply.</param>
    public async Task SetBaseColorAsync(BaseColor baseColor)
    {
        if (_baseColor == baseColor) return;

        _baseColor = baseColor;
        await ApplyThemeAsync(_isDarkMode, _baseColor, _primaryColor);
        await SaveBaseColorAsync(_baseColor);

        OnThemeChanged?.Invoke();
    }

    /// <summary>
    /// Sets the primary color of the theme.
    /// </summary>
    /// <param name="primaryColor">The primary color to apply.</param>
    public async Task SetPrimaryColorAsync(PrimaryColor primaryColor)
    {
        if (_primaryColor == primaryColor) return;

        _primaryColor = primaryColor;
        await ApplyThemeAsync(_isDarkMode, _baseColor, _primaryColor);
        await SavePrimaryColorAsync(_primaryColor);

        OnThemeChanged?.Invoke();
    }

    /// <summary>
    /// Sets the complete theme (base color, primary color, and dark mode).
    /// </summary>
    /// <param name="baseColor">The base color to apply.</param>
    /// <param name="primaryColor">The primary color to apply.</param>
    /// <param name="isDark">True for dark mode, false for light mode.</param>
    public async Task SetThemeAsync(BaseColor baseColor, PrimaryColor primaryColor, bool isDark)
    {
        _baseColor = baseColor;
        _primaryColor = primaryColor;
        _isDarkMode = isDark;

        await ApplyThemeAsync(_isDarkMode, _baseColor, _primaryColor);
        await SaveThemeAsync(_isDarkMode);
        await SaveBaseColorAsync(_baseColor);
        await SavePrimaryColorAsync(_primaryColor);

        OnThemeChanged?.Invoke();
    }

    /// <summary>
    /// Applies the theme by adding or removing CSS classes on the HTML element.
    /// </summary>
    private async Task ApplyThemeAsync(bool isDark, BaseColor baseColor, PrimaryColor primaryColor)
    {
        try
        {
            var baseColorClass = $"base-{baseColor.ToString().ToLowerInvariant()}";
            
            // Only apply primary color class if not Default
            var primaryColorClass = primaryColor == PrimaryColor.Default 
                ? "" 
                : $"primary-{primaryColor.ToString().ToLowerInvariant()}";

            // Use named JavaScript function instead of eval for CSP compatibility
            await _jsRuntime.InvokeVoidAsync("theme.apply", new
            {
                @base = baseColorClass,
                primary = primaryColorClass,
                dark = isDark
            });
        }
        catch
        {
            // Ignore errors during SSR
        }
    }

    /// <summary>
    /// Saves the theme preference to localStorage.
    /// </summary>
    private async Task SaveThemeAsync(bool isDark)
    {
        try
        {
            var theme = isDark ? "dark" : "light";
            await _jsRuntime.InvokeVoidAsync("localStorage.setItem", "theme", theme);
        }
        catch
        {
            // Ignore errors if localStorage is not available
        }
    }

    /// <summary>
    /// Saves the base color preference to localStorage.
    /// </summary>
    private async Task SaveBaseColorAsync(BaseColor baseColor)
    {
        try
        {
            await _jsRuntime.InvokeVoidAsync("localStorage.setItem", "baseColor", baseColor.ToString());
        }
        catch
        {
            // Ignore errors if localStorage is not available
        }
    }

    /// <summary>
    /// Saves the primary color preference to localStorage.
    /// </summary>
    private async Task SavePrimaryColorAsync(PrimaryColor primaryColor)
    {
        try
        {
            await _jsRuntime.InvokeVoidAsync("localStorage.setItem", "primaryColor", primaryColor.ToString());
        }
        catch
        {
            // Ignore errors if localStorage is not available
        }
    }
}
