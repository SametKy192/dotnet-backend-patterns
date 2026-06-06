namespace WebhookPattern.Infrastructure.Models;

/// <summary>
/// Webhook gönderim kaydı — her gönderim denemesi loglanır
/// </summary>
public class WebhookDelivery
{
    public int Id { get; set; }
    public int SubscriptionId { get; set; }
    public string EventType { get; set; } = string.Empty;
    public string Payload { get; set; } = string.Empty;

    /// <summary>
    /// HTTP status kodu — 200, 404, 500 vs.
    /// </summary>
    public int? StatusCode { get; set; }

    /// <summary>
    /// Başarılı mı
    /// </summary>
    public bool IsSuccess { get; set; }

    /// <summary>
    /// Kaç kez denendi
    /// </summary>
    public int AttemptCount { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}