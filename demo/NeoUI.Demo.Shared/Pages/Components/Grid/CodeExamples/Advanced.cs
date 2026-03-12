namespace NeoUI.Demo.Shared.Pages.Components.Grid;

partial class Advanced
{
    private const string _fillWidthCode = """
<DataGrid Items="@orders" FillWidth="true">
    <Columns>
        <DataGridColumn Field="Id"        Header="Order ID"   Width="90px" />
        <DataGridColumn Field="Customer"  Header="Customer"   Sortable="true" />
        <DataGridColumn Field="Status"    Header="Status"     Width="110px" />
        <DataGridColumn Field="Amount"    Header="Amount"     Width="110px" />
        <DataGridColumn Field="OrderDate" Header="Order Date" Sortable="true" />
        <DataGridColumn Field="ShipTo"    Header="Ship To"    Sortable="true" />
    </Columns>
</DataGrid>
""";

    private const string _flexProportionalCode = """
<DataGrid Items="@orders" FillWidth="true">
    <Columns>
        <DataGridColumn Field="Id"        Header="Order ID"   Width="90px" />
        <DataGridColumn Field="Customer"  Header="Customer"   Flex="3" Sortable="true" />
        <DataGridColumn Field="Status"    Header="Status"     Flex="1" />
        <DataGridColumn Field="Amount"    Header="Amount"     Width="110px" />
        <DataGridColumn Field="OrderDate" Header="Order Date" Flex="2" Sortable="true" />
    </Columns>
</DataGrid>
""";

    private const string _autoSizeCode = """
<DataGrid @ref="_grid" Items="@orders" AutoSizeColumns="true">
    <Columns>
        <DataGridColumn Field="Id"        Header="#"          Sortable="true" />
        <DataGridColumn Field="Customer"  Header="Customer"   Sortable="true" />
        <DataGridColumn Field="Status"    Header="Status"     Sortable="true" />
        <DataGridColumn Field="Amount"    Header="Amount ($)" Sortable="true" />
        <DataGridColumn Field="OrderDate" Header="Date"       Sortable="true" />
    </Columns>
</DataGrid>

@code {
    private DataGrid<Order>? _grid;

    // On-demand resize from Blazor code:
    private async Task ManualAutoSize()
        => await _grid!.AutoSizeColumnsAsync();
}
""";
}
