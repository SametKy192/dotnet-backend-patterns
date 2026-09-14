namespace StrategyPattern.Api.Models;

public record ShipOrderRequest(
    decimal        WeightKg,
    decimal        TotalValue,
    int            ItemCount,
    string         Destination,
    ShippingMethod Method = ShippingMethod.Standard
)
{
    public Order ToOrder() => new()
    {
        WeightKg    = WeightKg,
        TotalValue  = TotalValue,
        ItemCount   = ItemCount,
        Destination = Destination
    };
}
