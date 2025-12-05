using BlazorUI.Components.Utilities;
using Microsoft.AspNetCore.Components;

namespace BlazorUI.Components.Alert;

/// <summary>
/// A description component for the Alert that displays additional details.
/// </summary>
/// <remarks>
/// <para>
/// AlertDescription should be used within an Alert component to provide
/// additional context or details for the alert message.
/// </para>
/// </remarks>
/// <example>
/// <code>
/// &lt;Alert&gt;
///     &lt;AlertTitle&gt;Heads up!&lt;/AlertTitle&gt;
///     &lt;AlertDescription&gt;You can add components and dependencies to your app using the cli.&lt;/AlertDescription&gt;
/// &lt;/Alert&gt;
/// </code>
/// </example>
public partial class AlertDescription : ComponentBase
{
    /// <summary>
    /// Gets or sets additional CSS classes to apply to the description.
    /// </summary>
    [Parameter]
    public string? Class { get; set; }

    /// <summary>
    /// Gets or sets the content to be rendered inside the description.
    /// </summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    /// <summary>
    /// Gets the computed CSS classes for the description element.
    /// </summary>
    private string CssClass => ClassNames.cn(
        "text-sm [&_p]:leading-relaxed",
        Class
    );
}
