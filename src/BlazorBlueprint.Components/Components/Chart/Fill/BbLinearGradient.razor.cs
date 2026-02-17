using Microsoft.AspNetCore.Components;

namespace BlazorBlueprint.Components;

/// <summary>
/// Defines a linear gradient fill for chart series.
/// </summary>
/// <remarks>
/// <para>
/// LinearGradient creates an ECharts linear gradient object from child <see cref="BbGradientStop"/>
/// components. The gradient direction is controlled by the <see cref="Direction"/> parameter.
/// Must be placed inside a <see cref="BbFill"/> component.
/// </para>
/// </remarks>
/// <example>
/// <code>
/// &lt;Fill&gt;
///     &lt;LinearGradient Direction="GradientDirection.Vertical"&gt;
///         &lt;GradientStop Offset="0" Color="var(--chart-1)" Opacity="0.8" /&gt;
///         &lt;GradientStop Offset="1" Color="var(--chart-1)" Opacity="0.1" /&gt;
///     &lt;/LinearGradient&gt;
/// &lt;/Fill&gt;
/// </code>
/// </example>
public partial class BbLinearGradient : ComponentBase, IFillComponent
{
    [CascadingParameter]
    private BbFill? ParentFill { get; set; }

    /// <summary>
    /// Gets or sets the gradient direction.
    /// </summary>
    /// <remarks>
    /// <see cref="GradientDirection.Vertical"/> creates a top-to-bottom gradient.
    /// <see cref="GradientDirection.Horizontal"/> creates a left-to-right gradient.
    /// Default is <see cref="GradientDirection.Vertical"/>.
    /// </remarks>
    [Parameter]
    public GradientDirection Direction { get; set; } = GradientDirection.Vertical;

    /// <summary>
    /// Gets or sets the child content containing <see cref="BbGradientStop"/> components.
    /// </summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    private readonly List<BbGradientStop> stops = [];

    protected override void OnInitialized() =>
        ParentFill?.RegisterFill(this);

    /// <summary>
    /// Called by child <see cref="BbGradientStop"/> components to register themselves.
    /// </summary>
    /// <param name="stop">The gradient stop to add.</param>
    internal void RegisterStop(BbGradientStop stop) =>
        stops.Add(stop);

    /// <inheritdoc />
    object IFillComponent.BuildFill()
    {
        var gradient = new EChartsLinearGradient();

        if (Direction == GradientDirection.Vertical)
        {
            gradient.X = 0;
            gradient.Y = 0;
            gradient.X2 = 0;
            gradient.Y2 = 1;
        }
        else
        {
            gradient.X = 0;
            gradient.Y = 0;
            gradient.X2 = 1;
            gradient.Y2 = 0;
        }

        var sortedStops = stops.OrderBy(s => s.Offset).ToList();
        foreach (var stop in sortedStops)
        {
            var colorStop = new EChartsColorStop
            {
                Offset = stop.Offset,
                Color = stop.ResolvedColor
            };
            gradient.ColorStops.Add(colorStop);
        }

        return gradient;
    }
}
