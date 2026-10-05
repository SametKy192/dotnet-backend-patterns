namespace StatePattern.Api.Models;

public class InvalidTransitionException(string action, OrderStatus current)
    : InvalidOperationException($"Cannot '{action}' an order that is {current}.")
{
    public string Action { get; } = action;
    public OrderStatus Current { get; } = current;
}
