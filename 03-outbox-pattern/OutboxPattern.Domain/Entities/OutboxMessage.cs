namespace OutboxPattern.Domain.Entities;

/// <summary>
/// Outbox mesajı — gönderilememiş event'leri veritabanında saklar.
/// Servis çökse bile mesaj kaybolmaz.
/// ProcessedAt null ise henüz işlenmemiş demektir.
/// </summary>
public class OutboxMessage
{
    public int Id { get; set; }

    /// <summary>
    /// Event tipi — hangi event olduğunu belirtir (örn: OrderCreated)
    /// </summary>
    public string EventType { get; set; } = string.Empty;

    /// <summary>
    /// Event verisi — JSON olarak saklanır
    /// </summary>
    public string Payload { get; set; } = string.Empty;

    /// <summary>
    /// Oluşturulma zamanı
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// İşlenme zamanı — null ise henüz işlenmemiş
    /// </summary>
    public DateTime? ProcessedAt { get; set; }
}