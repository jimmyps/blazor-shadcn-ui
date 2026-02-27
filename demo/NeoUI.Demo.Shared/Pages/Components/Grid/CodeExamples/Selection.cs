namespace NeoUI.Demo.Shared.Pages.Components.Grid;

partial class Selection
{
    private const string _customIdFieldCode = """
// Model with custom ID field
public class Product
{
    public Guid ProductId { get; set; }  // ← Custom ID field
    public string Name { get; set; }
    public decimal Price { get; set; }
}

<Grid Items="@products" 
      SelectionMode="DataGridSelectionMode.Multiple"
      IdField="ProductId"
      @bind-SelectedItems="@selectedProducts">
    <Columns>
        <DataGridColumn Field="ProductId" Header="ID" />
        <DataGridColumn Field="Name" Header="Product" />
    </Columns>
</Grid>
""";

    private const string _singleSelectionCode = """
<Grid Items="@orders" 
      SelectionMode="DataGridSelectionMode.Single" 
      @bind-SelectedItems="@selectedOrders">
    <Columns>...</Columns>
</Grid>

@if (selectedOrders.Count > 0)
{
    var order = selectedOrders.First();
    <p>Selected: @order.Customer</p>
}
""";

    private const string _multiSelectionCode = """
<Grid Items="@orders" 
      SelectionMode="DataGridSelectionMode.Multiple" 
      @bind-SelectedItems="@selectedOrders">
    <Columns>...</Columns>
</Grid>

<Button OnClick="DeleteSelected">Delete Selected</Button>

@code {
    private void DeleteSelected()
    {
        orders.RemoveAll(o => selectedOrders.Contains(o));
        selectedOrders = Array.Empty<Order>();
    }
}
""";

    private const string _controlledSelectionCode = """
<Button OnClick="SelectHighValueOrders">Select High Value</Button>

<Grid Items="@orders" 
      SelectionMode="DataGridSelectionMode.Multiple"
      @bind-SelectedItems="@selectedItems">
    <Columns>...</Columns>
</Grid>

@code {
    private IReadOnlyCollection<Order> selectedItems = Array.Empty<Order>();

    private void SelectHighValueOrders()
    {
        selectedItems = orders.Where(o => o.Amount > 5000).ToList();
    }

    private void ClearSelection()
    {
        selectedItems = Array.Empty<Order>();
    }
}
""";

    private const string _trackedObservableCode = """
@using NeoUI.Blazor

@code {
    // Use TrackedObservableCollection for automatic delta updates
    private TrackedObservableCollection<Order> orders = new();
    private IReadOnlyCollection<Order> selectedOrders = Array.Empty<Order>();

    private void DeleteSelected()
    {
        foreach (var order in selectedOrders.ToList())
        {
            orders.Remove(order);  // ← Triggers CollectionChanged event
        }
        selectedOrders = Array.Empty<Order>();
    }

    private void UpdateOrder()
    {
        var order = orders[Random.Shared.Next(orders.Count)];
        order.Amount = Random.Shared.Next(100, 10000);
        orders.NotifyItemsChanged(order);  // ← Triggers ItemsChanged event → Grid.transaction
    }
}
""";
}
