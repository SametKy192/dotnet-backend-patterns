using Microsoft.AspNetCore.Http;

namespace MultiTenancy.Infrastructure.Services;

/// <summary>
/// Aktif tenant'ı HTTP header'dan okur.
/// Gerçek projede JWT claim'den veya subdomain'den okunur.
/// </summary>
public class TenantService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public TenantService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    /// <summary>
    /// X-Tenant-Id header'ından tenant'ı okur.
    /// Header yoksa query string'den "tenantId" parametresine bakar.
    /// İkisi de yoksa "default" döner.
    /// </summary>
    public string GetTenantId()
    {
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext == null)
        {
            return "default";
        }

        // 1. Header'dan oku
        if (httpContext.Request.Headers.TryGetValue("X-Tenant-Id", out var headerValue) &&
            !string.IsNullOrEmpty(headerValue.FirstOrDefault()))
        {
            return headerValue.FirstOrDefault()!;
        }

        // 2. Query string'den oku (fallback)
        if (httpContext.Request.Query.TryGetValue("tenantId", out var queryValue) &&
            !string.IsNullOrEmpty(queryValue.FirstOrDefault()))
        {
            return queryValue.FirstOrDefault()!;
        }

        return "default";
    }
}