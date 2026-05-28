namespace UnitTesting.Application.Models;

/// <summary>
/// Ürün modeli
/// </summary>
public class Product
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
}