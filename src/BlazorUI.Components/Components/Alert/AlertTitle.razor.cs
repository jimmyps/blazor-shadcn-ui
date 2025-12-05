using BlazorUI.Components.Utilities;
using Microsoft.AspNetCore.Components;

namespace BlazorUI.Components.Alert;

/// <summary>
/// A title component for the Alert that displays a prominent heading.
/// </summary>
/// <remarks>
/// <para>
/// AlertTitle should be used within an Alert component to provide
/// a clear, bold heading for the alert message.
/// </para>
/// </remarks>
/// <example>
/// <code>
/// &lt;Alert&gt;
///     &lt;AlertTitle&gt;Heads up!&lt;/AlertTitle&gt;
///     &lt;AlertDescription&gt;Your message here&lt;/AlertDescription&gt;
/// &lt;/Alert&gt;
/// </code>
/// </example>
public partial class AlertTitle : ComponentBase
{
    /// <summary>
    /// Gets or sets additional CSS classes to apply to the title.
    /// </summary>
    [Parameter]
    public string? Class { get; set; }

    /// <summary>
    /// Gets or sets the content to be rendered inside the title.
    /// </summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    /// <summary>
    /// Gets the computed CSS classes for the title element.
    /// </summary>
    private string CssClass => ClassNames.cn(
        "mb-1 font-medium leading-none tracking-tight",
        Class
    );
}
