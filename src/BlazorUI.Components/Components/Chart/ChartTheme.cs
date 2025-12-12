namespace BlazorUI.Components.Chart;

/// <summary>
/// Represents theme configuration for charts.
/// </summary>
public class ChartTheme
{
    public string Background { get; set; } = "";
    public string Foreground { get; set; } = "";
    public string Border { get; set; } = "";
    public string Muted { get; set; } = "";
    public string MutedForeground { get; set; } = "";
    public string FontFamily { get; set; } = "";
    public string[] ChartColors { get; set; } = Array.Empty<string>();
}
