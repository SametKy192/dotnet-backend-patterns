# 47 — Observer Pattern

Demonstrates the **Observer** design pattern in ASP.NET Core — a behavioral pattern where a *subject* (Observable) maintains a list of *observers* and notifies them automatically when its state changes.

## Concept

Instead of polling or tight-coupling between components, observers register interest in a subject. When an event occurs, the subject pushes the update to all subscribers simultaneously — decoupling producers from consumers.

```
StockMarket (Subject)
    │
    ├── Subscribe(symbol, IStockObserver)
    └── UpdatePriceAsync(symbol, price)
              │
              ├── EmailAlertObserver   — fires at ≥ 5% move
              ├── SmsAlertObserver     — fires at ≥ 10% move
              ├── DashboardObserver    — fires on every update
              └── AuditLogObserver     — fires on every update (compliance)
```

All four observers are notified **concurrently** via `Task.WhenAll`, keeping the notification pipeline non-blocking.

## Project Structure

```
47-observer-pattern/
├── ObserverPattern.Api/
│   ├── Models/
│   │   ├── StockPrice.cs          # Price snapshot with change metrics
│   │   ├── StockAlert.cs          # Alert record + AlertSeverity enum
│   │   └── Requests.cs            # UpdatePriceRequest, SubscribeRequest
│   ├── Observers/
│   │   ├── IStockObserver.cs      # Observer interface (Name, UpdateAsync, Alerts)
│   │   ├── EmailAlertObserver.cs  # Alert at ≥ 5%, Critical at ≥ 10%
│   │   ├── SmsAlertObserver.cs    # Alert at ≥ 10%, always Critical
│   │   ├── DashboardObserver.cs   # Records every tick (live feed)
│   │   └── AuditLogObserver.cs    # Records every tick (compliance log)
│   ├── Subject/
│   │   ├── IStockMarket.cs        # Subject contract
│   │   └── StockMarket.cs         # Concrete subject — concurrent notification
│   ├── Controllers/
│   │   └── StocksController.cs    # 6 endpoints
│   └── Program.cs                 # DI wiring — singletons for subject + observers
└── ObserverPattern.Tests/
    └── ObserverPatternTests.cs    # 24 unit tests
```

## Key Abstractions

### `IStockObserver`
```csharp
public interface IStockObserver
{
    string Name { get; }
    Task UpdateAsync(StockPrice price);
    IReadOnlyList<StockAlert> Alerts { get; }
}
```

### `IStockMarket` (Subject)
```csharp
public interface IStockMarket
{
    void Subscribe(string symbol, IStockObserver observer);
    void Unsubscribe(string symbol, IStockObserver observer);
    Task UpdatePriceAsync(string symbol, decimal newPrice);
    IReadOnlyList<StockPrice> GetPriceHistory(string symbol);
    IReadOnlyList<string> GetTrackedSymbols();
}
```

### `StockMarket` — Concurrent Notification
```csharp
// Notify all observers concurrently
await Task.WhenAll(observers.Select(o => o.UpdateAsync(price)));
```

## Observer Behaviour

| Observer         | Threshold         | Severity               | Use Case               |
|------------------|-------------------|------------------------|------------------------|
| **EmailAlert**   | ≥ 5% change       | Warning / Critical     | Significant moves      |
| **SmsAlert**     | ≥ 10% change      | Critical only          | Urgent alerts          |
| **Dashboard**    | Every update      | Info                   | Live price feed        |
| **AuditLog**     | Every update      | Info                   | Compliance audit trail |

> [!NOTE] Critical severity is triggered when `|ChangePercent| ≥ 10%` for EmailAlert and always for SmsAlert.

## DI Wiring — `Program.cs`

Observers are registered as **singletons** under both their concrete type and `IStockObserver`, ensuring the same instance is used across subscribe and controller injection:

```csharp
builder.Services.AddSingleton<IStockMarket, StockMarket>();

builder.Services.AddSingleton<EmailAlertObserver>();
builder.Services.AddSingleton<IStockObserver>(sp => sp.GetRequiredService<EmailAlertObserver>());
// ...same pattern for SmsAlert, Dashboard, AuditLog
```

## API Endpoints

| Method | Route                            | Description                                     |
|--------|----------------------------------|-------------------------------------------------|
| POST   | `/api/stocks/subscribe`          | Subscribe a named observer to a symbol          |
| DELETE | `/api/stocks/subscribe`          | Unsubscribe a named observer from a symbol      |
| POST   | `/api/stocks/price`              | Publish a price update — notifies all observers |
| GET    | `/api/stocks/symbols`            | List all tracked symbols                        |
| GET    | `/api/stocks/{symbol}/history`   | Full price history for a symbol                 |
| GET    | `/api/stocks/alerts/{type}`      | Alerts from a specific observer                 |
| GET    | `/api/stocks/alerts`             | All alerts from all observers                   |

### Observer Types
`EmailAlert` · `SmsAlert` · `Dashboard` · `AuditLog`

## Running the API

```bash
dotnet run --project ObserverPattern.Api
```

## Running Tests

```bash
dotnet test
# Başarılı! - Başarısız: 0, Başarılı: 24, Atlanan: 0
```

## Benefits vs. Direct Coupling

| Concern             | Direct Coupling                         | Observer Pattern                              |
|---------------------|-----------------------------------------|-----------------------------------------------|
| Adding a consumer   | Modify the publisher class              | Add a new class, register in DI               |
| Testing a consumer  | Must test with the full publisher       | Unit test each observer independently         |
| Runtime subscribe   | Hard-coded wiring                       | `market.Subscribe(symbol, observer)`          |
| Notification order  | Sequential                              | Concurrent via `Task.WhenAll`                 |
| Open/Closed         | ❌ Violated on every new consumer        | ✅ Open for extension, closed for modification |
