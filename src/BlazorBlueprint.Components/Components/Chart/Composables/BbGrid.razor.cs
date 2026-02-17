using Microsoft.AspNetCore.Components;

namespace BlazorBlueprint.Components;

/// <summary>
/// Configures the grid (plot area) for a chart. Must be placed inside a chart component.
/// </summary>
/// <remarks>
/// <para>
/// Grid controls the positioning and sizing of the chart plot area. The default values
/// provide compact padding with <see cref="ContainLabel"/> enabled to prevent axis labels
/// from being clipped.
/// </para>
/// </remarks>
/// <example>
/// <code>
/// &lt;BarChart Data="@data"&gt;
///     &lt;Grid Left="5%" Right="5%" Top="15%" Bottom="5%" /&gt;
///     &lt;XAxis DataKey="month" /&gt;
///     &lt;YAxis /&gt;
/// &lt;/BarChart&gt;
/// </code>
/// </example>
public partial class BbGrid : ComponentBase, IChartComponent, IDisposable
{
    [CascadingParameter]
    private BbChartBase? ParentChart { get; set; }

    /// <summary>
    /// Gets or sets the distance between the grid and the left side of the container.
    /// </summary>
    /// <remarks>
    /// Accepts percentage strings (e.g., "3%") or pixel values (e.g., "40").
    /// </remarks>
    [Parameter]
    public string Left { get; set; } = "12";

    /// <summary>
    /// Gets or sets the distance between the grid and the right side of the container.
    /// </summary>
    /// <remarks>
    /// Accepts percentage strings (e.g., "4%") or pixel values (e.g., "40").
    /// </remarks>
    [Parameter]
    public string Right { get; set; } = "12";

    /// <summary>
    /// Gets or sets the distance between the grid and the top of the container.
    /// </summary>
    /// <remarks>
    /// Accepts percentage strings (e.g., "10%") or pixel values (e.g., "40").
    /// </remarks>
    [Parameter]
    public string Top { get; set; } = "0";

    /// <summary>
    /// Gets or sets the distance between the grid and the bottom of the container.
    /// </summary>
    /// <remarks>
    /// Accepts percentage strings (e.g., "3%") or pixel values (e.g., "40").
    /// </remarks>
    [Parameter]
    public string Bottom { get; set; } = "0";

    /// <summary>
    /// Gets or sets whether the grid area contains axis labels.
    /// </summary>
    /// <remarks>
    /// When <c>true</c>, the grid size is adjusted to include axis tick labels,
    /// preventing them from being clipped or overlapping other elements.
    /// </remarks>
    [Parameter]
    public bool ContainLabel { get; set; } = true;

    protected override void OnInitialized() =>
        ParentChart?.RegisterComponent(this);

    void IChartComponent.ApplyTo(EChartsOption option)
    {
        option.Grid = new EChartsGridOption
        {
            Left = Left,
            Right = Right,
            Top = Top,
            Bottom = Bottom,
            ContainLabel = ContainLabel
        };
    }

    public void Dispose()
    {
        ParentChart?.UnregisterComponent(this);
        GC.SuppressFinalize(this);
    }
}
