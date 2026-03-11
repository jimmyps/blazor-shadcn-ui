namespace NeoUI.Demo.Shared.Pages.Components.Filter;

partial class AllFieldTypesExample
{
    private static readonly string _code = """
        <FilterBuilder TData="Employee"
                       @bind-Filters="activeFilters"
                       OnFilterChange="HandleFilterChange">
            <FilterFields>
                <FilterField Field="Name"       Label="Name"       Icon="user"        Type="FilterFieldType.Text" />
                <FilterField Field="Salary"     Label="Salary"     Icon="dollar-sign" Type="FilterFieldType.Number"
                             EditorType="FilterEditorType.Currency" Min="0" />
                <FilterField Field="HireDate"   Label="Hire Date"  Icon="calendar"    Type="FilterFieldType.Date" />
                <FilterField Field="Department" Label="Department" Icon="building-2"  Type="FilterFieldType.Select"
                             Options="@departmentOptions" />
                <FilterField Field="Skills"     Label="Skills"     Icon="zap"         Type="FilterFieldType.MultiSelect"
                             Options="@skillOptions" />
                <FilterField Field="IsActive"   Label="Active"     Icon="activity"    Type="FilterFieldType.Boolean" />
                <FilterField Field="Phone"      Label="Phone"      Icon="phone"       Type="FilterFieldType.Text"
                             EditorType="FilterEditorType.Masked" Placeholder="+1 (000) 000-0000" />
            </FilterFields>
        </FilterBuilder>
        """;
}
