namespace ObserverPattern.Api.Models;

/// <summary>Request body for updating a stock price.</summary>
public record UpdatePriceRequest(string Symbol, decimal Price);

/// <summary>Request body for subscribing a named observer to a stock symbol.</summary>
public record SubscribeRequest(string Symbol, string ObserverType);
