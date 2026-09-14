using StrategyPattern.Api.Models;

namespace StrategyPattern.Api.Strategies;

/// <summary>
/// Defines the contract for a shipping cost calculation strategy.
/// Each implementation encapsulates a distinct pricing algorithm.
/// </summary>
public interface IShippingStrategy
{
    /// <summary>The shipping method this strategy handles.</summary>
    ShippingMethod Method { get; }

    /// <summary>Returns true if this strategy can fulfil the given order.</summary>
    bool CanHandle(Order order);

    /// <summary>Calculates a shipping quote for the order.</summary>
    ShippingQuote Calculate(Order order);
}
