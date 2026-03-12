namespace NeoUI.Demo.Shared.Pages.Components.Grid;

partial class Basic
{
    private const string _simpleGridCode = """
<Grid Items="@orders">
    <Columns>
        <DataGridColumn Field="Id" Header="Order ID" Sortable="true" Width="100px" />
        <DataGridColumn Field="Customer" Header="Customer" Sortable="true" />
        <DataGridColumn Field="Amount" Header="Amount" Sortable="true" Width="120px" />
    </Columns>
</Grid>
""";

    private const string _paginatedCode = """
<Grid Items="@orders" PagingMode="DataGridPagingMode.Client" PageSize="25">
    <Columns>
        <DataGridColumn Field="Id" Header="Order ID" Sortable="true" Width="100px" />
        <DataGridColumn Field="Customer" Header="Customer" Sortable="true" />
        <DataGridColumn Field="Status" Header="Status" Sortable="true" Width="120px" />
        <DataGridColumn Field="Amount" Header="Amount" Sortable="true" Width="120px" />
        <DataGridColumn Field="OrderDate" Header="Date" Sortable="true" Width="140px" />
    </Columns>
</Grid>
""";

    private const string _fixedHeightCode = """
<Grid Items="@orders" Height="400px" VirtualizationMode="DataGridVirtualizationMode.Auto">
    <Columns>
        <DataGridColumn Field="Id" Header="Order ID" Sortable="true" Width="100px" />
        <DataGridColumn Field="Customer" Header="Customer" Sortable="true" />
        <DataGridColumn Field="Status" Header="Status" Sortable="true" Width="120px" />
        <DataGridColumn Field="Amount" Header="Amount" Sortable="true" Width="120px" />
        <DataGridColumn Field="ShipTo" Header="Ship To" Sortable="true" Width="150px" />
    </Columns>
</Grid>
""";

    private const string _containerFillingCode = """
<div style="height: 600px;">
    <Grid Items="@orders" Height="100%" VirtualizationMode="DataGridVirtualizationMode.Auto">
        <Columns>
            <DataGridColumn Field="Id" Header="Order ID" Sortable="true" Width="100px" />
            <DataGridColumn Field="Customer" Header="Customer" Sortable="true" />
            <DataGridColumn Field="Status" Header="Status" Sortable="true" Width="120px" />
            <DataGridColumn Field="Amount" Header="Amount" Sortable="true" Width="120px" />
            <DataGridColumn Field="OrderDate" Header="Date" Sortable="true" Width="140px" />
        </Columns>
    </Grid>
</div>
""";

    private const string _visualStylesCode = """
<DataGrid Items="@orders" VisualStyle="DataGridStyle.Striped">
    <Columns>...</Columns>
</DataGrid>

<DataGrid Items="@orders" VisualStyle="DataGridStyle.Bordered">
    <Columns>...</Columns>
</DataGrid>

<DataGrid Items="@orders" VisualStyle="DataGridStyle.Minimal">
    <Columns>...</Columns>
</DataGrid>
""";

    private const string _densityCode = """
<DataGrid Items="@orders" Density="DataGridDensity.Compact">
    <Columns>...</Columns>
</DataGrid>

<DataGrid Items="@orders" Density="DataGridDensity.Medium">
    <Columns>...</Columns>
</DataGrid>

<DataGrid Items="@orders" Density="DataGridDensity.Spacious">
    <Columns>...</Columns>
</DataGrid>
""";
}
