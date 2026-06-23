# 23 — Saga State Machine

A .NET 8 implementation of the Saga pattern using MassTransit State Machine for distributed workflow management with compensating transactions.

## What You'll Learn
- Saga pattern for distributed transactions
- MassTransit State Machine
- Compensating transactions (rollback)
- Event-driven state transitions
- In-memory saga repository

## The Problem
Order → Reserve Stock → Process Payment → Ship Order
What if Payment fails?

→ Stock must be released (compensating transaction)

→ Order must be cancelled

## State Machine Flow
[Initial]

→ OrderSubmitted

→ Send ReserveStock command

→ State: Submitted
[Submitted]

→ StockReserved

→ Send ProcessPayment command

→ State: PaymentProcessing

→ StockReservationFailed

→ Publish OrderCancelled

→ State: Cancelled ❌
[PaymentProcessing]

→ PaymentCompleted

→ Publish OrderCompleted

→ State: Completed ✅

→ PaymentFailed

→ Send ReleaseStock (compensating transaction)

→ Publish OrderCancelled

→ State: Cancelled ❌

## Test Scenarios

| Scenario | Input | Result |
|----------|-------|--------|
| Happy path | ProductId != 999, Amount <= 1000 | Completed ✅ |
| Stock failure | ProductId = 999 | Cancelled ❌ |
| Payment failure | Amount > 1000 | ReleaseStock + Cancelled ❌ |

## Project Structure
SagaStateMachine.Contracts/

├── Events/

│   └── OrderEvents.cs      ← All domain events

└── Commands/

└── StockCommands.cs    ← Commands between services
SagaStateMachine.Saga/

├── OrderSaga.cs            ← State machine definition

└── Consumers/

├── StockConsumer.cs    ← Handles stock commands

└── PaymentConsumer.cs  ← Handles payment commands
SagaStateMachine.Api/

└── Controllers/

└── OrdersController.cs ← Triggers the saga

## Run

```bash
cd SagaStateMachine.Api
dotnet run
```

Watch the console for state transitions.

## Endpoints

| Method | URL | Description |
|--------|-----|-------------|
| POST | /api/orders | Submit order, start saga |

## Packages Used

| Package | Purpose |
|---------|---------|
| MassTransit | Message bus + saga support |
| MassTransit InMemory | In-memory transport + repository |