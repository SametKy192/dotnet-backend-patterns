using System.Collections.Concurrent;
using StatePattern.Api.Models;

namespace StatePattern.Api.Services;

public class OrderRepository
{
    private readonly ConcurrentDictionary<Guid, Order> _orders = new();

    public Order Add(Order order) { _orders[order.Id] = order; return order; }
    public Order? Find(Guid id) => _orders.GetValueOrDefault(id);
    public IEnumerable<Order> All() => _orders.Values;
}
