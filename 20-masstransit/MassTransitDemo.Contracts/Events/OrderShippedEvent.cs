namespace MassTransitDemo.Contracts.Events;

/// <summary>
/// Sipariş kargoya verildi event'i
/// </summary>
public record OrderShippedEvent
{
    public int OrderId { get; init; }
    public string TrackingNumber { get; init; } = string.Empty;
    public DateTime ShippedAt { get; init; } = DateTime.UtcNow;
}