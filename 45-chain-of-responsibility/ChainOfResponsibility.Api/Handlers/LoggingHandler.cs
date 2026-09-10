using ChainOfResponsibility.Api.Models;

namespace ChainOfResponsibility.Api.Handlers;

public class LoggingHandler : BaseTicketHandler
{
    private readonly ILogger<LoggingHandler> _logger;
    public LoggingHandler(ILogger<LoggingHandler> logger) => _logger = logger;

    public override Task<TicketResult> HandleAsync(SupportTicket ticket)
    {
        _logger.LogInformation("Processing ticket {TicketId}: [{Priority}] {Title} from {RequestedBy}",
            ticket.Id, ticket.Priority, ticket.Title, ticket.RequestedBy);
        ticket.ProcessingLog.Add($"[Logging] Ticket recorded at {DateTime.UtcNow:O}.");
        return PassToNextAsync(ticket);
    }
}
