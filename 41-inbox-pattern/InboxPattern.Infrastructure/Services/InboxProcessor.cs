using InboxPattern.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace InboxPattern.Infrastructure.Services;

public class InboxProcessor : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<InboxProcessor> _logger;

    public InboxProcessor(IServiceProvider serviceProvider, ILogger<InboxProcessor> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await ProcessInboxMessagesAsync(stoppingToken);
            await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
        }
    }

    private async Task ProcessInboxMessagesAsync(CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<InboxDbContext>();

        var messages = await dbContext.InboxMessages
            .Where(m => m.ProcessedAt == null)
            .OrderBy(m => m.OccurredOn)
            .Take(20)
            .ToListAsync(cancellationToken);

        foreach (var message in messages)
        {
            try
            {
                _logger.LogInformation("Processing inbox message {MessageId} ({EventType})", message.Id, message.EventType);

                // Simulate handling of domain/event logic
                // Actual integration might call a mediator dispatch or direct handler
                await Task.Delay(100, cancellationToken); 

                message.ProcessedAt = DateTime.UtcNow;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to process inbox message {MessageId}", message.Id);
                message.ErrorMessage = ex.Message;
            }
        }

        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
