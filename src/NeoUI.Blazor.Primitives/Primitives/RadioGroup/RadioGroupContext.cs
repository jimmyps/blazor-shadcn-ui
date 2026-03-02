namespace BlazorUI.Primitives.RadioGroup;

/// <summary>
/// Context class for sharing state between RadioGroup and RadioGroupItem components.
/// </summary>
/// <typeparam name="TValue">The type of the value associated with radio group items.</typeparam>
/// <remarks>
/// This context is provided via Blazor's CascadingValue mechanism from the RadioGroup
/// to its child RadioGroupItem components.
/// </remarks>
public class RadioGroupContext<TValue>
{
    /// <summary>
    /// Gets or sets the currently selected value in the radio group.
    /// </summary>
    public TValue? Value { get; set; }

    /// <summary>
    /// Gets or sets whether the radio group is disabled.
    /// </summary>
    public bool Disabled { get; set; }

    /// <summary>
    /// Gets or sets the name for all radio items in this group.
    /// </summary>
    /// <remarks>
    /// This name is shared by all radio items in the group for form submission.
    /// </remarks>
    public string? Name { get; set; }

    /// <summary>
    /// Gets or sets whether a selection is required in this radio group.
    /// </summary>
    public bool Required { get; set; }

    /// <summary>
    /// Gets or sets the callback invoked when a radio item is selected.
    /// </summary>
    public Func<TValue, Task>? SelectValue { get; set; }

    /// <summary>
    /// Gets or sets the list of registered radio group items.
    /// </summary>
    public List<RadioGroupItem<TValue>> Items { get; set; } = new();
}
