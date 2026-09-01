namespace EventSourcing.Api.Infrastructure;

/// <summary>
/// Event store statistics helper.
/// Returns aggregate and event counts for observability.
/// </summary>
public static class EventStoreStats
{
    public static object GetStats(InMemoryEventStore store)
    {
        var all = store.GetAll().ToList();
        return new
        {
            TotalEvents = all.Count,
            TotalAggregates = all.Select(e => e.AggregateId).Distinct().Count(),
            EventTypeCounts = all
                .GroupBy(e => e.EventType)
                .Select(g => new { EventType = g.Key, Count = g.Count() })
                .OrderByDescending(x => x.Count)
        };
    }
}
