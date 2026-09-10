using ChainOfResponsibility.Api.Models;
using System.Collections.Concurrent;

namespace ChainOfResponsibility.Api.Handlers;

public class RateLimitHandler : BaseTicketHandler
{
    private static readonly ConcurrentDictionary<string, (int Count, DateTime WindowStart)> _counts = new();
    private const int MaxPerMinute = 5;

    public override Task<TicketResult> HandleAsync(SupportTicket ticket)
    {
        ticket.ProcessingLog.Add("[RateLimit] Checking rate limit...");
        var key = ticket.ApiKey ?? ticket.RequestedBy;
        var now = DateTime.UtcNow;
        var entry = _counts.GetOrAdd(key, _ => (0, now));
        if ((now - entry.WindowStart).TotalMinutes >= 1) entry = (0, now);
        if (entry.Count >= MaxPerMinute)
            return Task.FromResult(Reject(ticket, $"Rate limit exceeded. Max {MaxPerMinute} requests/min."));
        _counts[key] = (entry.Count + 1, entry.WindowStart);
        ticket.ProcessingLog.Add($"[RateLimit] OK ({entry.Count + 1}/{MaxPerMinute} this minute).");
        return PassToNextAsync(ticket);
    }
}
