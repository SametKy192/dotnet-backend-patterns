namespace MultiTenancy.Domain.Entities;

/// <summary>
/// Tenant'a ait ürün entity'si.
/// Her tenant sadece kendi ürünlerini görür.
/// </summary>
public class Product : ITenantEntity
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }

    /// <summary>
    /// Hangi tenant'a ait — SaveChanges'da otomatik set edilir
    /// </summary>
    public string TenantId { get; set; } = string.Empty;
}