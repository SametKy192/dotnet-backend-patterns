using MediatR;

namespace DomainEvents.Domain.Common;

/// <summary>
/// Domain event marker interface that extends INotification for MediatR.
/// </summary>
public interface IDomainEvent : INotification
{
    DateTime OccurredOn { get; }
}
