namespace DapperDemo.Infrastructure.Models;

/// <summary>
/// Ürün modeli — Dapper otomatik map eder
/// Property isimleri SQL kolon isimleriyle eşleşmeli
/// </summary>
public class Product
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}