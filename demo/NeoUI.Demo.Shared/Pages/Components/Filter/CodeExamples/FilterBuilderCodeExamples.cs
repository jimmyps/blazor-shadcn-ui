namespace NeoUI.Demo.Shared.Pages.Components.Filter;

public partial class FilterBuilderDemo
{
    private static readonly string _quickStartCode = """
        <FilterBuilder TData="Product"
                       @bind-Filters="activeFilters"
                       OnFilterChange="HandleFilterChange">
            <FilterFields>
                <FilterField Field="Name"   Label="Name"   Icon="tag"         Type="FilterFieldType.Text" />
                <FilterField Field="Price"  Label="Price"  Icon="dollar-sign" Type="FilterFieldType.Number"
                             EditorType="FilterEditorType.Currency" Min="0" />
                <FilterField Field="Status" Label="Status" Icon="activity"    Type="FilterFieldType.Select"
                             Options="@statusOptions" />
            </FilterFields>
            <FilterPresets>
                <FilterPreset Name="High Value" Icon="trending-up" Filters="@highValuePreset" />
            </FilterPresets>
        </FilterBuilder>

        @code {
            private FilterGroup activeFilters = new();

            private void HandleFilterChange(FilterGroup filters)
                => filteredItems = allItems.ApplyFilters(filters).ToList();
        }
        """;

    private static readonly IReadOnlyList<DemoPropRow> _filterBuilderProps =
    [
        new("TData",          "type parameter",                    null,             "The data model type being filtered."),
        new("Filters",        "FilterGroup?",                      "null",           "Current filter state. Use <code>@bind-Filters</code> for two-way binding."),
        new("FiltersChanged", "EventCallback&lt;FilterGroup&gt;",  null,             "Two-way binding callback."),
        new("OnFilterChange", "EventCallback&lt;FilterGroup&gt;",  null,             "Fires immediately when any condition changes."),
        new("FilterFields",   "RenderFragment?",                   "null",           "<code>FilterField</code> child declarations."),
        new("FilterPresets",  "RenderFragment?",                   "null",           "<code>FilterPreset</code> child declarations. Adds a Presets dropdown."),
        new("ButtonText",     "string",                            "\"Filter\"",     "Label on the add-filter button (shown when no conditions are active)."),
        new("Class",          "string?",                           "null",           "Additional CSS classes for the wrapper element."),
    ];

    private static readonly IReadOnlyList<DemoPropRow> _filterFieldProps =
    [
        new("Field",           "string",                                    "required", "Property name on the data model."),
        new("Label",           "string",                                    "required", "Human-readable label shown in the chip and field picker."),
        new("Icon",            "string?",                                   "null",     "Lucide icon name shown in the chip and field picker dropdown."),
        new("Type",            "FilterFieldType",                           "Text",     "Data type — controls the default operators list."),
        new("EditorType",      "FilterEditorType",                          "Auto",     "Value input widget. Auto infers from Type. Override with Currency, Numeric, Masked, etc."),
        new("Operators",       "IEnumerable&lt;FilterOperator&gt;?",        "null",     "Allowed operators. Auto-populated from Type when null."),
        new("DefaultOperator", "FilterOperator?",                           "null",     "Operator pre-selected when a new condition is created."),
        new("Options",         "IEnumerable&lt;SelectOption&gt;?",          "null",     "Options for Select / MultiSelect fields."),
        new("Placeholder",     "string?",                                   "null",     "Placeholder text for the value input."),
        new("Min",             "object?",                                   "null",     "Minimum value (Number fields)."),
        new("Max",             "object?",                                   "null",     "Maximum value (Number fields)."),
        new("Step",            "object?",                                   "null",     "Step increment (Number fields)."),
        new("ChildContent",    "RenderFragment&lt;FilterCondition&gt;?",    "null",     "Custom value input for FilterFieldType.Custom fields."),
    ];
}
