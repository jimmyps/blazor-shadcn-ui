using System.Text.Json.Serialization;

namespace BlazorUI.Components.Grid;

/// <summary>
/// Specifies the visual theme for a grid.
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum GridTheme
{
    /// <summary>Standard borders and hover states.</summary>
    Default,
    
    /// <summary>Alternating row backgrounds.</summary>
    Striped,
    
    /// <summary>Bordered cells with vertical dividers.</summary>
    Bordered,
    
    /// <summary>Minimal borders, subtle styling.</summary>
    Minimal
}
