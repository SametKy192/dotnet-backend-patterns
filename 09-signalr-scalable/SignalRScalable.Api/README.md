
# =============================================
# 09-signalr-scalable/README.md
# =============================================

# 09 — SignalR Scalable

A .NET 8 real-time notification system using SignalR with group management and Redis backplane support.

## What You'll Learn
- Real-time messaging with SignalR
- Group management
- Redis backplane for horizontal scaling
- Sending messages from outside the hub via IHubContext

## How It Works

```
Client → WebSocket → Connect to Hub → Join Group
REST API → IHubContext → Send to Group → Hub → Client receives
```

## Why Redis Backplane?

```
Server A ←→ Redis ←→ Server B
Client on A receives messages sent to Server B via Redis
Without Redis: A and B are isolated
```

## Start Redis

```bash
docker run -p 6379:6379 redis
```

## Project Structure

```
SignalRScalable.Api/
├── Controllers/
│   └── NotificationController.cs   ← Send via REST
└── Hubs/
    └── NotificationHub.cs          ← WebSocket endpoint
```

## Run

```bash
cd SignalRScalable.Api
dotnet run
```

## Endpoints

| Method | URL | Description |
|--------|-----|-------------|
| POST | /api/notification/broadcast | Send to all clients |
| POST | /api/notification/group/{name} | Send to group |
| WS | /hubs/notification | SignalR connection point |

## Packages Used

| Package | Purpose |
|---------|---------|
| SignalR.StackExchangeRedis | Redis backplane |

---