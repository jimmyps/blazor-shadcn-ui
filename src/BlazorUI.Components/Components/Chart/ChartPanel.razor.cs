using BlazorUI.Components.Utilities;
using Microsoft.AspNetCore.Components;

namespace BlazorUI.Components.Chart;

/// <summary>
/// Container component for charts with header, content, and footer sections.
/// Provides consistent styling and structure for chart displays.
/// </summary>
public class ChartPanelBase : ComponentBase
{
    /// <summary>
    /// Gets or sets the header content of the chart panel.
    /// </summary>
    [Parameter]
    public RenderFragment? ChartPanelHeader { get; set; }
    
    /// <summary>
    /// Gets or sets the main content of the chart panel.
    /// </summary>
    [Parameter]
    public RenderFragment? ChartPanelContent { get; set; }
    
    /// <summary>
    /// Gets or sets the footer content of the chart panel.
    /// </summary>
    [Parameter]
    public RenderFragment? ChartPanelFooter { get; set; }
    
    /// <summary>
    /// Gets or sets additional CSS classes to apply to the component.
    /// </summary>
    [Parameter]
    public string? Class { get; set; }
    
    /// <summary>
    /// Gets the computed CSS classes for the component.
    /// </summary>
    protected string CssClass => ClassNames.cn(
        "rounded-lg border bg-card text-card-foreground shadow-sm",
        Class
    );
}
