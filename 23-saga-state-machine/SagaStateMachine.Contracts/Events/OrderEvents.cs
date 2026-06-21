namespace SagaStateMachine.Contracts.Events;

/// <summary>
/// Sipariş başlatıldı — saga'yı tetikler
/// </summary>
public record OrderSubmitted
{
    public Guid OrderId { get; init; }
    public string CustomerName { get; init; } = string.Empty;
    public decimal TotalAmount { get; init; }
    public int ProductId { get; init; }
    public int Quantity { get; init; }
}

/// <summary>
/// Stok rezerve edildi — saga'nın bir sonraki adımına geçer
/// </summary>
public record StockReserved
{
    public Guid OrderId { get; init; }
    public string ReservationId { get; init; } = string.Empty;
}

/// <summary>
/// Stok yetersiz — saga compensating transaction başlatır
/// </summary>
public record StockReservationFailed
{
    public Guid OrderId { get; init; }
    public string Reason { get; init; } = string.Empty;
}

/// <summary>
/// Ödeme tamamlandı
/// </summary>
public record PaymentCompleted
{
    public Guid OrderId { get; init; }
    public string PaymentId { get; init; } = string.Empty;
}

/// <summary>
/// Ödeme başarısız — stok rezervasyonu geri alınır
/// </summary>
public record PaymentFailed
{
    public Guid OrderId { get; init; }
    public string Reason { get; init; } = string.Empty;
}

/// <summary>
/// Sipariş tamamlandı
/// </summary>
public record OrderCompleted
{
    public Guid OrderId { get; init; }
}

/// <summary>
/// Sipariş iptal edildi — herhangi bir adımda hata olunca
/// </summary>
public record OrderCancelled
{
    public Guid OrderId { get; init; }
    public string Reason { get; init; } = string.Empty;
}