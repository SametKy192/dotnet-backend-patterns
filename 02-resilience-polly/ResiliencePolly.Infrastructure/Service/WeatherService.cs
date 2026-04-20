namespace ResiliencePolly.Infrastructure.Services;

/// <summary>
/// Dış bir API'ye istek atan servis.
/// Resilience pattern'ı test etmek için kasıtlı 500 döndüren URL kullanıyoruz.
/// </summary>
public class WeatherService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<WeatherService> _logger;

    public WeatherService(HttpClient httpClient, ILogger<WeatherService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    /// <summary>
    /// Dış API'ye istek atar.
    /// 500 döndüren URL — retry policy bunu yakalayıp tekrar dener.
    /// </summary>
    public async Task<string> GetWeatherAsync()
    {
        _logger.LogInformation("Dış API'ye istek atılıyor...");
        var response = await _httpClient.GetAsync("https://httpstat.us/500");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsStringAsync();
    }
}