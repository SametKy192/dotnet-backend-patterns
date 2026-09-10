using ChainOfResponsibility.Api.Models;

namespace ChainOfResponsibility.Api.Handlers;

public interface ITicketHandler
{
    ITicketHandler? Next { get; }
    ITicketHandler SetNext(ITicketHandler handler);
    Task<TicketResult> HandleAsync(SupportTicket ticket);
}
