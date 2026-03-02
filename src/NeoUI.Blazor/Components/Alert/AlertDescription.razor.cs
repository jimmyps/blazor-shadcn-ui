using BlazorUI.Components.Utilities;
using Microsoft.AspNetCore.Components;

namespace BlazorUI.Components.Alert;

/// <summary>
/// The description component for an Alert.
/// </summary>
/// <remarks>
/// <para>
/// AlertDescription provides detailed information or additional context
/// for an alert message.
/// </para>
/// </remarks>
/// <example>
/// <code>
/// &lt;Alert&gt;
///     &lt;AlertTitle&gt;Warning&lt;/AlertTitle&gt;
///     &lt;AlertDescription&gt;
///         Please review your settings before continuing.
///     &lt;/AlertDescription&gt;
/// &lt;/Alert&gt;
/// </code>
/// </example>
public partial class AlertDescription : ComponentBase
{
    /// <summary>
    /// Gets or sets additional CSS classes to apply to the alert description.
    /// </summary>
    [Parameter]
    public string? Class { get; set; }

    /// <summary>
    /// Gets or sets the content to be rendered inside the alert description.
    /// </summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    /// <summary>
    /// Gets the computed CSS classes for the alert description element.
    /// </summary>
    private string CssClass => ClassNames.cn(
        "text-sm [&_p]:leading-relaxed",
        Class
    );
}
