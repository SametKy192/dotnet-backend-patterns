using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OutboxPattern.Infrastructure.Persistence;

namespace OutboxPattern.Infrastructure.Services;

/// <summary>
/// Background service — her 5 saniyede bir işlenmemiş outbox mesajlarını işler.
/// Gerçek projede burası RabbitMQ, Kafka gibi message broker'a gönderir.
/// </summary>
public class OutboxProcessor : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<OutboxProcessor> _logger;

    public OutboxProcessor(IServiceProvider serviceProvider, ILogger<OutboxProcessor> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await ProcessOutboxMessagesAsync();
            // 5 saniye bekle, tekrar kontrol et
            await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
        }
    }

    private async Task ProcessOutboxMessagesAsync()
    {
        // Her işlemde yeni scope aç — DbContext scoped servis olduğu için
        using var scope = _serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        // İşlenmemiş mesajları getir
        var messages = await dbContext.OutboxMessages
            .Where(m => m.ProcessedAt == null)
            .ToListAsync();

        foreach (var message in messages)
        {
            // Gerçek projede burası message broker'a gönderir
            _logger.LogInformation(
                "Outbox mesajı işlendi: {EventType} — {Payload}",
                message.EventType,
                message.Payload);

            // İşlendi olarak işaretle
            message.ProcessedAt = DateTime.UtcNow;
        }

        await dbContext.SaveChangesAsync();
    }
}