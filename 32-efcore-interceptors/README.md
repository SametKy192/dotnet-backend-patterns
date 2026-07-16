# 32 — EF Core SaveChanges Interceptor (Auditing)

A .NET 10 implementation demonstrating how to use Entity Framework Core **SaveChanges Interceptors** to automate entity audit logging (track who created/modified a record and when) without cluttering the business services.

## What You'll Learn
- What EF Core Interceptors are and how they differ from override methods
- How to implement a custom `SaveChangesInterceptor`
- How to automatically inject and resolve the current user context using `IHttpContextAccessor`
- How to configure interceptors in the DbContext registration
- Real-world database audit trail behavior upon inserting and updating entities

---

## What is an EF Core Interceptor?

EF Core Interceptors allow you to run custom logic before or after certain database operations occur. For example:
- **Connection Interceptors**: Intercept opening/closing database connections
- **Command Interceptors**: Intercept raw SQL execution (useful for query logging or rewriting queries)
- **SaveChanges Interceptors**: Intercept `SaveChanges` and `SaveChangesAsync` operations (perfect for auditing, validation, or dispatching domain events)

Interceptors are powerful because they run inside the EF Core infrastructure, separating cross-cutting concerns (like auditing) from the core business logic.

---

## Interceptor vs. Overriding SaveChanges

Before interceptors were introduced in EF Core, the standard way to implement auditing was to override `SaveChanges`/`SaveChangesAsync` directly in the `DbContext` class. 

| Feature | Overriding `SaveChanges` | SaveChanges Interceptor |
|---|---|---|
| **Separation of Concerns** | Weak (auditing logic resides in the DbContext itself) | Strong (auditing logic is encapsulated in a separate class) |
| **Dependency Injection** | Harder (requires passing services to DbContext constructor) | Native (interceptors can be registered as scoped services with full DI) |
| **Reusability** | Low (must be duplicated or inherited in every DbContext) | High (same interceptor can be registered to multiple DbContexts) |

---

## Implementation Details

### 1. The Auditable Interface
Any entity requiring audit tracking implements the `IAuditable` contract:
```csharp
public interface IAuditable
{
    DateTime CreatedAtUtc { get; set; }
    string? CreatedBy { get; set; }
    DateTime? LastModifiedAtUtc { get; set; }
    string? LastModifiedBy { get; set; }
}
```

### 2. The SaveChanges Interceptor
The interceptor captures entries from the EF Core `ChangeTracker` and applies the updates:
```csharp
public class AuditSaveChangesInterceptor : SaveChangesInterceptor
{
    private readonly ICurrentUserService _currentUserService;

    public AuditSaveChangesInterceptor(ICurrentUserService currentUserService)
    {
        _currentUserService = currentUserService;
    }

    public override InterceptionResult<int> SavingChanges(
        DbContextEventData eventData, InterceptionResult<int> result)
    {
        UpdateAuditableEntities(eventData.Context);
        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
    {
        UpdateAuditableEntities(eventData.Context);
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private void UpdateAuditableEntities(DbContext? context)
    {
        if (context == null) return;

        var entries = context.ChangeTracker.Entries<IAuditable>();
        var currentTime = DateTime.UtcNow;
        var currentUser = _currentUserService.UserId ?? "System";

        foreach (var entry in entries)
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedAtUtc = currentTime;
                entry.Entity.CreatedBy = currentUser;
            }
            else if (entry.State == EntityState.Modified)
            {
                entry.Entity.LastModifiedAtUtc = currentTime;
                entry.Entity.LastModifiedBy = currentUser;
            }
        }
    }
}
```

### 3. Registering the Interceptor
We register the interceptor in `Program.cs` and pass it to the DbContext options:
```csharp
builder.Services.AddScoped<AuditSaveChangesInterceptor>();

builder.Services.AddDbContext<AppDbContext>((sp, options) =>
{
    var auditInterceptor = sp.GetRequiredService<AuditSaveChangesInterceptor>();
    options.UseInMemoryDatabase("InterceptorsDb")
           .AddInterceptors(auditInterceptor);
});
```

---

## Running the Project

```bash
cd EfCoreInterceptors.Api
dotnet run
```

- Swagger UI is available at `http://localhost:5032/swagger`.
- Use the provided `requests.http` file to execute API requests.
- Observe that sending a `POST` request with the header `X-User-Id: Alice` automatically populates the `CreatedBy` field.
- Observe that a subsequent `PUT` request with `X-User-Id: Bob` updates the `LastModifiedBy` field, while the original creation data remains untouched!
