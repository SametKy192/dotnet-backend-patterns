using CleanArchitecture.Domain.Entities;
using CleanArchitecture.Domain.Interfaces;

namespace CleanArchitecture.Application.Products;

/// <summary>
/// Application servisi — use case'leri orkestre eder.
/// Domain entity'lerini kullanır, Infrastructure'ı tanımaz.
/// IProductRepository interface'i üzerinden çalışır — gerçek implementasyonu bilmez.
/// </summary>
public class ProductService
{
    private readonly IProductRepository _repository;

    public ProductService(IProductRepository repository)
    {
        _repository = repository;
    }

    /// <summary>
    /// Tüm ürünleri getirir
    /// </summary>
    public async Task<List<Product>> GetAllAsync()
    {
        return await _repository.GetAllAsync();
    }

    /// <summary>
    /// Id ile ürün getirir — bulunamazsa null döner
    /// </summary>
    public async Task<Product?> GetByIdAsync(int id)
    {
        return await _repository.GetByIdAsync(id);
    }

    /// <summary>
    /// Yeni ürün oluşturur — Domain factory method kullanır
    /// </summary>
    public async Task<Product> CreateAsync(string name, decimal price)
    {
        // Domain factory method — iş kuralları Domain'de
        var product = Product.Create(name, price);
        await _repository.AddAsync(product);
        return product;
    }

    /// <summary>
    /// Ürün günceller — Domain update method kullanır
    /// </summary>
    public async Task UpdateAsync(int id, string name, decimal price)
    {
        var product = await _repository.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"Ürün bulunamadı: {id}");

        // Domain metodu — iş kuralları Domain'de korunur
        product.Update(name, price);
        await _repository.UpdateAsync(product);
    }

    /// <summary>
    /// Ürün siler
    /// </summary>
    public async Task DeleteAsync(int id)
    {
        await _repository.DeleteAsync(id);
    }
}