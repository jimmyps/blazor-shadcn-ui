namespace NeoUI.Demo.Shared.Pages.Components.Grid;

partial class Transactions
{
    private const string _addTransactionCode = """
@code {
    private Grid<Order> addGrid;
    private List<Order> orders = new();

    private async Task AddSingleRow()
    {
        var newOrder = new Order { /* ... */ };
        orders.Add(newOrder);
        await addGrid.AddRowsAsync(newOrder);
    }

    private async Task AddMultipleRows()
    {
        var newOrders = GenerateOrders(5);
        orders.AddRange(newOrders);
        await addGrid.AddRowsAsync(newOrders.ToArray());
    }
}
""";

    private const string _updateTransactionCode = """
@code {
    private async Task UpdateRandomRow()
    {
        var order = updateOrders[Random.Shared.Next(updateOrders.Count)];
        order.Amount = Math.Round((decimal)(Random.Shared.NextDouble() * 10000 + 100), 2);
        order.Status = statuses[Random.Shared.Next(statuses.Length)];
        await updateGrid.UpdateRowsAsync(order);
    }

    private async Task UpdateAllPending()
    {
        var pendingOrders = updateOrders.Where(o => o.Status == OrderStatus.Pending).ToArray();
        foreach (var order in pendingOrders)
            order.Status = OrderStatus.Shipped;
        await updateGrid.UpdateRowsAsync(pendingOrders);
    }
}
""";

    private const string _removeTransactionCode = """
@code {
    private async Task RemoveSelected()
    {
        orders.RemoveAll(o => selectedOrders.Contains(o));
        await removeGrid.RemoveRowsAsync(selectedOrders.ToArray());
        selectedOrders = Array.Empty<Order>();
    }

    private async Task RemoveDelivered()
    {
        var delivered = orders.Where(o => o.Status == "Delivered").ToArray();
        orders.RemoveAll(o => delivered.Contains(o));
        await removeGrid.RemoveRowsAsync(delivered);
    }
}
""";

    private const string _combinedTransactionCode = """
@code {
    private async Task ProcessOrders()
    {
        // 1. Ship pending orders (UPDATE)
        var toShip = orders.Where(o => o.Status == "Pending").ToArray();
        foreach (var order in toShip)
            order.Status = "Shipped";

        // 2. Archive delivered orders (REMOVE)
        var toArchive = orders.Where(o => o.Status == "Delivered").ToArray();
        orders.RemoveAll(o => toArchive.Contains(o));

        // 3. Add new incoming orders (ADD)
        var newOrders = GenerateOrders(3);
        orders.AddRange(newOrders);

        await combinedGrid.UpdateRowsAsync(toShip);
        await combinedGrid.RemoveRowsAsync(toArchive);
        await combinedGrid.AddRowsAsync(newOrders.ToArray());
    }
}
""";
}
