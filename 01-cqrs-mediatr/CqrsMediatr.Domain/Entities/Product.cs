namespace CqrsMediatr.Domain.Entities;

/// <summary>
/// Ürün entity'si — veritabanındaki products tablosunu temsil eder
/// </summary>
public class Product
{
    /// <summary>
    /// Benzersiz kimlik — veritabanı tarafından otomatik atanır
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Ürün adı — boş olamaz
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Ürün fiyatı — negatif olamaz
    /// </summary>
    public decimal Price { get; set; }

    /// <summary>
    /// Oluşturulma tarihi — kayıt eklendiğinde otomatik set edilir
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}