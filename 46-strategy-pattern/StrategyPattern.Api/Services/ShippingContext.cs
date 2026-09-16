using StrategyPattern.Api.Models;
using StrategyPattern.Api.Strategies;

namespace StrategyPattern.Api.Services;

/// <summary>
/// The Context class that holds a reference to one of the strategy objects
/// and delegates the shipping calculation to it.
/// The strategy can be swapped at runtime via <see cref="SetStrategy"/>.
/// </summary>
public class ShippingContext
{
    private IShippingStrategy _strategy;

    public ShippingContext(IShippingStrategy strategy)
    {
        _strategy = strategy;
    }

    /// <summary>The method of the currently active strategy.</summary>
    public ShippingMethod CurrentMethod => _strategy.Method;

    /// <summary>Replaces the active strategy at runtime.</summary>
    public void SetStrategy(IShippingStrategy strategy)
    {
        _strategy = strategy;
    }

    /// <summary>Delegates the quote calculation to the active strategy.</summary>
    public ShippingQuote Execute(Order order)
    {
        return _strategy.Calculate(order);
    }
}
