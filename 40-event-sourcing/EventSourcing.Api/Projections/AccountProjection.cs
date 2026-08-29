using EventSourcing.Api.Domain.Events;
using EventSourcing.Api.Infrastructure;

namespace EventSourcing.Api.Projections;

/// <summary>
/// Read model for account summary — projected from events.
/// This is the "Query" side in CQRS + Event Sourcing.
/// </summary>
public class AccountSummary
{
    public string AccountId { get; set; } = string.Empty;
    public string OwnerName { get; set; } = string.Empty;
    public decimal Balance { get; set; }
    public bool IsClosed { get; set; }
    public int TotalTransactions { get; set; }
}

/// <summary>
/// Builds AccountSummary read models by replaying events from the event store.
/// </summary>
public class AccountProjection
{
    private readonly InMemoryEventStore _eventStore;

    public AccountProjection(InMemoryEventStore eventStore)
    {
        _eventStore = eventStore;
    }

    public AccountSummary? Project(string accountId)
    {
        var events = _eventStore.LoadEvents(accountId).ToList();
        if (!events.Any()) return null;

        var summary = new AccountSummary();
        foreach (var evt in events)
        {
            switch (evt)
            {
                case AccountOpenedEvent e:
                    summary.AccountId = e.AccountId;
                    summary.OwnerName = e.OwnerName;
                    summary.Balance = e.InitialBalance;
                    break;
                case MoneyDepositedEvent e:
                    summary.Balance += e.Amount;
                    summary.TotalTransactions++;
                    break;
                case MoneyWithdrawnEvent e:
                    summary.Balance -= e.Amount;
                    summary.TotalTransactions++;
                    break;
                case AccountClosedEvent:
                    summary.IsClosed = true;
                    break;
            }
        }
        return summary;
    }
}
