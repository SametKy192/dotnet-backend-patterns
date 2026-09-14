namespace StrategyPattern.Api.Models;

public enum ShippingMethod { Standard, Express, Overnight, Free, International }

public class ShippingQuote
{
    public Guid           OrderId           { get; init; }
    public ShippingMethod Method            { get; init; }
    public string         MethodName        { get; init; } = string.Empty;
    public decimal        Cost              { get; init; }
    public string         EstimatedDelivery { get; init; } = string.Empty;
    public string         Description       { get; init; } = string.Empty;
    public bool           IsAvailable       { get; init; }
    public string?        UnavailableReason { get; init; }
}
