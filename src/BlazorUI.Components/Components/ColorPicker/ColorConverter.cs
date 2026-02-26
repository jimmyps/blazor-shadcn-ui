namespace BlazorUI.Components.ColorPicker;

/// <summary>
/// Utility class for color conversions between RGB, HSL, and Hex formats.
/// </summary>
public static class ColorConverter
{
    /// <summary>
    /// Converts RGB to HSL.
    /// </summary>
    /// <param name="r">Red (0-255)</param>
    /// <param name="g">Green (0-255)</param>
    /// <param name="b">Blue (0-255)</param>
    /// <returns>HSL values (H: 0-360, S: 0-100, L: 0-100)</returns>
    public static (double h, double s, double l) RgbToHsl(int r, int g, int b)
    {
        double rd = r / 255.0;
        double gd = g / 255.0;
        double bd = b / 255.0;

        double max = Math.Max(rd, Math.Max(gd, bd));
        double min = Math.Min(rd, Math.Min(gd, bd));
        double delta = max - min;

        double h = 0, s = 0, l = (max + min) / 2;

        if (delta != 0)
        {
            s = l > 0.5 ? delta / (2 - max - min) : delta / (max + min);

            if (max == rd)
                h = ((gd - bd) / delta + (gd < bd ? 6 : 0)) / 6;
            else if (max == gd)
                h = ((bd - rd) / delta + 2) / 6;
            else
                h = ((rd - gd) / delta + 4) / 6;
        }

        return (h * 360, s * 100, l * 100);
    }

    /// <summary>
    /// Converts HSL to RGB.
    /// </summary>
    /// <param name="h">Hue (0-360)</param>
    /// <param name="s">Saturation (0-100)</param>
    /// <param name="l">Lightness (0-100)</param>
    /// <returns>RGB values (0-255)</returns>
    public static (int r, int g, int b) HslToRgb(double h, double s, double l)
    {
        h = h / 360.0;
        s = s / 100.0;
        l = l / 100.0;

        double r, g, b;

        if (s == 0)
        {
            r = g = b = l;
        }
        else
        {
            var q = l < 0.5 ? l * (1 + s) : l + s - l * s;
            var p = 2 * l - q;

            r = HueToRgb(p, q, h + 1.0 / 3.0);
            g = HueToRgb(p, q, h);
            b = HueToRgb(p, q, h - 1.0 / 3.0);
        }

        return ((int)Math.Round(r * 255), (int)Math.Round(g * 255), (int)Math.Round(b * 255));
    }

    private static double HueToRgb(double p, double q, double t)
    {
        if (t < 0) t += 1;
        if (t > 1) t -= 1;
        if (t < 1.0 / 6.0) return p + (q - p) * 6 * t;
        if (t < 1.0 / 2.0) return q;
        if (t < 2.0 / 3.0) return p + (q - p) * (2.0 / 3.0 - t) * 6;
        return p;
    }

    /// <summary>
    /// Converts RGB to Hex string.
    /// </summary>
    public static string RgbToHex(int r, int g, int b, int? a = null)
    {
        if (a.HasValue)
            return $"#{r:X2}{g:X2}{b:X2}{a:X2}";
        return $"#{r:X2}{g:X2}{b:X2}";
    }

    /// <summary>
    /// Parses a hex color string to RGB.
    /// </summary>
    public static (int r, int g, int b, int a)? ParseHex(string? hex)
    {
        if (string.IsNullOrWhiteSpace(hex))
            return null;

        hex = hex.Trim().TrimStart('#');

        if (hex.Length == 6)
        {
            return (
                Convert.ToInt32(hex.Substring(0, 2), 16),
                Convert.ToInt32(hex.Substring(2, 2), 16),
                Convert.ToInt32(hex.Substring(4, 2), 16),
                255
            );
        }
        else if (hex.Length == 8)
        {
            return (
                Convert.ToInt32(hex.Substring(0, 2), 16),
                Convert.ToInt32(hex.Substring(2, 2), 16),
                Convert.ToInt32(hex.Substring(4, 2), 16),
                Convert.ToInt32(hex.Substring(6, 2), 16)
            );
        }
        else if (hex.Length == 3)
        {
            return (
                Convert.ToInt32(hex.Substring(0, 1) + hex.Substring(0, 1), 16),
                Convert.ToInt32(hex.Substring(1, 1) + hex.Substring(1, 1), 16),
                Convert.ToInt32(hex.Substring(2, 1) + hex.Substring(2, 1), 16),
                255
            );
        }

        return null;
    }

    /// <summary>
    /// Parses RGB/RGBA string to components.
    /// </summary>
    public static (int r, int g, int b, double a)? ParseRgb(string? rgb)
    {
        if (string.IsNullOrWhiteSpace(rgb))
            return null;

        rgb = rgb.Trim();

        // rgb(255, 128, 64) or rgba(255, 128, 64, 0.5)
        var isRgba = rgb.StartsWith("rgba", StringComparison.OrdinalIgnoreCase);
        var start = rgb.IndexOf('(') + 1;
        var end = rgb.IndexOf(')');

        if (start < 1 || end < 0)
            return null;

        var values = rgb.Substring(start, end - start)
            .Split(',')
            .Select(v => v.Trim())
            .ToArray();

        if (values.Length < 3)
            return null;

        var r = int.Parse(values[0]);
        var g = int.Parse(values[1]);
        var b = int.Parse(values[2]);
        var a = values.Length > 3 ? double.Parse(values[3]) : 1.0;

        return (r, g, b, a);
    }

    /// <summary>
    /// Parses HSL/HSLA string to components.
    /// </summary>
    public static (double h, double s, double l, double a)? ParseHsl(string? hsl)
    {
        if (string.IsNullOrWhiteSpace(hsl))
            return null;

        hsl = hsl.Trim();

        var start = hsl.IndexOf('(') + 1;
        var end = hsl.IndexOf(')');

        if (start < 1 || end < 0)
            return null;

        var values = hsl.Substring(start, end - start)
            .Split(',')
            .Select(v => v.Trim().TrimEnd('%'))
            .ToArray();

        if (values.Length < 3)
            return null;

        var h = double.Parse(values[0]);
        var s = double.Parse(values[1]);
        var l = double.Parse(values[2]);
        var a = values.Length > 3 ? double.Parse(values[3]) : 1.0;

        return (h, s, l, a);
    }
}
