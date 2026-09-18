# 46 — Strategy Pattern

Demonstrates the **Strategy** design pattern in ASP.NET Core — a behavioral pattern that defines a family of interchangeable algorithms (strategies), encapsulates each one, and makes them swappable at runtime without changing the client code.

## Concept

Instead of embedding multiple `if/else` or `switch` branches for different shipping calculations inside a single service, each pricing algorithm lives in its own class that implements a common interface. The calling code works only against the interface.

```
Client  →  ShippingContext  →  IShippingStrategy
                                  ├── StandardShippingStrategy
                                  ├── ExpressShippingStrategy
                                  ├── OvernightShippingStrategy
                                  ├── FreeShippingStrategy
                                  └── InternationalShippingStrategy
```

## Project Structure

```
46-strategy-pattern/
├── StrategyPattern.Api/
│   ├── Models/
│   │   ├── Order.cs                        # Domain entity
│   │   ├── ShipOrderRequest.cs             # Inbound request DTO
│   │   └── ShippingQuote.cs                # Result DTO + ShippingMethod enum
│   ├── Strategies/
│   │   ├── IShippingStrategy.cs            # Strategy interface
│   │   ├── StandardShippingStrategy.cs     # $5 + $1.50/kg — 5–7 days
│   │   ├── ExpressShippingStrategy.cs      # $15 + $2.50/kg — 2 days
│   │   ├── OvernightShippingStrategy.cs    # $35 + $5.00/kg — next day
│   │   ├── FreeShippingStrategy.cs         # $0 — orders ≥ $50 — 7–10 days
│   │   └── InternationalShippingStrategy.cs# $25 + $4.00/kg — 10–14 days
│   ├── Services/
│   │   ├── ShippingContext.cs              # Context — holds and executes a strategy
│   │   └── ShippingStrategyFactory.cs      # Resolves strategies via DI
│   ├── Controllers/
│   │   └── ShippingController.cs           # 3 endpoints
│   └── Program.cs
└── StrategyPattern.Tests/
    └── ShippingStrategyTests.cs            # 25 unit tests
```

## Key Abstractions

### `IShippingStrategy`
```csharp
public interface IShippingStrategy
{
    ShippingMethod Method   { get; }
    bool    CanHandle(Order order);
    ShippingQuote Calculate(Order order);
}
```

### `ShippingContext`
```csharp
public class ShippingContext
{
    private IShippingStrategy _strategy;

    public void SetStrategy(IShippingStrategy strategy) => _strategy = strategy;
    public ShippingQuote Execute(Order order)           => _strategy.Calculate(order);
}
```

The context is created **per-request** in the controller, making the strategy swap explicit and visible.

## Shipping Strategies

| Method            | Cost Formula                    | Delivery      | Condition                    |
|-------------------|---------------------------------|---------------|------------------------------|
| **Standard**      | $5.00 + $1.50/kg                | 5–7 days      | Domestic only                |
| **Express**       | $15.00 + $2.50/kg               | 2 days        | Domestic only                |
| **Overnight**     | $35.00 + $5.00/kg               | Next day      | Domestic only                |
| **Free**          | $0.00                           | 7–10 days     | Domestic + order ≥ $50       |
| **International** | $25.00 + $4.00/kg (+10% customs)| 10–14 days    | International orders only    |

> [!NOTE] The Free strategy's `CanHandle` check automatically excludes it from optimal selection when the order value is below the threshold.

## DI Wiring — `Program.cs`

All strategies are registered under the same interface so the factory receives `IEnumerable<IShippingStrategy>`:

```csharp
builder.Services.AddSingleton<IShippingStrategy, StandardShippingStrategy>();
builder.Services.AddSingleton<IShippingStrategy, ExpressShippingStrategy>();
// ...

builder.Services.AddSingleton<ShippingStrategyFactory>();
// Factory constructor: ShippingStrategyFactory(IEnumerable<IShippingStrategy> strategies)
```

## API Endpoints

| Method | Route                     | Description                               |
|--------|---------------------------|-------------------------------------------|
| POST   | `/api/shipping/quote`     | Quote for a specific shipping method      |
| POST   | `/api/shipping/all`       | Quotes from all five strategies           |
| POST   | `/api/shipping/optimal`   | Cheapest available option (auto-select)   |

### Request Body

```json
{
  "weightKg": 3.5,
  "totalValue": 120.00,
  "itemCount": 4,
  "destination": "domestic",
  "method": 0
}
```

> `method` enum: `0` = Standard, `1` = Express, `2` = Overnight, `3` = Free, `4` = International

## Running the API

```bash
dotnet run --project StrategyPattern.Api
```

## Running Tests

```bash
dotnet test
# Başarılı! - Başarısız: 0, Başarılı: 25, Atlanan: 0
```

## Benefits vs. Switch-Based Approach

| Concern              | Switch / If-Else                        | Strategy Pattern                              |
|----------------------|-----------------------------------------|-----------------------------------------------|
| Adding a new method  | Modify the service class               | Add a new class, register in DI              |
| Testing each method  | Must mock large service                | Unit test each strategy independently         |
| Runtime swap         | Conditional code                        | `context.SetStrategy(newStrategy)`            |
| Open/Closed          | ❌ Violated on every new method         | ✅ Open for extension, closed for modification |
| Readable DI graph    | Hidden inside a service               | Explicit registrations in `Program.cs`        |
