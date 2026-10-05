using StatePattern.Api.Models;

namespace StatePattern.Api.States;

/// <summary>Rejects every action by default; concrete states override only what they allow.</summary>
public abstract class OrderStateBase : IOrderState
{
    public abstract OrderStatus Status { get; }

    public virtual void Pay(Order order)     => throw new InvalidTransitionException("pay", Status);
    public virtual void Ship(Order order)    => throw new InvalidTransitionException("ship", Status);
    public virtual void Deliver(Order order) => throw new InvalidTransitionException("deliver", Status);
    public virtual void Cancel(Order order)  => throw new InvalidTransitionException("cancel", Status);
    public virtual void Refund(Order order)  => throw new InvalidTransitionException("refund", Status);
}
