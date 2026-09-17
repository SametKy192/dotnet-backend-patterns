using Microsoft.AspNetCore.Mvc;
using StrategyPattern.Api.Models;
using StrategyPattern.Api.Services;

namespace StrategyPattern.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ShippingController : ControllerBase
{
    private readonly ShippingStrategyFactory _factory;

    public ShippingController(ShippingStrategyFactory factory)
    {
        _factory = factory;
    }

    /// <summary>
    /// Returns a shipping quote for a specific method.
    /// The ShippingContext holds the selected strategy and can be swapped at runtime.
    /// </summary>
    [HttpPost("quote")]
    public IActionResult GetQuote([FromBody] ShipOrderRequest request)
    {
        var order    = request.ToOrder();
        var strategy = _factory.GetStrategy(request.Method);
        var context  = new ShippingContext(strategy);
        var quote    = context.Execute(order);

        return quote.IsAvailable ? Ok(quote) : BadRequest(quote);
    }

    /// <summary>
    /// Returns quotes from all five registered strategies — available and unavailable.
    /// Useful for comparison views.
    /// </summary>
    [HttpPost("all")]
    public IActionResult GetAllQuotes([FromBody] ShipOrderRequest request)
    {
        var order  = request.ToOrder();
        var quotes = _factory.GetAllQuotes(order);
        return Ok(quotes);
    }

    /// <summary>
    /// Returns the cheapest available shipping option for the order.
    /// Free shipping is preferred when the order qualifies.
    /// </summary>
    [HttpPost("optimal")]
    public IActionResult GetOptimalQuote([FromBody] ShipOrderRequest request)
    {
        var order = request.ToOrder();
        var quote = _factory.GetOptimalQuote(order);
        return Ok(quote);
    }
}
