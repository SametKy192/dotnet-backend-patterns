
# =============================================
# 08-background-service/README.md
# =============================================

# 08 — Background Services

A .NET 8 implementation of the Producer-Consumer pattern using BackgroundService and ConcurrentQueue.

## What You'll Learn
- IHostedService vs BackgroundService
- Producer-Consumer pattern
- Fire-and-forget operations
- Thread-safe queue with ConcurrentQueue

## How It Works

```
POST /api/email
    → Add to EmailQueueService
    → Return 202 immediately (user doesn't wait)

EmailWorker (background)
    → Checks queue every 2s
    → If email exists → sends (log)
    → If queue empty → waits
```

## Why Use It?

Email, SMS, notifications should not block the user. Add to queue → return immediately → process in background.

## Project Structure

```
BackgroundService.Api/
├── Controllers/
│   └── EmailController.cs       ← Adds to queue
└── Services/
    ├── EmailQueueService.cs     ← Thread-safe queue
    └── EmailWorker.cs           ← Background worker
```

## Run

```bash
cd BackgroundService.Api
dotnet run
```

Watch the console for email send logs.

## Endpoints

| Method | URL | Description |
|--------|-----|-------------|
| POST | /api/email | Add email to queue |

## Packages Used

| Package | Purpose |
|---------|---------|
| Built-in .NET 8 | BackgroundService, ConcurrentQueue |

---