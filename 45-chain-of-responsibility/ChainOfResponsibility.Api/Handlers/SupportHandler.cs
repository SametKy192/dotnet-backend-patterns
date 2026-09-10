using ChainOfResponsibility.Api.Models;

namespace ChainOfResponsibility.Api.Handlers;

public class SupportHandler : BaseTicketHandler
{
    private static readonly Dictionary<TicketCategory, string> Teams = new()
    {
        [TicketCategory.General]   = "General Support Team",
        [TicketCategory.Technical] = "Technical Team",
        [TicketCategory.Billing]   = "Billing Team",
        [TicketCategory.Security]  = "Security Response Team"
    };

    public override Task<TicketResult> HandleAsync(SupportTicket ticket)
    {
        var team = Teams.GetValueOrDefault(ticket.Category, "General Support Team");
        ticket.ProcessingLog.Add($"[Support] Ticket assigned to {team}.");
        return Task.FromResult(new TicketResult
        {
            TicketId = ticket.Id,
            IsProcessed = true,
            IsRejected = false,
            AssignedTo = team,
            ProcessingLog = ticket.ProcessingLog
        });
    }
}
