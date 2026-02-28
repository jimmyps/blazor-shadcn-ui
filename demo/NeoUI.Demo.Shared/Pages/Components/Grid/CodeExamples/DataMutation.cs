namespace NeoUI.Demo.Shared.Pages.Components.Grid;

partial class DataMutation
{
    private const string _naturalMutationCode = """
@code {
    private Grid<Order> mutationGrid;
    private TrackedObservableCollection<Order> mutationOrders = new();

    private void UpdateRandomOrder()
    {
        if (mutationOrders.Count == 0) return;

        var order = mutationOrders[Random.Shared.Next(mutationOrders.Count)];

        // Natural mutation
        order.Status = OrderStatus.Shipped;
        order.Amount = Math.Round((decimal)(Random.Shared.NextDouble() * 10000 + 100), 2);

        // Signal the change
        mutationOrders.NotifyItemsChanged(order);
    }

    private void UpdateMultipleOrders()
    {
        var ordersToUpdate = mutationOrders
            .OrderBy(x => Random.Shared.Next())
            .Take(3)
            .ToArray();

        // Batch mutations
        foreach (var order in ordersToUpdate)
        {
            order.Status = OrderStatus.Delivered;
            order.Amount *= 0.9m; // 10% discount
        }

        // Signal all changes at once
        mutationOrders.NotifyItemsChanged(ordersToUpdate);
    }
}
""";

    private const string _batchUpdatesCode = """
@code {
    private void ApplyDiscount()
    {
        var highValueOrders = batchOrders
            .Where(o => o.Amount > 5000)
            .ToArray();

        if (highValueOrders.Length == 0) return;

        foreach (var order in highValueOrders)
        {
            order.Amount *= 0.9m;
        }

        // Signal all changes in one batch
        batchOrders.NotifyItemsChanged(highValueOrders);
    }

    private void MarkOldOrdersDelivered()
    {
        var cutoffDate = DateTime.Now.AddDays(-15);
        var oldOrders = batchOrders
            .Where(o => o.OrderDate < cutoffDate && o.Status != OrderStatus.Delivered)
            .ToArray();

        if (oldOrders.Length == 0) return;

        foreach (var order in oldOrders)
        {
            order.Status = OrderStatus.Delivered;
        }

        batchOrders.NotifyItemsChanged(oldOrders);
    }
}
""";
}
