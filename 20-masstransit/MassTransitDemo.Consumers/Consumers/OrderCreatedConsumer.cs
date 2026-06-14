using MassTransit;
using MassTransitDemo.Contracts.Events;
using Microsoft.Extensions.Logging;

namespace MassTransitDemo.Consumers.Consumers;

/// <summary>
/// OrderCreatedEvent consumer'ı — event gelince otomatik çalışır.
/// Publisher ve consumer birbirini tanımaz, sadece contract'ı bilir.
/// </summary>
public class OrderCreatedConsumer : IConsumer<OrderCreatedEvent>
{
    private readonly ILogger<OrderCreatedConsumer> _logger;

    public OrderCreatedConsumer(ILogger<OrderCreatedConsumer> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// OrderCreatedEvent gelince MassTransit bu metodu çağırır.
    /// Gerçek projede: email gönder, stok güncelle, fatura oluştur vs.
    /// </summary>
    public async Task Consume(ConsumeContext<OrderCreatedEvent> context)
    {
        var order = context.Message;

        _logger.LogInformation(
            "Sipariş alındı: #{OrderId} — {CustomerName} — {Amount:C}",
            order.OrderId,
            order.CustomerName,
            order.TotalAmount);

        // Gerçek projede burası DB işlemi, email vs. olur
        await Task.Delay(100);

        _logger.LogInformation("Sipariş işlendi: #{OrderId}", order.OrderId);
    }
}