using System.Text.Json;
using IdempotencyPattern.Infrastructure.Models;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace IdempotencyPattern.Infrastructure.Services;

/// <summary>
/// Idempotency servisi — aynı isteğin tekrar geldiğinde
/// işlemi tekrar yapmadan önceki sonucu döndürür.
/// Gerçek projede Redis kullanılır — in-memory yerine.
/// </summary>
public class IdempotencyService
{
    private readonly IMemoryCache _cache;
    private readonly ILogger<IdempotencyService> _logger;

    /// <summary>
    /// Idempotency key'inin cache'de ne kadar kalacağı — 24 saat
    /// </summary>
    private static readonly TimeSpan CacheDuration = TimeSpan.FromHours(24);

    public IdempotencyService(IMemoryCache cache, ILogger<IdempotencyService> logger)
    {
        _cache = cache;
        _logger = logger;
    }

    /// <summary>
    /// Bu key daha önce işlendi mi kontrol et
    /// </summary>
    public bool TryGetCachedResponse(string key, out IdempotencyRecord? record)
    {
        return _cache.TryGetValue(key, out record);
    }

    /// <summary>
    /// İşlem sonucunu cache'e kaydet
    /// </summary>
    public void SaveResponse(string key, int statusCode, object responseBody)
    {
        var record = new IdempotencyRecord
        {
            Key = key,
            StatusCode = statusCode,
            ResponseBody = JsonSerializer.Serialize(responseBody),
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.Add(CacheDuration)
        };

        _cache.Set(key, record, CacheDuration);

        _logger.LogInformation("Idempotency kaydı oluşturuldu: {Key}", key);
    }
}