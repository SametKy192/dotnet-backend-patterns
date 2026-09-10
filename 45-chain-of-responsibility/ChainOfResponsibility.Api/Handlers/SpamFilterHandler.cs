using ChainOfResponsibility.Api.Models;

namespace ChainOfResponsibility.Api.Handlers;

public class SpamFilterHandler : BaseTicketHandler
{
    private static readonly string[] SpamKeywords = ["free", "winner", "prize", "click here", "buy now"];

    public override Task<TicketResult> HandleAsync(SupportTicket ticket)
    {
        ticket.ProcessingLog.Add("[SpamFilter] Checking for spam content...");
        var content = $"{ticket.Title} {ticket.Description}".ToLowerInvariant();
        if (SpamKeywords.Any(k => content.Contains(k)))
            return Task.FromResult(Reject(ticket, "Ticket flagged as spam."));
        ticket.ProcessingLog.Add("[SpamFilter] Passed spam check.");
        return PassToNextAsync(ticket);
    }
}
