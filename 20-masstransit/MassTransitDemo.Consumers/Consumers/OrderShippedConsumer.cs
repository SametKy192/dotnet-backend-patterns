using MassTransit;
using MassTransitDemo.Contracts.Events;
using Microsoft.Extensions.Logging;

namespace MassTransitDemo.Consumers.Consumers;

/// <summary>
/// OrderShippedEvent consumer'ı
/// </summary>
public class OrderShippedConsumer : IConsumer<OrderShippedEvent>
{
    private readonly ILogger<OrderShippedConsumer> _logger;

    public OrderShippedConsumer(ILogger<OrderShippedConsumer> logger)
    {
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<OrderShippedEvent> context)
    {
        var order = context.Message;

        _logger.LogInformation(
            "Kargo gönderildi: #{OrderId} — Takip: {Tracking}",
            order.OrderId,
            order.TrackingNumber);

        // Gerçek projede: müşteriye bildirim gönder
        await Task.Delay(50);

        _logger.LogInformation("Kargo bildirimi gönderildi: #{OrderId}", order.OrderId);
    }
}