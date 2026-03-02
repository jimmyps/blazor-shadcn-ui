namespace BlazorUI.Components.Grid;

/// <summary>
/// Represents a batched transaction of changes to apply to the grid.
/// This enables efficient delta updates instead of full data refreshes.
/// </summary>
/// <typeparam name="TItem">The type of items in the grid.</typeparam>
public class GridTransaction<TItem>
{
    /// <summary>
    /// Gets or sets the items to add to the grid.
    /// </summary>
    public List<TItem>? Add { get; set; }

    /// <summary>
    /// Gets or sets the items to remove from the grid.
    /// </summary>
    public List<TItem>? Remove { get; set; }

    /// <summary>
    /// Gets or sets the items to update in the grid.
    /// </summary>
    public List<TItem>? Update { get; set; }

    /// <summary>
    /// Gets whether this transaction has any changes.
    /// </summary>
    public bool HasChanges => 
        (Add?.Count > 0) || 
        (Remove?.Count > 0) || 
        (Update?.Count > 0);
}
