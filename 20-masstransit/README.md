# 20 — Event-Driven Architecture with MassTransit

A .NET 8 event-driven implementation using MassTransit with RabbitMQ for async message processing.

## What You'll Learn
- Event-Driven Architecture fundamentals
- MassTransit message bus
- Publisher/Consumer pattern
- RabbitMQ as message broker
- Shared contracts between services

## How It Works
POST /api/orders

→ OrdersController publishes OrderCreatedEvent

→ MassTransit routes to RabbitMQ

→ OrderCreatedConsumer receives and processes

→ Logs: "Sipariş alındı: #1234"

## Why Event-Driven?
Without Events (tight coupling):

OrderService → calls → EmailService → calls → InventoryService
With Events (loose coupling):

OrderService → publishes → OrderCreatedEvent

↓

EmailConsumer (sends email)

InventoryConsumer (updates stock)

InvoiceConsumer (creates invoice)

## Start RabbitMQ

```bash
docker run -p 5672:5672 -p 15672:15672 rabbitmq:management
```

RabbitMQ UI: http://localhost:15672 (guest/guest)

## Project Structure
MassTransitDemo.Contracts/

└── Events/

├── OrderCreatedEvent.cs    ← Shared contract

└── OrderShippedEvent.cs
MassTransitDemo.Consumers/

└── Consumers/

├── OrderCreatedConsumer.cs ← Processes order events

└── OrderShippedConsumer.cs ← Processes shipping events
MassTransitDemo.Api/

└── Controllers/

└── OrdersController.cs    ← Publishes events

## Run

```bash
# Start RabbitMQ first
docker run -p 5672:5672 -p 15672:15672 rabbitmq:management

# Then API
cd MassTransitDemo.Api
dotnet run
```

## Endpoints

| Method | URL | Description |
|--------|-----|-------------|
| POST | /api/orders | Create order + publish event |
| POST | /api/orders/{id}/ship | Publish shipping event |

## Packages Used

| Package | Purpose |
|---------|---------|
| MassTransit | Message bus abstraction |
| MassTransit.RabbitMQ | RabbitMQ transport |
