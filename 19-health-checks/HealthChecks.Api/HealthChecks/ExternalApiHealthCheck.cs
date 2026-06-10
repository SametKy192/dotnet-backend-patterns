using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace HealthChecks.Api.HealthChecks;

/// <summary>
/// Dış API sağlık kontrolü — bağımlı servisleri test eder
/// </summary>
public class ExternalApiHealthCheck : IHealthCheck
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<ExternalApiHealthCheck> _logger;

    public ExternalApiHealthCheck(HttpClient httpClient, ILogger<ExternalApiHealthCheck> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // Dış API'ye ping at
            var response = await _httpClient.GetAsync(
                "https://httpstat.us/200",
                cancellationToken);

            if (response.IsSuccessStatusCode)
                return HealthCheckResult.Healthy("Dış API erişilebilir.");

            return HealthCheckResult.Degraded($"Dış API yavaş: {response.StatusCode}");
        }
        catch (Exception ex)
        {
            _logger.LogError("Dış API health check hatası: {Error}", ex.Message);
            return HealthCheckResult.Unhealthy("Dış API erişilemiyor.", ex);
        }
    }
}