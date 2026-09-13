# 45 — Chain of Responsibility Pattern

Demonstrates the **Chain of Responsibility** design pattern in ASP.NET Core — a behavioral pattern that passes requests along a chain of handlers, where each handler decides to process the request or delegate it to the next handler in the chain.

## Concept

Instead of embedding multiple validation and processing concerns in a single service, each concern is encapsulated in its own handler. Handlers are chained at startup and each one decides independently whether to handle the request or pass it along.

```
Request → SpamFilter → Authentication → Authorization → RateLimit → Logging → Support
              │               │               │              │           │          │
           [reject]       [reject]        [reject]       [reject]    [pass]    [process]
```

## Project Structure

```
45-chain-of-responsibility/
├── ChainOfResponsibility.Api/
│   ├── Handlers/
│   │   ├── ITicketHandler.cs         # Handler interface (SetNext + HandleAsync)
│   │   ├── BaseTicketHandler.cs      # Abstract base with PassToNextAsync + Reject helpers
│   │   ├── SpamFilterHandler.cs      # Rejects tickets with spam keywords
│   │   ├── AuthenticationHandler.cs  # Validates API key
│   │   ├── AuthorizationHandler.cs   # Checks category permissions per API key
│   │   ├── RateLimitHandler.cs       # Sliding-window rate limit (5 req/min per key)
│   │   ├── LoggingHandler.cs         # Logs ticket metadata, passes through
│   │   └── SupportHandler.cs         # Terminal handler — assigns ticket to a team
│   ├── Models/
│   │   ├── SupportTicket.cs          # Ticket entity + Priority/Category enums
│   │   ├── CreateTicketRequest.cs    # Inbound request DTO
│   │   └── TicketResult.cs           # Result including processing log
│   ├── Controllers/
│   │   └── TicketsController.cs      # POST /api/tickets — entry point
│   └── Program.cs                    # DI wiring — builds and registers the chain
└── ChainOfResponsibility.Tests/
    └── HandlerChainTests.cs          # 26 unit tests
```

## Key Abstractions

### `ITicketHandler`
```csharp
public interface ITicketHandler
{
    ITicketHandler? Next { get; }
    ITicketHandler SetNext(ITicketHandler handler);
    Task<TicketResult> HandleAsync(SupportTicket ticket);
}
```

### `BaseTicketHandler`
```csharp
public abstract class BaseTicketHandler : ITicketHandler
{
    // Fluent chaining: spam.SetNext(auth).SetNext(authz)...
    public ITicketHandler SetNext(ITicketHandler handler) { ... }

    // If no next handler → returns a rejection result
    protected Task<TicketResult> PassToNextAsync(SupportTicket ticket) { ... }

    // Helper to build a standardised rejection result
    protected static TicketResult Reject(SupportTicket ticket, string reason) { ... }
}
```

## Handler Responsibilities

| Handler               | Action on Failure                          | Action on Pass                   |
|-----------------------|--------------------------------------------|----------------------------------|
| `SpamFilterHandler`   | Reject — spam keywords detected            | Pass to `AuthenticationHandler`  |
| `AuthenticationHandler`| Reject — invalid/missing API key           | Pass to `AuthorizationHandler`   |
| `AuthorizationHandler`| Reject — category not allowed for this key | Pass to `RateLimitHandler`       |
| `RateLimitHandler`    | Reject — > 5 requests/min per key          | Pass to `LoggingHandler`         |
| `LoggingHandler`      | N/A                                         | Pass to `SupportHandler`         |
| `SupportHandler`      | N/A (terminal)                             | Return processed result + team   |

## API Key Permissions

| API Key            | Allowed Categories                                |
|--------------------|---------------------------------------------------|
| `key-admin-001`    | General, Technical, Billing, **Security**         |
| `key-user-002`     | General, Technical, Billing                       |
| `key-support-003`  | General, Technical                                |

## DI Wiring — `Program.cs`

The pipeline is built once and registered as a singleton `ITicketHandler`:

```csharp
builder.Services.AddSingleton<ITicketHandler>(sp =>
{
    var spam    = sp.GetRequiredService<SpamFilterHandler>();
    var auth    = sp.GetRequiredService<AuthenticationHandler>();
    // ...

    spam.SetNext(auth).SetNext(authz).SetNext(rate).SetNext(log).SetNext(support);
    return spam; // entry point
});
```

## API Endpoint

| Method | Route           | Description                             |
|--------|-----------------|-----------------------------------------|
| POST   | `/api/tickets`  | Submit a support ticket through the chain |

### Request Body

```json
{
  "title": "Cannot connect to database",
  "description": "Getting timeout errors on prod",
  "priority": 2,
  "category": 1,
  "requestedBy": "dev@example.com",
  "apiKey": "key-admin-001"
}
```

### Success Response (200 OK)

```json
{
  "ticketId": "3fa85f64-...",
  "isProcessed": true,
  "isRejected": false,
  "assignedTo": "Technical Team",
  "processingLog": [
    "[SpamFilter] Passed spam check.",
    "[Authentication] API key verified.",
    "[Authorization] Permission granted.",
    "[RateLimit] OK (1/5 this minute).",
    "[Logging] Ticket recorded at 2026-09-13T...",
    "[Support] Ticket assigned to Technical Team."
  ]
}
```

### Rejection Response (400 Bad Request)

```json
{
  "ticketId": "...",
  "isProcessed": false,
  "isRejected": true,
  "rejectionReason": "Invalid or missing API key.",
  "processingLog": [
    "[SpamFilter] Passed spam check.",
    "[Authentication] Verifying API key...",
    "[REJECTED] Invalid or missing API key."
  ]
}
```

## Running the API

```bash
dotnet run --project ChainOfResponsibility.Api
```

## Running Tests

```bash
dotnet test
# Başarılı! - Başarısız: 0, Başarılı: 26, Atlanan: 0
```

## Benefits

| Concern               | Without CoR                          | With CoR                                 |
|-----------------------|--------------------------------------|------------------------------------------|
| Adding a new check    | Modify the service class             | Add a new handler, insert in chain       |
| Testing each check    | Integration test the whole service   | Unit test each handler in isolation      |
| Order of checks       | Buried inside `if` statements        | Explicit and readable chain in `Program.cs` |
| Skipping a check      | Comment-out code                     | Remove handler from the chain            |
| Reusability           | Logic is coupled to domain           | Handlers are independently reusable      |
