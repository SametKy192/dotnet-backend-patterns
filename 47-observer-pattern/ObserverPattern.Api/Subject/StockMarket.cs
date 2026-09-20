using ObserverPattern.Api.Models;
using ObserverPattern.Api.Observers;

namespace ObserverPattern.Api.Subject;

/// <summary>
/// Concrete subject (Observable). Maintains the observer registry per symbol
/// and notifies all subscribers concurrently when a price is updated.
/// </summary>
public class StockMarket : IStockMarket
{
    private readonly Dictionary<string, List<IStockObserver>> _subscriptions
        = new(StringComparer.OrdinalIgnoreCase);

    private readonly Dictionary<string, List<StockPrice>> _history
        = new(StringComparer.OrdinalIgnoreCase);

    // ── Subscription management ───────────────────────────────────────────────

    public void Subscribe(string symbol, IStockObserver observer)
    {
        if (!_subscriptions.TryGetValue(symbol, out var observers))
        {
            observers = [];
            _subscriptions[symbol] = observers;
        }

        if (!observers.Contains(observer))
            observers.Add(observer);
    }

    public void Unsubscribe(string symbol, IStockObserver observer)
    {
        if (_subscriptions.TryGetValue(symbol, out var observers))
            observers.Remove(observer);
    }

    // ── Core notification ─────────────────────────────────────────────────────

    public async Task UpdatePriceAsync(string symbol, decimal newPrice)
    {
        var upperSymbol   = symbol.ToUpperInvariant();
        var history       = _history.GetValueOrDefault(upperSymbol, []);
        var previousPrice = history.Count > 0 ? history[^1].Price : 0m;

        var price = new StockPrice(upperSymbol, newPrice, previousPrice, DateTime.UtcNow);

        if (!_history.ContainsKey(upperSymbol))
            _history[upperSymbol] = [];
        _history[upperSymbol].Add(price);

        if (!_subscriptions.TryGetValue(upperSymbol, out var observers) || observers.Count == 0)
            return;

        // Notify all observers concurrently
        await Task.WhenAll(observers.Select(o => o.UpdateAsync(price)));
    }

    // ── Query helpers ─────────────────────────────────────────────────────────

    public IReadOnlyList<StockPrice> GetPriceHistory(string symbol)
        => _history.TryGetValue(symbol.ToUpperInvariant(), out var h) ? h : [];

    public IReadOnlyList<string> GetTrackedSymbols()
        => [.. _subscriptions.Keys];
}
