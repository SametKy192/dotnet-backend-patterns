using HealthChecks.Api.HealthChecks;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddHttpClient<ExternalApiHealthCheck>();

// Health check'leri kaydet
builder.Services.AddHealthChecks()
    // Custom health check'ler
    .AddCheck<DatabaseHealthCheck>("database",
        tags: new[] { "db", "critical" })
    .AddCheck<RedisHealthCheck>("redis",
        tags: new[] { "cache" })
    .AddCheck<ExternalApiHealthCheck>("external-api",
        tags: new[] { "external" })
    // Built-in memory check
    .AddCheck("memory", () =>
    {
        var allocated = GC.GetTotalMemory(false);
        var limit = 500 * 1024 * 1024; // 500MB

        return allocated < limit
            ? Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckResult.Healthy(
                $"Memory: {allocated / 1024 / 1024}MB")
            : Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckResult.Degraded(
                $"Memory high: {allocated / 1024 / 1024}MB");
    }, tags: new[] { "memory" });

// Health Checks UI
builder.Services.AddHealthChecksUI(opt =>
{
    opt.SetEvaluationTimeInSeconds(30); // 30 saniyede bir kontrol et
    opt.AddHealthCheckEndpoint("API", "/health");
}).AddInMemoryStorage();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

// Health check endpoint'leri
app.MapHealthChecks("/health", new HealthCheckOptions
{
    // Tüm check'lerin detaylı sonucunu döndür
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

// Sadece kritik check'ler
app.MapHealthChecks("/health/critical", new HealthCheckOptions
{
    Predicate = check => check.Tags.Contains("critical"),
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

// Health Checks UI dashboard
app.MapHealthChecksUI(options => options.UIPath = "/health-ui");

app.Run();