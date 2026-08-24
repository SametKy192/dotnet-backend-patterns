# 41 — Inbox Pattern

This project demonstrates the Inbox Pattern to guarantee idempotent and reliable processing of incoming messages/events on the consumer side.

## Why Inbox Pattern?

When receiving messages from a message broker (e.g. RabbitMQ, Kafka), there are scenarios where a message is delivered multiple times (at-least-once delivery guarantee).
The **Inbox Pattern** ensures that:
- Messages are persisted in an "inbox" table as soon as they are received.
- Each message is processed exactly once (de-duplication).
- If processing fails, it can be retried without losing the message.

## Mechanics

1. **Message Reception**: An API/Consumer endpoint receives an event, checks if `MessageId` is already in the `InboxMessages` table. If not, it saves the message structure with `ProcessedAt = null`.
2. **Background Processing**: A background worker (`InboxProcessor`) polls the inbox table for unprocessed messages, processes them, and flags them as processed.

## Project Structure

```
41-inbox-pattern/
├── InboxPattern.Infrastructure/
│   ├── Entities/
│   │   └── InboxMessage.cs
│   ├── Persistence/
│   │   └── InboxDbContext.cs
│   ├── Services/
│   │   └── InboxProcessor.cs
│   └── InboxPattern.Infrastructure.csproj
└── README.md
```
