# 40 — Event Sourcing

A .NET 10 implementation of the **Event Sourcing** pattern applied to a bank account domain. Instead of storing the current state of an entity, every state change is stored as an immutable domain event. The current state is derived by **replaying** these events.

## Core Idea

```
Traditional Approach (State-Stored):
┌──────────────────────────────────┐
│  accounts table                  │
│  id | balance | is_closed        │
│  1  | 850.00  | false   ← MUTABLE│
└──────────────────────────────────┘

Event Sourcing Approach:
┌──────────────────────────────────────────────────────────────────┐
│  event_store table                                               │
│  AccountOpenedEvent  → { accountId: "1", initialBalance: 1000 } │
│  MoneyWithdrawnEvent → { accountId: "1", amount: 200 }          │
│  MoneyDepositedEvent → { accountId: "1", amount: 50 }           │
│  ↑ IMMUTABLE — only appended, never updated or deleted          │
└──────────────────────────────────────────────────────────────────┘
Current balance = 1000 - 200 + 50 = 850 (computed by replaying events)
```

## Key Components

| Component | Role |
|---|---|
| `DomainEvents.cs` | Immutable records representing each state change |
| `BankAccount.cs` | Aggregate that raises events and rebuilds state via `Rehydrate()` |
| `InMemoryEventStore.cs` | Appends and loads events per aggregate (serialized as JSON) |
| `AccountProjection.cs` | Builds `AccountSummary` read model by replaying events |
| `AccountsController.cs` | REST API exposing open/deposit/withdraw/close/event log endpoints |

## Advantages of Event Sourcing

- **Complete Audit Log**: Every change is recorded — you know *what* happened, *when*, and in *what order*
- **Temporal Queries**: You can reconstruct the state at any point in time
- **Event Replay**: Rebuild read models or fix bugs by replaying past events against new projections
- **CQRS Compatibility**: Events naturally feed into separate read models (projections)

## Running the Project

```bash
cd EventSourcing.Api
dotnet run
```
- Swagger UI: `http://localhost:5040/swagger`
- Use `requests.http` to open accounts, deposit/withdraw funds, and inspect the raw event log.
