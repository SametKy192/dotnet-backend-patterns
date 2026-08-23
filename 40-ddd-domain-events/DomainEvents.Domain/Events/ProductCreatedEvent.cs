using DomainEvents.Domain.Common;

namespace DomainEvents.Domain.Events;

public record ProductCreatedEvent(int ProductId, string Name, decimal Price) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}
