namespace BlazorUI.Components.Chart;

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
        "hsl(var(--chart-1))",
        "hsl(var(--chart-2))",
        "hsl(var(--chart-3))",
        "hsl(var(--chart-4))",
        "hsl(var(--chart-5))"
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
        // Handle CSS variable format: hsl(var(--chart-X))
        if (color.StartsWith("hsl(var(--chart-") && color.EndsWith("))"))
        {
            var startIndex = "hsl(var(--chart-".Length;
            var endIndex = color.IndexOf(')', startIndex);
            if (endIndex > startIndex)
            {
                var chartNum = color.Substring(startIndex, endIndex - startIndex);
                return $"hsla(var(--chart-{chartNum}), {alpha})";
            }
        }
        
        // Fallback: return original color
        return color;
    }
}
