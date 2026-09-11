using ChainOfResponsibility.Api.Handlers;
using ChainOfResponsibility.Api.Models;
using Microsoft.AspNetCore.Mvc;

namespace ChainOfResponsibility.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TicketsController : ControllerBase
{
    private readonly ITicketHandler _pipeline;

    public TicketsController(ITicketHandler pipeline)
    {
        _pipeline = pipeline;
    }

    /// <summary>
    /// Submits a support ticket through the handler chain.
    /// The chain: SpamFilter → Authentication → Authorization → RateLimit → Logging → Support
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Submit([FromBody] CreateTicketRequest request)
    {
        var ticket = new SupportTicket
        {
            Title       = request.Title,
            Description = request.Description,
            Priority    = request.Priority,
            Category    = request.Category,
            RequestedBy = request.RequestedBy,
            ApiKey      = request.ApiKey
        };

        var result = await _pipeline.HandleAsync(ticket);

        return result.IsRejected
            ? BadRequest(result)
            : Ok(result);
    }
}
