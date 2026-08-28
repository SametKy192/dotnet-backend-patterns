namespace EventSourcing.Api.Domain.Events;

/// <summary>
/// Base class for all domain events.
/// Each event is immutable and represents something that happened.
/// </summary>
public abstract record DomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredAt { get; } = DateTime.UtcNow;
}

// Bank Account domain events
public record AccountOpenedEvent(string AccountId, string OwnerName, decimal InitialBalance) : DomainEvent;
public record MoneyDepositedEvent(string AccountId, decimal Amount) : DomainEvent;
public record MoneyWithdrawnEvent(string AccountId, decimal Amount) : DomainEvent;
public record AccountClosedEvent(string AccountId) : DomainEvent;
