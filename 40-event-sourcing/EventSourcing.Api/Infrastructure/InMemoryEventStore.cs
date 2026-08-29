using EventSourcing.Api.Domain.Events;
using System.Text.Json;

namespace EventSourcing.Api.Infrastructure;

/// <summary>
/// Persisted event record. Each row in the store represents one domain event.
/// </summary>
public class EventRecord
{
    public Guid EventId { get; init; }
    public string AggregateId { get; init; } = string.Empty;
    public string EventType { get; init; } = string.Empty;
    public string Payload { get; init; } = string.Empty;
    public DateTime OccurredAt { get; init; }
}

/// <summary>
/// In-memory Event Store.
/// Persists all domain events as serialized JSON records.
/// Supports loading events by aggregate ID (rehydration).
/// </summary>
public class InMemoryEventStore
{
    private readonly List<EventRecord> _store = new();

    // Known event types for deserialization
    private static readonly Dictionary<string, Type> _eventTypes = new()
    {
        [nameof(AccountOpenedEvent)] = typeof(AccountOpenedEvent),
        [nameof(MoneyDepositedEvent)] = typeof(MoneyDepositedEvent),
        [nameof(MoneyWithdrawnEvent)] = typeof(MoneyWithdrawnEvent),
        [nameof(AccountClosedEvent)] = typeof(AccountClosedEvent),
    };

    public void Append(string aggregateId, IEnumerable<DomainEvent> events)
    {
        foreach (var evt in events)
        {
            _store.Add(new EventRecord
            {
                EventId = evt.EventId,
                AggregateId = aggregateId,
                EventType = evt.GetType().Name,
                Payload = JsonSerializer.Serialize(evt, evt.GetType()),
                OccurredAt = evt.OccurredAt,
            });
        }
    }

    public IEnumerable<DomainEvent> LoadEvents(string aggregateId)
    {
        return _store
            .Where(r => r.AggregateId == aggregateId)
            .OrderBy(r => r.OccurredAt)
            .Select(Deserialize)
            .Where(e => e != null)
            .Cast<DomainEvent>();
    }

    public IEnumerable<EventRecord> GetAll() => _store.AsReadOnly();

    private static DomainEvent? Deserialize(EventRecord record)
    {
        if (!_eventTypes.TryGetValue(record.EventType, out var type)) return null;
        return (DomainEvent?)JsonSerializer.Deserialize(record.Payload, type);
    }
}
