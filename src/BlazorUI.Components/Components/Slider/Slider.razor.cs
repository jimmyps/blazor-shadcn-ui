using BlazorUI.Components.Utilities;
using Microsoft.AspNetCore.Components;

namespace BlazorUI.Components.Slider;

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

    private string CssClass => ClassNames.cn(
        "w-full h-2 bg-secondary rounded-full appearance-none cursor-pointer",
        "focus:outline-none focus-visible:ring-2 focus-visible:ring-ring focus-visible:ring-offset-2",
        "[&::-webkit-slider-thumb]:appearance-none [&::-webkit-slider-thumb]:h-5 [&::-webkit-slider-thumb]:w-5",
        "[&::-webkit-slider-thumb]:rounded-full [&::-webkit-slider-thumb]:bg-primary",
        "[&::-webkit-slider-thumb]:border-2 [&::-webkit-slider-thumb]:border-primary",
        "[&::-webkit-slider-thumb]:transition-colors [&::-webkit-slider-thumb]:cursor-pointer",
        "[&::-moz-range-thumb]:h-5 [&::-moz-range-thumb]:w-5 [&::-moz-range-thumb]:rounded-full",
        "[&::-moz-range-thumb]:bg-primary [&::-moz-range-thumb]:border-2 [&::-moz-range-thumb]:border-primary",
        "[&::-moz-range-thumb]:transition-colors [&::-moz-range-thumb]:cursor-pointer",
        Disabled ? "opacity-50 cursor-not-allowed" : ""
    );
}
