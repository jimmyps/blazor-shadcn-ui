using BlazorUI.Components.Utilities;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using System.Linq.Expressions;

namespace BlazorUI.Components.ColorPicker;

/// <summary>
/// A color picker component with popover, canvas-based selection, and preset swatches.
/// </summary>
public partial class ColorPicker : ComponentBase, IAsyncDisposable
{
    private ElementReference _canvasRef;
    private IJSObjectReference? _colorPickerModule;
    private DotNetObjectReference<ColorPicker>? _dotNetRef;
    private bool _isOpen;
    private bool _isInitialized;

    // Color components
    private int _red;
    private int _green;
    private int _blue;
    private double _alpha = 1.0;
    private double _hue;
    private double _saturation = 100;
    private double _lightness = 50;

    // Validation
    private EditContext? _previousEditContext;
    private FieldIdentifier _fieldIdentifier;

    [Inject]
    private IJSRuntime JSRuntime { get; set; } = default!;

    [CascadingParameter]
    private EditContext? EditContext { get; set; }

    /// <summary>
    /// The selected color value as a string.
    /// </summary>
    [Parameter]
    public string? Color { get; set; }

    /// <summary>
    /// Event callback invoked when the color changes.
    /// </summary>
    [Parameter]
    public EventCallback<string?> ColorChanged { get; set; }

    /// <summary>
    /// The color format (Hex, RGB, HSL).
    /// </summary>
    [Parameter]
    public ColorFormat Format { get; set; } = ColorFormat.Hex;

    /// <summary>
    /// The size of the color picker.
    /// </summary>
    [Parameter]
    public ColorPickerSize Size { get; set; } = ColorPickerSize.Compact;

    /// <summary>
    /// Whether to show alpha/opacity control.
    /// </summary>
    [Parameter]
    public bool ShowAlpha { get; set; }

    /// <summary>
    /// Whether to show RGB input fields.
    /// </summary>
    [Parameter]
    public bool ShowInputs { get; set; } = true;

    /// <summary>
    /// Whether to show preset color swatches.
    /// </summary>
    [Parameter]
    public bool ShowPresets { get; set; } = true;

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
    /// Whether to show validation errors.
    /// </summary>
    [Parameter]
    public bool ShowValidationError { get; set; }

    /// <summary>
    /// Additional CSS classes.
    /// </summary>
    [Parameter]
    public string? Class { get; set; }

    /// <summary>
    /// HTML id attribute.
    /// </summary>
    [Parameter]
    public string? Id { get; set; }

    /// <summary>
    /// Name for form submission.
    /// </summary>
    [Parameter]
    public string? Name { get; set; }

    /// <summary>
    /// Expression that identifies the bound value.
    /// </summary>
    [Parameter]
    public Expression<Func<string?>>? ValueExpression { get; set; }

    /// <summary>
    /// Preset color swatches.
    /// </summary>
    private static readonly string[] PresetColors = new[]
    {
        "#000000", "#FFFFFF", "#EF4444", "#10B981", "#3B82F6", "#F59E0B", "#EC4899", "#06B6D4",
        "#FF6B35", "#6366F1", "#808080", "#C0C0C0", "#800000", "#22C55E", "#1E3A8A", "#FBBF24"
    };

    /// <summary>
    /// Get preset colors based on current size.
    /// </summary>
    private string[] VisiblePresetColors => Size == ColorPickerSize.Compact 
        ? PresetColors.Take(12).ToArray() 
        : PresetColors;

    private string TriggerCssClass => ClassNames.cn(
        "flex justify-start items-center w-full rounded-md border border-input shadow-xs",
        "bg-background h-9 px-2 py-2 text-sm",
        "hover:bg-accent hover:text-accent-foreground",
        "focus:outline-none focus:ring-ring focus:ring-[2px] focus:ring-ring/50",
        "disabled:cursor-not-allowed disabled:opacity-50",
        "transition-colors",
        Class
    );

    private string CurrentColorString => GetColorString();

    // Size-dependent properties
    private string PopoverWidth => Size == ColorPickerSize.Compact ? "w-72" : "w-80";
    private string PopoverPadding => Size == ColorPickerSize.Compact ? "p-3" : "p-4";
    private string CanvasHeight => Size == ColorPickerSize.Compact ? "h-32" : "h-48";
    private string PresetColumns => Size == ColorPickerSize.Compact ? "grid-cols-6" : "grid-cols-8";
    private string PresetSize => Size == ColorPickerSize.Compact ? "h-6 w-6" : "h-8 w-8";
    private string SpacingClass => Size == ColorPickerSize.Compact ? "space-y-2" : "space-y-3";

    protected override void OnInitialized()
    {
        Id ??= $"colorpicker-{Guid.NewGuid():N}";
        
        // Initialize with default or current color
        if (!string.IsNullOrWhiteSpace(Color))
        {
            ParseColor(Color);
        }
        else
        {
            // Default to blue (#3B82F6) for better visual feedback
            _red = 59;
            _green = 130;
            _blue = 246;
            _alpha = 1.0;
            UpdateHslFromRgb();
        }
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            try
            {
                _colorPickerModule = await JSRuntime.InvokeAsync<IJSObjectReference>(
                    "import", "./_content/NeoBlazorUI.Components/js/color-picker.js");
            }
            catch (JSException)
            {
                // JS module not available
            }
        }

        // Reset initialization when popover closes
        if (!_isOpen && _isInitialized)
        {
            _isInitialized = false;
        }

        if (_isOpen && !_isInitialized && _colorPickerModule != null)
        {
            try
            {
                // Add small delay to ensure canvas is fully rendered in DOM
                await Task.Delay(50);
                
                _dotNetRef = DotNetObjectReference.Create(this);
                
                await _colorPickerModule.InvokeVoidAsync("initializeColorCanvas",
                    $"{Id}-canvas", _hue, _saturation, _lightness, _dotNetRef);
                _isInitialized = true;
                
                StateHasChanged();
            }
            catch (JSException)
            {
                // Ignore initialization errors
            }
        }
    }

    [JSInvokable]
    public async Task OnCanvasColorChanged(double hue, double saturation, double lightness)
    {
        _hue = hue;
        _saturation = saturation;
        _lightness = lightness;
        
        UpdateRgbFromHsl();
        await NotifyColorChanged();
    }

    private void UpdateRgbFromHsl()
    {
        var (r, g, b) = ColorConverter.HslToRgb(_hue, _saturation, _lightness);
        _red = r;
        _green = g;
        _blue = b;
    }

    private void UpdateHslFromRgb()
    {
        var (h, s, l) = ColorConverter.RgbToHsl(_red, _green, _blue);
        _hue = h;
        _saturation = s;
        _lightness = l;
    }

    private async Task HandleHueChange(ChangeEventArgs e)
    {
        if (double.TryParse(e.Value?.ToString(), out var hue))
        {
            _hue = hue;
            UpdateRgbFromHsl();
            await UpdateCanvas();
            await NotifyColorChanged();
        }
    }

    private async Task HandleAlphaChange(ChangeEventArgs e)
    {
        if (double.TryParse(e.Value?.ToString(), out var alpha))
        {
            _alpha = alpha / 100.0;
            await NotifyColorChanged();
        }
    }

    private async Task HandleRedChange(int value)
    {
        if (value >= 0 && value <= 255)
        {
            _red = value;
            UpdateHslFromRgb();
            await UpdateCanvas();
            await NotifyColorChanged();
        }
    }

    private async Task HandleGreenChange(int value)
    {
        if (value >= 0 && value <= 255)
        {
            _green = value;
            UpdateHslFromRgb();
            await UpdateCanvas();
            await NotifyColorChanged();
        }
    }

    private async Task HandleBlueChange(int value)
    {
        if (value >= 0 && value <= 255)
        {
            _blue = value;
            UpdateHslFromRgb();
            await UpdateCanvas();
            await NotifyColorChanged();
        }
    }

    private async Task HandleHexInputChange(string value)
    {
        var parsed = ColorConverter.ParseHex(value);
        if (parsed.HasValue)
        {
            var (r, g, b, a) = parsed.Value;
            _red = r;
            _green = g;
            _blue = b;
            _alpha = a / 255.0;
            UpdateHslFromRgb();
            await UpdateCanvas();
            await NotifyColorChanged();
        }
    }

    private async Task HandlePresetClick(string presetColor)
    {
        ParseColor(presetColor);
        await UpdateCanvas();
        await NotifyColorChanged();
    }

    private void ParseColor(string color)
    {
        color = color.Trim();

        if (color.StartsWith("#"))
        {
            var parsed = ColorConverter.ParseHex(color);
            if (parsed.HasValue)
            {
                var (r, g, b, a) = parsed.Value;
                _red = r;
                _green = g;
                _blue = b;
                _alpha = a / 255.0;
                UpdateHslFromRgb();
            }
        }
        else if (color.StartsWith("rgb"))
        {
            var parsed = ColorConverter.ParseRgb(color);
            if (parsed.HasValue)
            {
                var (r, g, b, a) = parsed.Value;
                _red = r;
                _green = g;
                _blue = b;
                _alpha = a;
                UpdateHslFromRgb();
            }
        }
        else if (color.StartsWith("hsl"))
        {
            var parsed = ColorConverter.ParseHsl(color);
            if (parsed.HasValue)
            {
                var (h, s, l, a) = parsed.Value;
                _hue = h;
                _saturation = s;
                _lightness = l;
                _alpha = a;
                UpdateRgbFromHsl();
            }
        }
    }

    private string GetColorString()
    {
        return Format switch
        {
            ColorFormat.RGB when ShowAlpha => $"rgba({_red}, {_green}, {_blue}, {_alpha:F2})",
            ColorFormat.RGB => $"rgb({_red}, {_green}, {_blue})",
            ColorFormat.HSL when ShowAlpha => $"hsla({_hue:F0}, {_saturation:F0}%, {_lightness:F0}%, {_alpha:F2})",
            ColorFormat.HSL => $"hsl({_hue:F0}, {_saturation:F0}%, {_lightness:F0}%)",
            _ => ShowAlpha && _alpha < 1.0 
                ? ColorConverter.RgbToHex(_red, _green, _blue, (int)(_alpha * 255))
                : ColorConverter.RgbToHex(_red, _green, _blue)
        };
    }

    private string GetOpaqueColorString()
    {
        return ColorConverter.RgbToHex(_red, _green, _blue);
    }

    private async Task NotifyColorChanged()
    {
        var colorString = GetColorString();
        Color = colorString;
        
        if (ColorChanged.HasDelegate)
        {
            await ColorChanged.InvokeAsync(colorString);
        }

        StateHasChanged();
    }

    private async Task UpdateCanvas()
    {
        if (_isInitialized && _colorPickerModule != null)
        {
            try
            {
                await _colorPickerModule.InvokeVoidAsync("updateColorCanvas",
                    $"{Id}-canvas", _hue, _saturation, _lightness);
            }
            catch (JSException)
            {
                // Ignore
            }
        }
    }

    public async ValueTask DisposeAsync()
    {
        if (_colorPickerModule != null && _isInitialized)
        {
            try
            {
                await _colorPickerModule.InvokeVoidAsync("disposeColorCanvas", $"{Id}-canvas");
                await _colorPickerModule.DisposeAsync();
            }
            catch (JSException)
            {
                // Ignore
            }
        }

        _dotNetRef?.Dispose();
    }
}
