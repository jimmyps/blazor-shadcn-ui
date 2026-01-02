using System.Text.Json.Serialization;

namespace BlazorUI.Components.Grid;

/// <summary>
/// Specifies the filter operator for grid column filtering.
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum GridFilterOperator
{
    /// <summary>Equals comparison.</summary>
    Equals,
    
    /// <summary>Not equals comparison.</summary>
    NotEquals,
    
    /// <summary>Contains substring.</summary>
    Contains,
    
    /// <summary>Does not contain substring.</summary>
    NotContains,
    
    /// <summary>Starts with substring.</summary>
    StartsWith,
    
    /// <summary>Ends with substring.</summary>
    EndsWith,
    
    /// <summary>Less than comparison.</summary>
    LessThan,
    
    /// <summary>Less than or equal comparison.</summary>
    LessThanOrEqual,
    
    /// <summary>Greater than comparison.</summary>
    GreaterThan,
    
    /// <summary>Greater than or equal comparison.</summary>
    GreaterThanOrEqual,
    
    /// <summary>Value is empty or null.</summary>
    IsEmpty,
    
    /// <summary>Value is not empty or null.</summary>
    IsNotEmpty
}
