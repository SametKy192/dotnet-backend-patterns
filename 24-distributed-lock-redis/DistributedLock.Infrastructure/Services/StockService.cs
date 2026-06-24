using DistributedLock.Infrastructure.Models;
using Microsoft.Extensions.Logging;
using RedLockNet;

namespace DistributedLock.Infrastructure.Services;

/// <summary>
/// Stok servisi — distributed lock ile race condition engellenir.
/// Birden fazla API replica aynı anda stok düşürmeye çalışsa bile
/// sadece biri işlem yapabilir.
/// </summary>
public class StockService
{
    private readonly IDistributedLockFactory _lockFactory;
    private readonly ILogger<StockService> _logger;

    /// <summary>
    /// In-memory ürün listesi — gerçek projede DB olur
    /// </summary>
    private static readonly Dictionary<int, Product> _products = new()
    {
        { 1, new Product { Id = 1, Name = "Laptop", Stock = 10 } },
        { 2, new Product { Id = 2, Name = "Mouse", Stock = 5 } }
    };

    /// <summary>
    /// Lock süresi — bu süre içinde işlem tamamlanmalı
    /// </summary>
    private static readonly TimeSpan LockExpiry = TimeSpan.FromSeconds(30);

    /// <summary>
    /// Lock almak için bekleme süresi
    /// </summary>
    private static readonly TimeSpan LockWait = TimeSpan.FromSeconds(10);

    /// <summary>
    /// Lock yenileme aralığı
    /// </summary>
    private static readonly TimeSpan LockRetry = TimeSpan.FromSeconds(1);

    public StockService(IDistributedLockFactory lockFactory, ILogger<StockService> logger)
    {
        _lockFactory = lockFactory;
        _logger = logger;
    }

    /// <summary>
    /// Stok düşür — distributed lock ile güvenli.
    /// Aynı anda sadece bir işlem stok düşürebilir.
    /// </summary>
    public async Task<bool> DecrementStockAsync(int productId, int quantity)
    {
        // Lock key — hangi kaynağı kilitlediğimizi belirtir
        var lockKey = $"stock:product:{productId}";

        _logger.LogInformation("Lock alınmaya çalışılıyor: {LockKey}", lockKey);

        // Redis'te distributed lock al
        await using var redLock = await _lockFactory.CreateLockAsync(
            lockKey,
            LockExpiry,
            LockWait,
            LockRetry);

        if (!redLock.IsAcquired)
        {
            // Lock alınamadı — başka bir işlem devam ediyor
            _logger.LogWarning("Lock alınamadı: {LockKey}", lockKey);
            return false;
        }

        _logger.LogInformation("Lock alındı: {LockKey}", lockKey);

        try
        {
            if (!_products.TryGetValue(productId, out var product))
            {
                _logger.LogWarning("Ürün bulunamadı: {ProductId}", productId);
                return false;
            }

            if (product.Stock < quantity)
            {
                _logger.LogWarning("Stok yetersiz: {ProductId}, Stock={Stock}, Requested={Qty}",
                    productId, product.Stock, quantity);
                return false;
            }

            // Kritik bölge — sadece lock sahibi buraya giriyor
            await Task.Delay(100); // DB işlemi simülasyonu
            product.Stock -= quantity;

            _logger.LogInformation("Stok düşürüldü: {ProductId}, Yeni Stok={Stock}",
                productId, product.Stock);

            return true;
        }
        finally
        {
            // Lock otomatik serbest bırakılır (using bloğu)
            _logger.LogInformation("Lock serbest bırakıldı: {LockKey}", lockKey);
        }
    }

    /// <summary>
    /// Lock olmadan stok düşür — race condition simülasyonu için
    /// </summary>
    public async Task<bool> DecrementStockUnsafeAsync(int productId, int quantity)
    {
        _logger.LogWarning("Lock OLMADAN stok düşürülüyor — race condition riski var!");

        if (!_products.TryGetValue(productId, out var product))
            return false;

        if (product.Stock < quantity)
            return false;

        // Race condition: iki thread aynı anda buraya girebilir
        await Task.Delay(100);
        product.Stock -= quantity;

        return true;
    }

    public IEnumerable<Product> GetProducts() => _products.Values;
}