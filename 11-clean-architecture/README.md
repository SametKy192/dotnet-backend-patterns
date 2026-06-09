
# =============================================
# 11-clean-architecture/README.md
# =============================================

# 11 — Clean Architecture

A production-ready Clean Architecture implementation in .NET 8 with strict layer separation and dependency inversion.

## What You'll Learn
- Clean Architecture layer responsibilities
- Dependency Inversion Principle
- Domain-driven entity design with factory methods
- Repository pattern with interface abstraction

## Layer Responsibilities

| Layer | Responsibility | Dependencies |
|-------|---------------|-------------|
| Domain | Entities, business rules, interfaces | None |
| Application | Use cases, orchestration | Domain only |
| Infrastructure | DB, external services | Domain only |
| Api | HTTP, controllers | Application only |

## How It Works

```
HTTP Request
    → Api (Controller)
    → Application (ProductService)
    → Domain (Product entity + IProductRepository)
    → Infrastructure (ProductRepository + AppDbContext)
```

## Key Concepts

### Dependency Inversion
Domain defines `IProductRepository`. Infrastructure implements it. Domain never depends on Infrastructure.

### Factory Method
```csharp
// Instead of: new Product { Name = name, Price = price }
// Use: Product.Create(name, price) — business rules enforced
var product = Product.Create(name, price);
```

## Project Structure

```
CleanArchitecture.Domain/
├── Entities/
│   └── Product.cs              ← Business rules here
└── Interfaces/
    └── IProductRepository.cs   ← Defined in Domain

CleanArchitecture.Application/
└── Products/
    └── ProductService.cs       ← Orchestrates use cases

CleanArchitecture.Infrastructure/
├── Persistence/
│   └── AppDbContext.cs
└── Repositories/
    └── ProductRepository.cs    ← Implements IProductRepository

CleanArchitecture.Api/
└── Controllers/
    └── ProductsController.cs
```

## Run

```bash
cd CleanArchitecture.Api
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

---