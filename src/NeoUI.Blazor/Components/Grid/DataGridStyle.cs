using System.Text.Json.Serialization;

namespace NeoUI.Blazor;

/// <summary>
/// Specifies the visual style modifiers for a grid.
/// These styles work with any AG DataGrid theme (Alpine, Balham, Material, Quartz).
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum DataGridStyle
{
    /// <summary>Standard appearance with default borders and hover states.</summary>
    Default,
    
    /// <summary>Alternating row background colors for easier readability.</summary>
    Striped,
    
    /// <summary>Bordered cells with visible vertical dividers between columns.</summary>
    Bordered,
    
    /// <summary>Minimal borders with subtle, clean styling.</summary>
    Minimal
}
