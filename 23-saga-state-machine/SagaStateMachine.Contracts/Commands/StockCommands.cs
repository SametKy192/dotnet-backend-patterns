namespace SagaStateMachine.Contracts.Commands;

/// <summary>
/// Stok rezerve et komutu
/// </summary>
public record ReserveStock
{
    public Guid OrderId { get; init; }
    public int ProductId { get; init; }
    public int Quantity { get; init; }
}

/// <summary>
/// Stok rezervasyonunu geri al — compensating transaction
/// </summary>
public record ReleaseStock
{
    public Guid OrderId { get; init; }
    public string ReservationId { get; init; } = string.Empty;
}

/// <summary>
/// Ödeme al komutu
/// </summary>
public record ProcessPayment
{
    public Guid OrderId { get; init; }
    public decimal Amount { get; init; }
    public string CustomerName { get; init; } = string.Empty;
}