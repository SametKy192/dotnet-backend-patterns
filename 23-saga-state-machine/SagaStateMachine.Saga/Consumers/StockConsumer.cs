using MassTransit;
using Microsoft.Extensions.Logging;
using SagaStateMachine.Contracts.Commands;
using SagaStateMachine.Contracts.Events;

namespace SagaStateMachine.Saga.Consumers;

/// <summary>
/// Stok servisi consumer'ı — ReserveStock komutunu işler.
/// Gerçek projede ayrı bir mikroservis olur.
/// </summary>
public class StockConsumer : IConsumer<ReserveStock>
{
    private readonly ILogger<StockConsumer> _logger;

    public StockConsumer(ILogger<StockConsumer> logger)
    {
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<ReserveStock> context)
    {
        var command = context.Message;
        _logger.LogInformation("Stok rezervasyonu: OrderId={OrderId}, Product={ProductId}, Qty={Qty}",
            command.OrderId, command.ProductId, command.Quantity);

        await Task.Delay(100);

        // Simülasyon: Product 999 için stok yetersiz
        if (command.ProductId == 999)
        {
            _logger.LogWarning("Stok yetersiz: ProductId={ProductId}", command.ProductId);
            await context.Publish(new StockReservationFailed
            {
                OrderId = command.OrderId,
                Reason = "Stok yetersiz"
            });
            return;
        }

        // Başarılı rezervasyon
        var reservationId = $"RES-{Guid.NewGuid():N}";
        _logger.LogInformation("Stok rezerve edildi: {ReservationId}", reservationId);

        await context.Publish(new StockReserved
        {
            OrderId = command.OrderId,
            ReservationId = reservationId
        });
    }
}

/// <summary>
/// Stok geri bırakma consumer'ı — compensating transaction
/// </summary>
public class ReleaseStockConsumer : IConsumer<ReleaseStock>
{
    private readonly ILogger<ReleaseStockConsumer> _logger;

    public ReleaseStockConsumer(ILogger<ReleaseStockConsumer> logger)
    {
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<ReleaseStock> context)
    {
        _logger.LogInformation("Stok geri bırakıldı: ReservationId={ReservationId}",
            context.Message.ReservationId);
        await Task.Delay(50);
    }
}