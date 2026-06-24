namespace DistributedLock.Infrastructure.Models;

/// <summary>
/// Ürün — stok yönetimi için
/// </summary>
public class Product
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Stok miktarı — concurrent erişimde race condition riski var
    /// </summary>
    public int Stock { get; set; }
}