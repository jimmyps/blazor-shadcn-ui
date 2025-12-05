using BlazorUI.Components.Utilities;
using Microsoft.AspNetCore.Components;

namespace BlazorUI.Components.Progress;

/// <summary>
/// A progress bar component that shows completion status.
/// </summary>
/// <remarks>
/// <para>
/// The Progress component provides visual feedback about the progress of a task.
/// It follows the shadcn/ui design system and accessibility best practices.
/// </para>
/// <para>
/// Features:
/// - Configurable value and max
/// - Smooth transition animations
/// - Semantic HTML with role="progressbar" and ARIA attributes
/// - Dark mode compatible via CSS variables
/// </para>
/// </remarks>
/// <example>
/// <code>
/// &lt;Progress Value="33" /&gt;
/// &lt;Progress Value="66" Max="100" /&gt;
/// </code>
/// </example>
public partial class Progress : ComponentBase
{
    /// <summary>
    /// Gets or sets the current progress value.
    /// </summary>
    /// <remarks>
    /// Value should be between 0 and Max (default 100).
    /// </remarks>
    [Parameter]
    public double Value { get; set; } = 0;

    /// <summary>
    /// Gets or sets the maximum value for the progress.
    /// </summary>
    [Parameter]
    public double Max { get; set; } = 100;

    /// <summary>
    /// Gets or sets additional CSS classes to apply to the progress bar.
    /// </summary>
    [Parameter]
    public string? Class { get; set; }

    /// <summary>
    /// Gets the computed CSS classes for the progress container.
    /// </summary>
    private string CssClass => ClassNames.cn(
        "relative h-4 w-full overflow-hidden rounded-full bg-secondary",
        Class
    );

    /// <summary>
    /// Gets the computed CSS classes for the progress indicator.
    /// </summary>
    private string IndicatorCssClass => "h-full w-full flex-1 bg-primary transition-all";

    /// <summary>
    /// Gets the inline style for the indicator transform.
    /// </summary>
    private string IndicatorStyle
    {
        get
        {
            var percentage = Max > 0 ? (Value / Max * 100) : 0;
            percentage = Math.Max(0, Math.Min(100, percentage));
            return $"transform: translateX(-{100 - percentage}%)";
        }
    }
}
