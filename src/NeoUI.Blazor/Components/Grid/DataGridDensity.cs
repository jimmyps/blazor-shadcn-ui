using System.Text.Json.Serialization;

namespace NeoUI.Blazor;

/// <summary>
/// Specifies the spacing density for grid rows and cells.
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum DataGridDensity
{
    /// <summary>Standard padding (default).</summary>
    Comfortable,
    
    /// <summary>Reduced padding for more rows.</summary>
    Compact,
    
    /// <summary>Increased padding for readability.</summary>
    Spacious
}
