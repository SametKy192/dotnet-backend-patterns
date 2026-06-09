# =============================================
# 05-multitenancy/README.md
# =============================================

# 05 — Multi-tenancy

A .NET 8 implementation of row-level multi-tenancy using EF Core global query filters.

## What You'll Learn
- What multi-tenancy is
- Row-level tenant isolation
- EF Core global query filter
- Reading tenant from HTTP header

## How It Works

```
Request → X-Tenant-Id: tenant-a header
    → TenantService reads header
    → DbContext adds WHERE TenantId = 'tenant-a' to every query
    → Tenant A cannot see Tenant B's data
```

## Test Scenario

1. Add product with Tenant A
2. Add product with Tenant B
3. List with Tenant A → only Tenant A's product
4. List with Tenant B → only Tenant B's product

## Project Structure

```
MultiTenancy.Domain/
└── Entities/
    ├── ITenantEntity.cs     ← Tenant interface
    └── Product.cs

MultiTenancy.Infrastructure/
├── Persistence/
│   └── AppDbContext.cs      ← Global query filter here
└── Services/
    └── TenantService.cs     ← Reads tenant from header

MultiTenancy.Api/
└── Controllers/
    └── ProductsController.cs
```

## Run

```bash
cd MultiTenancy.Api
dotnet run
```

## Endpoints

| Method | URL | Header |
|--------|-----|--------|
| GET | /api/products | X-Tenant-Id: tenant-a |
| POST | /api/products | X-Tenant-Id: tenant-a |

## Packages Used

| Package | Purpose |
|---------|---------|
| EF Core InMemory | In-memory database |

---