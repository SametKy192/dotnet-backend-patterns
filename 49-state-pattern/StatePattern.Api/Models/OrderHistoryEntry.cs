namespace StatePattern.Api.Models;

public record OrderHistoryEntry(int Step, string Action, OrderStatus From, OrderStatus To, DateTime At);
