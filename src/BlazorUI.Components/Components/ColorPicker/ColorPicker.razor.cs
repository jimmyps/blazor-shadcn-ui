using BlazorUI.Components.Utilities;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using System.Linq.Expressions;

namespace BlazorUI.Components.ColorPicker;

/// <summary>
/// A color picker component with popover, canvas-based selection, and preset swatches.
/// </summary>
/// <remarks>
/// Supports multiple color formats (Hex, RGB, HSL) with alpha/opacity control, 
/// canvas-based color selection with hue slider, and customizable preset color swatches.
/// </remarks>
public partial class ColorPicker : ComponentBase, IAsyncDisposable
{
    /// <summary>
    /// Reference to the canvas element for color selection.
    /// </summary>
    private ElementReference _canvasRef;
    
    /// <summary>
    /// JavaScript module reference for canvas interaction.
    /// </summary>
    private IJSObjectReference? _colorPickerModule;
    
    /// <summary>
    /// .NET object reference for JavaScript callbacks.
    /// </summary>
    private DotNetObjectReference<ColorPicker>? _dotNetRef;
    
    /// <summary>
    /// Indicates whether the popover is currently open.
    /// </summary>
    private bool _isOpen;
    
    /// <summary>
    /// Indicates whether the canvas has been initialized.
    /// </summary>
    private bool _isInitialized;

    /// <summary>
    /// Red component of the current color (0-255).
    /// </summary>
    private int _red;
    
    /// <summary>
    /// Green component of the current color (0-255).
    /// </summary>
    private int _green;
    
    /// <summary>
    /// Blue component of the current color (0-255).
    /// </summary>
    private int _blue;
    
    /// <summary>
    /// Alpha/opacity value (0.0-1.0).
    /// </summary>
    private double _alpha = 1.0;
    
    /// <summary>
    /// Hue value (0-360 degrees).
    /// </summary>
    private double _hue;
    
    /// <summary>
    /// Saturation percentage (0-100).
    /// </summary>
    private double _saturation = 100;
    
    /// <summary>
    /// Lightness percentage (0-100).
    /// </summary>
    private double _lightness = 50;

    /// <summary>
    /// Tracks the previous EditContext for validation subscriptions.
    /// </summary>
    private EditContext? _previousEditContext;
    
    /// <summary>
    /// Field identifier for validation in EditContext.
    /// </summary>
    private FieldIdentifier _fieldIdentifier;

    /// <summary>
    /// Gets or sets the JavaScript runtime for interop operations.
    /// </summary>
    [Inject]
    private IJSRuntime JSRuntime { get; set; } = default!;

    /// <summary>
    /// Gets the cascaded EditContext from an EditForm.
    /// </summary>
    [CascadingParameter]
    private EditContext? EditContext { get; set; }

    /// <summary>
    /// Gets or sets the selected color value as a string in the specified format.
    /// </summary>
    [Parameter]
    public string? Color { get; set; }

    /// <summary>
    /// Gets or sets the callback invoked when the color changes.
    /// </summary>
    [Parameter]
    public EventCallback<string?> ColorChanged { get; set; }

    /// <summary>
    /// Gets or sets the color format for display and output (Hex, RGB, or HSL).
    /// </summary>
    [Parameter]
    public ColorFormat Format { get; set; } = ColorFormat.Hex;

    /// <summary>
    /// Gets or sets the size of the color picker (Compact or Full).
    /// </summary>
    [Parameter]
    public ColorPickerSize Size { get; set; } = ColorPickerSize.Compact;

    /// <summary>
    /// Gets or sets whether to show alpha/opacity control slider.
    /// </summary>
    [Parameter]
    public bool ShowAlpha { get; set; }

    /// <summary>
    /// Gets or sets whether to show RGB/HSL input fields for manual entry.
    /// </summary>
    [Parameter]
    public bool ShowInputs { get; set; } = true;

    /// <summary>
    /// Gets or sets whether to show preset color swatches for quick selection.
    /// </summary>
    [Parameter]
    public bool ShowPresets { get; set; } = true;

    /// <summary>
    /// Gets or sets whether the color picker is disabled.
    /// </summary>
    [Parameter]
    public bool Disabled { get; set; }

    /// <summary>
    /// Gets or sets whether a color selection is required.
    /// </summary>
    [Parameter]
    public bool Required { get; set; }

    /// <summary>
    /// Gets or sets whether to automatically show validation errors from EditContext.
    /// </summary>
    [Parameter]
    public bool ShowValidationError { get; set; }

    /// <summary>
    /// Gets or sets additional CSS classes to apply to the trigger button.
    /// </summary>
    [Parameter]
    public string? Class { get; set; }

    /// <summary>
    /// Gets or sets the HTML id attribute.
    /// </summary>
    [Parameter]
    public string? Id { get; set; }

    /// <summary>
    /// Gets or sets the name of the input for form submission.
    /// </summary>
    [Parameter]
    public string? Name { get; set; }

    /// <summary>
    /// Gets or sets an expression that identifies the bound value for validation.
    /// </summary>
    [Parameter]
    public Expression<Func<string?>>? ValueExpression { get; set; }

    /// <summary>
    /// Gets the array of preset color values to display as swatches.
    /// </summary>
    private static readonly string[] PresetColors = new[]
    {
        "#000000", "#FFFFFF", "#EF4444", "#10B981", "#3B82F6", "#F59E0B", "#EC4899", "#06B6D4",
        "#FF6B35", "#6366F1", "#808080", "#C0C0C0", "#800000", "#22C55E", "#1E3A8A", "#FBBF24"
    };

    /// <summary>
    /// Gets the visible preset colors based on current size setting.
    /// </summary>
    private string[] VisiblePresetColors => Size == ColorPickerSize.Compact 
        ? PresetColors.Take(12).ToArray() 
        : PresetColors;

    /// <summary>
    /// Gets the CSS classes for the trigger button.
    /// </summary>
    private string TriggerCssClass => ClassNames.cn(
        "flex justify-start items-center w-full rounded-md border border-input shadow-xs",
        "bg-background h-9 px-2 py-2 text-sm",
        "hover:bg-accent hover:text-accent-foreground",
        "focus:outline-none focus:ring-ring focus:ring-[2px] focus:ring-ring/50",
        "disabled:cursor-not-allowed disabled:opacity-50",
        "transition-colors",
        Class
    );

    /// <summary>
    /// Gets the current color as a formatted string.
    /// </summary>
    private string CurrentColorString => GetColorString();

    /// <summary>
    /// Gets the width class for the popover based on size.
    /// </summary>
    private string PopoverWidth => Size == ColorPickerSize.Compact ? "w-72" : "w-80";
    
    /// <summary>
    /// Gets the padding class for the popover based on size.
    /// </summary>
    private string PopoverPadding => Size == ColorPickerSize.Compact ? "p-3" : "p-4";
    
    /// <summary>
    /// Gets the height class for the canvas based on size.
    /// </summary>
    private string CanvasHeight => Size == ColorPickerSize.Compact ? "h-32" : "h-48";
    
    /// <summary>
    /// Gets the grid columns class for presets based on size.
    /// </summary>
    private string PresetColumns => Size == ColorPickerSize.Compact ? "grid-cols-6" : "grid-cols-8";
    
    /// <summary>
    /// Gets the size class for preset swatches based on size.
    /// </summary>
    private string PresetSize => Size == ColorPickerSize.Compact ? "h-6 w-6" : "h-8 w-8";
    
    /// <summary>
    /// Gets the spacing class based on size.
    /// </summary>
    private string SpacingClass => Size == ColorPickerSize.Compact ? "space-y-2" : "space-y-3";

    /// <summary>
    /// Initializes the component with default color values.
    /// </summary>
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

    /// <summary>
    /// Imports JavaScript module and initializes canvas when popover opens.
    /// </summary>
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

    /// <summary>
    /// JavaScript-invokable callback when canvas color selection changes.
    /// </summary>
    [JSInvokable]
    public async Task OnCanvasColorChanged(double hue, double saturation, double lightness)
    {
        _hue = hue;
        _saturation = saturation;
        _lightness = lightness;
        
        UpdateRgbFromHsl();
        await NotifyColorChanged();
    }

    /// <summary>
    /// Converts HSL values to RGB color components.
    /// </summary>
    private void UpdateRgbFromHsl()
    {
        var (r, g, b) = ColorConverter.HslToRgb(_hue, _saturation, _lightness);
        _red = r;
        _green = g;
        _blue = b;
    }

    /// <summary>
    /// Converts RGB values to HSL color components.
    /// </summary>
    private void UpdateHslFromRgb()
    {
        var (h, s, l) = ColorConverter.RgbToHsl(_red, _green, _blue);
        _hue = h;
        _saturation = s;
        _lightness = l;
    }

    /// <summary>
    /// Handles changes to the hue slider.
    /// </summary>
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

    /// <summary>
    /// Handles changes to the alpha/opacity slider.
    /// </summary>
    private async Task HandleAlphaChange(ChangeEventArgs e)
    {
        if (double.TryParse(e.Value?.ToString(), out var alpha))
        {
            _alpha = alpha / 100.0;
            await NotifyColorChanged();
        }
    }

    /// <summary>
    /// Handles changes to the red input field.
    /// </summary>
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

    /// <summary>
    /// Handles changes to the green input field.
    /// </summary>
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

    /// <summary>
    /// Handles changes to the blue input field.
    /// </summary>
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

    /// <summary>
    /// Handles changes to the hex color input field.
    /// </summary>
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

    /// <summary>
    /// Handles clicking a preset color swatch.
    /// </summary>
    private async Task HandlePresetClick(string presetColor)
    {
        ParseColor(presetColor);
        await UpdateCanvas();
        await NotifyColorChanged();
    }

    /// <summary>
    /// Parses a color string (hex, rgb, or hsl) into color components.
    /// </summary>
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

    /// <summary>
    /// Gets the color as a formatted string in the current format.
    /// </summary>
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

    /// <summary>
    /// Gets the color as an opaque hex string (without alpha).
    /// </summary>
    private string GetOpaqueColorString()
    {
        return ColorConverter.RgbToHex(_red, _green, _blue);
    }

    /// <summary>
    /// Notifies parent component of color changes.
    /// </summary>
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

    /// <summary>
    /// Updates the canvas display via JavaScript interop.
    /// </summary>
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

    /// <summary>
    /// Disposes JavaScript resources and cleans up canvas.
    /// </summary>
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
