using StrategyPattern.Api.Models;
using StrategyPattern.Api.Strategies;

namespace StrategyPattern.Api.Services;

/// <summary>
/// Resolves and orchestrates all registered <see cref="IShippingStrategy"/> implementations.
/// Registered as a singleton via DI so all strategies are shared across requests.
/// </summary>
public class ShippingStrategyFactory
{
    private readonly Dictionary<ShippingMethod, IShippingStrategy> _strategies;

    public ShippingStrategyFactory(IEnumerable<IShippingStrategy> strategies)
    {
        _strategies = strategies.ToDictionary(s => s.Method);
    }

    /// <summary>Returns the strategy for the requested shipping method.</summary>
    /// <exception cref="InvalidOperationException">Thrown when no strategy is registered for the method.</exception>
    public IShippingStrategy GetStrategy(ShippingMethod method)
    {
        if (!_strategies.TryGetValue(method, out var strategy))
            throw new InvalidOperationException(
                $"No shipping strategy is registered for method '{method}'.");
        return strategy;
    }

    /// <summary>Returns a quote from every registered strategy (available or not).</summary>
    public IReadOnlyList<ShippingQuote> GetAllQuotes(Order order)
    {
        return _strategies.Values
            .Select(s => s.Calculate(order))
            .ToList();
    }

    /// <summary>
    /// Returns the cheapest available quote for the order.
    /// Free shipping is selected over paid options when the order qualifies.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown when no strategy can handle the order.</exception>
    public ShippingQuote GetOptimalQuote(Order order)
    {
        var available = _strategies.Values
            .Where(s => s.CanHandle(order))
            .Select(s => s.Calculate(order))
            .Where(q => q.IsAvailable)
            .OrderBy(q => q.Cost)
            .ToList();

        if (available.Count == 0)
            throw new InvalidOperationException(
                "No shipping strategy is available for this order.");

        return available[0];
    }
}
