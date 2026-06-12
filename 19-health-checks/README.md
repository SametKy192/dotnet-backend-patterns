# 19 — Health Checks

A .NET 8 health monitoring implementation with custom health checks, tags, and a UI dashboard.

## What You'll Learn
- Custom IHealthCheck implementation
- Health check tags and filtering
- Built-in health checks (memory)
- Health Checks UI dashboard
- Kubernetes/Docker readiness and liveness probes

## Health Check Results

| Status | Meaning |
|--------|---------|
| Healthy | Everything works fine |
| Degraded | Working but with issues |
| Unhealthy | Not working |

## Endpoints

| URL | Description |
|-----|-------------|
| /health | All checks (JSON) |
| /health/critical | Critical checks only |
| /health-ui | Visual dashboard |
| /api/status | Custom formatted report |

## Custom Health Check
```csharp
public class DatabaseHealthCheck : IHealthCheck
{
    public async Task CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken)
    {
        // Check your DB connection
        return HealthCheckResult.Healthy("DB OK", data);
    }
}
```

## Tags for Filtering
```csharp
.AddCheck("database", tags: new[] { "critical" })

// Endpoint for specific tag
app.MapHealthChecks("/health/critical", new HealthCheckOptions
{
    Predicate = check => check.Tags.Contains("critical")
});
```

## Project Structure
HealthChecks.Api/

├── Controllers/

│   └── StatusController.cs

├── HealthChecks/

│   ├── DatabaseHealthCheck.cs

│   ├── RedisHealthCheck.cs

│   └── ExternalApiHealthCheck.cs

└── Program.cs

## Run
```bash
cd HealthChecks.Api
dotnet run
```

Then open http://localhost:5155/health-ui for the dashboard.

## Packages Used
| Package | Purpose |
|---------|---------|
| AspNetCore.HealthChecks.UI | Visual dashboard |
| AspNetCore.HealthChecks.UI.Client | JSON response writer |
| AspNetCore.HealthChecks.UI.InMemory.Storage | In-memory UI storage |