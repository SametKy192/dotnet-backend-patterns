namespace WebhookPattern.Infrastructure.Models;

/// <summary>
/// Webhook aboneliği — hangi URL'e hangi event gönderileceğini tanımlar
/// </summary>
public class WebhookSubscription
{
    public int Id { get; set; }

    /// <summary>
    /// Event tipi — örn: "order.created", "payment.completed"
    /// </summary>
    public string EventType { get; set; } = string.Empty;

    /// <summary>
    /// Hedef URL — event gelince buraya POST atılır
    /// </summary>
    public string TargetUrl { get; set; } = string.Empty;

    /// <summary>
    /// İmza için gizli anahtar — payload doğrulamak için
    /// </summary>
    public string Secret { get; set; } = string.Empty;

    /// <summary>
    /// Aktif mi
    /// </summary>
    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}