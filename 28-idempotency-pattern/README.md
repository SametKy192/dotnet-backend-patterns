# 28 — Idempotency Pattern

A .NET 8 implementation of the Idempotency pattern using middleware and in-memory cache to prevent duplicate operations.

## What You'll Learn
- What idempotency is and why it matters
- Idempotency-Key header standard
- Middleware-based idempotency implementation
- Duplicate request detection and response caching
- Real-world use cases (payments, orders)

## What is Idempotency?

An operation is idempotent if calling it multiple times produces the same result as calling it once.
GET, PUT, DELETE → naturally idempotent
POST → NOT idempotent by default → needs Idempotency-Key

## The Problem
User clicks "Pay" button
→ Request sent → network timeout → user clicks again
→ Two payments processed! ❌

## The Solution
User clicks "Pay" button
→ Request sent with Idempotency-Key: abc-123
→ Network timeout → user clicks again
→ Same Idempotency-Key: abc-123 sent
→ Cached response returned, payment NOT processed again ✅

## How It Works
POST /api/orders
Header: Idempotency-Key: abc-123
[IdempotencyMiddleware]
→ Key exists in cache? → return cached response
→ Key not in cache? → process request → cache response → return

## Response Headers
X-Idempotency-Key: abc-123
X-Idempotency-Replayed: true  ← only on cached responses

## Project Structure
IdempotencyPattern.Infrastructure/
├── Models/
│   ├── Order.cs
│   └── IdempotencyRecord.cs  ← Cached response store
└── Services/
└── IdempotencyService.cs ← Cache get/set
IdempotencyPattern.Api/
├── Middleware/
│   └── IdempotencyMiddleware.cs ← Intercepts POST requests
└── Controllers/
└── OrdersController.cs

## Real-World Use Cases

| Scenario | Without Idempotency | With Idempotency |
|----------|-------------------|-----------------|
| Payment | Double charge ❌ | One charge ✅ |
| Order creation | Duplicate orders ❌ | One order ✅ |
| Email sending | Multiple emails ❌ | One email ✅ |
| Coupon redemption | Used twice ❌ | Used once ✅ |

## Run

```bash
cd IdempotencyPattern.Api
dotnet run
```

## Endpoints

| Method | URL | Idempotent |
|--------|-----|-----------|
| POST | /api/orders | Yes (with header) |
| POST | /api/orders/{id}/payment | Yes (with header) |
| GET | /api/orders/{id} | Naturally |

## Packages Used

| Package | Purpose |
|---------|---------|
| IMemoryCache | Response caching (use Redis in production) |