
# =============================================
# 13-minimal-api/README.md
# =============================================

# 13 — Minimal API + OpenAPI

A .NET 8 Minimal API implementation with OpenAPI documentation and endpoint grouping.

## What You'll Learn
- Minimal API vs Controller-based API
- Endpoint grouping with MapGroup
- OpenAPI metadata with WithSummary, WithName
- Results helper methods

## Minimal API vs Controller

| | Controller | Minimal API |
|--|-----------|-------------|
| Boilerplate | More | Less |
| Organization | Class-based | Function-based |
| Performance | Slightly slower | Slightly faster |
| Best for | Large APIs | Small/medium APIs |

## How It Works

```
Program.cs
    → app.MapProductEndpoints()
        → MapGroup("/api/products")
            → MapGet, MapPost, MapPut, MapDelete
                → Lambda handlers
```

## Project Structure

```
MinimalApi.Api/
├── Data/
│   └── AppDbContext.cs
├── Endpoints/
│   └── ProductEndpoints.cs   ← All product endpoints
├── Models/
│   └── Product.cs
└── Program.cs                 ← No controllers needed
```

## Run

```bash
cd MinimalApi.Api
dotnet run
```

## Endpoints

| Method | URL | Description |
|--------|-----|-------------|
| GET | /api/products | List all products |
| GET | /api/products/{id} | Get by id |
| POST | /api/products | Create product |
| PUT | /api/products/{id} | Update product |
| DELETE | /api/products/{id} | Delete product |

## Packages Used

| Package | Purpose |
|---------|---------|
| EF Core InMemory | In-memory database |
| Swashbuckle | OpenAPI/Swagger |

---
