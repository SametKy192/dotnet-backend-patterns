
# =============================================
# 06-api-versioning/README.md
# =============================================

# 06 — API Versioning

A .NET 8 implementation of URL-based API versioning using Asp.Versioning.

## What You'll Learn
- Why API versioning is needed
- URL versioning strategy
- Adding new versions without breaking old ones

## How It Works

```
GET /api/v1/products → { id, name }
GET /api/v2/products → { id, name, price }
GET /api/v2/products/1 → new endpoint added in V2
```

## Why Versioning?

When V2 is released, V1 clients should not break. Each version evolves independently.

## Project Structure

```
ApiVersioning.Api/
└── Controllers/
    ├── V1/
    │   └── ProductsController.cs   ← V1 endpoints
    └── V2/
        └── ProductsController.cs   ← V2 endpoints
```

## Run

```bash
cd ApiVersioning.Api
dotnet run
```

## Endpoints

| Method | URL | Version | Difference |
|--------|-----|---------|------------|
| GET | /api/v1/products | V1 | Id, Name |
| GET | /api/v2/products | V2 | Id, Name, Price |
| GET | /api/v2/products/{id} | V2 only | New endpoint |

## Packages Used

| Package | Purpose |
|---------|---------|
| Asp.Versioning.Mvc | API versioning |
| Asp.Versioning.Mvc.ApiExplorer | Swagger integration |

---
