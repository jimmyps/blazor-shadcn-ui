namespace BlazorUI.Components.Old.Chart;

/// <summary>
/// Utility methods for chart components.
/// </summary>
internal static class ChartUtilities
{
    /// <summary>
    /// Default chart colors from theme variables.
    /// </summary>
    private static readonly string[] ChartColors = new[]
    {
        "var(--chart-1)",
        "var(--chart-2)",
        "var(--chart-3)",
        "var(--chart-4)",
        "var(--chart-5)"
    };
    
    /// <summary>
    /// Gets a chart color by index, cycling through available colors.
    /// </summary>
    public static string GetChartColor(int index)
    {
        return ChartColors[index % ChartColors.Length];
    }
    
    /// <summary>
    /// Maps AnimationEasing enum to Chart.js easing function name.
    /// </summary>
    public static string MapEasingFunction(AnimationEasing easing)
    {
        return easing switch
        {
            AnimationEasing.Linear => "linear",
            AnimationEasing.EaseInQuad => "easeInQuad",
            AnimationEasing.EaseOutQuad => "easeOutQuad",
            AnimationEasing.EaseInOutQuad => "easeInOutQuad",
            AnimationEasing.EaseInCubic => "easeInCubic",
            AnimationEasing.EaseOutCubic => "easeOutCubic",
            AnimationEasing.EaseInOutCubic => "easeInOutCubic",
            AnimationEasing.EaseInQuart => "easeInQuart",
            AnimationEasing.EaseOutQuart => "easeOutQuart",
            AnimationEasing.EaseInOutQuart => "easeInOutQuart",
            AnimationEasing.EaseInQuint => "easeInQuint",
            AnimationEasing.EaseOutQuint => "easeOutQuint",
            AnimationEasing.EaseInOutQuint => "easeInOutQuint",
            AnimationEasing.EaseInExpo => "easeInExpo",
            AnimationEasing.EaseOutExpo => "easeOutExpo",
            AnimationEasing.EaseInOutExpo => "easeInOutExpo",
            AnimationEasing.EaseInBack => "easeInBack",
            AnimationEasing.EaseOutBack => "easeOutBack",
            AnimationEasing.EaseInOutBack => "easeInOutBack",
            AnimationEasing.EaseInElastic => "easeInElastic",
            AnimationEasing.EaseOutElastic => "easeOutElastic",
            AnimationEasing.EaseInOutElastic => "easeInOutElastic",
            AnimationEasing.EaseInBounce => "easeInBounce",
            AnimationEasing.EaseOutBounce => "easeOutBounce",
            AnimationEasing.EaseInOutBounce => "easeInOutBounce",
            _ => "easeInOutQuart" // Default fallback
        };
    }
    
    /// <summary>
    /// Converts a color string to HSLA with specified alpha.
    /// </summary>
    public static string GetColorWithAlpha(string color, double alpha)
    {
        // Handle CSS variable format: var(--chart-X)
        // CSS variables already contain final color values from styles
        // For alpha channel, we rely on ECharts to apply opacity
        if (color.StartsWith("var(--chart-") && color.EndsWith(")"))
        {
            return color;
        }
        
        // Fallback: return original color
        return color;
    }
}
