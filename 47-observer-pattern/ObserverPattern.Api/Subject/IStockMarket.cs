using ObserverPattern.Api.Models;
using ObserverPattern.Api.Observers;

namespace ObserverPattern.Api.Subject;

/// <summary>
/// Subject (Observable) contract.
/// Maintains a list of observers per stock symbol and notifies them on price changes.
/// </summary>
public interface IStockMarket
{
    /// <summary>Registers an observer to receive updates for the given symbol.</summary>
    void Subscribe(string symbol, IStockObserver observer);

    /// <summary>Removes an observer from the given symbol's notification list.</summary>
    void Unsubscribe(string symbol, IStockObserver observer);

    /// <summary>
    /// Updates the price for <paramref name="symbol"/> and notifies all subscribed observers.
    /// </summary>
    Task UpdatePriceAsync(string symbol, decimal newPrice);

    /// <summary>Returns the full price history for a symbol.</summary>
    IReadOnlyList<StockPrice> GetPriceHistory(string symbol);

    /// <summary>Returns the list of all tracked symbols.</summary>
    IReadOnlyList<string> GetTrackedSymbols();
}
