using Microsoft.AspNetCore.Components;

namespace BlazorUI.Components.SelectableList;

/// <summary>
/// SelectableList component that provides an elegant, declarative API for styled selectable item lists.
/// Leverages CommandList for listbox semantics and keyboard navigation.
/// </summary>
/// <typeparam name="TItem">The type of items in the list</typeparam>
public partial class SelectableList<TItem>
{
    /// <summary>
    /// Gets or sets the collection of items to display.
    /// Required when using automatic rendering (not using ChildContent).
    /// </summary>
    [Parameter]
    public IEnumerable<TItem> Items { get; set; } = Enumerable.Empty<TItem>();

    /// <summary>
    /// Gets or sets the function to extract the value from an item.
    /// This value is used for selection tracking.
    /// Required when using automatic rendering (not using ChildContent).
    /// </summary>
    [Parameter]
    public Func<TItem, string>? ValueSelector { get; set; }

    /// <summary>
    /// Gets or sets the currently selected value.
    /// </summary>
    [Parameter]
    public string? SelectedValue { get; set; }

    /// <summary>
    /// Gets or sets the callback invoked when the selected value changes.
    /// </summary>
    [Parameter]
    public EventCallback<string> SelectedValueChanged { get; set; }

    /// <summary>
    /// Gets or sets the function to extract display text from an item for search filtering.
    /// If not provided, uses the ValueSelector.
    /// </summary>
    [Parameter]
    public Func<TItem, string>? SearchTextSelector { get; set; }

    /// <summary>
    /// Gets or sets the template for rendering each item.
    /// The context is the item itself.
    /// Required when using automatic rendering (not using ChildContent).
    /// </summary>
    [Parameter]
    public RenderFragment<TItem>? ItemTemplate { get; set; }

    /// <summary>
    /// Gets or sets the child content for manual composition.
    /// When provided, Items, ValueSelector, ItemTemplate, and GroupSelector are ignored.
    /// Use CommandGroup and CommandItem directly within this content.
    /// </summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    /// <summary>
    /// Gets or sets the function to group items.
    /// If provided, items will be grouped with headings.
    /// </summary>
    [Parameter]
    public Func<TItem, string>? GroupSelector { get; set; }

    /// <summary>
    /// Gets or sets whether to show the search input.
    /// Default is false.
    /// </summary>
    [Parameter]
    public bool ShowSearch { get; set; }

    /// <summary>
    /// Gets or sets the placeholder text for the search input.
    /// Only used when ShowSearch is true.
    /// </summary>
    [Parameter]
    public string SearchPlaceholder { get; set; } = "Search...";

    /// <summary>
    /// Gets or sets additional CSS classes to apply to the container.
    /// </summary>
    [Parameter]
    public string? ContainerClass { get; set; }

    /// <summary>
    /// Gets or sets additional CSS classes to apply to each item.
    /// </summary>
    [Parameter]
    public string? ItemClass { get; set; }

    /// <summary>
    /// Gets or sets a function that determines if an item is disabled.
    /// </summary>
    [Parameter]
    public Func<TItem, bool>? IsDisabled { get; set; }

    /// <summary>
    /// Handles the value change from the Command component.
    /// </summary>
    private async Task HandleValueChanged(string value)
    {
        SelectedValue = value;
        if (SelectedValueChanged.HasDelegate)
        {
            await SelectedValueChanged.InvokeAsync(value);
        }
    }

    /// <summary>
    /// Renders a single item.
    /// </summary>
    private RenderFragment RenderItem(TItem item) => builder =>
    {
        if (ValueSelector == null || ItemTemplate == null)
        {
            throw new InvalidOperationException("ValueSelector and ItemTemplate are required when using automatic rendering.");
        }

        var value = ValueSelector(item);
        var searchText = SearchTextSelector?.Invoke(item) ?? value;
        var isDisabled = IsDisabled?.Invoke(item) ?? false;

        builder.OpenComponent<Command.CommandItem>(0);
        builder.AddAttribute(1, nameof(Command.CommandItem.Value), value);
        builder.AddAttribute(2, nameof(Command.CommandItem.SearchText), searchText);
        builder.AddAttribute(3, nameof(Command.CommandItem.Disabled), isDisabled);
        builder.AddAttribute(4, nameof(Command.CommandItem.Class), ItemClass);
        builder.AddAttribute(5, nameof(Command.CommandItem.ChildContent), ItemTemplate(item));
        builder.CloseComponent();
    };
}
