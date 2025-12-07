using Microsoft.AspNetCore.Components;

namespace BlazorUI.Components.HeightAnimation;

/// <summary>
/// Configuration for the HeightAnimation component.
/// </summary>
public class HeightAnimationConfig
{
    /// <summary>
    /// CSS selector for the content element to observe (e.g., '[role="listbox"]', '.content').
    /// This is the element whose height changes will be animated.
    /// </summary>
    public required string ContentSelector { get; set; }

    /// <summary>
    /// Optional CSS selector for a fixed-height header/input element (e.g., '.header', '[role="combobox"]').
    /// If provided, this element's height will be added to the total container height.
    /// </summary>
    public string? InputSelector { get; set; }

    /// <summary>
    /// Optional maximum height in pixels. If null, the initial content height is used as the maximum.
    /// </summary>
    public int? MaxHeight { get; set; }

    /// <summary>
    /// Whether to include the input/header height in the total container height.
    /// Default is true.
    /// </summary>
    public bool IncludeInputHeight { get; set; } = true;
}
