# 36 — Domain-Driven Design (DDD) Value Objects & EF Core Mapping

A .NET 10 implementation demonstrating how to design immutable **Value Objects** using C# records, and how to map them to relational databases using **Entity Framework Core (Owned Types & Value Converters)**.

## What is a Value Object?
In Domain-Driven Design (DDD), a **Value Object** is an object that represents a descriptive aspect of the domain and has **no conceptual identity**. It is defined solely by its attributes.

Key characteristics:
1. **Immutability**: Once created, its state cannot be modified. Any operations must return a new instance.
2. **Value Equality**: Two Value Objects are equal if all their properties are equal (which is why C# `record` types are perfect for this!).
3. **Self-Validation**: A Value Object must validate its constraints during construction, ensuring it can never exist in an invalid state.

Examples implemented in this project:
- `Email`: Ensures proper format using regular expressions.
- `Money`: Combines amount and currency. Contains operator overloads (`+` / `-`) preventing math between different currencies (e.g. adding USD to EUR).
- `Address`: Combines Street, City, ZipCode, Country.

---

## EF Core Mapping Strategies

Mapping custom rich types to flat database columns:

### 1. Owned Types (e.g., `Address`, `Money`)
An owned type allows you to map a class/record as a set of properties on another entity.
```csharp
builder.OwnsOne(c => c.BillingAddress, address =>
{
    address.Property(a => a.Street).HasColumnName("Street").IsRequired();
    address.Property(a => a.City).HasColumnName("City").IsRequired();
});
```

### 2. Value Converters (e.g., `Email`)
A value converter translates a domain type to a primitive database type (e.g., `string`) on save, and reconstructs it on read.
```csharp
builder.Property(c => c.Email)
    .HasConversion(
        email => email.Value,
        value => new Email(value));
```

---

## Running the Project

```bash
cd DddValueObjects.Api
dotnet run
```
- Swagger UI: `http://localhost:5036/swagger`
- Use the `requests.http` file to execute testing scenarios (validations, currency mismatch errors, insufficient funds, etc.).
