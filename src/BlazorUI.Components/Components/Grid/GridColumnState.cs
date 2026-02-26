using System.Text.Json.Serialization;

namespace BlazorUI.Components.Grid;

/// <summary>
/// Represents the state of a single grid column.
/// </summary>
public class GridColumnState
{
    /// <summary>
    /// Gets or sets the field name of the column.
    /// </summary>
    [JsonPropertyName("field")]
    public string Field { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets whether the column is visible.
    /// </summary>
    [JsonPropertyName("visible")]
    public bool Visible { get; set; } = true;

    /// <summary>
    /// Gets or sets the column width (e.g., "100px", "20%").
    /// </summary>
    [JsonPropertyName("width")]
    public string? Width { get; set; }

    /// <summary>
    /// Gets or sets the column pinning position.
    /// </summary>
    [JsonPropertyName("pinned")]
    public GridColumnPinPosition Pinned { get; set; } = GridColumnPinPosition.None;

    /// <summary>
    /// Gets or sets the column display order.
    /// </summary>
    [JsonPropertyName("order")]
    public int Order { get; set; }
    
    /// <summary>
    /// Gets or sets the sort direction ("asc", "desc", or null for no sort).
    /// </summary>
    [JsonPropertyName("sort")]
    public string? Sort { get; set; }
    
    /// <summary>
    /// Gets or sets the sort index for multi-column sorting.
    /// </summary>
    [JsonPropertyName("sortIndex")]
    public int? SortIndex { get; set; }
    
    /// <summary>
    /// Gets or sets the aggregation function for this column.
    /// </summary>
    [JsonPropertyName("aggFunc")]
    public string? AggFunc { get; set; }
    
    /// <summary>
    /// Gets or sets whether this column is used for row grouping.
    /// </summary>
    [JsonPropertyName("rowGroup")]
    public bool? RowGroup { get; set; }
    
    /// <summary>
    /// Gets or sets the index of this column in row grouping.
    /// </summary>
    [JsonPropertyName("rowGroupIndex")]
    public int? RowGroupIndex { get; set; }
    
    /// <summary>
    /// Gets or sets whether this column is used for pivoting.
    /// </summary>
    [JsonPropertyName("pivot")]
    public bool? Pivot { get; set; }
    
    /// <summary>
    /// Gets or sets the index of this column in pivoting.
    /// </summary>
    [JsonPropertyName("pivotIndex")]
    public int? PivotIndex { get; set; }
    
    /// <summary>
    /// Gets or sets the flex sizing value for this column.
    /// </summary>
    [JsonPropertyName("flex")]
    public int? Flex { get; set; }
}
