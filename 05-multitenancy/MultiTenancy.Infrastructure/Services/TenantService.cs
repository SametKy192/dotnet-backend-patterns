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
    /// Header yoksa "default" döner.
    /// </summary>
    public string GetTenantId()
    {
        return _httpContextAccessor.HttpContext?
            .Request.Headers["X-Tenant-Id"]
            .FirstOrDefault() ?? "default";
    }
}