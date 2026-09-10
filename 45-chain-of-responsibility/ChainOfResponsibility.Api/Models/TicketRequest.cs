namespace ChainOfResponsibility.Api.Models;

public record CreateTicketRequest(
    string Title,
    string Description,
    TicketPriority Priority,
    TicketCategory Category,
    string RequestedBy,
    string? ApiKey = null
);
