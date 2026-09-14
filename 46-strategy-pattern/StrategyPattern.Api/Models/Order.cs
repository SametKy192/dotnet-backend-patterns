namespace StrategyPattern.Api.Models;

public class Order
{
    public Guid    Id          { get; init; } = Guid.NewGuid();
    public decimal WeightKg   { get; init; }
    public decimal TotalValue { get; init; }
    public int     ItemCount  { get; init; }
    public string  Destination { get; init; } = "domestic";
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;

    public bool IsInternational =>
        string.Equals(Destination, "international", StringComparison.OrdinalIgnoreCase);
}
