namespace NeoUI.Demo.Shared.Pages.Components.Filter;

partial class PresetsExample
{
    private static readonly string _presetsCode = """
        <FilterBuilder TData="Order" @bind-Filters="activeFilters" OnFilterChange="HandleFilterChange"
                       PresetsVariant="FilterPresetsVariant.Tabs">
            <FilterFields>
                <FilterField Field="Status"   Label="Status"   Icon="activity"     Type="FilterFieldType.Select"  Options="@statusOptions" />
                <FilterField Field="Priority" Label="Priority" Icon="circle-alert" Type="FilterFieldType.Select"  Options="@priorityOptions" />
                <FilterField Field="Amount"   Label="Amount"   Icon="dollar-sign"  Type="FilterFieldType.Number"
                             EditorType="FilterEditorType.Currency" Min="0" />
                <FilterField Field="Customer" Label="Customer" Icon="user"         Type="FilterFieldType.Select"
                             EditorType="FilterEditorType.Combobox" Options="@customerOptions" />
            </FilterFields>
            <FilterPresets>
                <FilterPreset Name="High Priority"  Icon="circle-alert" Filters="@GetHighPriorityFilters()" />
                <FilterPreset Name="Pending Orders" Icon="clock"        Filters="@GetPendingFilters()" />
                <FilterPreset Name="Large Orders"   Icon="trending-up"  Filters="@GetLargeOrdersFilters()" />
            </FilterPresets>
        </FilterBuilder>
        """;
}
