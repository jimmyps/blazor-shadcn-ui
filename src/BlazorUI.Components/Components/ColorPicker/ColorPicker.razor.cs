using BlazorUI.Components.Utilities;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using System.Linq.Expressions;
using System.Text.RegularExpressions;

namespace BlazorUI.Components.ColorPicker;

/// <summary>
/// A color picker component that follows the shadcn/ui design system.
/// </summary>
public partial class ColorPicker : ComponentBase
{
    private EditContext? _previousEditContext;
    private FieldIdentifier _fieldIdentifier;
    
    private int _red = 0;
    private int _green = 0;
    private int _blue = 0;
    private double _alpha = 1.0;
    
    private int _hue = 0;
    private int _saturation = 0;
    private int _lightness = 0;

    [CascadingParameter]
    private EditContext? EditContext { get; set; }

    /// <summary>
    /// The selected color value.
    /// </summary>
    [Parameter]
    public string? Color { get; set; }

    /// <summary>
    /// Event callback invoked when the color changes.
    /// </summary>
    [Parameter]
    public EventCallback<string?> ColorChanged { get; set; }

    /// <summary>
    /// The color format to use.
    /// </summary>
    [Parameter]
    public ColorFormat Format { get; set; } = ColorFormat.Hex;

    /// <summary>
    /// Whether to show the alpha channel input.
    /// </summary>
    [Parameter]
    public bool ShowAlpha { get; set; }

    /// <summary>
    /// Whether the color picker is disabled.
    /// </summary>
    [Parameter]
    public bool Disabled { get; set; }

    /// <summary>
    /// Whether the color is required.
    /// </summary>
    [Parameter]
    public bool Required { get; set; }

    /// <summary>
    /// Whether to show validation error messages.
    /// </summary>
    [Parameter]
    public bool ShowValidationError { get; set; } = true;

    /// <summary>
    /// Additional CSS classes for the component.
    /// </summary>
    [Parameter]
    public string? Class { get; set; }

    /// <summary>
    /// HTML id attribute for the component.
    /// </summary>
    [Parameter]
    public string? Id { get; set; }

    /// <summary>
    /// Name of the component for form submission.
    /// </summary>
    [Parameter]
    public string? Name { get; set; }

    /// <summary>
    /// Expression that identifies the bound value.
    /// </summary>
    [Parameter]
    public Expression<Func<string?>>? ValueExpression { get; set; }

    private static readonly string[] PresetColors = new[]
    {
        "#000000", "#374151", "#6B7280", "#9CA3AF", "#D1D5DB", "#E5E7EB", "#F3F4F6", "#FFFFFF",
        "#EF4444", "#F97316", "#F59E0B", "#EAB308", "#84CC16", "#22C55E", "#10B981", "#14B8A6",
        "#06B6D4", "#0EA5E9", "#3B82F6", "#6366F1", "#8B5CF6", "#A855F7", "#D946EF", "#EC4899"
    };

    protected override void OnParametersSet()
    {
        base.OnParametersSet();

        if (!string.IsNullOrWhiteSpace(Color))
        {
            ParseColor(Color);
        }

        if (EditContext != null && ValueExpression != null)
        {
            if (EditContext != _previousEditContext)
            {
                _previousEditContext = EditContext;
                _fieldIdentifier = FieldIdentifier.Create(ValueExpression);
            }
        }
    }

    private void ParseColor(string color)
    {
        color = color.Trim();

        if (color.StartsWith("#"))
        {
            ParseHex(color);
        }
        else if (color.StartsWith("rgb"))
        {
            ParseRgb(color);
        }
        else if (color.StartsWith("hsl"))
        {
            ParseHsl(color);
        }
    }

    private void ParseHex(string hex)
    {
        hex = hex.TrimStart('#');
        
        if (hex.Length == 6 || hex.Length == 8)
        {
            _red = Convert.ToInt32(hex.Substring(0, 2), 16);
            _green = Convert.ToInt32(hex.Substring(2, 2), 16);
            _blue = Convert.ToInt32(hex.Substring(4, 2), 16);
            
            if (hex.Length == 8)
            {
                _alpha = Convert.ToInt32(hex.Substring(6, 2), 16) / 255.0;
            }
            else
            {
                _alpha = 1.0;
            }

            RgbToHsl(_red, _green, _blue);
        }
    }

    private void ParseRgb(string rgb)
    {
        var match = Regex.Match(rgb, @"rgba?\s*\(\s*(\d+)\s*,\s*(\d+)\s*,\s*(\d+)\s*(?:,\s*([0-9.]+)\s*)?\)");
        if (match.Success)
        {
            _red = int.Parse(match.Groups[1].Value);
            _green = int.Parse(match.Groups[2].Value);
            _blue = int.Parse(match.Groups[3].Value);
            
            if (match.Groups[4].Success)
            {
                _alpha = double.Parse(match.Groups[4].Value);
            }
            else
            {
                _alpha = 1.0;
            }

            RgbToHsl(_red, _green, _blue);
        }
    }

    private void ParseHsl(string hsl)
    {
        var match = Regex.Match(hsl, @"hsla?\s*\(\s*(\d+)\s*,\s*(\d+)%\s*,\s*(\d+)%\s*(?:,\s*([0-9.]+)\s*)?\)");
        if (match.Success)
        {
            _hue = int.Parse(match.Groups[1].Value);
            _saturation = int.Parse(match.Groups[2].Value);
            _lightness = int.Parse(match.Groups[3].Value);
            
            if (match.Groups[4].Success)
            {
                _alpha = double.Parse(match.Groups[4].Value);
            }
            else
            {
                _alpha = 1.0;
            }

            HslToRgb(_hue, _saturation, _lightness);
        }
    }

    private void RgbToHsl(int r, int g, int b)
    {
        double rd = r / 255.0;
        double gd = g / 255.0;
        double bd = b / 255.0;

        double max = Math.Max(rd, Math.Max(gd, bd));
        double min = Math.Min(rd, Math.Min(gd, bd));
        double delta = max - min;

        double h = 0, s = 0, l = (max + min) / 2.0;

        if (delta != 0)
        {
            s = l > 0.5 ? delta / (2.0 - max - min) : delta / (max + min);

            if (max == rd)
                h = ((gd - bd) / delta + (gd < bd ? 6 : 0)) / 6.0;
            else if (max == gd)
                h = ((bd - rd) / delta + 2) / 6.0;
            else
                h = ((rd - gd) / delta + 4) / 6.0;
        }

        _hue = (int)(h * 360);
        _saturation = (int)(s * 100);
        _lightness = (int)(l * 100);
    }

    private void HslToRgb(int h, int s, int l)
    {
        double hd = h / 360.0;
        double sd = s / 100.0;
        double ld = l / 100.0;

        double r, g, b;

        if (sd == 0)
        {
            r = g = b = ld;
        }
        else
        {
            double q = ld < 0.5 ? ld * (1 + sd) : ld + sd - ld * sd;
            double p = 2 * ld - q;
            r = HueToRgb(p, q, hd + 1.0 / 3.0);
            g = HueToRgb(p, q, hd);
            b = HueToRgb(p, q, hd - 1.0 / 3.0);
        }

        _red = (int)(r * 255);
        _green = (int)(g * 255);
        _blue = (int)(b * 255);
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

    private string GetColorString()
    {
        return Format switch
        {
            ColorFormat.Hex => ShowAlpha
                ? $"#{_red:X2}{_green:X2}{_blue:X2}{(int)(_alpha * 255):X2}"
                : $"#{_red:X2}{_green:X2}{_blue:X2}",
            ColorFormat.RGB => ShowAlpha
                ? $"rgba({_red}, {_green}, {_blue}, {_alpha:F2})"
                : $"rgb({_red}, {_green}, {_blue})",
            ColorFormat.HSL => ShowAlpha
                ? $"hsla({_hue}, {_saturation}%, {_lightness}%, {_alpha:F2})"
                : $"hsl({_hue}, {_saturation}%, {_lightness}%)",
            _ => $"#{_red:X2}{_green:X2}{_blue:X2}"
        };
    }

    private async Task HandleRedChange(string? value)
    {
        if (int.TryParse(value, out int intValue))
        {
            _red = Math.Clamp(intValue, 0, 255);
            RgbToHsl(_red, _green, _blue);
            await UpdateColor();
        }
    }

    private async Task HandleGreenChange(string? value)
    {
        if (int.TryParse(value, out int intValue))
        {
            _green = Math.Clamp(intValue, 0, 255);
            RgbToHsl(_red, _green, _blue);
            await UpdateColor();
        }
    }

    private async Task HandleBlueChange(string? value)
    {
        if (int.TryParse(value, out int intValue))
        {
            _blue = Math.Clamp(intValue, 0, 255);
            RgbToHsl(_red, _green, _blue);
            await UpdateColor();
        }
    }

    private async Task HandleHueChange(string? value)
    {
        if (int.TryParse(value, out int intValue))
        {
            _hue = Math.Clamp(intValue, 0, 360);
            HslToRgb(_hue, _saturation, _lightness);
            await UpdateColor();
        }
    }

    private async Task HandleSaturationChange(string? value)
    {
        if (int.TryParse(value, out int intValue))
        {
            _saturation = Math.Clamp(intValue, 0, 100);
            HslToRgb(_hue, _saturation, _lightness);
            await UpdateColor();
        }
    }

    private async Task HandleLightnessChange(string? value)
    {
        if (int.TryParse(value, out int intValue))
        {
            _lightness = Math.Clamp(intValue, 0, 100);
            HslToRgb(_hue, _saturation, _lightness);
            await UpdateColor();
        }
    }

    private async Task HandleAlphaChange(string? value)
    {
        if (double.TryParse(value, out double doubleValue))
        {
            _alpha = Math.Clamp(doubleValue, 0, 1);
            await UpdateColor();
        }
    }

    private async Task HandleHexChange(string? value)
    {
        if (!string.IsNullOrWhiteSpace(value))
        {
            ParseHex(value);
            await UpdateColor();
        }
    }

    private async Task HandlePresetClick(string preset)
    {
        if (!Disabled)
        {
            ParseColor(preset);
            await UpdateColor();
        }
    }

    private async Task UpdateColor()
    {
        var newColor = GetColorString();
        Color = newColor;
        await ColorChanged.InvokeAsync(newColor);

        if (EditContext != null && ValueExpression != null)
        {
            EditContext.NotifyFieldChanged(_fieldIdentifier);
        }
    }

    private string GetPreviewStyle()
    {
        return $"background-color: {GetColorString()};";
    }

    private string CssClass => ClassNames.cn(
        "space-y-4",
        Class
    );
}
