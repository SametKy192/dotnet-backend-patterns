# dotnet-backend-patterns

Production-ready .NET backend patterns for modern applications.

## Patterns

| # | Pattern | Topics |
|---|---------|--------|
| 01 | [CQRS + MediatR](./01-cqrs-mediatr) | Pipeline behavior, validation, commands/queries |
| 02 | [Resilience with Polly](./02-resilience-polly) | Retry, circuit breaker, exponential backoff |
| 03 | [Outbox Pattern](./03-outbox-pattern) | Reliable messaging, eventual consistency |
| 04 | [Distributed Caching](./04-distributed-caching) | Redis, cache-aside, invalidation |
| 05 | [Multi-tenancy](./05-multitenancy) | Row-level filtering, tenant isolation |
| 06 | [API Versioning](./06-api-versioning) | URL versioning, backward compatibility |
| 07 | [Rate Limiting](./07-rate-limiting) | Fixed window, sliding window, token bucket |
| 08 | [Background Services](./08-background-service) | Producer-consumer, fire-and-forget |
| 09 | [SignalR Scalable](./09-signalr-scalable) | Real-time, Redis backplane, groups |
| 10 | [gRPC](./10-grpc-demo) | Protobuf, unary, server streaming |
| 11 | [Clean Architecture](./11-clean-architecture) | Layer separation, dependency inversion |
| 12 | [JWT Auth + Refresh Token](./12-jwt-auth) | BCrypt, token rotation, role-based auth |
| 13 | [Minimal API + OpenAPI](./13-minimal-api) | Endpoint grouping, filters, OpenAPI |
| 14 | [EF Core + PostgreSQL](./14-efcore-postgresql) | Migrations, relationships, seeding |
| 15 | [Unit Testing](./15-unit-testing) | xUnit, Moq, FluentAssertions |
| 16 | [Docker + docker-compose](./16-docker) | Containerization, multi-service setup |
| 17 | [Dapper](./17-dapper) | Raw SQL, stored procedures, performance |
| 18 | [Webhook Pattern](./18-webhook) | Event delivery, retry, signature validation |
| 19 | [Health Checks](./19-health-checks) | Endpoint monitoring, dashboard |
| 20 | [Event-Driven with MassTransit](./20-masstransit) | RabbitMQ, consumers, sagas |
| 21 | [Observability with OpenTelemetry](./21-observability-opentelemetry) | Distributed tracing, metrics, logs, Prometheus, Jaeger |
| 22 | [API Gateway with YARP](./22-api-gateway-yarp) | Reverse proxy, routing, load balancing |
| 23 | [Saga State Machine](./23-saga-state-machine) | MassTransit Saga, state transitions, orchestration |
| 24 | [Distributed Lock with Redis](./24-distributed-lock-redis) | RedLock, resource locking, concurrency protection |
| 25 | [Feature Flags](./25-feature-flags) | Microsoft.FeatureManagement, runtime toggle |
| 26 | [Pagination, Filtering & Sorting](./26-pagination-filtering-sorting) | Dynamic query filters, sort expression builders, page results |
| 27 | [Global Exception Handling](./27-global-exception-handling) | Custom exceptions, IExceptionHandler middleware |
| 28 | [Idempotency Pattern](./28-idempotency-pattern) | Idempotency middleware, requests replay caching |
| 29 | [Specification Pattern](./29-specification-pattern) | Reusable query business rules, combination (And/Or/Not) |
| 30 | [Options Pattern](./30-options-pattern) | Config validation, reload dynamic check, IOptionsMonitor |
| 31 | [Unit of Work Pattern](./31-unit-of-work-pattern) | Transaction context sharing, commit and rollback safety |
| 32 | [EF Core Interceptors (Auditing)](./32-efcore-interceptors) | Automatic entity auditing (timestamp & user tracking) using SaveChangesInterceptor |


## Requirements
- .NET 8 / 10 SDK
- Docker (for Redis, PostgreSQL, RabbitMQ, Jaeger)

## License
MIT