using BlazorUI.Components.Utilities;
using Microsoft.AspNetCore.Components;

namespace BlazorUI.Components.ToggleGroup;

/// <summary>
/// A toggle group component that manages multiple toggle items.
/// </summary>
/// <remarks>
/// <para>
/// The ToggleGroup component provides a container for managing toggle items
/// with single or multiple selection modes. It follows the shadcn/ui design system.
/// </para>
/// <para>
/// Features:
/// - Single selection mode (radio behavior)
/// - Multiple selection mode (checkbox behavior)
/// - Cascading value to child ToggleGroupItem components
/// - Keyboard navigation
/// - Accessible with ARIA roles
/// - Dark mode compatible via CSS variables
/// </para>
/// </remarks>
/// <example>
/// <code>
/// &lt;ToggleGroup Type="ToggleGroupType.Single" @bind-Value="SelectedValue"&gt;
///     &lt;ToggleGroupItem Value="left"&gt;Left&lt;/ToggleGroupItem&gt;
///     &lt;ToggleGroupItem Value="center"&gt;Center&lt;/ToggleGroupItem&gt;
///     &lt;ToggleGroupItem Value="right"&gt;Right&lt;/ToggleGroupItem&gt;
/// &lt;/ToggleGroup&gt;
/// </code>
/// </example>
public partial class ToggleGroup : ComponentBase
{
    /// <summary>
    /// Gets or sets the type of toggle group (single or multiple selection).
    /// </summary>
    [Parameter]
    public ToggleGroupType Type { get; set; } = ToggleGroupType.Single;

    /// <summary>
    /// Gets or sets the currently selected value (for single selection mode).
    /// </summary>
    [Parameter]
    public string? Value { get; set; }

    /// <summary>
    /// Gets or sets the callback that is invoked when the value changes.
    /// </summary>
    [Parameter]
    public EventCallback<string?> ValueChanged { get; set; }

    /// <summary>
    /// Gets or sets the currently selected values (for multiple selection mode).
    /// </summary>
    [Parameter]
    public List<string>? Values { get; set; }

    /// <summary>
    /// Gets or sets the callback that is invoked when the values change.
    /// </summary>
    [Parameter]
    public EventCallback<List<string>?> ValuesChanged { get; set; }

    /// <summary>
    /// Gets or sets whether the toggle group is disabled.
    /// </summary>
    [Parameter]
    public bool Disabled { get; set; }

    /// <summary>
    /// Gets or sets additional CSS classes to apply to the toggle group.
    /// </summary>
    [Parameter]
    public string? Class { get; set; }

    /// <summary>
    /// Gets or sets the content to be rendered inside the toggle group.
    /// </summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    /// <summary>
    /// Handles selection of a toggle item.
    /// </summary>
    public async Task OnItemSelectedAsync(string itemValue)
    {
        if (Type == ToggleGroupType.Single)
        {
            Value = Value == itemValue ? null : itemValue;
            await ValueChanged.InvokeAsync(Value);
        }
        else
        {
            // Multiple selection - create new list to ensure change detection
            var currentValues = Values?.ToList() ?? new List<string>();
            if (currentValues.Contains(itemValue))
            {
                currentValues.Remove(itemValue);
            }
            else
            {
                currentValues.Add(itemValue);
            }
            Values = currentValues;
            await ValuesChanged.InvokeAsync(Values);
        }
    }

    /// <summary>
    /// Checks if an item is selected.
    /// </summary>
    public bool IsItemSelected(string itemValue)
    {
        return Type == ToggleGroupType.Single
            ? Value == itemValue
            : Values?.Contains(itemValue) ?? false;
    }

    private string CssClass => ClassNames.cn(
        "inline-flex items-center justify-center rounded-md",
        Class
    );
}
