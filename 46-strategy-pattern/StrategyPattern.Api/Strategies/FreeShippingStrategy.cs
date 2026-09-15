using StrategyPattern.Api.Models;

namespace StrategyPattern.Api.Strategies;

/// <summary>
/// $0.00 — 7-10 business days — domestic only.
/// Only available when the order total value is $50.00 or more.
/// </summary>
public class FreeShippingStrategy : IShippingStrategy
{
    private const decimal MinimumOrderValue = 50.00m;

    public ShippingMethod Method => ShippingMethod.Free;

    public bool CanHandle(Order order) =>
        !order.IsInternational && order.TotalValue >= MinimumOrderValue;

    public ShippingQuote Calculate(Order order)
    {
        if (order.IsInternational)
            return Unavailable(order, "Free shipping is only available for domestic orders.");

        if (order.TotalValue < MinimumOrderValue)
            return Unavailable(order,
                $"Free shipping requires a minimum order value of ${MinimumOrderValue:F2}. " +
                $"Current order: ${order.TotalValue:F2}.");

        return new ShippingQuote
        {
            OrderId           = order.Id,
            Method            = Method,
            MethodName        = "Free Shipping",
            Cost              = 0.00m,
            EstimatedDelivery = "7–10 business days",
            Description       = $"Free shipping on orders over ${MinimumOrderValue:F2}",
            IsAvailable       = true
        };
    }

    private static ShippingQuote Unavailable(Order order, string reason) => new()
    {
        OrderId = order.Id, Method = ShippingMethod.Free,
        MethodName = "Free Shipping", IsAvailable = false, UnavailableReason = reason
    };
}
