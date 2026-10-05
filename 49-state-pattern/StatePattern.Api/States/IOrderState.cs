using StatePattern.Api.Models;

namespace StatePattern.Api.States;

/// <summary>State interface — every action is either a valid transition or throws.</summary>
public interface IOrderState
{
    OrderStatus Status { get; }
    void Pay(Order order);
    void Ship(Order order);
    void Deliver(Order order);
    void Cancel(Order order);
    void Refund(Order order);
}
