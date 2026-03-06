namespace NeoUI.Demo.Shared.Pages.Components.Filter;

public partial class FilterBuilderDemo
{
    private static readonly string _quickStartCode = """
        <FilterBuilder TData="Product"
                       @bind-Filters="_filters"
                       OnFilterChange="HandleFilterChange">
            <FilterFields>
                <FilterField Field="Name"
                             Label="Product Name"
                             Type="FilterFieldType.Text"
                             Placeholder="Search by name..." />
                <FilterField Field="Price"
                             Label="Price"
                             Type="FilterFieldType.Number"
                             Min="0" />
                <FilterField Field="Category"
                             Label="Category"
                             Type="FilterFieldType.Select"
                             Options="@categoryOptions" />
            </FilterFields>
        </FilterBuilder>

        @code {
            private FilterGroup _filters = new();

            private void HandleFilterChange(FilterGroup filters)
            {
                var results = allProducts.ApplyFilters(filters).ToList();
            }
        }
        """;

    private static readonly IReadOnlyList<DemoPropRow> _filterBuilderProps =
    [
        new("TData",           "type parameter",                null,           "The data model type being filtered."),
        new("Filters",         "FilterGroup?",                  "null",         "Current filter state. Use <code>@bind-Filters</code> for two-way binding."),
        new("FiltersChanged",  "EventCallback&lt;FilterGroup&gt;", null,        "Callback for two-way binding."),
        new("OnFilterChange",  "EventCallback&lt;FilterGroup&gt;", null,        "Fires immediately when Apply Filters is clicked."),
        new("FilterFields",    "RenderFragment?",               "null",         "<code>FilterField</code> child declarations."),
        new("FilterPresets",   "RenderFragment?",               "null",         "<code>FilterPreset</code> child declarations."),
        new("ButtonText",      "string",                        "\"Filters\"",  "Label on the trigger button."),
        new("ButtonVariant",   "ButtonVariant",                 "Outline",      "Visual variant of the trigger button."),
        new("ButtonSize",      "ButtonSize",                    "Default",      "Size of the trigger button."),
        new("ShowChips",       "bool",                          "true",         "Show active filter chips above the button."),
        new("Class",           "string?",                       "null",         "Additional CSS classes for the wrapper element."),
    ];

    private static readonly IReadOnlyList<DemoPropRow> _filterFieldProps =
    [
        new("Field",           "string",                        "required",     "Property name on the data model."),
        new("Label",           "string",                        "required",     "Human-readable label shown in the field picker."),
        new("Type",            "FilterFieldType",               "Text",         "Field type. Controls default operators and value input widget."),
        new("Operators",       "IEnumerable&lt;FilterOperator&gt;?", "null",    "Allowed operators. Auto-populated from Type when null."),
        new("DefaultOperator", "FilterOperator?",               "null",         "Operator pre-selected when a new condition is created."),
        new("Options",         "IEnumerable&lt;SelectOption&gt;?", "null",      "Options for <code>Select</code> / <code>MultiSelect</code> fields."),
        new("Placeholder",     "string?",                       "null",         "Placeholder text for the value input."),
        new("Min",             "object?",                       "null",         "Minimum value (Number fields)."),
        new("Max",             "object?",                       "null",         "Maximum value (Number fields)."),
        new("Step",            "object?",                       "null",         "Step increment (Number fields)."),
        new("ChildContent",    "RenderFragment&lt;FilterCondition&gt;?", "null","Custom value input. Replaces the default widget for <code>FilterFieldType.Custom</code>."),
    ];
}
