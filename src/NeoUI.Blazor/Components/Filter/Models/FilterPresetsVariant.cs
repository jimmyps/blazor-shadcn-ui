namespace NeoUI.Blazor;

/// <summary>Controls how filter presets are rendered inside <see cref="FilterBuilder{TData}"/>.</summary>
public enum FilterPresetsVariant
{
    /// <summary>Presets shown as a "Presets" dropdown button (default).</summary>
    Dropdown,

    /// <summary>
    /// Presets shown as horizontal tab buttons (Stripe-style).
    /// Clicking a tab replaces the active filter conditions with that preset's conditions.
    /// An implicit "All" tab clears all filters.
    /// </summary>
    Tabs
}
