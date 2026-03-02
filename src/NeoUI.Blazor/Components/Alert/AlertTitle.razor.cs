using BlazorUI.Components.Utilities;
using Microsoft.AspNetCore.Components;

namespace BlazorUI.Components.Alert;

/// <summary>
/// The title/headline component for an Alert.
/// </summary>
/// <remarks>
/// <para>
/// AlertTitle provides the main heading for an alert message.
/// It uses semantic HTML (h5) for accessibility.
/// </para>
/// </remarks>
/// <example>
/// <code>
/// &lt;Alert&gt;
///     &lt;AlertTitle&gt;Success!&lt;/AlertTitle&gt;
///     &lt;AlertDescription&gt;Your changes have been saved.&lt;/AlertDescription&gt;
/// &lt;/Alert&gt;
/// </code>
/// </example>
public partial class AlertTitle : ComponentBase
{
    /// <summary>
    /// Gets or sets additional CSS classes to apply to the alert title.
    /// </summary>
    [Parameter]
    public string? Class { get; set; }

    /// <summary>
    /// Gets or sets the content to be rendered inside the alert title.
    /// </summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    /// <summary>
    /// Gets the computed CSS classes for the alert title element.
    /// </summary>
    private string CssClass => ClassNames.cn(
        "mb-1 font-medium leading-none tracking-tight",
        Class
    );
}
