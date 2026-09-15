using StrategyPattern.Api.Models;

namespace StrategyPattern.Api.Strategies;

/// <summary>
/// $25.00 base + $4.00/kg — 10-14 business days — international only.
/// A 10% customs surcharge is applied when order value exceeds $1,000.
/// </summary>
public class InternationalShippingStrategy : IShippingStrategy
{
    private const decimal CustomsThreshold  = 1_000.00m;
    private const decimal CustomsSurcharge  = 0.10m;

    public ShippingMethod Method => ShippingMethod.International;

    public bool CanHandle(Order order) => order.IsInternational;

    public ShippingQuote Calculate(Order order)
    {
        if (!CanHandle(order))
            return Unavailable(order, "International shipping is only available for international orders.");

        var baseCost = 25.00m + (order.WeightKg * 4.00m);
        var customs  = order.TotalValue > CustomsThreshold ? baseCost * CustomsSurcharge : 0m;
        var total    = Math.Round(baseCost + customs, 2);

        var desc = customs > 0
            ? $"Base $25.00 + $4.00/kg × {order.WeightKg}kg + 10% customs surcharge (order value > ${CustomsThreshold:F0})"
            : $"Base $25.00 + $4.00/kg × {order.WeightKg}kg";

        return new ShippingQuote
        {
            OrderId           = order.Id,
            Method            = Method,
            MethodName        = "International Shipping",
            Cost              = total,
            EstimatedDelivery = "10–14 business days",
            Description       = desc,
            IsAvailable       = true
        };
    }

    private static ShippingQuote Unavailable(Order order, string reason) => new()
    {
        OrderId = order.Id, Method = ShippingMethod.International,
        MethodName = "International Shipping", IsAvailable = false, UnavailableReason = reason
    };
}
