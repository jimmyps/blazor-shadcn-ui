namespace NeoUI.Blazor;

/// <summary>
/// Controls how the selection indicator is rendered inside a DataView item.
/// </summary>
public enum DataViewCheckVariant
{
    /// <summary>A filled circle-check icon; faint when unselected (default).</summary>
    CircleCheck,
    /// <summary>A plain checkmark icon with no circle; invisible when unselected. Common in mobile-style UIs.</summary>
    Check,
    /// <summary>No built-in indicator. Manage selection feedback entirely through the item templates.</summary>
    None
}
