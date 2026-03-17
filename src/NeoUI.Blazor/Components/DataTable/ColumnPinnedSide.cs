namespace NeoUI.Blazor;

/// <summary>
/// Specifies which side a DataTable column is pinned to during horizontal scrolling.
/// </summary>
public enum ColumnPinnedSide
{
    /// <summary>The column scrolls normally (not pinned).</summary>
    None,

    /// <summary>The column sticks to the left edge of the table viewport.</summary>
    Left,

    /// <summary>The column sticks to the right edge of the table viewport.</summary>
    Right
}
