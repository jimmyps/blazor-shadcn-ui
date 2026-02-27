namespace NeoUI.Demo.Shared.Pages.Components.Grid;

partial class Templating
{
    private const string _dataFormatCode = """
<DataGridColumn Field="Amount" Header="Amount" DataFormatString="C" />
<DataGridColumn Field="OrderDate" Header="Order Date" DataFormatString="d" />
<DataGridColumn Field="Id" Header="Order ID" DataFormatString="N0" />
""";

    private const string _cellTemplateCode = """
<DataGridColumn Field="Status" Header="Status">
    <CellTemplate Context="order">
        <Badge Variant="@GetStatusVariant(order.Status)">@order.Status</Badge>
    </CellTemplate>
</DataGridColumn>
""";

    private const string _headerTemplateCode = """
<DataGridColumn Field="Customer">
    <HeaderTemplate>
        <div class="flex items-center gap-2">
            <LucideIcon Name="user" Size="16" />
            <span>Customer</span>
        </div>
    </HeaderTemplate>
</DataGridColumn>
""";

    private const string _rowActionsCode = """
<Grid Items="@orders" ActionHost="this">
    <Columns>
        <DataGridColumn Id="actions" Header="Actions">
            <CellTemplate Context="order">
                <!-- ✅ Use Blazor components with data-action attributes -->
                <Button data-action="Edit">
                    <LucideIcon Name="pencil" />
                </Button>
                <Button data-action="Delete">
                    <LucideIcon Name="trash-2" />
                </Button>
            </CellTemplate>
        </DataGridColumn>
    </Columns>
</Grid>

@code {
    // ✅ Auto-discovery: Just add [DataGridAction] attribute!
    [DataGridAction]
    private void Edit(Order order) { Toasts.Show($"Editing order #{order.Id}"); }

    [DataGridAction]
    private void Delete(Order order) { Toasts.Error($"Deleting order #{order.Id}"); }
}
""";

    private const string _conditionalStylingCode = """
@code {
    private string GetAmountClass(decimal amount)
        => amount > 5000 ? "font-bold text-green-600" : "";
}
""";
}
