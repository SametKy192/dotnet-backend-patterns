# 42 — Integration Testing with Testcontainers

This project demonstrates how to implement robust Integration Tests using **Testcontainers for .NET** and **WebApplicationFactory**.

## Why Testcontainers?

Traditionally, integration tests either run against a shared database (which is prone to state leakage and concurrency issues) or mock the database (which doesn't test real SQL queries, constraints, or migrations).
**Testcontainers** solves this by spinning up a lightweight Docker container of the actual database (e.g. PostgreSQL, MSSQL, Redis) on-demand for the duration of the tests.

## Key Features

- **Database Isolation**: Every test run gets a pristine database container.
- **Realistic Testing**: Runs migrations and tests real queries against a real PostgreSQL engine.
- **Fast Startup**: Docker containers boot up dynamically in seconds.

## Project Structure

```
42-integration-testing-testcontainers/
├── IntegrationTests.Tests/
│   ├── ProductApiTests.cs
│   └── IntegrationTests.Tests.csproj
└── README.md
```
