using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using SignalRScalable.Api.Hubs;

namespace SignalRScalable.Api.Controllers;

/// <summary>
/// REST API üzerinden SignalR mesajı gönderme.
/// Gerçek projede sipariş oluşunca, ödeme onaylanınca burası çağrılır.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class NotificationController : ControllerBase
{
    /// <summary>
    /// IHubContext — hub dışından controller veya servis üzerinden mesaj göndermek için
    /// </summary>
    private readonly IHubContext<NotificationHub> _hubContext;

    public NotificationController(IHubContext<NotificationHub> hubContext)
    {
        _hubContext = hubContext;
    }

    /// <summary>
    /// Tüm bağlı istemcilere mesaj gönder
    /// POST /api/notification/broadcast
    /// </summary>
    [HttpPost("broadcast")]
    public async Task<IActionResult> Broadcast([FromBody] string message)
    {
        await _hubContext.Clients.All.SendAsync("ReceiveMessage", message);
        return Ok(new { Message = "Broadcast gönderildi", Content = message });
    }

    /// <summary>
    /// Belirli bir gruba mesaj gönder
    /// POST /api/notification/group/{groupName}
    /// </summary>
    [HttpPost("group/{groupName}")]
    public async Task<IActionResult> SendToGroup(string groupName, [FromBody] string message)
    {
        await _hubContext.Clients.Group(groupName).SendAsync("ReceiveMessage", message);
        return Ok(new { Message = $"Gruba gönderildi: {groupName}", Content = message });
    }
}