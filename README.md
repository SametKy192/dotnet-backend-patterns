# dotnet-backend-patterns

Production-ready .NET 8 backend patterns for modern applications.

## Patterns

| # | Pattern | Topics |
|---|---------|--------|
| 01 | [CQRS + MediatR](./01-cqrs-mediatr) | Pipeline behavior, validation, commands/queries |
| 02 | [Resilience with Polly](./02-resilience-polly) | Retry, circuit breaker, bulkhead |
| 03 | [Outbox Pattern](./03-outbox-pattern) | Reliable messaging, idempotency, PostgreSQL |
| 04 | [Distributed Caching](./04-distributed-caching) | Redis, cache-aside, invalidation |
| 05 | [Multi-tenancy](./05-multitenancy) | Row-level filtering, tenant isolation |
| 06 | [API Versioning](./06-api-versioning) | URL/header/query versioning |
| 07 | [Rate Limiting](./07-rate-limiting) | Sliding window, token bucket, per-user |
| 08 | [Background Services](./08-background-service) | IHostedService, job scheduling |
| 09 | [SignalR Scalable](./09-signalr-scalable) | Redis backplane, groups |
| 10 | [gRPC](./10-grpc-demo) | Protobuf, streaming, interceptor |

## Requirements
- .NET 8 SDK
- Docker (for Redis, PostgreSQL)

## License
MIT