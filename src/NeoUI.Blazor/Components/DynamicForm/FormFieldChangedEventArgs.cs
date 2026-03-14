namespace NeoUI.Blazor;

/// <summary>
/// Event args raised by <see cref="DynamicForm"/> when a field value changes.
/// </summary>
public sealed class FormFieldChangedEventArgs
{
    /// <summary>Gets the field name that changed.</summary>
    public string FieldName { get; }

    /// <summary>Gets the new value.</summary>
    public object? NewValue { get; }

    /// <summary>Gets the previous value.</summary>
    public object? OldValue { get; }

    /// <summary>Initialises a new instance.</summary>
    public FormFieldChangedEventArgs(string fieldName, object? newValue, object? oldValue)
    {
        FieldName = fieldName;
        NewValue  = newValue;
        OldValue  = oldValue;
    }
}
