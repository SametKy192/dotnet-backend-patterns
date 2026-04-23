namespace OutboxPattern.Domain.Entities;

/// <summary>
/// Sipariş entity'si — veritabanındaki orders tablosunu temsil eder
/// </summary>
public class Order
{
    public int Id { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}