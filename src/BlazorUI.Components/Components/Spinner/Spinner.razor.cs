using BlazorUI.Components.Utilities;
using Microsoft.AspNetCore.Components;

namespace BlazorUI.Components.Spinner;

/// <summary>
/// A spinner/loading indicator component.
/// </summary>
/// <remarks>
/// <para>
/// The Spinner component provides a visual loading indicator for async operations.
/// It follows the shadcn/ui design system.
/// </para>
/// <para>
/// Features:
/// - Multiple size options
/// - Animated rotation
/// - Accessible with ARIA attributes
/// - Dark mode compatible via CSS variables
/// </para>
/// </remarks>
/// <example>
/// <code>
/// &lt;Spinner /&gt;
///
/// &lt;Spinner Size="SpinnerSize.Large" /&gt;
/// </code>
/// </example>
public partial class Spinner : ComponentBase
{
    /// <summary>
    /// Gets or sets the size of the spinner.
    /// </summary>
    [Parameter]
    public SpinnerSize Size { get; set; } = SpinnerSize.Medium;

    /// <summary>
    /// Gets or sets additional CSS classes to apply to the spinner.
    /// </summary>
    [Parameter]
    public string? Class { get; set; }

    /// <summary>
    /// Gets the computed CSS classes for the spinner element.
    /// </summary>
    private string CssClass => ClassNames.cn(
        "animate-spin",
        Size switch
        {
            SpinnerSize.Small => "h-4 w-4",
            SpinnerSize.Medium => "h-8 w-8",
            SpinnerSize.Large => "h-12 w-12",
            _ => "h-8 w-8"
        },
        Class
    );
}
