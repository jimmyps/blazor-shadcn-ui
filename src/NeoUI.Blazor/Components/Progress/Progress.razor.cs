using BlazorUI.Components.Utilities;
using Microsoft.AspNetCore.Components;

namespace BlazorUI.Components.Progress;

/// <summary>
/// A progress bar component for displaying completion status.
/// </summary>
/// <remarks>
/// <para>
/// The Progress component provides a visual indication of progress or completion.
/// It follows the shadcn/ui design system with semantic HTML and ARIA attributes.
/// </para>
/// <para>
/// Features:
/// - Configurable value and maximum
/// - Accessible with ARIA progressbar role
/// - Smooth transitions
/// - Dark mode compatible via CSS variables
/// </para>
/// </remarks>
/// <example>
/// <code>
/// &lt;Progress Value="60" Max="100" /&gt;
///
/// &lt;Progress Value="@uploadProgress" /&gt;
/// </code>
/// </example>
public partial class Progress : ComponentBase
{
    /// <summary>
    /// Gets or sets the current progress value.
    /// </summary>
    [Parameter]
    public double Value { get; set; }

    /// <summary>
    /// Gets or sets the maximum value for the progress bar.
    /// </summary>
    /// <remarks>
    /// Default value is 100.
    /// </remarks>
    [Parameter]
    public double Max { get; set; } = 100;

    /// <summary>
    /// Gets or sets additional CSS classes to apply to the progress bar.
    /// </summary>
    [Parameter]
    public string? Class { get; set; }

    /// <summary>
    /// Gets the computed CSS classes for the progress element.
    /// </summary>
    private string CssClass => ClassNames.cn(
        "relative h-4 w-full overflow-hidden rounded-full bg-secondary",
        Class
    );

    /// <summary>
    /// Gets the percentage value for display.
    /// </summary>
    private double PercentageValue => Math.Min(100, Math.Max(0, (Value / Max) * 100));
}
