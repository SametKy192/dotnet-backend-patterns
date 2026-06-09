
# =============================================
# 02-resilience-polly/README.md
# =============================================

# 02 — Resilience with Polly

A .NET 8 implementation of Retry and Circuit Breaker patterns using Microsoft.Extensions.Http.Resilience.

## What You'll Learn
- Retry pattern with exponential backoff and jitter
- Circuit Breaker pattern
- .NET 8 built-in resilience pipeline

## How It Works

### Retry
```
Request → Fail → Wait 1s → Retry → Fail → Wait 2s → Retry → Fail → Wait 4s → Final Fail
```

### Circuit Breaker
```
Too many failures → Circuit OPEN → Stop requests for 30s → Circuit HALF-OPEN → Test → CLOSED
```

## Project Structure

```
ResiliencePolly.Infrastructure/
└── Services/
    └── WeatherService.cs    ← Calls external API

ResiliencePolly.Api/
├── Controllers/
│   └── WeatherController.cs
└── Program.cs               ← Retry + Circuit Breaker pipeline
```

## Run

```bash
cd ResiliencePolly.Api
dotnet run
```

## Endpoints

| Method | URL | Description |
|--------|-----|-------------|
| GET | /api/weather | Retry + Circuit Breaker test |

## Packages Used

| Package | Purpose |
|---------|---------|
| Microsoft.Extensions.Http.Resilience | Retry, Circuit Breaker pipeline |

---