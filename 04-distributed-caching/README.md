

# =============================================
# 04-distributed-caching/README.md
# =============================================

# 04 — Distributed Caching with Redis

A .NET 8 implementation of the Cache-Aside pattern using Redis as a distributed cache.

## What You'll Learn
- Cache-Aside pattern
- Redis with IDistributedCache
- Cache invalidation strategy
- Cache hit vs cache miss

## How It Works

```
GET /api/products/1
    → Check cache
    → Cache miss → Get from DB → Write to cache → Return
    → Cache hit  → Return directly (no DB call)
```

## Start Redis

```bash
docker run -p 6379:6379 redis
```

## Project Structure

```
DistributedCaching.Infrastructure/
└── Services/
    └── ProductCacheService.cs   ← Cache-aside logic

DistributedCaching.Api/
├── Controllers/
│   └── ProductsController.cs
└── Program.cs                   ← Redis registration
```

## Run

```bash
cd DistributedCaching.Api
dotnet run
```

## Endpoints

| Method | URL | Description |
|--------|-----|-------------|
| GET | /api/products/{id} | Get product with cache-aside |
| DELETE | /api/products/{id}/cache | Invalidate cache |

## Packages Used

| Package | Purpose |
|---------|---------|
| StackExchangeRedis | Redis client |
| Newtonsoft.Json | JSON serialization |

---