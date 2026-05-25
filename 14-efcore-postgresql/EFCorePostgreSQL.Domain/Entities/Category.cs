namespace EFCorePostgreSQL.Domain.Entities;

/// <summary>
/// Kategori entity'si — ürünlerle one-to-many ilişki kurar
/// </summary>
public class Category
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Navigation property — bir kategorinin birden fazla ürünü olabilir
    /// </summary>
    public List<Product> Products { get; set; } = new();
}