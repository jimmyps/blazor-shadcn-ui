using Microsoft.AspNetCore.Components;

namespace NeoUI.Blazor;

/// <summary>
/// Defines the item rendering template for list layout mode of a <see cref="DataView{TItem}"/>.
/// Place this component inside a <see cref="DataView{TItem}"/> to specify how each item is rendered in
/// list mode. Use alongside <see cref="DataViewGridTemplate{TItem}"/> to enable the toolbar layout-toggle.
/// </summary>
/// <typeparam name="TItem">The type of data items in the view.</typeparam>
/// <remarks>
/// When only a <see cref="DataViewListTemplate{TItem}"/> is placed (without a
/// <see cref="DataViewGridTemplate{TItem}"/>) the view locks into list mode and hides the layout-toggle.
/// Provide both to enable the toggle.
/// </remarks>
/// <example>
/// <code>
/// &lt;DataView TItem="Person" Items="@people"&gt;
///     &lt;DataViewListTemplate TItem="Person" Context="person"&gt;
///         &lt;div class="flex items-center gap-3 p-4"&gt;@person.Name&lt;/div&gt;
///     &lt;/DataViewListTemplate&gt;
/// &lt;/DataView&gt;
/// </code>
/// </example>
public partial class DataViewListTemplate<TItem> : ComponentBase where TItem : class
{
    /// <summary>Render fragment used to display each item in list layout.</summary>
    [Parameter]
    public RenderFragment<TItem>? ChildContent { get; set; }

    /// <summary>Parent DataView — automatically set via cascading parameter.</summary>
    [CascadingParameter]
    internal DataView<TItem>? ParentView { get; set; }

    protected override void OnInitialized()
    {
        if (ParentView is null)
            throw new InvalidOperationException(
                $"{nameof(DataViewListTemplate<TItem>)} must be placed inside a {nameof(DataView<TItem>)} component.");

        ParentView.SetListTemplate(ChildContent);
    }
}
