using System.Text.Json.Serialization;

namespace BlazorUI.Components.Grid;

/// <summary>
/// Specifies the virtualization strategy for the grid.
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum GridVirtualizationMode
{
    /// <summary>Renderer decides based on data size.</summary>
    Auto,
    /// <summary>No virtualization.</summary>
    None,
    /// <summary>Row virtualization only.</summary>
    RowOnly,
    /// <summary>Both row and column virtualization.</summary>
    RowAndColumn
}
