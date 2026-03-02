using System.Collections.Concurrent;
using System.Reflection;

namespace BlazorUI.Components.Chart;

/// <summary>
/// Utility for extracting data from objects using string-based keys with caching.
/// </summary>
internal static class DataExtractor
{
    private static readonly ConcurrentDictionary<(Type, string), Func<object, object?>> _cache = new();
    
    /// <summary>
    /// Extracts a value from an object using a data key (property or field name).
    /// </summary>
    public static object? GetValue<TData>(TData data, string dataKey)
    {
        if (data == null || string.IsNullOrEmpty(dataKey))
        {
            return null;
        }
        
        var type = typeof(TData);
        var key = (type, dataKey);
        
        var accessor = _cache.GetOrAdd(key, _ => CreateAccessor<TData>(dataKey));
        return accessor(data);
    }
    
    /// <summary>
    /// Creates a cached accessor function for a property or field.
    /// </summary>
    private static Func<object, object?> CreateAccessor<TData>(string dataKey)
    {
        var type = typeof(TData);
        
        // Try property first
        var property = type.GetProperty(dataKey, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
        if (property != null)
        {
            return obj => property.GetValue(obj);
        }
        
        // Try field
        var field = type.GetField(dataKey, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
        if (field != null)
        {
            return obj => field.GetValue(obj);
        }
        
        // Not found
        return _ => null;
    }
    
    /// <summary>
    /// Extracts values from a collection of data items using a data key.
    /// </summary>
    public static List<object?> GetValues<TData>(IEnumerable<TData> data, string dataKey)
    {
        return data.Select(item => GetValue(item, dataKey)).ToList();
    }
    
    /// <summary>
    /// Extracts string values (for category axes).
    /// </summary>
    public static List<string> GetStringValues<TData>(IEnumerable<TData> data, string dataKey)
    {
        return data.Select(item => GetValue(item, dataKey)?.ToString() ?? string.Empty).ToList();
    }
    
    /// <summary>
    /// Extracts numeric values.
    /// </summary>
    public static List<double> GetNumericValues<TData>(IEnumerable<TData> data, string dataKey)
    {
        return data.Select(item =>
        {
            var value = GetValue(item, dataKey);
            return Convert.ToDouble(value ?? 0);
        }).ToList();
    }
}
