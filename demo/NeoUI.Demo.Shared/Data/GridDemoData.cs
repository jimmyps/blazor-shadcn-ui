namespace BlazorUI.Demo.Shared.Data;

/// <summary>
/// Order model for Grid component demos.
/// </summary>
public class Order
{
    public int Id { get; set; }
    public string Customer { get; set; } = "";
    public OrderStatus Status { get; set; }
    public decimal Amount { get; set; }
    public DateTime OrderDate { get; set; }
    public string ShipTo { get; set; } = "";
}

/// <summary>
/// Order status enumeration.
/// </summary>
public enum OrderStatus
{
    Pending,
    Processing,
    Shipped,
    Delivered,
    Cancelled
}

/// <summary>
/// Demo data generator for Grid component examples.
/// </summary>
public static class GridDemoData
{
    /// <summary>
    /// Generates a list of sample orders.
    /// </summary>
    /// <param name="count">Number of orders to generate.</param>
    /// <returns>List of generated orders.</returns>
    public static List<Order> GenerateOrders(int count = 1000)
    {
        var statuses = new[] 
        { 
            OrderStatus.Pending, 
            OrderStatus.Processing, 
            OrderStatus.Shipped, 
            OrderStatus.Delivered, 
            OrderStatus.Cancelled 
        };
        
        var customers = new[] 
        { 
            "Acme Corp", 
            "TechStart Inc", 
            "GlobalTrade LLC", 
            "FastShip Co", 
            "MegaMart",
            "DataFlow Systems",
            "CloudNine Solutions",
            "Vertex Industries",
            "PrimeLogistics",
            "EcoGoods Market"
        };
        
        var cities = new[] 
        { 
            "New York", 
            "Los Angeles", 
            "Chicago", 
            "Houston", 
            "Phoenix",
            "Philadelphia",
            "San Antonio",
            "San Diego",
            "Dallas",
            "San Jose"
        };
        
        var random = new Random(42); // Fixed seed for reproducibility
        
        return Enumerable.Range(1, count)
            .Select(i => new Order
            {
                Id = i,
                Customer = customers[random.Next(customers.Length)],
                Status = statuses[random.Next(statuses.Length)],
                Amount = Math.Round((decimal)(random.NextDouble() * 10000), 2),
                OrderDate = DateTime.Now.AddDays(-random.Next(365)),
                ShipTo = cities[random.Next(cities.Length)]
            })
            .ToList();
    }
}
