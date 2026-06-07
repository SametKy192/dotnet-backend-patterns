using Microsoft.AspNetCore.Mvc;
using WebhookPattern.Infrastructure.Services;

namespace WebhookPattern.Api.Controllers;

/// <summary>
/// Webhook controller — abonelik ve event tetikleme
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class WebhookController : ControllerBase
{
    private readonly WebhookService _webhookService;

    public WebhookController(WebhookService webhookService)
    {
        _webhookService = webhookService;
    }

    /// <summary>
    /// Webhook aboneliği oluştur
    /// POST /api/webhook/subscribe
    /// </summary>
    [HttpPost("subscribe")]
    public IActionResult Subscribe([FromBody] SubscribeRequest request)
    {
        var subscription = _webhookService.Subscribe(request.EventType, request.TargetUrl);
        return Ok(subscription);
    }

    /// <summary>
    /// Aboneliği iptal et
    /// DELETE /api/webhook/{id}
    /// </summary>
    [HttpDelete("{id}")]
    public IActionResult Unsubscribe(int id)
    {
        _webhookService.Unsubscribe(id);
        return NoContent();
    }

    /// <summary>
    /// Event tetikle — abonelere gönder
    /// POST /api/webhook/trigger
    /// </summary>
    [HttpPost("trigger")]
    public async Task<IActionResult> Trigger([FromBody] TriggerRequest request)
    {
        await _webhookService.TriggerAsync(request.EventType, request.Payload);
        return Ok(new { Message = "Event tetiklendi" });
    }

    /// <summary>
    /// Tüm abonelikleri listele
    /// GET /api/webhook/subscriptions
    /// </summary>
    [HttpGet("subscriptions")]
    public IActionResult GetSubscriptions()
    {
        return Ok(_webhookService.GetSubscriptions());
    }

    /// <summary>
    /// Gönderim loglarını listele
    /// GET /api/webhook/deliveries
    /// </summary>
    [HttpGet("deliveries")]
    public IActionResult GetDeliveries()
    {
        return Ok(_webhookService.GetDeliveries());
    }

    /// <summary>
    /// Test endpoint — webhook alıcısı simülasyonu
    /// POST /api/webhook/receiver
    /// </summary>
    [HttpPost("receiver")]
    public IActionResult Receive(
        [FromHeader(Name = "X-Webhook-Signature")] string? signature,
        [FromHeader(Name = "X-Webhook-Event")] string? eventType,
        [FromBody] object payload)
    {
        // Gerçek projede burada imza doğrulanır
        return Ok(new
        {
            Received = true,
            EventType = eventType,
            Signature = signature,
            Payload = payload
        });
    }
}

public record SubscribeRequest(string EventType, string TargetUrl);
public record TriggerRequest(string EventType, object Payload);