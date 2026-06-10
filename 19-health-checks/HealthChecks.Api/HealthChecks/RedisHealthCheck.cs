using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace HealthChecks.Api.HealthChecks;

/// <summary>
/// Redis sağlık kontrolü
/// </summary>
public class RedisHealthCheck : IHealthCheck
{
    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // Gerçek projede: await _redis.PingAsync()
            await Task.Delay(5, cancellationToken);

            return HealthCheckResult.Healthy("Redis bağlantısı başarılı.", new Dictionary<string, object>
            {
                { "ResponseTime", "5ms" },
                { "UsedMemory", "50MB" }
            });
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy("Redis'e bağlanılamıyor.", ex);
        }
    }
}