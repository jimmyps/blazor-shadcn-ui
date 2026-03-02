namespace NeoUI.Blazor.Primitives;

/// <summary>
/// Context class for sharing state between DropdownMenuRadioGroupPrimitive and DropdownMenuRadioItemPrimitive components.
/// </summary>
/// <typeparam name="TValue">The type of the value associated with radio items.</typeparam>
public class DropdownMenuRadioGroupContext<TValue>
{
    /// <summary>
    /// Gets or sets the currently selected value in the radio group.
    /// </summary>
    public TValue? Value { get; set; }

    /// <summary>
    /// Gets or sets the callback invoked when a radio item is selected.
    /// </summary>
    public Func<TValue?, Task>? SelectValue { get; set; }
}
