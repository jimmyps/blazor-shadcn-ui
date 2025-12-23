using System.Text.Json.Serialization;

namespace BlazorUI.Components.Grid;

/// <summary>
/// Specifies the row selection behavior for a grid.
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum GridSelectionMode
{
    /// <summary>No row selection allowed.</summary>
    None,
    /// <summary>Only one row can be selected at a time.</summary>
    Single,
    /// <summary>Multiple rows can be selected simultaneously.</summary>
    Multiple
}
