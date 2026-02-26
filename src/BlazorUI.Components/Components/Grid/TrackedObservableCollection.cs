using System.Collections.ObjectModel;

namespace BlazorUI.Components.Grid;

/// <summary>
/// An ObservableCollection that supports explicit change notification for item mutations.
/// Use NotifyItemsChanged() to signal when items in the collection have been modified.
/// </summary>
/// <typeparam name="T">The type of elements in the collection.</typeparam>
public class TrackedObservableCollection<T> : ObservableCollection<T>
{
    /// <summary>
    /// Occurs when one or more items in the collection have been modified.
    /// </summary>
    public event EventHandler<ItemsChangedEventArgs<T>>? ItemsChanged;
    
    /// <summary>
    /// Notifies listeners that the specified items have been modified.
    /// </summary>
    /// <param name="items">The items that have been modified.</param>
    public void NotifyItemsChanged(params T[] items)
    {
        if (items == null || items.Length == 0)
            return;
            
        ItemsChanged?.Invoke(this, new ItemsChangedEventArgs<T>(items));
    }
    
    /// <summary>
    /// Notifies listeners that the specified items have been modified.
    /// </summary>
    /// <param name="items">The items that have been modified.</param>
    public void NotifyItemsChanged(IEnumerable<T> items)
    {
        if (items == null)
            return;
            
        var itemsList = items.ToList();
        if (itemsList.Count == 0)
            return;
            
        ItemsChanged?.Invoke(this, new ItemsChangedEventArgs<T>(itemsList));
    }
}

/// <summary>
/// Provides data for the ItemsChanged event.
/// </summary>
/// <typeparam name="T">The type of elements that changed.</typeparam>
public class ItemsChangedEventArgs<T> : EventArgs
{
    /// <summary>
    /// Gets the list of items that have been modified.
    /// </summary>
    public IReadOnlyList<T> ChangedItems { get; }
    
    /// <summary>
    /// Initializes a new instance of the <see cref="ItemsChangedEventArgs{T}"/> class.
    /// </summary>
    /// <param name="changedItems">The items that have been modified.</param>
    public ItemsChangedEventArgs(IReadOnlyList<T> changedItems)
    {
        ChangedItems = changedItems ?? throw new ArgumentNullException(nameof(changedItems));
    }
}
