using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using WebhookPattern.Infrastructure.Models;

namespace WebhookPattern.Infrastructure.Services;

/// <summary>
/// Webhook gönderim servisi.
/// Abonelere event gönderir, başarısız olursa retry yapar.
/// HMAC imzası ile güvenli gönderim sağlar.
/// </summary>
public class WebhookService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<WebhookService> _logger;

    // In-memory abonelik listesi — gerçek projede DB'den gelir
    private static readonly List<WebhookSubscription> _subscriptions = new();
    private static readonly List<WebhookDelivery> _deliveries = new();

    public WebhookService(HttpClient httpClient, ILogger<WebhookService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    /// <summary>
    /// Yeni webhook aboneliği ekle
    /// </summary>
    public WebhookSubscription Subscribe(string eventType, string targetUrl)
    {
        var subscription = new WebhookSubscription
        {
            Id = _subscriptions.Count + 1,
            EventType = eventType,
            TargetUrl = targetUrl,
            // Rastgele gizli anahtar üret
            Secret = Convert.ToBase64String(RandomNumberGenerator.GetBytes(32))
        };

        _subscriptions.Add(subscription);
        _logger.LogInformation("Webhook aboneliği eklendi: {EventType} → {Url}", eventType, targetUrl);
        return subscription;
    }

    /// <summary>
    /// Aboneliği iptal et
    /// </summary>
    public void Unsubscribe(int subscriptionId)
    {
        var subscription = _subscriptions.FirstOrDefault(s => s.Id == subscriptionId);
        if (subscription != null)
            subscription.IsActive = false;
    }

    /// <summary>
    /// Event tetikle — tüm aktif abonelere gönder
    /// </summary>
    public async Task TriggerAsync(string eventType, object payload)
    {
        // Bu event tipine abone olanları bul
        var subscribers = _subscriptions
            .Where(s => s.EventType == eventType && s.IsActive)
            .ToList();

        if (!subscribers.Any())
        {
            _logger.LogInformation("Abone yok: {EventType}", eventType);
            return;
        }

        var payloadJson = JsonSerializer.Serialize(new
        {
            EventType = eventType,
            Timestamp = DateTime.UtcNow,
            Data = payload
        });

        // Her aboneye gönder
        foreach (var subscription in subscribers)
        {
            await SendWithRetryAsync(subscription, payloadJson);
        }
    }

    /// <summary>
    /// Retry mekanizması — başarısız olursa 3 kez dener
    /// </summary>
    private async Task SendWithRetryAsync(WebhookSubscription subscription, string payload)
    {
        var delivery = new WebhookDelivery
        {
            Id = _deliveries.Count + 1,
            SubscriptionId = subscription.Id,
            EventType = subscription.EventType,
            Payload = payload
        };

        _deliveries.Add(delivery);

        // 3 kez dene
        for (int attempt = 1; attempt <= 3; attempt++)
        {
            delivery.AttemptCount = attempt;

            try
            {
                // HMAC imzası oluştur — alıcı doğrulayabilir
                var signature = ComputeHmacSignature(payload, subscription.Secret);

                var request = new HttpRequestMessage(HttpMethod.Post, subscription.TargetUrl)
                {
                    Content = new StringContent(payload, Encoding.UTF8, "application/json")
                };

                // İmzayı header'a ekle
                request.Headers.Add("X-Webhook-Signature", signature);
                request.Headers.Add("X-Webhook-Event", subscription.EventType);

                var response = await _httpClient.SendAsync(request);
                delivery.StatusCode = (int)response.StatusCode;

                if (response.IsSuccessStatusCode)
                {
                    delivery.IsSuccess = true;
                    _logger.LogInformation("Webhook gönderildi: {Url} → {Status}", 
                        subscription.TargetUrl, delivery.StatusCode);
                    return;
                }

                _logger.LogWarning("Webhook başarısız (deneme {Attempt}): {Status}", 
                    attempt, delivery.StatusCode);
            }
            catch (Exception ex)
            {
                _logger.LogError("Webhook hatası (deneme {Attempt}): {Error}", attempt, ex.Message);
            }

            // Sonraki denemeden önce bekle — exponential backoff
            if (attempt < 3)
                await Task.Delay(TimeSpan.FromSeconds(Math.Pow(2, attempt)));
        }

        _logger.LogError("Webhook tüm denemeler başarısız: {Url}", subscription.TargetUrl);
    }

    /// <summary>
    /// HMAC-SHA256 imzası — payload'ın değiştirilmediğini kanıtlar
    /// </summary>
    private static string ComputeHmacSignature(string payload, string secret)
    {
        var keyBytes = Encoding.UTF8.GetBytes(secret);
        var payloadBytes = Encoding.UTF8.GetBytes(payload);

        using var hmac = new HMACSHA256(keyBytes);
        var hash = hmac.ComputeHash(payloadBytes);
        return "sha256=" + Convert.ToHexString(hash).ToLower();
    }

    public List<WebhookSubscription> GetSubscriptions() => _subscriptions;
    public List<WebhookDelivery> GetDeliveries() => _deliveries;
}