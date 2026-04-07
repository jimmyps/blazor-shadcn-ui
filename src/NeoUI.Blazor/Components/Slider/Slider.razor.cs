using Microsoft.AspNetCore.Components;

namespace NeoUI.Blazor;

/// <summary>
/// A slider component for selecting a numeric value from a range.
/// </summary>
/// <remarks>
/// <para>
/// The Slider component provides an interactive range input control.
/// It follows the shadcn/ui design system with full accessibility support.
/// </para>
/// <para>
/// Features:
/// - Configurable min, max, and step values
/// - Two-way data binding with @bind-Value
/// - Keyboard navigation (arrow keys)
/// - Mouse and touch interaction
/// - Disabled state support
/// - Dark mode compatible via CSS variables
/// </para>
/// </remarks>
/// <example>
/// <code>
/// &lt;Slider @bind-Value="Volume" Min="0" Max="100" Step="1" /&gt;
/// </code>
/// </example>
public partial class Slider : ComponentBase
{
    /// <summary>
    /// Gets or sets the current value of the slider.
    /// </summary>
    [Parameter]
    public double Value { get; set; }

    /// <summary>
    /// Gets or sets the callback that is invoked when the value changes.
    /// </summary>
    [Parameter]
    public EventCallback<double> ValueChanged { get; set; }

    /// <summary>
    /// Gets or sets the minimum value.
    /// </summary>
    [Parameter]
    public double Min { get; set; } = 0;

    /// <summary>
    /// Gets or sets the maximum value.
    /// </summary>
    [Parameter]
    public double Max { get; set; } = 100;

    /// <summary>
    /// Gets or sets the step increment.
    /// </summary>
    [Parameter]
    public double Step { get; set; } = 1;

    /// <summary>
    /// Gets or sets whether the slider is disabled.
    /// </summary>
    [Parameter]
    public bool Disabled { get; set; }

    /// <summary>
    /// Gets or sets additional CSS classes to apply to the slider.
    /// </summary>
    [Parameter]
    public string? Class { get; set; }

    [CascadingParameter(Name = "StyleVariant")]
    private StyleVariant _styleVariant { get; set; } = StyleVariant.Default;

    private async Task OnInputAsync(ChangeEventArgs e)
    {
        if (double.TryParse(e.Value?.ToString(), out var newValue))
        {
            Value = newValue;
            await ValueChanged.InvokeAsync(Value);
        }
    }

    private async Task OnChangeAsync(ChangeEventArgs e)
    {
        if (double.TryParse(e.Value?.ToString(), out var newValue))
        {
            Value = newValue;
            await ValueChanged.InvokeAsync(Value);
        }
    }

    private string ContainerCssClass => ClassNames.cn(
        "relative flex w-full touch-none select-none items-center",
        Class
    );

    /// <summary>
    /// Computes an inline CSS gradient for the active track fill (WebKit).
    /// The filled portion (left of thumb) uses --primary; the rest uses --input.
    /// </summary>
    private string TrackStyle
    {
        get
        {
            var pct = Max > Min ? ((Value - Min) / (Max - Min)) * 100 : 0;
            return $"background: linear-gradient(to right, var(--primary) {pct:F1}%, var(--input) {pct:F1}%)";
        }
    }

    private string CssClass => ClassNames.cn(
        // Track — no bg-* class, gradient is applied via inline style
        "w-full h-2 rounded-full appearance-none cursor-default outline-none",
        // Firefox: active progress and track via moz pseudo-elements
        "[&::-moz-range-track]:bg-input [&::-moz-range-track]:rounded-full [&::-moz-range-track]:h-2",
        "[&::-moz-range-progress]:bg-primary [&::-moz-range-progress]:rounded-full",
        // WebKit thumb — base: circle, bg-background, border-primary
        "[&::-webkit-slider-thumb]:appearance-none",
        "[&::-webkit-slider-thumb]:h-4 [&::-webkit-slider-thumb]:w-4",
        "[&::-webkit-slider-thumb]:rounded-full",
        "[&::-webkit-slider-thumb]:bg-white",
        "[&::-webkit-slider-thumb]:border-2 [&::-webkit-slider-thumb]:border-primary",
        "[&::-webkit-slider-thumb]:shadow-sm",
        "[&::-webkit-slider-thumb]:cursor-pointer",
        "[&::-webkit-slider-thumb]:transition-[color,box-shadow,background-color]",
        // Hover/focus ring — only when not disabled (uses native :disabled selector)
        "[&:not(:disabled)::-webkit-slider-thumb:hover]:ring-2 [&:not(:disabled)::-webkit-slider-thumb:hover]:ring-ring/50",
        "[&:not(:disabled):focus-visible::-webkit-slider-thumb]:ring-2 [&:not(:disabled):focus-visible::-webkit-slider-thumb]:ring-ring/50",
        // Firefox thumb — mirror WebKit shape
        "[&::-moz-range-thumb]:h-4 [&::-moz-range-thumb]:w-4",
        "[&::-moz-range-thumb]:rounded-full",
        "[&::-moz-range-thumb]:bg-white",
        "[&::-moz-range-thumb]:border-2 [&::-moz-range-thumb]:border-primary",
        "[&::-moz-range-thumb]:shadow-sm",
        "[&::-moz-range-thumb]:cursor-pointer",
        "disabled:opacity-50 disabled:cursor-not-allowed",
        _styleVariant.GetClasses("Slider.Root"),
        Class
    );
}
