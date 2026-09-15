using StrategyPattern.Api.Models;

namespace StrategyPattern.Api.Strategies;

/// <summary>$15.00 base + $2.50/kg — 2 business days — domestic only.</summary>
public class ExpressShippingStrategy : IShippingStrategy
{
    public ShippingMethod Method => ShippingMethod.Express;

    public bool CanHandle(Order order) => !order.IsInternational;

    public ShippingQuote Calculate(Order order)
    {
        if (!CanHandle(order))
            return Unavailable(order, "Express shipping is not available for international orders.");

        var cost = 15.00m + (order.WeightKg * 2.50m);
        return new ShippingQuote
        {
            OrderId           = order.Id,
            Method            = Method,
            MethodName        = "Express Shipping",
            Cost              = Math.Round(cost, 2),
            EstimatedDelivery = "2 business days",
            Description       = $"Base $15.00 + $2.50/kg × {order.WeightKg}kg",
            IsAvailable       = true
        };
    }

    private static ShippingQuote Unavailable(Order order, string reason) => new()
    {
        OrderId = order.Id, Method = ShippingMethod.Express,
        MethodName = "Express Shipping", IsAvailable = false, UnavailableReason = reason
    };
}
