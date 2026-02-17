using Microsoft.AspNetCore.Components;

namespace BlazorBlueprint.Components;

/// <summary>
/// Configures a center label for Pie (donut) and RadialBar series.
/// </summary>
/// <remarks>
/// <para>
/// Must be placed as a child of a <see cref="BbPie"/> or <see cref="BbRadialBar"/> component
/// with a non-zero inner radius. Registers itself with the parent series to configure
/// the center-positioned label in the ECharts option.
/// </para>
/// </remarks>
/// <example>
/// <code>
/// &lt;Pie DataKey="visitors" NameKey="browser" InnerRadius="60"&gt;
///     &lt;CenterLabel Title="Browsers" Value="5" /&gt;
/// &lt;/Pie&gt;
/// </code>
/// </example>
public partial class BbCenterLabel : ComponentBase
{
    [CascadingParameter]
    private SeriesBase? ParentSeries { get; set; }

    /// <summary>
    /// Gets or sets the title text displayed above the value in the center.
    /// </summary>
    [Parameter]
    public string? Title { get; set; }

    /// <summary>
    /// Gets or sets the value text displayed below the title in the center.
    /// </summary>
    [Parameter]
    public string? Value { get; set; }

    /// <summary>
    /// Gets or sets the label text displayed in the center.
    /// When set, overrides <see cref="Title"/> and <see cref="Value"/>.
    /// </summary>
    [Parameter]
    public string? Text { get; set; }

    /// <summary>
    /// Gets or sets the font size of the center label value in pixels.
    /// </summary>
    [Parameter]
    public int FontSize { get; set; } = 24;

    /// <summary>
    /// Gets or sets the font weight of the center label value.
    /// </summary>
    [Parameter]
    public string FontWeight { get; set; } = "bold";

    /// <summary>
    /// Gets the computed display text for the center label.
    /// </summary>
    internal string DisplayText
    {
        get
        {
            if (!string.IsNullOrEmpty(Text))
            {
                return Text;
            }

            if (!string.IsNullOrEmpty(Title) && !string.IsNullOrEmpty(Value))
            {
                return $"{Title}\n{Value}";
            }

            return Title ?? Value ?? "";
        }
    }

    protected override void OnInitialized()
    {
        if (ParentSeries is BbPie pie)
        {
            pie.SetCenterLabel(this);
        }
        else if (ParentSeries is BbRadialBar radialBar)
        {
            radialBar.SetCenterLabel(this);
        }
    }
}
