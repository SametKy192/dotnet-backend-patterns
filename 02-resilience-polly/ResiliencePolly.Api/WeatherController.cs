using Microsoft.AspNetCore.Mvc;
using ResiliencePolly.Infrastructure.Services;

namespace ResiliencePolly.Api.Controllers;

/// <summary>
/// Resilience pattern test controller'ı.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class WeatherController : ControllerBase
{
    private readonly WeatherService _weatherService;

    public WeatherController(WeatherService weatherService)
    {
        _weatherService = weatherService;
    }

    /// <summary>
    /// Dış API'ye istek atar.
    /// Retry policy devredeyse başarısız istekleri otomatik tekrarlar.
    /// Circuit breaker açılırsa 503 döner.
    /// GET /api/weather
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> Get()
    {
        try
        {
            var result = await _weatherService.GetWeatherAsync();
            return Ok(result);
        }
        catch (Exception ex)
        {
            // Tüm retry'lar başarısız olursa veya circuit breaker açıksa buraya düşer
            return StatusCode(503, new { Error = ex.Message, Message = "Servis şu an kullanılamıyor" });
        }
    }
}