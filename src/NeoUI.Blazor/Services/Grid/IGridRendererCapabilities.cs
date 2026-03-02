namespace BlazorUI.Components.Services.Grid;

/// <summary>
/// Describes the capabilities of a grid renderer.
/// Used for diagnostics and feature detection.
/// </summary>
public interface IGridRendererCapabilities
{
    /// <summary>
    /// Gets a value indicating whether the renderer supports virtualization for large datasets.
    /// </summary>
    bool SupportsVirtualization { get; }

    /// <summary>
    /// Gets a value indicating whether the renderer supports column pinning (left/right).
    /// </summary>
    bool SupportsColumnPinning { get; }

    /// <summary>
    /// Gets a value indicating whether the renderer supports column reordering via drag-and-drop.
    /// </summary>
    bool SupportsColumnReordering { get; }

    /// <summary>
    /// Gets a value indicating whether the renderer supports column resizing.
    /// </summary>
    bool SupportsColumnResizing { get; }

    /// <summary>
    /// Gets a value indicating whether the renderer supports infinite scroll paging mode.
    /// </summary>
    bool SupportsInfiniteScroll { get; }

    /// <summary>
    /// Gets a value indicating whether the renderer supports server-side paging mode.
    /// </summary>
    bool SupportsServerSidePaging { get; }

    /// <summary>
    /// Gets a value indicating whether the renderer supports data export functionality.
    /// </summary>
    bool SupportsExport { get; }

    /// <summary>
    /// Gets an array of feature names that are not supported by this renderer.
    /// </summary>
    string[] UnsupportedFeatures { get; }
}
