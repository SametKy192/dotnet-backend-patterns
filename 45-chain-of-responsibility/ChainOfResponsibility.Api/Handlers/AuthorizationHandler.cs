using ChainOfResponsibility.Api.Models;

namespace ChainOfResponsibility.Api.Handlers;

public class AuthorizationHandler : BaseTicketHandler
{
    private static readonly Dictionary<string, HashSet<TicketCategory>> Permissions = new()
    {
        ["key-admin-001"]   = [TicketCategory.General, TicketCategory.Technical, TicketCategory.Billing, TicketCategory.Security],
        ["key-user-002"]    = [TicketCategory.General, TicketCategory.Technical, TicketCategory.Billing],
        ["key-support-003"] = [TicketCategory.General, TicketCategory.Technical]
    };

    public override Task<TicketResult> HandleAsync(SupportTicket ticket)
    {
        ticket.ProcessingLog.Add("[Authorization] Checking permissions...");
        if (ticket.ApiKey is null || !Permissions.TryGetValue(ticket.ApiKey, out var allowed) || !allowed.Contains(ticket.Category))
            return Task.FromResult(Reject(ticket, $"Not authorized to submit {ticket.Category} tickets."));
        ticket.ProcessingLog.Add("[Authorization] Permission granted.");
        return PassToNextAsync(ticket);
    }
}
