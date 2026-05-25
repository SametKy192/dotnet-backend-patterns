namespace EFCorePostgreSQL.Domain.Entities;

/// <summary>
/// Ürün entity'si — Category ile many-to-one ilişki
/// </summary>
public class Product
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Foreign key — hangi kategoriye ait
    /// </summary>
    public int CategoryId { get; set; }

    /// <summary>
    /// Navigation property — ilgili kategoriyi yükler
    /// </summary>
    public Category Category { get; set; } = null!;

    /// <summary>
    /// Navigation property — ürünün etiketleri (many-to-many)
    /// </summary>
    public List<Tag> Tags { get; set; } = new();
}