namespace BlazorUI.Components.Common;

/// <summary>
/// Specifies when an input component should update its bound value.
/// </summary>
public enum InputUpdateMode
{
    /// <summary>
    /// Update value immediately on every input event (keystroke).
    /// </summary>
    Input,
    
    /// <summary>
    /// Update value only when input loses focus (blur event).
    /// </summary>
    Change
}
