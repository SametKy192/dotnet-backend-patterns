using MassTransit;
using SagaStateMachine.Contracts.Commands;
using SagaStateMachine.Contracts.Events;

namespace SagaStateMachine.Saga;

/// <summary>
/// Sipariş saga state'i — saga'nın mevcut durumunu saklar.
/// Her OrderId için bir instance oluşturulur.
/// </summary>
public class OrderSagaState : SagaStateMachineInstance
{
    /// <summary>
    /// MassTransit'in saga'yı tanımlaması için kullandığı Id
    /// </summary>
    public Guid CorrelationId { get; set; }

    /// <summary>
    /// Mevcut durum — Submitted, StockReserved, PaymentProcessing, Completed, Cancelled
    /// </summary>
    public string CurrentState { get; set; } = string.Empty;

    public string CustomerName { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public int ProductId { get; set; }
    public int Quantity { get; set; }

    /// <summary>
    /// Stok rezervasyon Id'si — geri alma işlemi için saklanır
    /// </summary>
    public string? StockReservationId { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public string? FailureReason { get; set; }
}

/// <summary>
/// Sipariş Saga State Machine.
/// Sipariş → Stok Rezervasyonu → Ödeme → Tamamlandı
/// Herhangi bir adımda hata olursa compensating transaction çalışır.
/// </summary>
public class OrderSagaStateMachine : MassTransitStateMachine<OrderSagaState>
{
    // Durumlar
    public State Submitted { get; private set; } = null!;
    public State StockReserved { get; private set; } = null!;
    public State PaymentProcessing { get; private set; } = null!;
    public State Completed { get; private set; } = null!;
    public State Cancelled { get; private set; } = null!;

    // Event'ler
    public Event<OrderSubmitted> OrderSubmitted { get; private set; } = null!;
    public Event<StockReserved> StockReserved_ { get; private set; } = null!;
    public Event<StockReservationFailed> StockReservationFailed { get; private set; } = null!;
    public Event<PaymentCompleted> PaymentCompleted { get; private set; } = null!;
    public Event<PaymentFailed> PaymentFailed { get; private set; } = null!;

    public OrderSagaStateMachine()
    {
        // Hangi property state'i saklıyor
        InstanceState(x => x.CurrentState);

        // Event'leri CorrelationId ile eşleştir
        Event(() => OrderSubmitted, x => x.CorrelateById(ctx => ctx.Message.OrderId));
        Event(() => StockReserved_, x => x.CorrelateById(ctx => ctx.Message.OrderId));
        Event(() => StockReservationFailed, x => x.CorrelateById(ctx => ctx.Message.OrderId));
        Event(() => PaymentCompleted, x => x.CorrelateById(ctx => ctx.Message.OrderId));
        Event(() => PaymentFailed, x => x.CorrelateById(ctx => ctx.Message.OrderId));

        // State Machine geçişleri
        Initially(
            When(OrderSubmitted)
                .Then(ctx =>
                {
                    // Saga state'ini güncelle
                    ctx.Saga.CustomerName = ctx.Message.CustomerName;
                    ctx.Saga.TotalAmount = ctx.Message.TotalAmount;
                    ctx.Saga.ProductId = ctx.Message.ProductId;
                    ctx.Saga.Quantity = ctx.Message.Quantity;
                    ctx.Saga.CreatedAt = DateTime.UtcNow;
                })
                // Stok rezervasyon komutu gönder
                .Send(ctx => new ReserveStock
                {
                    OrderId = ctx.Message.OrderId,
                    ProductId = ctx.Message.ProductId,
                    Quantity = ctx.Message.Quantity
                })
                .TransitionTo(Submitted));

        During(Submitted,
            // Stok rezerve edildi → ödeme al
            When(StockReserved_)
                .Then(ctx =>
                {
                    ctx.Saga.StockReservationId = ctx.Message.ReservationId;
                })
                .Send(ctx => new ProcessPayment
                {
                    OrderId = ctx.Saga.CorrelationId,
                    Amount = ctx.Saga.TotalAmount,
                    CustomerName = ctx.Saga.CustomerName
                })
                .TransitionTo(PaymentProcessing),

            // Stok yetersiz → iptal et
            When(StockReservationFailed)
                .Then(ctx =>
                {
                    ctx.Saga.FailureReason = ctx.Message.Reason;
                })
                .Publish(ctx => new OrderCancelled
                {
                    OrderId = ctx.Saga.CorrelationId,
                    Reason = ctx.Message.Reason
                })
                .TransitionTo(Cancelled));

        During(PaymentProcessing,
            // Ödeme başarılı → tamamlandı
            When(PaymentCompleted)
                .Then(ctx =>
                {
                    ctx.Saga.CompletedAt = DateTime.UtcNow;
                })
                .Publish(ctx => new OrderCompleted
                {
                    OrderId = ctx.Saga.CorrelationId
                })
                .TransitionTo(Completed),

            // Ödeme başarısız → stok rezervasyonunu geri al (compensating transaction)
            When(PaymentFailed)
                .Then(ctx =>
                {
                    ctx.Saga.FailureReason = ctx.Message.Reason;
                })
                // Compensating transaction — stok geri bırak
                .Send(ctx => new ReleaseStock
                {
                    OrderId = ctx.Saga.CorrelationId,
                    ReservationId = ctx.Saga.StockReservationId ?? string.Empty
                })
                .Publish(ctx => new OrderCancelled
                {
                    OrderId = ctx.Saga.CorrelationId,
                    Reason = ctx.Message.Reason
                })
                .TransitionTo(Cancelled));
    }
}