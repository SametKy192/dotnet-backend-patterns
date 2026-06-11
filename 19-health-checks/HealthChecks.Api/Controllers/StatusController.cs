using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace HealthChecks.Api.Controllers;

/// <summary>
/// Sistem durumu controller'ı
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class StatusController : ControllerBase
{
    private readonly HealthCheckService _healthCheckService;

    public StatusController(HealthCheckService healthCheckService)
    {
        _healthCheckService = healthCheckService;
    }

    /// <summary>
    /// Tüm sağlık kontrollerini çalıştırır
    /// GET /api/status
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var report = await _healthCheckService.CheckHealthAsync();

        var result = new
        {
            Status = report.Status.ToString(),
            TotalDuration = report.TotalDuration.TotalMilliseconds + "ms",
            Checks = report.Entries.Select(e => new
            {
                Name = e.Key,
                Status = e.Value.Status.ToString(),
                Description = e.Value.Description,
                Duration = e.Value.Duration.TotalMilliseconds + "ms",
                Data = e.Value.Data
            })
        };

        var statusCode = report.Status == HealthStatus.Healthy ? 200 : 503;
        return StatusCode(statusCode, result);
    }
}