namespace NeoUI.Blazor;

/// <summary>
/// Defines the selection behavior for a DataView component.
/// </summary>
public enum DataViewSelectionMode
{
    /// <summary>No selection. Items are not interactive.</summary>
    None,
    /// <summary>Only one item can be selected at a time.</summary>
    Single,
    /// <summary>Multiple items can be selected simultaneously.</summary>
    Multiple
}
