namespace EFCorePostgreSQL.Domain.Entities;

/// <summary>
/// Etiket entity'si — ürünlerle many-to-many ilişki
/// </summary>
public class Tag
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Navigation property — many-to-many için
    /// </summary>
    public List<Product> Products { get; set; } = new();
}