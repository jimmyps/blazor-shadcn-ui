namespace NeoUI.Demo.Shared.Pages.Components.Filter;

partial class NestedGroupsExample
{
    private static readonly string _interactiveCode = """
        <FilterBuilder TData="Order"
                       @bind-Filters="activeFilters"
                       AllowGroups="true"
                       OnFilterChange="HandleFilterChange">
            <FilterFields>
                <FilterField Field="Status"   Label="Status"   Icon="activity"    Type="FilterFieldType.Select"  Options="@statusOptions" />
                <FilterField Field="Priority" Label="Priority" Icon="circle-alert" Type="FilterFieldType.Select" Options="@priorityOptions" />
                <FilterField Field="Amount"   Label="Amount"   Icon="dollar-sign" Type="FilterFieldType.Number"
                             EditorType="FilterEditorType.Currency" Min="0" />
                <FilterField Field="Customer" Label="Customer" Icon="user"        Type="FilterFieldType.Text" />
            </FilterFields>
        </FilterBuilder>
        """;

    private static readonly string _nestedCode = """
        // (Status = "Pending" OR Status = "Processing") AND Amount > 500
        var filters = new FilterGroup
        {
            Logic = LogicalOperator.And,
            Conditions =
            [
                new FilterCondition { Field = "Amount", Operator = FilterOperator.GreaterThan, Value = 500m }
            ],
            NestedGroups =
            [
                new FilterGroup
                {
                    Logic = LogicalOperator.Or,
                    Conditions =
                    [
                        new FilterCondition { Field = "Status", Operator = FilterOperator.Equals, Value = "Pending" },
                        new FilterCondition { Field = "Status", Operator = FilterOperator.Equals, Value = "Processing" }
                    ]
                }
            ]
        };

        // Pass to FilterBuilder via @bind-Filters or apply directly to a collection
        <FilterBuilder TData="Order"
                       @bind-Filters="filters"
                       AllowGroups="true"
                       OnFilterChange="HandleFilterChange">
            ...
        </FilterBuilder>
        """;

    private static readonly string _comboboxPickerCode = """
        <FilterBuilder TData="Employee"
                       @bind-Filters="employeeFilters"
                       AllowGroups="true"
                       FieldPickerVariant="FilterFieldPickerVariant.Combobox"
                       ComboboxSearchInterval="500"
                       OnFilterChange="HandleEmployeeFilterChange">
            <FilterFields>
                <FilterField Field="FirstName"  Label="First Name"  Icon="user"        Type="FilterFieldType.Text" />
                <FilterField Field="Department" Label="Department"  Icon="building-2"  Type="FilterFieldType.Select" Options="@departmentOptions" />
                <FilterField Field="Location"   Label="Location"    Icon="map-pin"     Type="FilterFieldType.Select" Options="@locationOptions" />
                <FilterField Field="Status"     Label="Status"      Icon="activity"    Type="FilterFieldType.Select" Options="@statusOptions" />
                <FilterField Field="Salary"     Label="Salary"      Icon="dollar-sign" Type="FilterFieldType.Number" EditorType="FilterEditorType.Currency" Min="0" />
                <FilterField Field="StartDate"  Label="Start Date"  Icon="calendar"    Type="FilterFieldType.Date" />
                @* ... 22 fields total — Combobox mode enables search for quick navigation *@
            </FilterFields>
        </FilterBuilder>
        """;

    private static readonly string _linqCode = """
        // Apply the nested filter group to any IEnumerable<T> or IQueryable<T>
        var results = allOrders.ApplyFilters(filters).ToList();
        """;
}
