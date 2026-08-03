# 37 — CQRS Read-Write Splitting (EF Core + Dapper)

A .NET 10 implementation of **CQRS (Command Query Responsibility Segregation) Read-Write Splitting**, separating the data models and database access technologies for read and write operations.

## What is CQRS?
CQRS stands for **Command Query Responsibility Segregation**. It is a pattern that segregates the responsibility of handling writes (Commands) from handling reads (Queries).

In enterprise applications:
*   **Commands (Writes)**: Focus on business rule enforcement, transactions, consistency, and complex validation. We use **Entity Framework Core (EF Core)** because of its rich change tracking, object-relational mapping, unit of work, and domain entity encapsulation.
*   **Queries (Reads)**: Focus on raw performance, data projection, and low latency. We use **Dapper** because it is a lightweight micro-ORM that runs raw SQL queries with negligible overhead and maps them straight to flat DTOs.

---

## Architectural Layout

```
                  ┌───────────────┐
                  │   Products    │
                  │  Controller   │
                  └───────┬───────┘
                          │
            ┌─────────────┴─────────────┐
            ▼                           ▼
      [ COMMANDS ]                 [ QUERIES ]
    (EF Core Writes)             (Dapper Reads)
    ┌───────────────┐            ┌──────────────┐
    │ Write Repo    │            │ Product      │
    │ (EF Core)     │            │ Queries      │
    └───────┬───────┘            └──────┬───────┘
            │                           │
            └─────────────┬─────────────┘
                          ▼
                  ┌───────────────┐
                  │ SQLite Database│
                  └───────────────┘
```

Both technologies share the exact same database connection in-memory to ensure transactions and writes are immediately readable.

---

## Running the Project

```bash
cd CqrsReadWriteSplitting.Api
dotnet run
```
- Swagger UI: `http://localhost:5037/swagger`
- Use `requests.http` to test commands and queries.
