
# =============================================
# 14-efcore-postgresql/README.md
# =============================================

# 14 — EF Core + PostgreSQL

A .NET 8 implementation of EF Core with relationships, migrations, and seed data.

## What You'll Learn
- One-to-many relationships
- Many-to-many relationships
- Eager loading with Include
- Seed data with HasData
- Filtering and ordering queries
- PostgreSQL with Npgsql

## Relationships

```
Category (1) ──── (*) Product (*) ──── (*) Tag
```

## Key EF Core Concepts

### Eager Loading
```csharp
_dbContext.Products
    .Include(p => p.Category)
    .Include(p => p.Tags)
    .ToListAsync();
```

### Seed Data
```csharp
modelBuilder.Entity<Category>().HasData(
    new Category { Id = 1, Name = "Electronics" }
);
```

### PostgreSQL Connection (Production)
```csharp
builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseNpgsql(connectionString));
```

## Project Structure

```
EFCorePostgreSQL.Domain/
└── Entities/
    ├── Product.cs    ← Many-to-one with Category
    ├── Category.cs   ← One-to-many with Products
    └── Tag.cs        ← Many-to-many with Products

EFCorePostgreSQL.Infrastructure/
├── Persistence/
│   └── AppDbContext.cs      ← Relationships + seeding
└── Repositories/
    └── ProductRepository.cs ← Query examples

EFCorePostgreSQL.Api/
└── Controllers/
    └── ProductsController.cs
```

## Run

```bash
cd EFCorePostgreSQL.Api
dotnet run
```

## Endpoints

| Method | URL | Description |
|--------|-----|-------------|
| GET | /api/products | All with category + tags |
| GET | /api/products/category/{id} | Filter by category |
| GET | /api/products/price?min=&max= | Filter by price range |
| POST | /api/products | Create product |
| DELETE | /api/products/{id} | Delete product |

## Packages Used

| Package | Purpose |
|---------|---------|
| EF Core | ORM |
| Npgsql.EFCore | PostgreSQL provider |
| EF Core InMemory | Development/testing |

---