namespace NeoUI.Blazor;

/// <summary>Defines filter operators for different data types.</summary>
public enum FilterOperator
{
    // Text
    Equals, NotEquals, Contains, NotContains, StartsWith, EndsWith, IsEmpty, IsNotEmpty,
    // Numeric/Date
    GreaterThan, LessThan, GreaterThanOrEqual, LessThanOrEqual, Between, NotBetween,
    // Collection
    IsAnyOf, IsNoneOf, IsAllOf,
    // Boolean
    IsTrue, IsFalse
}
