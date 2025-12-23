using System.Text.Json.Serialization;

namespace BlazorUI.Components.Grid;

/// <summary>
/// Specifies the pinning position of a grid column.
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum GridColumnPinPosition
{
    /// <summary>Column is not pinned.</summary>
    None,
    /// <summary>Column is pinned to the left side.</summary>
    Left,
    /// <summary>Column is pinned to the right side.</summary>
    Right
}
