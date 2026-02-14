using Microsoft.JSInterop;

namespace BlazorUI.Demo.Services;

/// <summary>
/// Base color options for the theme system.
/// </summary>
public enum BaseColor
{
    Zinc,
    Slate,
    Gray,
    Neutral,
    Stone
}

/// <summary>
/// Primary color options for the theme system.
/// </summary>
public enum PrimaryColor
{
    Red,
    Rose,
    Orange,
    Amber,
    Yellow,
    Lime,
    Green,
    Emerald,
    Teal,
    Cyan,
    Sky,
    Blue,
    Indigo,
    Violet,
    Purple,
    Fuchsia,
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
    private PrimaryColor _primaryColor = PrimaryColor.Blue;
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

            _isDarkMode = savedTheme == "dark";
            
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
            _primaryColor = PrimaryColor.Blue;
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
            var baseColorClass = $"base-{baseColor.ToString().ToLower()}";
            var primaryColorClass = $"primary-{primaryColor.ToString().ToLower()}";

            // Use a dedicated JavaScript helper function for safer DOM manipulation
            await _jsRuntime.InvokeVoidAsync("eval", $@"
                (function() {{
                    const html = document.documentElement;
                    const classes = html.className.split(' ');
                    const filteredClasses = classes.filter(c => !c.startsWith('base-') && !c.startsWith('primary-'));
                    html.className = filteredClasses.join(' ');
                    html.classList.add('{baseColorClass}');
                    html.classList.add('{primaryColorClass}');
                    if ({isDark.ToString().ToLower()}) {{
                        html.classList.add('dark');
                    }} else {{
                        html.classList.remove('dark');
                    }}
                }})();
            ");
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
