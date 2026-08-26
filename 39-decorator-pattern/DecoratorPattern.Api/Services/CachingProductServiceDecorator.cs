using Microsoft.Extensions.Caching.Memory;

namespace DecoratorPattern.Api.Services;

/// <summary>
/// DECORATOR 1: Caching
/// Wraps IProductService and adds in-memory cache.
/// The inner service has no knowledge of caching.
/// </summary>
public class CachingProductServiceDecorator : IProductService
{
    private readonly IProductService _inner;
    private readonly IMemoryCache _cache;
    private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(5);

    public CachingProductServiceDecorator(IProductService inner, IMemoryCache cache)
    {
        _inner = inner;
        _cache = cache;
    }

    public async Task<ProductDto?> GetByIdAsync(int id)
    {
        var key = $"product:{id}";
        if (_cache.TryGetValue(key, out ProductDto? cached))
            return cached;

        var product = await _inner.GetByIdAsync(id);
        if (product != null)
            _cache.Set(key, product, CacheDuration);

        return product;
    }

    public async Task<IEnumerable<ProductDto>> GetAllAsync()
    {
        const string key = "products:all";
        if (_cache.TryGetValue(key, out IEnumerable<ProductDto>? cached))
            return cached!;

        var products = await _inner.GetAllAsync();
        _cache.Set(key, products, CacheDuration);
        return products;
    }

    public async Task<ProductDto> CreateAsync(CreateProductDto dto)
    {
        var result = await _inner.CreateAsync(dto);
        // Invalidate list cache on write
        _cache.Remove("products:all");
        return result;
    }
}
