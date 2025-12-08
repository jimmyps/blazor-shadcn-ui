using BlazorUI.Components.Utilities;
using Microsoft.AspNetCore.Components;

namespace BlazorUI.Components.NativeSelect;

/// <summary>
/// A native HTML select component with shadcn/ui styling.
/// </summary>
/// <remarks>
/// <para>
/// The NativeSelect component wraps the native HTML select element
/// with consistent styling from the shadcn/ui design system.
/// </para>
/// <para>
/// Features:
/// - Two-way data binding with @bind-Value
/// - Placeholder option support
/// - Disabled and required states
/// - Accessible native HTML select
/// - Dark mode compatible via CSS variables
/// </para>
/// </remarks>
/// <example>
/// <code>
/// &lt;NativeSelect @bind-Value="SelectedValue" Placeholder="Choose an option"&gt;
///     &lt;NativeSelectOption Value="option1"&gt;Option 1&lt;/NativeSelectOption&gt;
///     &lt;NativeSelectOption Value="option2"&gt;Option 2&lt;/NativeSelectOption&gt;
/// &lt;/NativeSelect&gt;
/// </code>
/// </example>
public partial class NativeSelect : ComponentBase
{
    /// <summary>
    /// Gets or sets the selected value.
    /// </summary>
    [Parameter]
    public string? Value { get; set; }

    /// <summary>
    /// Gets or sets the callback that is invoked when the value changes.
    /// </summary>
    [Parameter]
    public EventCallback<string?> ValueChanged { get; set; }

    /// <summary>
    /// Gets or sets the id attribute for the select element.
    /// </summary>
    [Parameter]
    public string? Id { get; set; }

    /// <summary>
    /// Gets or sets the name attribute for the select element.
    /// </summary>
    [Parameter]
    public string? Name { get; set; }

    /// <summary>
    /// Gets or sets the placeholder text when no option is selected.
    /// </summary>
    [Parameter]
    public string? Placeholder { get; set; }

    /// <summary>
    /// Gets or sets whether the select is disabled.
    /// </summary>
    [Parameter]
    public bool Disabled { get; set; }

    /// <summary>
    /// Gets or sets whether the select is required.
    /// </summary>
    [Parameter]
    public bool Required { get; set; }

    /// <summary>
    /// Gets or sets additional CSS classes to apply to the select.
    /// </summary>
    [Parameter]
    public string? Class { get; set; }

    /// <summary>
    /// Gets or sets the content to be rendered inside the select (options).
    /// </summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    private async Task OnChangeAsync(ChangeEventArgs e)
    {
        Value = e.Value?.ToString();
        await ValueChanged.InvokeAsync(Value);
    }

    private string ContainerCssClass => ClassNames.cn(
        "relative w-full",
        Class
    );

    private string CssClass => ClassNames.cn(
        "flex h-10 w-full items-center justify-between rounded-md border border-input bg-background px-3 py-2 text-sm ring-offset-background",
        "placeholder:text-muted-foreground",
        "focus:outline-none focus:ring-2 focus:ring-ring focus:ring-offset-2",
        "disabled:cursor-not-allowed disabled:opacity-50",
        "appearance-none cursor-pointer"
    );
}
