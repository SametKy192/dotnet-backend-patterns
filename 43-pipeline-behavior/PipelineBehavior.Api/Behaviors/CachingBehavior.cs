using MediatR;
using System.Collections.Concurrent;
using System.Text.Json;

namespace PipelineBehavior.Api.Behaviors;

/// <summary>
/// Simple in-memory caching pipeline behavior.
/// Caches responses for requests that implement <see cref="ICacheableRequest"/>.
/// </summary>
public sealed class CachingBehavior<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private static readonly ConcurrentDictionary<string, (TResponse Value, DateTime ExpiresAt)> _cache = new();

    private readonly ILogger<CachingBehavior<TRequest, TResponse>> _logger;

    public CachingBehavior(ILogger<CachingBehavior<TRequest, TResponse>> logger)
    {
        _logger = logger;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (request is not ICacheableRequest cacheable)
            return await next();

        var cacheKey = $"{typeof(TRequest).Name}:{cacheable.CacheKey}";

        if (_cache.TryGetValue(cacheKey, out var entry) && entry.ExpiresAt > DateTime.UtcNow)
        {
            _logger.LogInformation("[Cache] HIT  for key: {CacheKey}", cacheKey);
            return entry.Value;
        }

        _logger.LogInformation("[Cache] MISS for key: {CacheKey}", cacheKey);

        var response = await next();

        var expiresAt = DateTime.UtcNow.Add(cacheable.CacheDuration);
        _cache[cacheKey] = (response, expiresAt);

        return response;
    }
}

/// <summary>
/// Marker interface for MediatR requests whose responses should be cached.
/// </summary>
public interface ICacheableRequest
{
    string CacheKey { get; }
    TimeSpan CacheDuration { get; }
}
