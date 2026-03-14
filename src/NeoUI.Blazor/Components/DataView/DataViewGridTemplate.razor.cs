using Microsoft.AspNetCore.Components;

namespace NeoUI.Blazor;

/// <summary>
/// Defines the item rendering template for grid layout mode of a <see cref="DataView{TItem}"/>.
/// Place this component inside a <see cref="DataView{TItem}"/> to specify how each item is rendered in
/// grid mode. Use alongside <see cref="DataViewListTemplate{TItem}"/> to enable the toolbar layout-toggle.
/// </summary>
/// <typeparam name="TItem">The type of data items in the view.</typeparam>
/// <remarks>
/// When only a <see cref="DataViewGridTemplate{TItem}"/> is placed (without a
/// <see cref="DataViewListTemplate{TItem}"/>) the view locks into grid mode and hides the layout-toggle.
/// Provide both to enable the toggle.
/// </remarks>
/// <example>
/// <code>
/// &lt;DataView TItem="Product" Items="@products"&gt;
///     &lt;DataViewGridTemplate TItem="Product" Context="product"&gt;
///         &lt;div class="p-4 border rounded-lg"&gt;@product.Name&lt;/div&gt;
///     &lt;/DataViewGridTemplate&gt;
/// &lt;/DataView&gt;
/// </code>
/// </example>
public partial class DataViewGridTemplate<TItem> : ComponentBase where TItem : class
{
    /// <summary>Render fragment used to display each item in grid layout.</summary>
    [Parameter]
    public RenderFragment<TItem>? ChildContent { get; set; }

    /// <summary>Parent DataView — automatically set via cascading parameter.</summary>
    [CascadingParameter]
    internal DataView<TItem>? ParentView { get; set; }

    protected override void OnInitialized()
    {
        if (ParentView is null)
            throw new InvalidOperationException(
                $"{nameof(DataViewGridTemplate<TItem>)} must be placed inside a {nameof(DataView<TItem>)} component.");

        ParentView.SetGridTemplate(ChildContent);
    }
}
