# 40 — DDD Domain Events Pattern

This pattern demonstrates how to model Domain Events within Domain entities and automatically dispatch them during the database save changes step using MediatR.

## Why Domain Events?

In Domain-Driven Design (DDD), domain events describe something important that happened in the domain that other parts of the system might care about (e.g., `OrderPlaced`, `ProductCreated`).
Using domain events helps keep the domain models pure, decoupled, and focused on business rules.

## Mechanics

1. **Entity captures events**: Aggregate Roots/Entities extend a base `Entity` class that holds a list of domain events.
2. **SaveChanges Hook**: When `AppDbContext.SaveChangesAsync()` is called, it queries all tracked entities with pending domain events.
3. **Dispatch**: Using MediatR, it dispatches these events to their respective handlers before committing the transaction.

## Project Structure

```
40-ddd-domain-events/
├── DomainEvents.Domain/
│   ├── Common/
│   │   ├── Entity.cs
│   │   └── IDomainEvent.cs
│   ├── Entities/
│   │   └── Product.cs
│   └── Events/
│       └── ProductCreatedEvent.cs
├── DomainEvents.Infrastructure/
│   └── Persistence/
│       └── AppDbContext.cs
└── README.md
```
