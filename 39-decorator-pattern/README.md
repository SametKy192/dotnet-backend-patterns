# 39 — Decorator Pattern in ASP.NET Core

A .NET 10 implementation demonstrating the **Decorator design pattern** applied to a service layer. Cross-cutting concerns (caching, logging) are layered transparently around the core business logic without modifying the original service.

## The Pattern

The Decorator pattern adds behaviour to an object **at runtime** by wrapping it inside another object that shares the same interface. The wrapped object and the wrapping object are interchangeable from the caller's perspective.

```
Controller → IProductService
              ↓
          LoggingDecorator     ← outermost: logs each call
              ↓
          CachingDecorator     ← middle: caches results in memory
              ↓
          ProductService       ← core: pure business logic, no cross-cutting concerns
```

Each decorator:
- Implements `IProductService`
- Receives another `IProductService` via constructor
- Delegates the actual work to the inner service
- Adds its own behaviour before/after delegation

## Why Use This Pattern?

| Approach | Cross-cutting concern lives in |
|---|---|
| Without Decorator | The core service (violates SRP) |
| With Decorator | A dedicated, composable wrapper |

The **ProductService** only handles data. It has zero knowledge of caching or logging. Adding new concerns (e.g., validation, retry, metrics) only requires creating a new decorator — the core service stays unchanged.

## Decorator Chain (DI Composition)

```csharp
IProductService service = new ProductService();                  // 1. Core
service = new CachingProductServiceDecorator(service, cache);   // 2. Cache layer
service = new LoggingProductServiceDecorator(service, logger);  // 3. Log layer
```

## Running the Project

```bash
cd DecoratorPattern.Api
dotnet run
```
- Swagger UI: `http://localhost:5039/swagger`
- Use `requests.http` to send requests and observe logs in the console.
