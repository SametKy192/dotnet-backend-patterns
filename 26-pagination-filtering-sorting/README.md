# 26 — Pagination + Filtering + Sorting

A .NET 8 implementation of server-side pagination, filtering, and sorting using IQueryable for optimal database performance.

## What You'll Learn
- Server-side pagination with metadata
- Dynamic filtering with multiple criteria
- Dynamic sorting by any field
- IQueryable chain — single optimized SQL query
- PageSize limit to prevent abuse

## Why Server-Side?
Client-side pagination (bad):
SELECT * FROM Products → return 50,000 rows → client filters
Server-side pagination (good):
SELECT * FROM Products WHERE ... ORDER BY ... OFFSET 0 ROWS FETCH NEXT 10 ROWS ONLY
→ return 10 rows ✅

## Response Structure

```json
{
  "items": [...],        ← Current page data
  "totalCount": 50,      ← Total matching records
  "page": 1,             ← Current page
  "pageSize": 10,        ← Items per page
  "totalPages": 5,       ← Total pages
  "hasPreviousPage": false,
  "hasNextPage": true
}
```

## Query Parameters

| Parameter | Type | Description | Example |
|-----------|------|-------------|---------|
| page | int | Page number (default: 1) | ?page=2 |
| pageSize | int | Items per page (max: 100) | ?pageSize=20 |
| search | string | Search in name | ?search=laptop |
| category | string | Filter by category | ?category=electronics |
| minPrice | decimal | Minimum price | ?minPrice=100 |
| maxPrice | decimal | Maximum price | ?maxPrice=500 |
| sortBy | string | Sort field | ?sortBy=price |
| sortOrder | string | asc or desc | ?sortOrder=desc |

## IQueryable Chain

```csharp
var query = _dbContext.Products.AsQueryable();

// Each filter adds WHERE clause
if (!string.IsNullOrEmpty(search))
    query = query.Where(p => p.Name.Contains(search));

// Sorting
query = query.OrderBy(p => p.Price);

// Pagination — single SQL with all filters
var total = await query.CountAsync();
var items = await query.Skip((page-1) * pageSize).Take(pageSize).ToListAsync();
```

## Project Structure
PaginationDemo.Application/
├── Common/
│   ├── PagedResult.cs    ← Generic response wrapper
│   └── ProductQuery.cs   ← Query parameters
├── Data/
│   └── AppDbContext.cs   ← 50 seed products
└── Services/
└── ProductService.cs ← Filtering + sorting + pagination
PaginationDemo.Api/
└── Controllers/
└── ProductsController.cs

## Run

```bash
cd PaginationDemo.Api
dotnet run
```

## Endpoints

| Method | URL | Description |
|--------|-----|-------------|
| GET | /api/products | Filter, sort and paginate |

## Packages Used

| Package | Purpose |
|---------|---------|
| EF Core InMemory | In-memory database with 50 seed products |