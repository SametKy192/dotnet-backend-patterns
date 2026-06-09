
# =============================================
# 03-outbox-pattern/README.md
# =============================================

# 03 — Outbox Pattern

A reliable messaging implementation using the Transactional Outbox Pattern with EF Core and a background processor.

## What You'll Learn
- What the Outbox pattern is and why it's needed
- Saving events in the same transaction as business data
- Processing outbox messages with a background service
- Eventual consistency

## How It Works

```
POST /api/orders
    → Order + OutboxMessage saved in same transaction
    → OutboxProcessor runs every 5s
    → Finds unprocessed messages (ProcessedAt == null)
    → Processes them → sets ProcessedAt
```

## Why Outbox?

```
Without Outbox:
Order saved → Event send fails → Data inconsistency ❌

With Outbox:
Order + OutboxMessage in same transaction → Either both succeed or neither ✅
```

## Project Structure

```
OutboxPattern.Domain/
└── Entities/
    ├── Order.cs
    └── OutboxMessage.cs

OutboxPattern.Infrastructure/
├── Persistence/
│   └── AppDbContext.cs
└── Services/
    └── OutboxProcessor.cs   ← Background service

OutboxPattern.Api/
└── Controllers/
    └── OrdersController.cs
```

## Run

```bash
cd OutboxPattern.Api
dotnet run
```

## Endpoints

| Method | URL | Description |
|--------|-----|-------------|
| POST | /api/orders | Create order |
| GET | /api/orders | List all orders |
| GET | /api/orders/outbox | View outbox messages |

## Packages Used

| Package | Purpose |
|---------|---------|
| EF Core InMemory | In-memory database |
| Newtonsoft.Json | JSON serialization |

---