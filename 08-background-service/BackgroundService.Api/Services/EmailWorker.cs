using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace BackgroundService.Api.Services;

/// <summary>
/// Background worker — sürekli çalışır, kuyruktaki emailleri işler.
/// Uygulama başlayınca otomatik başlar, kapanana kadar devam eder.
/// </summary>
public class EmailWorker : Microsoft.Extensions.Hosting.BackgroundService
{
    private readonly EmailQueueService _queue;
    private readonly ILogger<EmailWorker> _logger;

    public EmailWorker(EmailQueueService queue, ILogger<EmailWorker> logger)
    {
        _queue = queue;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("EmailWorker başladı");

        // Uygulama durduğunda döngüden çık
        while (!stoppingToken.IsCancellationRequested)
        {
            if (_queue.TryDequeue(out var email))
            {
                // Gerçek projede burası SMTP veya SendGrid olur
                _logger.LogInformation("Email gönderildi: {Email}", email);
                await Task.Delay(500, stoppingToken);
            }
            else
            {
                // Kuyruk boşsa 2 saniye bekle
                await Task.Delay(2000, stoppingToken);
            }
        }

        _logger.LogInformation("EmailWorker durdu");
    }
}