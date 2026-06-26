# 24 — Distributed Lock with Redis

A .NET 8 implementation of distributed locking using RedLock algorithm to prevent race conditions in multi-replica environments.

## What You'll Learn
- What race conditions are and why they matter
- RedLock algorithm for distributed locking
- Redis as a distributed lock store
- Concurrency control in multi-replica APIs

## The Problem: Race Condition
Stock = 1 (only one item left)
Request A reads: Stock = 1 → OK, decrement

Request B reads: Stock = 1 → OK, decrement  ← reads before A writes!
Result: Stock = -1 ❌ (oversold!)

## The Solution: Distributed Lock
Request A → Acquires lock → reads Stock=1 → decrements → releases lock

Request B → Waits for lock → acquires lock → reads Stock=0 → fails ✅

## RedLock Algorithm

RedLock uses majority quorum across multiple Redis instances for safety:
- Lock acquired from N/2+1 instances → safe
- If Redis fails, lock expires automatically (no deadlock)

## When to Use

- Stock management (prevent overselling)
- Payment processing (prevent double charge)
- Coupon/voucher redemption (prevent double use)
- Any resource that must be accessed by one process at a time

## Project Structure
DistributedLock.Infrastructure/

├── Models/

│   └── Product.cs

└── Services/

└── StockService.cs    ← Safe + unsafe implementations
DistributedLock.Api/

└── Controllers/

└── StockController.cs ← Endpoints + race test

## Start Redis

```bash
docker run -p 6379:6379 redis
```

## Run

```bash
cd DistributedLock.Api
dotnet run
```

## Endpoints

| Method | URL | Description |
|--------|-----|-------------|
| GET | /api/stock | View all products + stock |
| POST | /api/stock/{id}/decrement | Safe decrement (with lock) |
| POST | /api/stock/{id}/decrement-unsafe | Unsafe (race condition demo) |
| POST | /api/stock/{id}/race-test | Send 5 concurrent requests |

## Packages Used

| Package | Purpose |
|---------|---------|
| StackExchange.Redis | Redis client |
| RedLock.net | RedLock distributed lock |