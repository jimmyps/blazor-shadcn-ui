using Microsoft.AspNetCore.Components;

namespace NeoUI.Blazor;

/// <summary>
/// Declares a data field for a <see cref="DataView{TItem}"/> with optional sorting and filtering support.
/// Place inside the <c>Fields</c> slot to enable toolbar search and the sort dropdown.
/// </summary>
/// <typeparam name="TItem">The item type — must match the parent DataView.</typeparam>
/// <example>
/// <code>
/// &lt;DataView Items="@_products"&gt;
///     &lt;Fields&gt;
///         &lt;DataViewColumn TItem="Product" Header="Name"  Property="@(p => p.Name)"  Filterable="true" Sortable="true" /&gt;
///         &lt;DataViewColumn TItem="Product" Header="Price" Property="@(p => p.Price)" Sortable="true" /&gt;
///     &lt;/Fields&gt;
///     &lt;ListTemplate Context="p"&gt;...&lt;/ListTemplate&gt;
/// &lt;/DataView&gt;
/// </code>
/// </example>
public partial class DataViewColumn<TItem> : ComponentBase
{
    /// <summary>Optional unique identifier. Defaults to a slug of Header.</summary>
    [Parameter] public string? Id { get; set; }

    /// <summary>Column display label shown in the sort dropdown.</summary>
    [Parameter] public string Header { get; set; } = "";

    /// <summary>Extracts the field value from an item (used for sort and filter).</summary>
    [Parameter] public Func<TItem, object?>? Property { get; set; }

    /// <summary>When true, this column is included in the toolbar sort dropdown.</summary>
    [Parameter] public bool Sortable { get; set; }

    /// <summary>When true, the global search input filters items by this column's value.</summary>
    [Parameter] public bool Filterable { get; set; }

    [CascadingParameter] private DataView<TItem>? ParentView { get; set; }

    internal string EffectiveId => Id ?? Header.ToLowerInvariant().Replace(" ", "-");

    protected override void OnInitialized()
    {
        if (ParentView is null)
            throw new InvalidOperationException(
                $"{nameof(DataViewColumn<TItem>)} must be placed inside a {nameof(DataView<TItem>)} Fields slot.");

        ParentView.RegisterColumn(this);
    }
}
