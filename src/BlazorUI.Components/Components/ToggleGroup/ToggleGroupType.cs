namespace BlazorUI.Components.ToggleGroup;

/// <summary>
/// Defines the type of toggle group (single or multiple selection).
/// </summary>
public enum ToggleGroupType
{
    /// <summary>
    /// Single selection mode (radio behavior).
    /// Only one item can be selected at a time.
    /// </summary>
    Single,

    /// <summary>
    /// Multiple selection mode (checkbox behavior).
    /// Multiple items can be selected simultaneously.
    /// </summary>
    Multiple
}
