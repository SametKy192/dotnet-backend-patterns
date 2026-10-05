using Microsoft.AspNetCore.Mvc;
using StatePattern.Api.Models;
using StatePattern.Api.Services;

namespace StatePattern.Api.Controllers;

/// <summary>REST façade — invalid transitions surface as 409 Conflict.</summary>
[ApiController]
[Route("api/orders")]
public class OrdersController(OrderRepository repo) : ControllerBase
{
    [HttpPost]
    public IActionResult Create([FromBody] CreateOrderRequest req)
    {
        if (string.IsNullOrWhiteSpace(req.Customer))
            return BadRequest(new { error = "Customer is required." });
        if (req.Amount <= 0)
            return BadRequest(new { error = "Amount must be positive." });

        var order = repo.Add(new Order { Customer = req.Customer, Amount = req.Amount });
        return Ok(ToDto(order));
    }

    [HttpGet]
    public IActionResult GetAll() => Ok(repo.All().Select(ToDto));

    [HttpGet("{id:guid}")]
    public IActionResult GetById(Guid id) =>
        repo.Find(id) is { } o ? Ok(ToDto(o)) : NotFound();

    [HttpGet("{id:guid}/history")]
    public IActionResult History(Guid id) =>
        repo.Find(id) is { } o ? Ok(o.History) : NotFound();

    [HttpPost("{id:guid}/pay")]     public IActionResult Pay(Guid id)     => Act(id, o => o.Pay());
    [HttpPost("{id:guid}/ship")]    public IActionResult Ship(Guid id)    => Act(id, o => o.Ship());
    [HttpPost("{id:guid}/deliver")] public IActionResult Deliver(Guid id) => Act(id, o => o.Deliver());
    [HttpPost("{id:guid}/cancel")]  public IActionResult Cancel(Guid id)  => Act(id, o => o.Cancel());
    [HttpPost("{id:guid}/refund")]  public IActionResult Refund(Guid id)  => Act(id, o => o.Refund());

    private IActionResult Act(Guid id, Action<Order> action)
    {
        var order = repo.Find(id);
        if (order is null) return NotFound();
        try
        {
            action(order);
            return Ok(ToDto(order));
        }
        catch (InvalidTransitionException ex)
        {
            return Conflict(new { error = ex.Message, current = ex.Current.ToString() });
        }
    }

    private static object ToDto(Order o) => new
    {
        o.Id, o.Customer, o.Amount, Status = o.Status.ToString()
    };
}
