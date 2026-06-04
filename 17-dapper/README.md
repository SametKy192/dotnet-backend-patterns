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