using StrategyPattern.Api.Models;

namespace StrategyPattern.Api.Strategies;

/// <summary>$35.00 base + $5.00/kg — next business day — domestic only.</summary>
public class OvernightShippingStrategy : IShippingStrategy
{
    public ShippingMethod Method => ShippingMethod.Overnight;

    public bool CanHandle(Order order) => !order.IsInternational;

    public ShippingQuote Calculate(Order order)
    {
        if (!CanHandle(order))
            return Unavailable(order, "Overnight shipping is not available for international orders.");

        var cost = 35.00m + (order.WeightKg * 5.00m);
        return new ShippingQuote
        {
            OrderId           = order.Id,
            Method            = Method,
            MethodName        = "Overnight Shipping",
            Cost              = Math.Round(cost, 2),
            EstimatedDelivery = "Next business day",
            Description       = $"Base $35.00 + $5.00/kg × {order.WeightKg}kg",
            IsAvailable       = true
        };
    }

    private static ShippingQuote Unavailable(Order order, string reason) => new()
    {
        OrderId = order.Id, Method = ShippingMethod.Overnight,
        MethodName = "Overnight Shipping", IsAvailable = false, UnavailableReason = reason
    };
}
