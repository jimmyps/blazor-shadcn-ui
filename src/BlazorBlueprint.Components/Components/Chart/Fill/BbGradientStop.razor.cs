using System.Globalization;
using Microsoft.AspNetCore.Components;

namespace BlazorBlueprint.Components;

/// <summary>
/// Defines a color stop within a <see cref="BbLinearGradient"/>.
/// </summary>
/// <remarks>
/// <para>
/// Each GradientStop specifies a color at a particular offset position within the gradient.
/// Stops are automatically sorted by offset when the gradient is built.
/// </para>
/// </remarks>
/// <example>
/// <code>
/// &lt;LinearGradient&gt;
///     &lt;GradientStop Offset="0" Color="var(--chart-1)" Opacity="0.8" /&gt;
///     &lt;GradientStop Offset="1" Color="var(--chart-1)" Opacity="0.1" /&gt;
/// &lt;/LinearGradient&gt;
/// </code>
/// </example>
public partial class BbGradientStop : ComponentBase
{
    [CascadingParameter]
    private BbLinearGradient? ParentGradient { get; set; }

    /// <summary>
    /// Gets or sets the position of this stop within the gradient (0 to 1).
    /// </summary>
    /// <remarks>
    /// 0 represents the start of the gradient, 1 represents the end.
    /// </remarks>
    [Parameter]
    public double Offset { get; set; }

    /// <summary>
    /// Gets or sets the color at this stop position.
    /// </summary>
    /// <remarks>
    /// Accepts any valid CSS color value or CSS variable reference.
    /// </remarks>
    [Parameter]
    public string? Color { get; set; }

    /// <summary>
    /// Gets or sets the opacity at this stop position (0 to 1).
    /// </summary>
    /// <remarks>
    /// When set, the color is wrapped in a color-mix() expression.
    /// When null, the color is used as-is.
    /// </remarks>
    [Parameter]
    public double? Opacity { get; set; }

    /// <summary>
    /// Gets the resolved color string, incorporating opacity if specified.
    /// </summary>
    internal string ResolvedColor
    {
        get
        {
            var color = Color ?? "transparent";

            if (Opacity.HasValue && Opacity.Value < 1.0)
            {
                return $"color-mix(in srgb, {color} {(Opacity.Value * 100).ToString("F0", CultureInfo.InvariantCulture)}%, transparent)";
            }

            return color;
        }
    }

    protected override void OnInitialized() =>
        ParentGradient?.RegisterStop(this);
}
