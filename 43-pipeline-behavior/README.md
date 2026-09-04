# 43 – MediatR Pipeline Behavior Pattern

Demonstrates how to layer **cross-cutting concerns** (logging, validation, caching) as
composable MediatR `IPipelineBehavior<TRequest, TResponse>` decorators, keeping handlers
focused on pure business logic.

## What's Inside

```
PipelineBehavior.Api/
├── Behaviors/
│   ├── LoggingBehavior.cs       # Logs every request with elapsed time & slow-request warnings
│   ├── ValidationBehavior.cs    # Runs FluentValidation before the handler
│   ├── CachingBehavior.cs       # In-memory cache for ICacheableRequest responses
│   └── ProductValidators.cs     # CreateProduct / UpdateProduct FluentValidation rules
├── Commands/
│   ├── ProductCommands.cs       # CreateProductCommand, UpdateProductCommand, DeleteProductCommand
│   └── ProductCommandHandlers.cs
├── Queries/
│   ├── ProductQueries.cs        # GetAllProductsQuery, GetProductByIdQuery (+ ICacheableRequest)
│   └── ProductQueryHandlers.cs
├── Endpoints/
│   └── ProductEndpoints.cs      # Minimal API route mapping
└── Models/
    ├── Product.cs
    └── ProductDto.cs
```

## Pipeline Order

```
Incoming Request
      │
      ▼
┌─────────────────────┐
│  LoggingBehavior    │  ← logs start / elapsed / slow warning
└─────────┬───────────┘
          │
          ▼
┌─────────────────────┐
│ ValidationBehavior  │  ← throws ValidationException on failure → 400 JSON
└─────────┬───────────┘
          │
          ▼
┌─────────────────────┐
│  CachingBehavior    │  ← short-circuits on HIT, stores response on MISS
└─────────┬───────────┘
          │
          ▼
┌─────────────────────┐
│    Handler          │  ← pure business logic
└─────────────────────┘
```

## Key Concepts

| Concept | File |
|---|---|
| `IPipelineBehavior<TRequest, TResponse>` | All three behavior files |
| `IValidator<T>` (FluentValidation) | `ProductValidators.cs` |
| `ICacheableRequest` marker interface | `CachingBehavior.cs` |
| DI registration order (outer → inner) | `Program.cs` |

## Running

```bash
cd 43-pipeline-behavior
dotnet run --project PipelineBehavior.Api
```

API base URL: `http://localhost:5000`

See [requests.http](./requests.http) for sample requests.

## Requirements

- .NET 10 SDK
- No Docker needed (in-memory store)
