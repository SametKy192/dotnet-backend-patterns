# 07 — Rate Limiting

A .NET 8 implementation of Fixed Window, Sliding Window, and Token Bucket rate limiting using built-in middleware.

## What You'll Learn
- Fixed Window, Sliding Window, Token Bucket differences
- .NET 8 built-in rate limiter
- Per-endpoint policy assignment

## Policy Comparison

| Policy | How It Works | When To Use |
|--------|-------------|-------------|
| Fixed Window | Window fills → reject → reset | Simple API protection |
| Sliding Window | Window slides → smoother | User-friendly API |
| Token Bucket | Tokens accumulate → allows burst | Upload, processing APIs |

## Run
```bash
cd RateLimiting.Api
dotnet run
```

## Endpoints
| Method | URL | Policy |
|--------|-----|--------|
| GET | /api/products | Fixed Window |
| GET | /api/products/search | Sliding Window |
| POST | /api/products | Token Bucket |
| GET | /api/products/health | No limit |

## Packages Used
| Package | Purpose |
|---------|---------|
| Built-in .NET 8 | Rate limiting middleware |