using System.Text.Json.Serialization;

namespace BlazorUI.Components.Grid;

/// <summary>
/// Specifies the spacing density for grid rows and cells.
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
