
# =============================================
# 17-dapper/README.md
# =============================================

# 17 — Dapper

A .NET 8 implementation of high-performance data access using Dapper with SQLite.

## What You'll Learn
- Dapper vs EF Core
- Raw SQL with parameterized queries
- JOIN queries mapped to C# models
- GROUP BY aggregation queries
- SQL injection prevention with parameters

## Dapper vs EF Core

| | EF Core | Dapper |
|--|---------|--------|
| Learning curve | Higher | Lower |
| SQL control | Limited | Full |
| Performance | Good | Better |
| Complex queries | Difficult | Easy |
| Migrations | Built-in | Manual |
| Best for | CRUD apps | Read-heavy, complex queries |

## Key Concepts

### Simple Query
```csharp
var products = await _connection.QueryAsync<Product>("SELECT * FROM Products");
```

### Parameterized Query (SQL injection safe)
```csharp
var product = await _connection.QueryFirstOrDefaultAsync<Product>(
    "SELECT * FROM Products WHERE Id = @Id",
    new { Id = id });
```

### JOIN Query
```csharp
var sql = @"
    SELECT p.Id, p.Name, c.Name AS CategoryName
    FROM Products p
    INNER JOIN Categories c ON p.CategoryId = c.Id";

var products = await _connection.QueryAsync<Product>(sql);
```

## Project Structure

```
DapperDemo.Infrastructure/
├── Data/
│   └── DbInitializer.cs         ← Creates tables + seed data
├── Models/
│   ├── Product.cs
│   └── Category.cs
└── Repositories/
    └── ProductRepository.cs     ← All SQL queries

DapperDemo.Api/
└── Controllers/
    └── ProductsController.cs
```

## Run

```bash
cd DapperDemo.Api
dotnet run
```

## Endpoints

| Method | URL | Description |
|--------|-----|-------------|
| GET | /api/products | All with category (JOIN) |
| GET | /api/products/{id} | Get by id |
| GET | /api/products/price?min=&max= | Filter by price |
| GET | /api/products/stats | Count by category (GROUP BY) |
| POST | /api/products | Create |
| PUT | /api/products/{id} | Update |
| DELETE | /api/products/{id} | Delete |

## Packages Used

| Package | Purpose |
|---------|---------|
| Dapper | Micro ORM |
| Microsoft.Data.Sqlite | SQLite driver |

---
