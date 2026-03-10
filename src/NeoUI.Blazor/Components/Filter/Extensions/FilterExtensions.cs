using System.Linq.Expressions;
using System.Reflection;

namespace NeoUI.Blazor;

/// <summary>LINQ extension methods for applying FilterGroup to IQueryable/IEnumerable.</summary>
public static class FilterExtensions
{
    /// <summary>Applies a FilterGroup to an IQueryable&lt;T&gt;.</summary>
    public static IQueryable<T> ApplyFilter<T>(this IQueryable<T> source, FilterGroup? filter)
    {
        if (filter == null || (!filter.Conditions.Any() && !filter.NestedGroups.Any()))
            return source;
        var param = Expression.Parameter(typeof(T), "x");
        var expr = BuildGroupExpression<T>(filter, param);
        if (expr == null) return source;
        return source.Where(Expression.Lambda<Func<T, bool>>(expr, param));
    }

    /// <summary>Applies a FilterGroup to an IEnumerable&lt;T&gt;.</summary>
    public static IEnumerable<T> ApplyFilter<T>(this IEnumerable<T> source, FilterGroup? filter)
        => source.AsQueryable().ApplyFilter(filter);

    /// <summary>Applies a FilterGroup to an IQueryable&lt;T&gt;. Alias for <see cref="ApplyFilter{T}(IQueryable{T}, FilterGroup?)"/>.</summary>
    public static IQueryable<T> ApplyFilters<T>(this IQueryable<T> source, FilterGroup? filter)
        => source.ApplyFilter(filter);

    /// <summary>Applies a FilterGroup to an IEnumerable&lt;T&gt;. Alias for <see cref="ApplyFilter{T}(IEnumerable{T}, FilterGroup?)"/>.</summary>
    public static IEnumerable<T> ApplyFilters<T>(this IEnumerable<T> source, FilterGroup? filter)
        => source.ApplyFilter(filter);

    private static Expression? BuildGroupExpression<T>(FilterGroup group, ParameterExpression param)
    {
        var parts = new List<Expression>();
        foreach (var condition in group.Conditions)
        {
            var expr = BuildConditionExpression<T>(condition, param);
            if (expr != null) parts.Add(expr);
        }
        foreach (var nested in group.NestedGroups)
        {
            var expr = BuildGroupExpression<T>(nested, param);
            if (expr != null) parts.Add(expr);
        }
        if (!parts.Any()) return null;
        return group.Logic == LogicalOperator.And
            ? parts.Aggregate(Expression.AndAlso)
            : parts.Aggregate(Expression.OrElse);
    }

    private static Expression? BuildConditionExpression<T>(FilterCondition condition, ParameterExpression param)
    {
        if (string.IsNullOrEmpty(condition.Field)) return null;
        var prop = typeof(T).GetProperty(condition.Field, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
        if (prop == null) return null;
        var member = Expression.Property(param, prop);
        var propType = prop.PropertyType;
        var underlyingType = Nullable.GetUnderlyingType(propType) ?? propType;

        // MultiSelect: property is a string collection and value is a list of selected strings
        if (condition.Value is List<string> multiValues && multiValues.Count > 0
            && underlyingType != typeof(string)
            && typeof(IEnumerable<string>).IsAssignableFrom(underlyingType))
        {
            return BuildMultiSelectExpression(member, multiValues);
        }

        Expression? result = condition.Operator switch
        {
            FilterOperator.IsEmpty => IsNullOrEmpty(member, propType),
            FilterOperator.IsNotEmpty => Expression.Not(IsNullOrEmpty(member, propType)),
            FilterOperator.IsTrue => Expression.Equal(member, Expression.Constant(true, propType)),
            FilterOperator.IsFalse => Expression.Equal(member, Expression.Constant(false, propType)),
            _ => BuildComparisonExpression(condition, member, propType, underlyingType)
        };
        return result;
    }

    /// <summary>
    /// Builds: Enumerable.Any(member, s => selectedValues.Contains(s))
    /// i.e. the collection property contains at least one of the selected values.
    /// </summary>
    private static Expression BuildMultiSelectExpression(MemberExpression member, List<string> selectedValues)
    {
        var anyMethod = typeof(Enumerable)
            .GetMethods()
            .First(m => m.Name == "Any" && m.GetParameters().Length == 2)
            .MakeGenericMethod(typeof(string));

        var sParam = Expression.Parameter(typeof(string), "s");
        var selectedConst = Expression.Constant(selectedValues, typeof(List<string>));
        var containsCall = Expression.Call(selectedConst, typeof(List<string>).GetMethod("Contains")!, sParam);
        var predicate = Expression.Lambda<Func<string, bool>>(containsCall, sParam);

        return Expression.Call(anyMethod, member, predicate);
    }

    private static Expression IsNullOrEmpty(Expression member, Type propType)
    {
        if (propType == typeof(string))
            return Expression.Call(typeof(string), nameof(string.IsNullOrEmpty), null, member);
        return Expression.Equal(member, Expression.Default(propType));
    }

    private static Expression? BuildComparisonExpression(FilterCondition condition, MemberExpression member, Type propType, Type underlyingType)
    {
        if (condition.Value == null) return null;
        try
        {
            var converted = Convert.ChangeType(condition.Value, underlyingType);
            var constant = Expression.Constant(converted, propType);
            return condition.Operator switch
            {
                FilterOperator.Equals => Expression.Equal(member, constant),
                FilterOperator.NotEquals => Expression.NotEqual(member, constant),
                FilterOperator.GreaterThan => Expression.GreaterThan(member, constant),
                FilterOperator.LessThan => Expression.LessThan(member, constant),
                FilterOperator.GreaterThanOrEqual => Expression.GreaterThanOrEqual(member, constant),
                FilterOperator.LessThanOrEqual => Expression.LessThanOrEqual(member, constant),
                FilterOperator.Contains when underlyingType == typeof(string) =>
                    Expression.Call(member, typeof(string).GetMethod(nameof(string.Contains), new[] { typeof(string), typeof(StringComparison) })!,
                        constant, Expression.Constant(StringComparison.OrdinalIgnoreCase)),
                FilterOperator.NotContains when underlyingType == typeof(string) =>
                    Expression.Not(Expression.Call(member, typeof(string).GetMethod(nameof(string.Contains), new[] { typeof(string), typeof(StringComparison) })!,
                        constant, Expression.Constant(StringComparison.OrdinalIgnoreCase))),
                FilterOperator.StartsWith when underlyingType == typeof(string) =>
                    Expression.Call(member, typeof(string).GetMethod(nameof(string.StartsWith), new[] { typeof(string), typeof(StringComparison) })!,
                        constant, Expression.Constant(StringComparison.OrdinalIgnoreCase)),
                FilterOperator.EndsWith when underlyingType == typeof(string) =>
                    Expression.Call(member, typeof(string).GetMethod(nameof(string.EndsWith), new[] { typeof(string), typeof(StringComparison) })!,
                        constant, Expression.Constant(StringComparison.OrdinalIgnoreCase)),
                FilterOperator.Between when condition.SecondaryValue != null =>
                    Expression.AndAlso(
                        Expression.GreaterThanOrEqual(member, constant),
                        Expression.LessThanOrEqual(member, Expression.Constant(Convert.ChangeType(condition.SecondaryValue, underlyingType), propType))),
                FilterOperator.NotBetween when condition.SecondaryValue != null =>
                    Expression.OrElse(
                        Expression.LessThan(member, constant),
                        Expression.GreaterThan(member, Expression.Constant(Convert.ChangeType(condition.SecondaryValue, underlyingType), propType))),
                _ => null
            };
        }
        catch { return null; /* Type conversion failed for this condition — skip it */ }
    }
}
