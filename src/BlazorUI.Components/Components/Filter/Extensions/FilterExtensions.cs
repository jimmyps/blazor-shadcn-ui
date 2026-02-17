using System.Linq.Expressions;

namespace BlazorUI.Components.Filter;

/// <summary>
/// Extension methods for applying filters to collections
/// </summary>
public static class FilterExtensions
{
    /// <summary>
    /// Apply filters to an IEnumerable using LINQ
    /// </summary>
    public static IEnumerable<T> ApplyFilters<T>(
        this IEnumerable<T> source, 
        FilterGroup? filterGroup)
    {
        if (filterGroup == null || 
            (!filterGroup.Conditions.Any() && !filterGroup.NestedGroups.Any()))
            return source;
            
        return source.Where(item => EvaluateGroup(item, filterGroup));
    }
    
    private static bool EvaluateGroup<T>(T item, FilterGroup group)
    {
        var conditionResults = group.Conditions
            .Select(c => EvaluateCondition(item, c))
            .ToList();
            
        var nestedResults = group.NestedGroups
            .Select(g => EvaluateGroup(item, g))
            .ToList();
            
        var allResults = conditionResults.Concat(nestedResults);
        
        return group.Logic == LogicalOperator.And
            ? allResults.All(r => r)
            : allResults.Any(r => r);
    }
    
    private static bool EvaluateCondition<T>(T item, FilterCondition condition)
    {
        var propertyValue = GetPropertyValue(item, condition.Field);
        
        return condition.Operator switch
        {
            FilterOperator.Equals => Equals(propertyValue, condition.Value),
            FilterOperator.NotEquals => !Equals(propertyValue, condition.Value),
            FilterOperator.Contains => propertyValue?.ToString()?.Contains(
                condition.Value?.ToString() ?? "", 
                StringComparison.OrdinalIgnoreCase) ?? false,
            FilterOperator.StartsWith => propertyValue?.ToString()?.StartsWith(
                condition.Value?.ToString() ?? "", 
                StringComparison.OrdinalIgnoreCase) ?? false,
            FilterOperator.EndsWith => propertyValue?.ToString()?.EndsWith(
                condition.Value?.ToString() ?? "", 
                StringComparison.OrdinalIgnoreCase) ?? false,
            FilterOperator.GreaterThan => Compare(propertyValue, condition.Value) > 0,
            FilterOperator.LessThan => Compare(propertyValue, condition.Value) < 0,
            FilterOperator.GreaterThanOrEqual => Compare(propertyValue, condition.Value) >= 0,
            FilterOperator.LessThanOrEqual => Compare(propertyValue, condition.Value) <= 0,
            FilterOperator.Between => CompareBetween(
                propertyValue, 
                condition.Value, 
                condition.SecondaryValue),
            FilterOperator.IsEmpty => string.IsNullOrEmpty(propertyValue?.ToString()),
            FilterOperator.IsNotEmpty => !string.IsNullOrEmpty(propertyValue?.ToString()),
            FilterOperator.IsAnyOf => IsAnyOf(propertyValue, condition.Value as List<string>),
            FilterOperator.IsNoneOf => !IsAnyOf(propertyValue, condition.Value as List<string>),
            FilterOperator.IsTrue => propertyValue is bool b && b,
            FilterOperator.IsFalse => propertyValue is bool bf && !bf,
            _ => false
        };
    }
    
    private static object? GetPropertyValue<T>(T item, string propertyName)
    {
        return typeof(T).GetProperty(propertyName)?.GetValue(item);
    }
    
    private static int Compare(object? a, object? b)
    {
        if (a is IComparable ca && b != null)
            return ca.CompareTo(Convert.ChangeType(b, a.GetType()));
        return 0;
    }
    
    private static bool CompareBetween(object? value, object? min, object? max)
    {
        if (value == null || min == null || max == null) return false;
        return Compare(value, min) >= 0 && Compare(value, max) <= 0;
    }
    
    private static bool IsAnyOf(object? value, List<string>? options)
    {
        if (value == null || options == null) return false;
        return options.Contains(value.ToString(), StringComparer.OrdinalIgnoreCase);
    }
}
