using BackgroundService.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace BackgroundService.Api.Controllers;

/// <summary>
/// Email controller — kuyruğa ekler, gönderme worker'a bırakılır.
/// Fire-and-forget: istek alındı, hemen 202 dön, arka planda işle.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class EmailController : ControllerBase
{
    private readonly EmailQueueService _queue;

    public EmailController(EmailQueueService queue)
    {
        _queue = queue;
    }

    /// <summary>
    /// Email kuyruğa ekler — hemen 202 döner, gönderme arka planda olur
    /// POST /api/email
    /// </summary>
    [HttpPost]
    public IActionResult Send([FromBody] string email)
    {
        // Kuyruğa ekle ve hemen dön — kullanıcıyı beklettirme
        _queue.Enqueue(email);

        // 202 Accepted — istek alındı, işleniyor
        return Accepted(new { Message = $"Email kuyruğa eklendi: {email}" });
    }
}