# 33 — Soft Delete with EF Core

A .NET 10 implementation of the **Soft Delete Pattern** using Entity Framework Core **Global Query Filters** and a custom **SaveChanges Interceptor** to automatically intercept deletions and hide deleted records.

## What You'll Learn
- What the Soft Delete Pattern is and when to use it
- How to implement a generic `ISoftDeletable` interface
- How to use an EF Core `SaveChangesInterceptor` to convert hard deletes (`SQL DELETE`) into soft deletes (`SQL UPDATE`)
- How to configure Global Query Filters in EF Core
- How to bypass global query filters using `.IgnoreQueryFilters()` to retrieve archived data or restore entities

---

## What is Soft Delete?

A **Soft Delete** marks a record as deleted in the database (usually by setting a boolean flag like `IsDeleted = true` and recording a `DeletedAtUtc` timestamp) instead of executing a physical SQL `DELETE` statement.

### Why Use Soft Delete?
1. **Accidental Deletion Recovery**: Allows users or administrators to restore deleted data easily.
2. **Audit & Compliance**: Retains historical records for auditing or analytics.
3. **Data Integrity**: Avoids breaking foreign key constraints or orphan records in related tables without requiring cascading deletions.

---

## How It Works

```
[HTTP DELETE /api/users/2]
        ↓
    _context.Users.Remove(user)
        ↓
[SoftDeleteInterceptor]
    - Catches entity in State = Deleted
    - Changes state to State = Modified
    - Sets IsDeleted = true, DeletedAtUtc = DateTime.UtcNow
        ↓
    [Database SQL UPDATE executed] (User remains in DB)
        ↓
[HTTP GET /api/users]
    - EF Core automatically appends "WHERE IsDeleted = 0" via Global Query Filter
    - User 2 is excluded from results!
```

---

## Implementation Details

### 1. Global Query Filter
In `AppDbContext.cs`, we register the global filter in `OnModelCreating`:
```csharp
modelBuilder.Entity<User>().HasQueryFilter(u => !u.IsDeleted);
```

### 2. SaveChanges Interceptor
The interceptor captures hard delete operations and intercepts them before database execution:
```csharp
public class SoftDeleteInterceptor : SaveChangesInterceptor
{
    private void UpdateSoftDeleteStatuses(DbContext? context)
    {
        if (context == null) return;

        foreach (var entry in context.ChangeTracker.Entries<ISoftDeletable>())
        {
            if (entry.State == EntityState.Deleted)
            {
                entry.State = EntityState.Modified; // Change DELETE to UPDATE
                entry.Entity.IsDeleted = true;
                entry.Entity.DeletedAtUtc = DateTime.UtcNow;
            }
        }
    }
}
```

### 3. Bypassing the Filter
When we need to retrieve deleted items (e.g., in an archive view or to restore a user), we use `.IgnoreQueryFilters()`:
```csharp
var user = await _context.Users
    .IgnoreQueryFilters()
    .FirstOrDefaultAsync(u => u.Id == id);
```

---

## Running the Project

```bash
cd SoftDeleteEfCore.Api
dotnet run
```

- Swagger UI is available at `http://localhost:5033/swagger`.
- Use the provided `requests.http` file to execute API requests.
- Observe that Charlie, who is seeded as deleted, is hidden by default.
- Delete Bob and see that he is removed from `/api/users` but appears in `/api/users/with-deleted`.
- Restore Bob to make him visible in standard queries again!
