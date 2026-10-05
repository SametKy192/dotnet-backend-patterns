using StatePattern.Api.Models;

namespace StatePattern.Api.States;

public sealed class PendingState : OrderStateBase
{
    public override OrderStatus Status => OrderStatus.Pending;
    public override void Pay(Order order)    => order.TransitionTo(new PaidState(), "pay");
    public override void Cancel(Order order) => order.TransitionTo(new CancelledState(), "cancel");
}

public sealed class PaidState : OrderStateBase
{
    public override OrderStatus Status => OrderStatus.Paid;
    public override void Ship(Order order)   => order.TransitionTo(new ShippedState(), "ship");
    public override void Cancel(Order order) => order.TransitionTo(new RefundedState(), "cancel");
    public override void Refund(Order order) => order.TransitionTo(new RefundedState(), "refund");
}

public sealed class ShippedState : OrderStateBase
{
    public override OrderStatus Status => OrderStatus.Shipped;
    public override void Deliver(Order order) => order.TransitionTo(new DeliveredState(), "deliver");
}

public sealed class DeliveredState : OrderStateBase
{
    public override OrderStatus Status => OrderStatus.Delivered;
    public override void Refund(Order order) => order.TransitionTo(new RefundedState(), "refund");
}

public sealed class CancelledState : OrderStateBase
{
    public override OrderStatus Status => OrderStatus.Cancelled;
}

public sealed class RefundedState : OrderStateBase
{
    public override OrderStatus Status => OrderStatus.Refunded;
}
