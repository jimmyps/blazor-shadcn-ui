using System.Text.Json.Serialization;

namespace BlazorUI.Components.Grid;

/// <summary>
/// Specifies the base theme for the grid.
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum GridTheme
{
    /// <summary>
    /// Shadcn theme integrated with shadcn/ui design tokens (default).
    /// Automatically adapts to your app's color scheme and supports dark mode.
    /// </summary>
    Shadcn = 0,
    
    /// <summary>AG Grid's Alpine theme (clean, modern look).</summary>
    Alpine = 1,
    
    /// <summary>AG Grid's Balham theme (professional business theme).</summary>
    Balham = 2,
    
    /// <summary>AG Grid's Material theme (Google Material Design styled).</summary>
    Material = 3,
    
    /// <summary>AG Grid's Quartz theme (modern, polished appearance).</summary>
    Quartz = 4,
}
