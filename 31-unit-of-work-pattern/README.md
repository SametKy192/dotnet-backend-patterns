# 31 — Unit of Work Pattern

A .NET 10 implementation of the **Unit of Work Pattern** combined with the Repository Pattern using Entity Framework Core. This project demonstrates how to group multiple repository operations into a single transactional boundary to ensure database consistency.

## What You'll Learn
- What the Unit of Work (UoW) Pattern is and why it is critical for business transactions
- How to share a single DbContext across repositories to maintain transaction boundary
- How to implement transactional commit and rollback orchestration in UoW
- Real-world database rollback scenario when business validation (stock check) fails

## What is the Unit of Work Pattern?

According to Martin Fowler, the **Unit of Work** pattern:
> *"Maintains a list of objects affected by a business transaction and coordinates the writing out of changes and the resolution of concurrency problems."*

In .NET and EF Core:
- `DbContext` itself acts as a combination of Repository and Unit of Work patterns under the hood.
- However, writing an explicit Unit of Work layer on top of repositories helps:
  1. Hide the database technology details from your services/application layer.
  2. Share the same database context (and transaction boundary) among multiple repositories.
  3. Ensure that multiple updates (e.g., updating stock AND creating order) either succeed together or fail together.

## The Problem
1. Customer buys a Mechanical Keyboard.
2. System decrements the product stock.
3. System saves the changes (Product stock saved to database).
4. System tries to save the Order record, but order database write fails.
5. **Result**: The product stock is decreased, but no order was recorded! ❌ (Data inconsistency)

## The Solution
By encapsulating the business transaction inside a Unit of Work:
1. Start database transaction: `_unitOfWork.BeginTransactionAsync()`
2. Decrement product stock: `_unitOfWork.Products.Update(product)`
3. Create order record: `await _unitOfWork.Orders.AddAsync(order)`
4. Save Changes: `await _unitOfWork.SaveChangesAsync()`
5. Commit: `await _unitOfWork.CommitTransactionAsync()`
6. If any step fails, catch the exception, and rollback: `await _unitOfWork.RollbackTransactionAsync()`.
7. **Result**: Both changes are rolled back. Product stock remains unchanged and no orphan order is created! ✅ (Consistent state)

## Run

```bash
cd UnitOfWorkPattern.Api
dotnet run
```

Access Swagger UI at `http://localhost:5031/swagger`.
Use `requests.http` to test successful order placement (which updates stock and records order) versus failed order placement due to insufficient stock (which rolls back all changes, leaving stock intact).
