# 49 — State Pattern

Demonstrates the **State** design pattern in ASP.NET Core — a behavioral pattern that lets an object change its behaviour when its internal state changes, replacing sprawling `if/switch` status checks with one class per state.

## Concept

An `Order` (the **Context**) never inspects its own status. Every action (`Pay`, `Ship`, ...) is delegated to the current `IOrderState`. Each concrete state decides which actions are legal and which state comes next; illegal actions throw `InvalidTransitionException`, which the API maps to `409 Conflict`.

```
Pending ──pay──▶ Paid ──ship──▶ Shipped ──deliver──▶ Delivered
   │               │                                      │
 cancel         cancel/refund                          refund
   ▼               ▼                                      ▼
Cancelled       Refunded ◀────────────────────────────────┘
```

`Cancelled` and `Refunded` are terminal states.

## Project Structure

```
49-state-pattern/
├── StatePattern.Api/
│   ├── Models/
│   │   ├── Order.cs                       # Context — delegates to current state
│   │   ├── OrderStatus.cs                 # Enum exposed to clients
│   │   ├── OrderHistoryEntry.cs           # Transition record
│   │   ├── InvalidTransitionException.cs  # Illegal action for current state
│   │   └── Requests.cs                    # Inbound DTOs
│   ├── States/
│   │   ├── IOrderState.cs                 # State interface
│   │   ├── OrderStateBase.cs              # Rejects everything by default
│   │   └── OrderStates.cs                 # Pending, Paid, Shipped, Delivered, Cancelled, Refunded
│   ├── Services/OrderRepository.cs        # In-memory store
│   ├── Controllers/OrdersController.cs    # 8 endpoints
│   └── Program.cs
└── StatePattern.Tests/
    └── StatePatternTests.cs               # 26 unit tests
```

## Allowed Transitions

| State     | Pay | Ship | Deliver | Cancel     | Refund   |
|-----------|-----|------|---------|------------|----------|
| Pending   | ✅ Paid | ❌ | ❌ | ✅ Cancelled | ❌ |
| Paid      | ❌ | ✅ Shipped | ❌ | ✅ Refunded | ✅ Refunded |
| Shipped   | ❌ | ❌ | ✅ Delivered | ❌ | ❌ |
| Delivered | ❌ | ❌ | ❌ | ❌ | ✅ Refunded |
| Cancelled | ❌ | ❌ | ❌ | ❌ | ❌ |
| Refunded  | ❌ | ❌ | ❌ | ❌ | ❌ |

## API Endpoints

| Method | Route                          | Description                       |
|--------|--------------------------------|-----------------------------------|
| POST   | `/api/orders`                  | Create an order (starts Pending)  |
| GET    | `/api/orders`                  | List all orders                   |
| GET    | `/api/orders/{id}`             | Get order by ID                   |
| GET    | `/api/orders/{id}/history`     | Transition history                |
| POST   | `/api/orders/{id}/pay`         | Pay                               |
| POST   | `/api/orders/{id}/ship`        | Ship                              |
| POST   | `/api/orders/{id}/deliver`     | Deliver                           |
| POST   | `/api/orders/{id}/cancel`      | Cancel                            |
| POST   | `/api/orders/{id}/refund`      | Refund                            |

Illegal transitions return `409 Conflict` with `{ "error": "...", "current": "Pending" }`.

## Running

```bash
dotnet run --project StatePattern.Api
dotnet test
```

## Benefits vs. Status Switches

| Concern        | `switch (status)` in service          | State Pattern                                  |
|----------------|---------------------------------------|------------------------------------------------|
| Adding a state | Edit every switch                     | Add one class                                  |
| Illegal moves  | Easy to forget a guard                | Rejected by default in the base class          |
| Testing        | Large matrix through one method       | Each state's rules tested in isolation         |
| Readability    | Transition rules scattered            | Rules live next to the state they belong to    |
