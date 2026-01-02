namespace BlazorUI.Components.Grid;

/// <summary>
/// Defines the AG Grid built-in theme to use.
/// Each theme has a different visual appearance and is provided by AG Grid.
/// </summary>
public enum GridTheme
{
    /// <summary>
    /// AG Grid Alpine theme - Clean, modern look (default).
    /// Requires: ag-theme-alpine.css
    /// </summary>
    Alpine = 0,

    /// <summary>
    /// AG Grid Balham theme - Professional business theme.
    /// Requires: ag-theme-balham.css
    /// </summary>
    Balham = 1,

    /// <summary>
    /// AG Grid Material theme - Google Material Design styled.
    /// Requires: ag-theme-material.css
    /// </summary>
    Material = 2,

    /// <summary>
    /// AG Grid Quartz theme - Modern, polished appearance.
    /// Requires: ag-theme-quartz.css
    /// </summary>
    Quartz = 3
}
