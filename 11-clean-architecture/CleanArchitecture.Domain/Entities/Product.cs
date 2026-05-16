namespace CleanArchitecture.Domain.Entities;

/// <summary>
/// Domain entity — iş kurallarını içerir, dış katmanlardan bağımsız.
/// Hiçbir pakete bağımlı değil, sadece C# class'ı.
/// </summary>
public class Product
{
    public int Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public decimal Price { get; private set; }
    public DateTime CreatedAt { get; private set; }

    /// <summary>
    /// Private constructor — factory method ile oluşturulur.
    /// Dışarıdan direkt new Product() yapılamaz, iş kuralları korunur.
    /// </summary>
    private Product() { }

    /// <summary>
    /// Factory method — geçerli bir Product oluşturur.
    /// İş kuralları burada uygulanır.
    /// </summary>
    public static Product Create(string name, decimal price)
    {
        // Domain'e ait iş kuralları burada — dış katmanlar bilmez
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Ürün adı boş olamaz.");

        if (price <= 0)
            throw new ArgumentException("Fiyat 0'dan büyük olmalıdır.");

        return new Product
        {
            Name = name,
            Price = price,
            CreatedAt = DateTime.UtcNow
        };
    }

    /// <summary>
    /// Ürün güncelleme — iş kuralları korunur
    /// </summary>
    public void Update(string name, decimal price)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Ürün adı boş olamaz.");

        if (price <= 0)
            throw new ArgumentException("Fiyat 0'dan büyük olmalıdır.");

        Name = name;
        Price = price;
    }
}