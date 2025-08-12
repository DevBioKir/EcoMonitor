
List<Order> orders = new()
{
    new Order { Id = 1, Total = 150 },
    new Order { Id = 2, Total = 50 },
    new Order { Id = 3, Total = 300 }
};

foreach (int order in GetAllIdOrders(orders))
{
    Console.WriteLine(order);
}

IEnumerable<int> GetAllIdOrders(List<Order> orders)
{
    return orders
        .Where(o => o.Total > 100)
        .OrderByDescending(o => o.Total)
        .Select(o => o.Id)
        .ToList();
}

class Order { public int Id { get; set; } public decimal Total { get; set; } }


