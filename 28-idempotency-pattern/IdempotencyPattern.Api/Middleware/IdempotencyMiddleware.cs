using System.Text.Json;
using IdempotencyPattern.Infrastructure.Services;

namespace IdempotencyPattern.Api.Middleware;

/// <summary>
/// Idempotency middleware — her POST isteğini kontrol eder.
/// Idempotency-Key header'ı varsa:
///   - Daha önce işlendiyse → cached response döndür
///   - İlk kez geliyorsa → işle ve kaydet
/// </summary>
public class IdempotencyMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<IdempotencyMiddleware> _logger;

    /// <summary>
    /// Idempotency header adı — standart isim
    /// </summary>
    private const string IdempotencyKeyHeader = "Idempotency-Key";

    public IdempotencyMiddleware(RequestDelegate next, ILogger<IdempotencyMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, IdempotencyService idempotencyService)
    {
        // Sadece POST isteklerini kontrol et
        if (context.Request.Method != HttpMethods.Post)
        {
            await _next(context);
            return;
        }

        // Idempotency-Key header'ı var mı
        if (!context.Request.Headers.TryGetValue(IdempotencyKeyHeader, out var idempotencyKey))
        {
            // Header yoksa normal devam et
            await _next(context);
            return;
        }

        var key = idempotencyKey.ToString();

        // Daha önce işlendi mi
        if (idempotencyService.TryGetCachedResponse(key, out var cachedRecord) && cachedRecord != null)
        {
            _logger.LogInformation("Idempotency hit — cached response döndürülüyor: {Key}", key);

            // Cached response'u döndür
            context.Response.StatusCode = cachedRecord.StatusCode;
            context.Response.ContentType = "application/json";
            context.Response.Headers["X-Idempotency-Replayed"] = "true";
            context.Response.Headers["X-Idempotency-Key"] = key;

            await context.Response.WriteAsync(cachedRecord.ResponseBody);
            return;
        }

        // İlk kez geliyor — response'u yakala
        var originalBody = context.Response.Body;

        using var memoryStream = new MemoryStream();
        context.Response.Body = memoryStream;

        // İsteği işle
        await _next(context);

        // Response'u oku
        memoryStream.Seek(0, SeekOrigin.Begin);
        var responseBody = await new StreamReader(memoryStream).ReadToEndAsync();

        // Başarılıysa cache'e kaydet (2xx)
        if (context.Response.StatusCode >= 200 && context.Response.StatusCode < 300)
        {
            try
            {
                var responseObject = JsonSerializer.Deserialize<object>(responseBody);
                idempotencyService.SaveResponse(key, context.Response.StatusCode, responseObject!);
                _logger.LogInformation("Idempotency kaydedildi: {Key}", key);
            }
            catch
            {
                // JSON parse hatası olursa kaydetme
            }
        }

        // Response header'larını ekle
        context.Response.Headers["X-Idempotency-Key"] = key;

        // Orijinal body'ye yaz
        memoryStream.Seek(0, SeekOrigin.Begin);
        await memoryStream.CopyToAsync(originalBody);
        context.Response.Body = originalBody;
    }
}