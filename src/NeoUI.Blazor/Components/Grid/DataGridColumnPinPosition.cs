using System.Text.Json.Serialization;

namespace NeoUI.Blazor;

/// <summary>
/// Specifies the pinning position for a grid column.
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum DataDataGridColumnPinPosition
{
    /// <summary>Not pinned.</summary>
    None,
    
    /// <summary>Pinned to left side.</summary>
    Left,
    
    /// <summary>Pinned to right side.</summary>
    Right
}
