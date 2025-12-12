using BlazorUI.Components.Utilities;
using Microsoft.AspNetCore.Components;

namespace BlazorUI.Components.Toggle;

/// <summary>
/// A toggle button component with pressed/unpressed states.
/// </summary>
/// <remarks>
/// <para>
/// The Toggle component provides a button that can be pressed and unpressed.
/// It follows the shadcn/ui design system with accessible ARIA attributes.
/// </para>
/// <para>
/// Features:
/// - Two-way binding with @bind-Pressed
/// - Multiple variants (Default, Outline)
/// - Multiple sizes (Default, Small, Large)
/// - Disabled state support
/// - Accessible with aria-pressed
/// - Dark mode compatible via CSS variables
/// </para>
/// </remarks>
/// <example>
/// <code>
/// &lt;Toggle @bind-Pressed="IsBold"&gt;
///     &lt;!-- Icon or text --&gt;
///     Bold
/// &lt;/Toggle&gt;
/// </code>
/// </example>
public partial class Toggle : ComponentBase
{
    /// <summary>
    /// Gets or sets whether the toggle is pressed.
    /// </summary>
    [Parameter]
    public bool Pressed { get; set; }

    /// <summary>
    /// Gets or sets the callback that is invoked when the pressed state changes.
    /// </summary>
    [Parameter]
    public EventCallback<bool> PressedChanged { get; set; }

    /// <summary>
    /// Gets or sets the visual variant of the toggle.
    /// </summary>
    [Parameter]
    public ToggleVariant Variant { get; set; } = ToggleVariant.Default;

    /// <summary>
    /// Gets or sets the size of the toggle.
    /// </summary>
    [Parameter]
    public ToggleSize Size { get; set; } = ToggleSize.Default;

    /// <summary>
    /// Gets or sets whether the toggle is disabled.
    /// </summary>
    [Parameter]
    public bool Disabled { get; set; }

    /// <summary>
    /// Gets or sets additional CSS classes to apply to the toggle.
    /// </summary>
    [Parameter]
    public string? Class { get; set; }

    /// <summary>
    /// Gets or sets the content to be rendered inside the toggle.
    /// </summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    private async Task OnClickAsync()
    {
        if (!Disabled)
        {
            Pressed = !Pressed;
            await PressedChanged.InvokeAsync(Pressed);
        }
    }

    private string CssClass => ClassNames.cn(
        // Base styles
        "inline-flex items-center justify-center rounded-md text-sm font-medium ring-offset-background transition-colors",
        "hover:bg-muted hover:text-muted-foreground",
        "focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-ring focus-visible:ring-offset-2",
        "disabled:pointer-events-none disabled:opacity-50",
        // Variant styles
        Variant switch
        {
            ToggleVariant.Default => "bg-transparent",
            ToggleVariant.Outline => "border border-input bg-transparent hover:bg-accent hover:text-accent-foreground",
            _ => "bg-transparent"
        },
        // Pressed state
        Pressed ? "bg-accent text-accent-foreground" : "",
        // Size styles
        Size switch
        {
            ToggleSize.Default => "h-10 px-3",
            ToggleSize.Small => "h-9 px-2.5",
            ToggleSize.Large => "h-11 px-5",
            _ => "h-10 px-3"
        },
        Class
    );
}
