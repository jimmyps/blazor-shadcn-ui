namespace NeoUI.Demo.Shared.Pages.Components.Grid;

partial class SortingFiltering
{
    private const string _columnSortingCode = """
<Grid Items="@orders">
    <Columns>
        <DataGridColumn Field="Id" Header="Order ID" Sortable="true" />
        <DataGridColumn Field="Customer" Header="Customer" Sortable="true" />
        <DataGridColumn Field="Amount" Header="Amount" Sortable="true" />
    </Columns>
</Grid>
""";

    private const string _controlledSortingCode = """
<Grid Items="@orders" @bind-State="@gridState">
    <Columns>
        <DataGridColumn Field="Amount" Sortable="true" />
    </Columns>
</Grid>

@code {
    private DataGridState gridState = new();

    private void SortByAmount()
    {
        gridState.SortDescriptors.Clear();
        gridState.SortDescriptors.Add(new DataGridSortDescriptor
        {
            Field = "Amount",
            Direction = DataGridSortDirection.Descending
        });
    }
}
""";

    private const string _columnFilteringCode = """
<Grid Items="@orders">
    <Columns>
        <DataGridColumn Field="Customer" Sortable="true" Filterable="true" />
    </Columns>
</Grid>
""";

    private const string _controlledFilteringCode = """
<Grid Items="@orders" @bind-State="@filterState" SuppressHeaderMenus="true">
    <Columns>
        <DataGridColumn Field="Status" />
    </Columns>
</Grid>

<Select @bind-Value="statusFilter">
    <SelectItem Value="@(-1)" TValue="int">All Statuses</SelectItem>
    <SelectItem Value="@((int)OrderStatus.Pending)" TValue="int">Pending</SelectItem>
    <!-- ... other status values ... -->
</Select>

@code {
    private DataGridState filterState = new();
    private int _statusFilter = -1;
    private int statusFilter
    {
        get => _statusFilter;
        set
        {
            if (_statusFilter != value)
            {
                _statusFilter = value;
                ApplyStatusFilter(value);
            }
        }
    }

    private void ApplyStatusFilter(int value)
    {
        filterState.FilterDescriptors.Clear();
        if (value >= 0)
        {
            filterState.FilterDescriptors.Add(new DataGridFilterDescriptor
            {
                Field = "Status",
                Operator = DataGridFilterOperator.Equals,
                Value = value
            });
        }
    }
}
""";
}
