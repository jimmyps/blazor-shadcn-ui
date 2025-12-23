using System.Text.Json.Serialization;

namespace BlazorUI.Components.Grid;

/// <summary>
/// Describes a filter operation on a grid column.
/// </summary>
public class GridFilterDescriptor
{
    /// <summary>
    /// Gets or sets the field name to filter.
    /// </summary>
    [JsonPropertyName("field")]
    public string Field { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the filter operator.
    /// </summary>
    [JsonPropertyName("operator")]
    public GridFilterOperator Operator { get; set; } = GridFilterOperator.Contains;

    /// <summary>
    /// Gets or sets the filter value.
    /// </summary>
    [JsonPropertyName("value")]
    public object? Value { get; set; }

    /// <summary>
    /// Gets or sets whether the filter is case-sensitive.
    /// </summary>
    [JsonPropertyName("caseSensitive")]
    public bool CaseSensitive { get; set; }
}
