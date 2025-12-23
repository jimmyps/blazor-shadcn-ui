using System.Text.Json.Serialization;

namespace BlazorUI.Components.Grid;

/// <summary>
/// Specifies filter comparison operators for grid columns.
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum GridFilterOperator
{
    /// <summary>Value equals filter value.</summary>
    Equals,
    /// <summary>Value does not equal filter value.</summary>
    NotEquals,
    /// <summary>Value contains filter value.</summary>
    Contains,
    /// <summary>Value does not contain filter value.</summary>
    NotContains,
    /// <summary>Value starts with filter value.</summary>
    StartsWith,
    /// <summary>Value ends with filter value.</summary>
    EndsWith,
    /// <summary>Value is less than filter value.</summary>
    LessThan,
    /// <summary>Value is less than or equal to filter value.</summary>
    LessThanOrEqual,
    /// <summary>Value is greater than filter value.</summary>
    GreaterThan,
    /// <summary>Value is greater than or equal to filter value.</summary>
    GreaterThanOrEqual,
    /// <summary>Value is empty or null.</summary>
    IsEmpty,
    /// <summary>Value is not empty or null.</summary>
    IsNotEmpty
}
