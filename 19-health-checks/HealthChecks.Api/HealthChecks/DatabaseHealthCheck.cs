using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace HealthChecks.Api.HealthChecks;

/// <summary>
/// Veritabanı sağlık kontrolü — DB bağlantısını test eder.
/// Gerçek projede connection string ile gerçek DB test edilir.
/// </summary>
public class DatabaseHealthCheck : IHealthCheck
{
    private readonly ILogger<DatabaseHealthCheck> _logger;

    public DatabaseHealthCheck(ILogger<DatabaseHealthCheck> logger)
    {
        _logger = logger;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // Gerçek projede: await _dbContext.Database.CanConnectAsync()
            // Simülasyon: rastgele başarı/başarısızlık
            await Task.Delay(10, cancellationToken);

            var isHealthy = DateTime.UtcNow.Second % 10 != 0; // %90 başarı

            if (isHealthy)
            {
                return HealthCheckResult.Healthy("Veritabanı bağlantısı başarılı.", new Dictionary<string, object>
                {
                    { "ResponseTime", "10ms" },
                    { "ConnectionPool", "5/20" }
                });
            }

            return HealthCheckResult.Degraded("Veritabanı yavaş yanıt veriyor.");
        }
        catch (Exception ex)
        {
            _logger.LogError("DB health check hatası: {Error}", ex.Message);
            return HealthCheckResult.Unhealthy("Veritabanına bağlanılamıyor.", ex);
        }
    }
}