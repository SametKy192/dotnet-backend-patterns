# 21 — Observability + OpenTelemetry

A .NET 8 implementation of full observability stack: structured logging with Serilog, distributed tracing with OpenTelemetry, and Jaeger integration.

## What You'll Learn
- Structured logging with Serilog
- Correlation ID for request tracking
- Distributed tracing with OpenTelemetry
- Manual span creation with ActivitySource
- Jaeger integration for trace visualization

## The Three Pillars of Observability

| Pillar | Tool | Purpose |
|--------|------|---------|
| Logs | Serilog | What happened |
| Traces | OpenTelemetry + Jaeger | Where it happened and how long |
| Metrics | Prometheus (future) | How often it happened |

## How It Works

### Structured Logging
```csharp
// Instead of: _logger.LogInformation("Order 123 created for Ahmet")
// Use: structured log with searchable fields
_logger.LogInformation("Order created: #{OrderId} — {CustomerName}", orderId, name);
```

### Distributed Tracing
```csharp
// Create a span — visible in Jaeger UI
using var activity = ActivitySource.StartActivity("GetOrderById");
activity?.SetTag("order.id", id);
activity?.SetTag("db.type", "postgresql");
```

### Correlation ID
Every request gets a unique ID that flows through all logs:
[10:30:01 INF] abc-123 HTTP GET /api/orders responded 200 in 45ms

[10:30:01 INF] abc-123 3 siparişler getirildi

## Start Jaeger

```bash
docker run -p 16686:16686 -p 4317:4317 jaegertracing/all-in-one
```

Jaeger UI: http://localhost:16686

## Project Structure
Observability.Api/

├── Controllers/

│   └── OrdersController.cs   ← Manual spans + structured logs

├── Program.cs                 ← Serilog + OpenTelemetry config

└── logs/                      ← Rolling log files

## Run

```bash
cd Observability.Api
dotnet run
```

## Endpoints

| Method | URL | Description |
|--------|-----|-------------|
| GET | /api/orders | List orders |
| GET | /api/orders/{id} | Get by id |
| POST | /api/orders | Create order |

## Packages Used

| Package | Purpose |
|---------|---------|
| Serilog.AspNetCore | Structured logging |
| Serilog.Sinks.Console | Console output |
| Serilog.Sinks.File | File output |
| OpenTelemetry.Extensions.Hosting | OTel host integration |
| OpenTelemetry.Instrumentation.AspNetCore | Auto HTTP tracing |
| OpenTelemetry.Exporter.Console | Console trace output |