using System.Text.Json.Serialization;

namespace BlazorUI.Components.Grid;

/// <summary>
/// Specifies the sort direction for a grid column.
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum GridSortDirection
{
    /// <summary>No sorting applied.</summary>
    None,
    /// <summary>Sort in ascending order.</summary>
    Ascending,
    /// <summary>Sort in descending order.</summary>
    Descending
}
