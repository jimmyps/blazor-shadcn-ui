using System.Text.Json.Serialization;

namespace BlazorUI.Components.Grid;

/// <summary>
/// Specifies the pinning position for a grid column.
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum GridColumnPinPosition
{
    /// <summary>Not pinned.</summary>
    None,
    
    /// <summary>Pinned to left side.</summary>
    Left,
    
    /// <summary>Pinned to right side.</summary>
    Right
}
