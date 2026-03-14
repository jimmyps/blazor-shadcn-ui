namespace NeoUI.Blazor;

/// <summary>
/// Defines how nodes are selected in a TreeView.
/// </summary>
public enum TreeSelectionMode
{
    /// <summary>No selection is allowed.</summary>
    None,
    /// <summary>Only one node can be selected at a time.</summary>
    Single,
    /// <summary>Multiple nodes can be selected simultaneously.</summary>
    Multiple
}
