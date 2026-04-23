using Microsoft.Extensions.Http.Resilience;
using ResiliencePolly.Infrastructure.Services;
using Microsoft.Extensions.Http.Resilience;
using Polly;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddHttpClient<WeatherService>()
    .AddResilienceHandler("weather-pipeline", pipeline =>
    {
        // Retry policy — başarısız istekleri 3 kez tekrarlar
        // Her denemede bekleme süresi katlanır (exponential backoff)
        pipeline.AddRetry(new HttpRetryStrategyOptions
        {
            MaxRetryAttempts = 3,
            Delay = TimeSpan.FromSeconds(1),
            BackoffType = DelayBackoffType.Exponential,
            // Jitter — aynı anda gelen isteklerin çakışmaması için rastgelelik
            UseJitter = true
        });

        // Circuit Breaker — çok fazla hata gelince devreyi açar
        // Açık devre: 30sn boyunca istek atmayı durdurur
        pipeline.AddCircuitBreaker(new HttpCircuitBreakerStrategyOptions
        {
            SamplingDuration = TimeSpan.FromSeconds(10),
            // %50 hata oranında devre açılır
            FailureRatio = 0.5,
            // En az 3 istek sonra değerlendirme yapar
            MinimumThroughput = 3,
            // 30sn devre açık kalır, sonra yarı açık duruma geçer
            BreakDuration = TimeSpan.FromSeconds(30)
        });
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();