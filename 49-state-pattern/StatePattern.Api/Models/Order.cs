using StatePattern.Api.States;

namespace StatePattern.Api.Models;

/// <summary>Context — delegates behaviour to its current state object.</summary>
public class Order
{
    private readonly List<OrderHistoryEntry> _history = [];
    private IOrderState _state = new PendingState();

    public Guid Id { get; } = Guid.NewGuid();
    public string Customer { get; init; } = string.Empty;
    public decimal Amount { get; init; }
    public OrderStatus Status => _state.Status;
    public IReadOnlyList<OrderHistoryEntry> History => _history;

    public void Pay()     => _state.Pay(this);
    public void Ship()    => _state.Ship(this);
    public void Deliver() => _state.Deliver(this);
    public void Cancel()  => _state.Cancel(this);
    public void Refund()  => _state.Refund(this);

    internal void TransitionTo(IOrderState next, string action)
    {
        _history.Add(new OrderHistoryEntry(_history.Count + 1, action, _state.Status, next.Status, DateTime.UtcNow));
        _state = next;
    }
}
