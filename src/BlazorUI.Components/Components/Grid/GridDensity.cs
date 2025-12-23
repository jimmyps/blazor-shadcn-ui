using System.Text.Json.Serialization;

namespace BlazorUI.Components.Grid;

/// <summary>
/// Specifies the row density (spacing) for the grid.
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum GridDensity
{
    /// <summary>Standard padding (default).</summary>
    Comfortable,
    /// <summary>Reduced padding for more rows.</summary>
    Compact,
    /// <summary>Increased padding for readability.</summary>
    Spacious
}
