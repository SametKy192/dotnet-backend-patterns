# 29 — Specification Pattern

A .NET 10 implementation of the Specification Pattern using Entity Framework Core, showcasing how to encapsulate query logic into reusable, combinable business rules.

## What You'll Learn
- What the Specification Pattern is and why it's useful
- How to separate query details from repositories
- How to implement combinable specifications (`And`, `Or`, `Not` operations)
- Eager loading (includes), sorting, and paging in specifications
- Real-world product filtering scenarios

## What is the Specification Pattern?

The **Specification Pattern** is a software design pattern where business rules (filtering, sorting, paging, eager loading) can be combined by chaining them together using boolean logic. 

In a typical repository pattern, as you need new queries (e.g. `GetActiveProducts`, `GetProductsByCategory`, `GetProductsByCategoryAndPrice`), your repository interface grows endlessly with custom methods. 
With the Specification Pattern, you have a single generic repository method:
```csharp
Task<IReadOnlyList<T>> ListAsync(ISpecification<T> spec);
```
And you encapsulate the query criteria into dedicated specification classes.

## How It Works

1. **`ISpecification<T>`**: Defines the contract for query building (Criteria, Includes, OrderBy, Paging).
2. **`BaseSpecification<T>`**: Implements the contract and provides boolean operators (`And`, `Or`, `Not`) using Expression Trees.
3. **`SpecificationEvaluator<T>`**: Takes an `IQueryable<T>` (e.g., DbContext DbSet) and applies the specification properties to construct the final SQL query.
4. **`GenericRepository<T>`**: Applies the evaluator on database calls.

## Combining Specifications

Because `BaseSpecification<T>` supports expressions, we can combine multiple specifications dynamically:
```csharp
var activeSpec = new ActiveProductSpecification();
var categorySpec = new ProductByCategorySpecification("Furniture");
var priceSpec = new ProductByPriceRangeSpecification(100m, 500m);

// Combine using AND logic (Active AND Furniture AND Price between 100 and 500)
var combinedSpec = activeSpec.And(categorySpec).And(priceSpec);

var products = await repository.ListAsync(combinedSpec);
```

## Project Structure
`SpecificationPattern.Application/`
- **`Models/Product.cs`**: The product entity.
- **`Specifications/ISpecification.cs`**: Specification contract.
- **`Specifications/BaseSpecification.cs`**: Abstract base specification with `And`/`Or`/`Not`.
- **`Specifications/ProductFilterSpecification.cs`**: Parameterized specification for filters, sorting, and paging.
- **`Specifications/ActiveProductSpecification.cs`**, `ProductByCategorySpecification.cs`, `ProductByPriceRangeSpecification.cs`: Single-responsibility specifications.
- **`Interfaces/IGenericRepository.cs`**: Generic repository contract.

`SpecificationPattern.Infrastructure/`
- **`Data/AppDbContext.cs`**: EF Core context with In-Memory DB configuration and seed data.
- **`Data/SpecificationEvaluator.cs`**: Translates specification details into EF Core queries.
- **`Data/GenericRepository.cs`**: Implements specification query evaluation.

`SpecificationPattern.Api/`
- **`Controllers/ProductsController.cs`**: Exposes endpoints demonstrating basic specification queries and combined specification queries.

## Run

```bash
cd SpecificationPattern.Api
dotnet run
```

## Endpoints

| Method | URL | Description |
|--------|-----|-------------|
| GET | `/api/products` | Lists products with optional filters (category, price, sorting, paging) |
| GET | `/api/products/combined-demo` | Demo showing combined specifications (Active & Furniture & Price [100-500]) |
| GET | `/api/products/{id}` | Gets a product by ID |
