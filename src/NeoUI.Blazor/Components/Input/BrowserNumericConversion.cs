using System.Globalization;
using Microsoft.AspNetCore.Components;

namespace NeoUI.Blazor;
internal static class BrowserNumericConversion
{
    internal static bool IsInteger<T>() => Type.GetTypeCode(Nullable.GetUnderlyingType(typeof(T)) ?? typeof(T)) is
        TypeCode.Byte or TypeCode.SByte or TypeCode.Int16 or TypeCode.UInt16 or TypeCode.Int32 or TypeCode.UInt32 or TypeCode.Int64 or TypeCode.UInt64;
    internal static bool TryParse<T>(string? text, CultureInfo culture, out T? value)
    {
        if (string.IsNullOrWhiteSpace(text)) { value = default; return true; }
        try { return BindConverter.TryConvertTo<T?>(text, culture, out value); }
        catch (Exception ex) when (ex is FormatException or OverflowException or InvalidOperationException) { value = default; return false; }
    }
    internal static T? Clamp<T>(T? value, string? minimum, string? maximum)
    {
        if (value is not IComparable comparable) return value;
        if (!string.IsNullOrWhiteSpace(minimum) && TryParse<T>(minimum,CultureInfo.InvariantCulture,out var min) && comparable.CompareTo(min) < 0) value = min;
        if (value is IComparable next && !string.IsNullOrWhiteSpace(maximum) && TryParse<T>(maximum,CultureInfo.InvariantCulture,out var max) && next.CompareTo(max) > 0) value = max;
        return value;
    }
}
