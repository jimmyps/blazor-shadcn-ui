namespace BlazorBlueprint.Primitives.RadioGroup;

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
    /// Gets or sets the callback invoked when a radio item is selected.
    /// </summary>
    public Func<TValue, Task>? SelectValue { get; set; }

    /// <summary>
    /// Gets or sets the list of registered radio group items.
    /// </summary>
    public List<BbRadioGroupItem<TValue>> Items { get; set; } = new();

    /// <summary>
    /// Gets or sets default CSS classes to apply to all items in the group.
    /// Cascaded from the parent RadioGroup's ItemClass parameter.
    /// </summary>
    public string? ItemClass { get; set; }
}
