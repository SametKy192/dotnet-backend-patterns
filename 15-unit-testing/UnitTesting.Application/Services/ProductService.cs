using Microsoft.Extensions.Logging;
using UnitTesting.Application.Interfaces;
using UnitTesting.Application.Models;

namespace UnitTesting.Application.Services;

/// <summary>
/// Ürün servisi — test edilecek business logic burada
/// </summary>
public class ProductService
{
    private readonly IProductRepository _repository;
    private readonly ILogger<ProductService> _logger;

    public ProductService(IProductRepository repository, ILogger<ProductService> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    /// <summary>
    /// Tüm ürünleri getirir
    /// </summary>
    public async Task<List<Product>> GetAllAsync()
    {
        return await _repository.GetAllAsync();
    }

    /// <summary>
    /// Id ile ürün getirir — bulunamazsa exception fırlatır
    /// </summary>
    public async Task<Product> GetByIdAsync(int id)
    {
        var product = await _repository.GetByIdAsync(id);

        if (product == null)
        {
            _logger.LogWarning("Ürün bulunamadı: {Id}", id);
            throw new KeyNotFoundException($"Ürün bulunamadı: {id}");
        }

        return product;
    }

    /// <summary>
    /// Yeni ürün oluşturur — iş kuralları burada
    /// </summary>
    public async Task<Product> CreateAsync(string name, decimal price)
    {
        // İş kuralı: fiyat 0'dan büyük olmalı
        if (price <= 0)
            throw new ArgumentException("Fiyat 0'dan büyük olmalıdır.");

        // İş kuralı: name boş olamaz
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Ürün adı boş olamaz.");

        var product = new Product { Name = name, Price = price };
        return await _repository.AddAsync(product);
    }

    /// <summary>
    /// Ürün siler
    /// </summary>
    public async Task DeleteAsync(int id)
    {
        // Önce var mı kontrol et
        await GetByIdAsync(id);
        await _repository.DeleteAsync(id);
    }
}