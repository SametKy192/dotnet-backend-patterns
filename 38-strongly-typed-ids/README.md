# 38 — Strongly-Typed IDs in Entity Framework Core

A .NET 10 implementation demonstrating how to use **Strongly-Typed IDs** (e.g. `CustomerId`, `ProductId`, `OrderId`) instead of primitive types (like `Guid` or `int`) for entity identifiers, ensuring **compile-time type safety** across the domain, and mapping them transparently using **EF Core Value Converters**.

## The Problem
Normally, entities use basic types for their identifiers:
```csharp
public class Order
{
    public Guid Id { get; set; }
    public Guid CustomerId { get; set; }
    public Guid ProductId { get; set; }
}
```

A common bug occurs when a developer accidentally passes IDs in the wrong order:
```csharp
// Compile succeeds, but causes runtime business logic errors!
var order = new Order(orderId, productId, customerId); 
```

## The Solution: Strongly-Typed IDs
We wrap the primitive identifier type in a lightweight `readonly record struct`:
```csharp
public readonly record struct CustomerId(Guid Value);
public readonly record struct ProductId(Guid Value);
```

Now, swapping the parameters causes a **compiler error**:
```csharp
// Compile fails! ProductId cannot be assigned to CustomerId.
var order = new Order(orderId, productId, customerId); 
```

---

## EF Core Mapping via Global Conventions
To avoid mapping these converters manually for each property, we configure them globally in our DbContext using `ConfigureConventions`:

```csharp
protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
{
    configurationBuilder.Properties<CustomerId>().HaveConversion<CustomerIdConverter>();
    configurationBuilder.Properties<ProductId>().HaveConversion<ProductIdConverter>();
    configurationBuilder.Properties<OrderId>().HaveConversion<OrderIdConverter>();
}
```

---

## Running the Project

```bash
cd StronglyTypedIds.Api
dotnet run
```
- Swagger UI: `http://localhost:5038/swagger`
- Use `requests.http` to test seed, order placement, and query actions.
