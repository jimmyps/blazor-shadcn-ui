namespace NeoUI.Demo.Shared.Pages.Components.Grid;

partial class Advanced
{
    private const string _flexColumnCode = """
<Grid Items="@orders">
    <Columns>
        <DataGridColumn Field="Id" Width="100px" />
        <DataGridColumn Field="Customer" MinWidth="150px" />
        <DataGridColumn Field="Status" Width="120px" />
        <DataGridColumn Field="OrderDate" MinWidth="150px" />
    </Columns>
</Grid>
""";
}
