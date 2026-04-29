using DistributedCaching.Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;

namespace DistributedCaching.Api.Controllers;

/// <summary>
/// Ürün controller'ı — Redis cache ile cache-aside pattern demo
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly ProductCacheService _cacheService;
    private readonly ILogger<ProductsController> _logger;

    public ProductsController(ProductCacheService cacheService, ILogger<ProductsController> logger)
    {
        _cacheService = cacheService;
        _logger = logger;
    }

    /// <summary>
    /// Cache-aside pattern ile ürün getirir.
    /// İlk istekte cache miss — DB'den alır, cache'e yazar.
    /// Sonraki isteklerde cache hit — DB'ye gitmez.
    /// GET /api/products/{id}
    /// </summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> Get(int id)
    {
        var product = await _cacheService.GetOrSetAsync(
            key: id.ToString(),
            factory: async () =>
            {
                // Gerçek projede burası DB sorgusu olur
                _logger.LogInformation("Cache miss — DB'den alınıyor: {Id}", id);
                await Task.Delay(100); // DB gecikmesini simüle et
                return new { Id = id, Name = $"Product {id}", Price = id * 10.0 };
            },
            expiration: TimeSpan.FromMinutes(2)
        );

        return Ok(product);
    }

    /// <summary>
    /// Cache'i temizler — ürün güncellenince çağrılır
    /// DELETE /api/products/{id}/cache
    /// </summary>
    [HttpDelete("{id}/cache")]
    public async Task<IActionResult> InvalidateCache(int id)
    {
        await _cacheService.RemoveAsync(id.ToString());
        _logger.LogInformation("Cache temizlendi: {Id}", id);
        return NoContent();
    }
}