namespace MassTransitDemo.Contracts.Events;

/// <summary>
/// Sipariş oluşturuldu event'i — publisher ve consumer arasında paylaşılan contract.
/// Her iki taraf da bu sınıfı bilir, birbirini tanımaz.
/// </summary>
public record OrderCreatedEvent
{
    /// <summary>Sipariş Id'si</summary>
    public int OrderId { get; init; }

    /// <summary>Müşteri adı</summary>
    public string CustomerName { get; init; } = string.Empty;

    /// <summary>Toplam tutar</summary>
    public decimal TotalAmount { get; init; }

    /// <summary>Event zamanı</summary>
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
}