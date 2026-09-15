using StrategyPattern.Api.Models;

namespace StrategyPattern.Api.Strategies;

/// <summary>$5.00 base + $1.50/kg — 5-7 business days — domestic only.</summary>
public class StandardShippingStrategy : IShippingStrategy
{
    public ShippingMethod Method => ShippingMethod.Standard;

    public bool CanHandle(Order order) => !order.IsInternational;

    public ShippingQuote Calculate(Order order)
    {
        if (!CanHandle(order))
            return Unavailable(order, "Standard shipping is not available for international orders.");

        var cost = 5.00m + (order.WeightKg * 1.50m);
        return new ShippingQuote
        {
            OrderId           = order.Id,
            Method            = Method,
            MethodName        = "Standard Shipping",
            Cost              = Math.Round(cost, 2),
            EstimatedDelivery = "5–7 business days",
            Description       = $"Base $5.00 + $1.50/kg × {order.WeightKg}kg",
            IsAvailable       = true
        };
    }

    private static ShippingQuote Unavailable(Order order, string reason) => new()
    {
        OrderId = order.Id, Method = ShippingMethod.Standard,
        MethodName = "Standard Shipping", IsAvailable = false, UnavailableReason = reason
    };
}
