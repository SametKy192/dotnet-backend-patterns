using ChainOfResponsibility.Api.Models;

namespace ChainOfResponsibility.Api.Handlers;

public abstract class BaseTicketHandler : ITicketHandler
{
    public ITicketHandler? Next { get; private set; }

    public ITicketHandler SetNext(ITicketHandler handler)
    {
        Next = handler;
        return handler;
    }

    public abstract Task<TicketResult> HandleAsync(SupportTicket ticket);

    protected Task<TicketResult> PassToNextAsync(SupportTicket ticket)
    {
        if (Next is null)
        {
            return Task.FromResult(new TicketResult
            {
                TicketId = ticket.Id,
                IsProcessed = false,
                IsRejected = true,
                RejectionReason = "No handler available to process this ticket.",
                ProcessingLog = ticket.ProcessingLog
            });
        }
        return Next.HandleAsync(ticket);
    }

    protected static TicketResult Reject(SupportTicket ticket, string reason)
    {
        ticket.ProcessingLog.Add($"[REJECTED] {reason}");
        return new TicketResult
        {
            TicketId = ticket.Id,
            IsProcessed = false,
            IsRejected = true,
            RejectionReason = reason,
            ProcessingLog = ticket.ProcessingLog
        };
    }
}
