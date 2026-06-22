using MassTransit;
using Microsoft.Extensions.Logging;
using SagaStateMachine.Contracts.Commands;
using SagaStateMachine.Contracts.Events;

namespace SagaStateMachine.Saga.Consumers;

/// <summary>
/// Ödeme servisi consumer'ı — ProcessPayment komutunu işler.
/// </summary>
public class PaymentConsumer : IConsumer<ProcessPayment>
{
    private readonly ILogger<PaymentConsumer> _logger;

    public PaymentConsumer(ILogger<PaymentConsumer> logger)
    {
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<ProcessPayment> context)
    {
        var command = context.Message;
        _logger.LogInformation("Ödeme işleniyor: OrderId={OrderId}, Amount={Amount}",
            command.OrderId, command.Amount);

        await Task.Delay(150);

        // Simülasyon: 1000'den büyük tutarlar başarısız
        if (command.Amount > 1000)
        {
            _logger.LogWarning("Ödeme başarısız: Amount={Amount}", command.Amount);
            await context.Publish(new PaymentFailed
            {
                OrderId = command.OrderId,
                Reason = "Ödeme limiti aşıldı"
            });
            return;
        }

        var paymentId = $"PAY-{Guid.NewGuid():N}";
        _logger.LogInformation("Ödeme tamamlandı: {PaymentId}", paymentId);

        await context.Publish(new PaymentCompleted
        {
            OrderId = command.OrderId,
            PaymentId = paymentId
        });
    }
}