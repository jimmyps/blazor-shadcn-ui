namespace NeoUI.Demo.Shared.Pages.Components.Filter;

public partial class FilterBuilderDemo
{
    private static readonly string _basicCode = """
        <FilterBuilder @bind-Value="_filter">
            <FilterField Field="Name" Label="Name" Type="FilterFieldType.Text" />
            <FilterField Field="Age" Label="Age" Type="FilterFieldType.Number" />
            <FilterField Field="CreatedAt" Label="Created" Type="FilterFieldType.Date" />
        </FilterBuilder>
        """;

    private static readonly string _withPresetsCode = """
        <FilterBuilder @bind-Value="_filterPresets" ShowPresets="true">
            <FilterField Field="Status" Label="Status" Type="FilterFieldType.Select">
                <SelectOption Value="active" Label="Active" />
                <SelectOption Value="inactive" Label="Inactive" />
                <SelectOption Value="pending" Label="Pending" />
            </FilterField>
            <FilterField Field="Priority" Label="Priority" Type="FilterFieldType.Select">
                <SelectOption Value="low" Label="Low" />
                <SelectOption Value="medium" Label="Medium" />
                <SelectOption Value="high" Label="High" />
            </FilterField>
            <FilterPreset Name="Active Items"
                          Filters="@(new FilterGroup { Conditions = [new FilterCondition { Field = "Status", Operator = FilterOperator.Equals, Value = "active" }] })"
                          Description="Show only active records" />
            <FilterPreset Name="High Priority"
                          Filters="@(new FilterGroup { Conditions = [new FilterCondition { Field = "Priority", Operator = FilterOperator.Equals, Value = "high" }] })"
                          Description="Show high priority items" />
        </FilterBuilder>
        """;

    private static readonly string _selectFieldsCode = """
        <FilterBuilder @bind-Value="_filterSelect">
            <FilterField Field="Category"
                         Label="Category"
                         Type="FilterFieldType.Select"
                         Options="@_categoryOptions" />
            <FilterField Field="Tags"
                         Label="Tags"
                         Type="FilterFieldType.MultiSelect"
                         Options="@_tagOptions" />
        </FilterBuilder>
        """;

    private static readonly string _readonlyCode = """
        @* Pre-populate filter to show display *@
        <FilterBuilder Value="@_prePopulated" ShowGroupLogic="true">
            <FilterField Field="Name" Label="Name" Type="FilterFieldType.Text" />
            <FilterField Field="Status" Label="Status" Type="FilterFieldType.Select"
                         Options="@_statusOptions" />
            <FilterField Field="Age" Label="Age" Type="FilterFieldType.Number" />
        </FilterBuilder>
        """;

    private static readonly string _customControlCode = """
        <FilterBuilder @bind-Value="_filterCustom">
            <FilterField Field="Rating" Label="Rating" Type="FilterFieldType.Custom">
                <ChildContent Context="condition">
                    <div class="flex items-center gap-1">
                        @for (int i = 1; i <= 5; i++)
                        {
                            var star = i;
                            <button class="@(GetStarClass(condition, star))"
                                    @onclick="() => SetRatingValue(condition, star)">
                                ★
                            </button>
                        }
                    </div>
                </ChildContent>
            </FilterField>
        </FilterBuilder>
        """;

    private static readonly IReadOnlyList<DemoPropRow> _filterBuilderProps =
    [
        new("Value",           "FilterGroup?",              "null",          "Current filter state. Use <code>@bind-Value</code> for two-way binding."),
        new("ValueChanged",    "EventCallback&lt;FilterGroup&gt;", null,     "Callback fired when the filter state changes."),
        new("ChildContent",    "RenderFragment?",           "null",          "Child content — <code>FilterField</code> and <code>FilterPreset</code> declarations."),
        new("Placeholder",     "string?",                   "\"No filters applied\"", "Text shown when no filters are active."),
        new("ShowPresets",     "bool",                      "true",          "Show the preset dropdown when presets are registered."),
        new("ShowGroupLogic",  "bool",                      "true",          "Show AND/OR toggle when more than one condition is active."),
        new("Class",           "string?",                   "null",          "Additional CSS classes for the filter bar container."),
    ];

    private static readonly IReadOnlyList<DemoPropRow> _filterFieldProps =
    [
        new("Field",           "string",                    "required",      "Property name on the data model (used for LINQ binding)."),
        new("Label",           "string",                    "required",      "Human-readable label shown in the field picker."),
        new("Type",            "FilterFieldType",           "Text",          "Field type. Controls default operators and value input widget."),
        new("Operators",       "IEnumerable&lt;FilterOperator&gt;?", "null", "Allowed operators. Auto-populated from Type when null."),
        new("DefaultOperator", "FilterOperator?",           "null",          "Operator pre-selected when a new condition is created."),
        new("Options",         "IEnumerable&lt;SelectOption&gt;?", "null",   "Options for <code>Select</code> / <code>MultiSelect</code> fields."),
        new("Placeholder",     "string?",                   "null",          "Placeholder text for the value input."),
        new("ChildContent",    "RenderFragment&lt;FilterCondition&gt;?", "null", "Custom value input. When provided, replaces the default widget."),
    ];
}
