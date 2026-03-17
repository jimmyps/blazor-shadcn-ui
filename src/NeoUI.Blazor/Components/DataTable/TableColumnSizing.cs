namespace NeoUI.Blazor;

/// <summary>
/// Controls whether the table uses a fixed or automatic column-sizing algorithm.
/// </summary>
public enum TableColumnSizing
{
    /// <summary>
    /// Browser distributes column widths automatically (<c>table-layout: auto</c>).
    /// Any <c>Width</c> values specified on columns are treated as hints only.
    /// This is the default and preserves the existing behaviour for tables without pinned columns.
    /// </summary>
    Auto,

    /// <summary>
    /// Column widths are strictly honoured (<c>table-layout: fixed</c>).
    /// A <c>&lt;colgroup&gt;</c> element is rendered so each column gets exactly the width
    /// specified via its <c>Width</c> parameter; columns whose total exceeds the container
    /// width cause it to scroll horizontally.
    /// Required when using column resizing or when you need guaranteed fixed widths without
    /// column pinning.  Tables with pinned columns always use fixed sizing regardless of
    /// this setting.
    /// </summary>
    Fixed,
}
