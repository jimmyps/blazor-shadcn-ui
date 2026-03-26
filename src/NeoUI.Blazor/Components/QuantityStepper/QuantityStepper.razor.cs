using Microsoft.AspNetCore.Components;

namespace NeoUI.Blazor;

/// <summary>
/// A quantity stepper with circular +/- buttons for incrementing and decrementing a numeric value.
/// </summary>
/// <remarks>
/// <para>
/// QuantityStepper is the standard quantity control in mobile e-commerce UIs — distinct from
/// <see cref="NumericInput"/> (which is a text field with arrows) and <see cref="Slider"/>
/// (which is a drag control). The stepper renders compact circular icon buttons flanking the value.
/// </para>
/// <para>
/// Key mobile/e-commerce feature: when <see cref="DestructiveAtMin"/> is true and the value
/// reaches <see cref="Min"/>, the decrement button switches from a minus icon to a trash icon
/// and fires <see cref="OnDestructiveClick"/> — enabling "remove item from cart" behaviour
/// without a separate delete button.
/// </para>
/// <para>
/// Compatible with .NET MAUI Blazor Hybrid apps.
/// </para>
/// </remarks>
/// <example>
/// <code>
/// &lt;!-- Basic product detail stepper --&gt;
/// &lt;QuantityStepper @bind-Value="qty" Min="1" Max="99" /&gt;
///
/// &lt;!-- Cart item with trash-on-zero --&gt;
/// &lt;QuantityStepper @bind-Value="item.Quantity"
///                  Min="1"
///                  DestructiveAtMin="true"
///                  OnDestructiveClick="() => RemoveItem(item)"
///                  Size="QuantityStepperSize.Sm" /&gt;
/// </code>
/// </example>
public partial class QuantityStepper : ComponentBase
{
    /// <summary>Gets or sets the current value.</summary>
    [Parameter]
    public int Value { get; set; } = 1;

    /// <summary>Fires when the value changes.</summary>
    [Parameter]
    public EventCallback<int> ValueChanged { get; set; }

    /// <summary>Gets or sets the minimum allowed value. Defaults to 1.</summary>
    [Parameter]
    public int Min { get; set; } = 1;

    /// <summary>Gets or sets the maximum allowed value. Null means no upper limit.</summary>
    [Parameter]
    public int? Max { get; set; }

    /// <summary>
    /// Gets or sets whether the decrement button becomes a destructive trash icon at <see cref="Min"/>.
    /// Fires <see cref="OnDestructiveClick"/> instead of decrementing when activated.
    /// Defaults to false.
    /// </summary>
    [Parameter]
    public bool DestructiveAtMin { get; set; }

    /// <summary>
    /// Fires when the user presses the decrement button while at <see cref="Min"/>
    /// and <see cref="DestructiveAtMin"/> is true.
    /// </summary>
    [Parameter]
    public EventCallback OnDestructiveClick { get; set; }

    /// <summary>Gets or sets the visual size. Defaults to <see cref="QuantityStepperSize.Default"/>.</summary>
    [Parameter]
    public QuantityStepperSize Size { get; set; } = QuantityStepperSize.Default;

    /// <summary>Gets or sets whether the entire stepper is disabled.</summary>
    [Parameter]
    public bool Disabled { get; set; }

    /// <summary>Gets or sets the accessible label for the group.</summary>
    [Parameter]
    public string AriaLabel { get; set; } = "Quantity";

    /// <summary>Gets or sets the accessible label for the decrement button.</summary>
    [Parameter]
    public string DecrementLabel { get; set; } = "Decrease quantity";

    /// <summary>Gets or sets the accessible label for the increment button.</summary>
    [Parameter]
    public string IncrementLabel { get; set; } = "Increase quantity";

    /// <summary>Gets or sets the accessible label for the trash/destructive button.</summary>
    [Parameter]
    public string DestructiveLabel { get; set; } = "Remove item";

    /// <summary>Gets or sets additional CSS classes on the wrapper.</summary>
    [Parameter]
    public string? Class { get; set; }

    private bool IsAtMin => Value <= Min;

    private (string button, string value, int iconSize) SizeTokens => Size switch
    {
        QuantityStepperSize.Sm => ("size-7", "w-5 text-sm", 14),
        QuantityStepperSize.Lg => ("size-11", "w-8 text-lg", 20),
        _ => ("size-8", "w-6 text-base", 16),
    };

    private int IconSize => SizeTokens.iconSize;

    private string CssClass => ClassNames.cn(
        "inline-flex items-center gap-2",
        Disabled ? "opacity-50 pointer-events-none" : string.Empty,
        Class
    );

    private string DecrementCssClass => ClassNames.cn(
        SizeTokens.button,
        "rounded-full border border-border flex items-center justify-center",
        "transition-colors focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-ring",
        IsAtMin && DestructiveAtMin
            ? "border-destructive text-destructive hover:bg-destructive/10"
            : "text-foreground hover:bg-accent disabled:opacity-40 disabled:cursor-not-allowed"
    );

    private string IncrementCssClass => ClassNames.cn(
        SizeTokens.button,
        "rounded-full bg-primary text-primary-foreground flex items-center justify-center",
        "transition-colors hover:bg-primary/90",
        "focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-ring",
        "disabled:opacity-40 disabled:cursor-not-allowed"
    );

    private string ValueCssClass => ClassNames.cn(
        SizeTokens.value,
        "text-center font-semibold text-foreground select-none tabular-nums"
    );

    private async Task HandleDecrement()
    {
        if (IsAtMin && DestructiveAtMin)
        {
            await OnDestructiveClick.InvokeAsync();
            return;
        }

        if (Value > Min)
        {
            await ValueChanged.InvokeAsync(Value - 1);
        }
    }

    private async Task HandleIncrement()
    {
        if (!Max.HasValue || Value < Max.Value)
        {
            await ValueChanged.InvokeAsync(Value + 1);
        }
    }
}
