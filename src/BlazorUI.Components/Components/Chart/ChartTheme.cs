namespace BlazorUI.Components.Chart;

/// <summary>
/// Represents theme configuration for charts.
/// </summary>
public class ChartTheme
{
    /// <summary>
    /// Gets or sets the background color for the chart.
    /// </summary>
    public string Background { get; set; } = "";
    
    /// <summary>
    /// Gets or sets the foreground (text) color for the chart.
    /// </summary>
    public string Foreground { get; set; } = "";
    
    /// <summary>
    /// Gets or sets the border color for chart elements.
    /// </summary>
    public string Border { get; set; } = "";
    
    /// <summary>
    /// Gets or sets the muted (subtle) color for chart elements.
    /// </summary>
    public string Muted { get; set; } = "";
    
    /// <summary>
    /// Gets or sets the muted foreground (secondary text) color for the chart.
    /// </summary>
    public string MutedForeground { get; set; } = "";
    
    /// <summary>
    /// Gets or sets the font family for chart text.
    /// </summary>
    public string FontFamily { get; set; } = "";
    
    /// <summary>
    /// Gets or sets the color palette for chart data series.
    /// </summary>
    public string[] ChartColors { get; set; } = Array.Empty<string>();
}
