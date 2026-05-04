# 01 — CQRS + MediatR

A clean implementation of the CQRS pattern using MediatR with FluentValidation pipeline behavior.

## What You'll Learn
- What CQRS is and why it's used
- Command/Query separation with MediatR
- Pipeline Behavior for validation

## Project Structure
CqrsMediatr.Domain/
└── Entities/
└── Product.cs
CqrsMediatr.Application/
├── Common/
│   └── Behaviors/
│       └── ValidationBehavior.cs
└── Products/
├── Commands/CreateProduct/
│   ├── CreateProductCommand.cs
│   ├── CreateProductCommandValidator.cs
│   └── CreateProductHandler.cs
└── Queries/GetProducts/
├── GetProductsQuery.cs
└── GetProductsHandler.cs
CqrsMediatr.Api/
└── Controllers/
└── ProductsController.cs

## How It Works
HTTP Request
→ Controller
→ MediatR.Send()
→ [ValidationBehavior]  ← returns 400 if invalid
→ Handler
→ Response

## Run

```bash
cd CqrsMediatr.Api
dotnet run
```

## Endpoints

| Method | URL | Description |
|--------|-----|-------------|
| GET | /api/products | List all products |
| POST | /api/products | Create a product |

## Validation Rules

| Field | Rule |
|-------|------|
| Name | Required, max 100 characters |
| Price | Must be greater than 0 |

## Packages Used

| Package | Purpose |
|---------|---------|
| MediatR | Command/Query dispatcher |
| FluentValidation | Validation rules |

## Notes
- In-memory list is used for simplicity. Replace with a real database in production.