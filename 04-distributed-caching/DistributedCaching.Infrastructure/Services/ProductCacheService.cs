using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;

namespace DistributedCaching.Infrastructure.Services;

/// <summary>
/// Redis cache servis — cache-aside pattern uygular.
/// Önce cache'e bakar, yoksa kaynaktan alır ve cache'e yazar.
/// </summary>
public class ProductCacheService
{
    private readonly IDistributedCache _cache;

    /// <summary>
    /// Cache key prefix — diğer servislerle çakışmaması için
    /// </summary>
    private const string KeyPrefix = "product:";

    public ProductCacheService(IDistributedCache cache)
    {
        _cache = cache;
    }

    /// <summary>
    /// Cache-aside pattern:
    /// 1. Cache'e bak
    /// 2. Varsa cache'den döndür (cache hit)
    /// 3. Yoksa factory fonksiyonu çalıştır, sonucu cache'e yaz (cache miss)
    /// </summary>
    public async Task<T?> GetOrSetAsync<T>(
        string key,
        Func<Task<T>> factory,
        TimeSpan? expiration = null)
    {
        var fullKey = KeyPrefix + key;

        // Cache'e bak
        var cached = await _cache.GetStringAsync(fullKey);

        if (cached != null)
        {
            // Cache hit — deserialize edip döndür
            return JsonConvert.DeserializeObject<T>(cached);
        }

        // Cache miss — factory'den al
        var value = await factory();

        // Cache'e yaz
        var options = new DistributedCacheEntryOptions
        {
            // Belirtilmezse 5 dakika varsayılan
            AbsoluteExpirationRelativeToNow = expiration ?? TimeSpan.FromMinutes(5)
        };

        await _cache.SetStringAsync(fullKey, JsonConvert.SerializeObject(value), options);

        return value;
    }

    /// <summary>
    /// Cache'den sil — veri güncellenince çağrılır (cache invalidation)
    /// </summary>
    public async Task RemoveAsync(string key)
    {
        await _cache.RemoveAsync(KeyPrefix + key);
    }
}