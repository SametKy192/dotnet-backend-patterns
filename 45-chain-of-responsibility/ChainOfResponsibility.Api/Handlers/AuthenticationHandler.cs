using ChainOfResponsibility.Api.Models;

namespace ChainOfResponsibility.Api.Handlers;

public class AuthenticationHandler : BaseTicketHandler
{
    private static readonly HashSet<string> ValidApiKeys = ["key-admin-001", "key-user-002", "key-support-003"];

    public override Task<TicketResult> HandleAsync(SupportTicket ticket)
    {
        ticket.ProcessingLog.Add("[Authentication] Verifying API key...");
        if (string.IsNullOrWhiteSpace(ticket.ApiKey) || !ValidApiKeys.Contains(ticket.ApiKey))
            return Task.FromResult(Reject(ticket, "Invalid or missing API key."));
        ticket.ProcessingLog.Add("[Authentication] API key verified.");
        return PassToNextAsync(ticket);
    }
}
