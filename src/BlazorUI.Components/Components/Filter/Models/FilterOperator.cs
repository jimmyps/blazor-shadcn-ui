namespace BlazorUI.Components.Filter;

/// <summary>
/// Defines filter operators for different data types
/// </summary>
public enum FilterOperator
{
    // Text operators
    Equals,
    NotEquals,
    Contains,
    NotContains,
    StartsWith,
    EndsWith,
    IsEmpty,
    IsNotEmpty,
    
    // Numeric/Date operators
    GreaterThan,
    LessThan,
    GreaterThanOrEqual,
    LessThanOrEqual,
    Between,
    NotBetween,
    
    // Collection operators
    IsAnyOf,
    IsNoneOf,
    IsAllOf,
    
    // Boolean operators
    IsTrue,
    IsFalse
}
