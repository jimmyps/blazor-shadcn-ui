namespace NeoUI.Demo.Shared.Pages.Components.Filter;

partial class BasicExample
{
    private static readonly string _basicCode = """
        <FilterBuilder TData="Product"
                       @bind-Filters="activeFilters"
                       OnFilterChange="HandleFilterChange">
            <FilterFields>
                <FilterField Field="Name"     Label="Product Name" Icon="tag"
                             Type="FilterFieldType.Text"   Placeholder="Search by name..." />
                <FilterField Field="Category" Label="Category"     Icon="layers"
                             Type="FilterFieldType.Select" Options="@categoryOptions" />
                <FilterField Field="Price"    Label="Price"        Icon="dollar-sign"
                             Type="FilterFieldType.Number"
                             EditorType="FilterEditorType.Currency" Min="0" />
                <FilterField Field="InStock"  Label="In Stock"     Icon="package"
                             Type="FilterFieldType.Boolean" />
            </FilterFields>
        </FilterBuilder>

        @code {
            private FilterGroup activeFilters = new();

            private void HandleFilterChange(FilterGroup filters)
            {
                filteredProducts = allProducts.ApplyFilters(filters).ToList();
            }
        }
        """;
}
