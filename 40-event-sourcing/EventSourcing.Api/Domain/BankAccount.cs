using EventSourcing.Api.Domain.Events;

namespace EventSourcing.Api.Domain;

/// <summary>
/// The BankAccount aggregate.
/// Its state is NOT stored directly — instead, the sequence of domain events is stored.
/// State is rebuilt by replaying events (Apply methods).
/// </summary>
public class BankAccount
{
    public string AccountId { get; private set; } = string.Empty;
    public string OwnerName { get; private set; } = string.Empty;
    public decimal Balance { get; private set; }
    public bool IsClosed { get; private set; }

    private readonly List<DomainEvent> _uncommittedEvents = new();
    public IReadOnlyList<DomainEvent> UncommittedEvents => _uncommittedEvents.AsReadOnly();

    // --- Factory Method ---
    public static BankAccount Open(string accountId, string ownerName, decimal initialBalance)
    {
        var account = new BankAccount();
        account.RaiseEvent(new AccountOpenedEvent(accountId, ownerName, initialBalance));
        return account;
    }

    // --- Behaviour Methods ---
    public void Deposit(decimal amount)
    {
        if (IsClosed) throw new InvalidOperationException("Cannot deposit to a closed account.");
        if (amount <= 0) throw new ArgumentException("Deposit amount must be positive.");
        RaiseEvent(new MoneyDepositedEvent(AccountId, amount));
    }

    public void Withdraw(decimal amount)
    {
        if (IsClosed) throw new InvalidOperationException("Cannot withdraw from a closed account.");
        if (amount <= 0) throw new ArgumentException("Withdrawal amount must be positive.");
        if (amount > Balance) throw new InvalidOperationException("Insufficient funds.");
        RaiseEvent(new MoneyWithdrawnEvent(AccountId, amount));
    }

    public void Close()
    {
        if (IsClosed) throw new InvalidOperationException("Account is already closed.");
        RaiseEvent(new AccountClosedEvent(AccountId));
    }

    // --- Event Application (State Reconstruction) ---
    private void Apply(AccountOpenedEvent e)
    {
        AccountId = e.AccountId;
        OwnerName = e.OwnerName;
        Balance = e.InitialBalance;
    }

    private void Apply(MoneyDepositedEvent e) => Balance += e.Amount;
    private void Apply(MoneyWithdrawnEvent e) => Balance -= e.Amount;
    private void Apply(AccountClosedEvent _) => IsClosed = true;

    // --- Reconstitution from history ---
    public static BankAccount Rehydrate(IEnumerable<DomainEvent> history)
    {
        var account = new BankAccount();
        foreach (var evt in history)
            account.ApplyEvent(evt);
        return account;
    }

    private void RaiseEvent(DomainEvent evt)
    {
        ApplyEvent(evt);
        _uncommittedEvents.Add(evt);
    }

    private void ApplyEvent(DomainEvent evt)
    {
        switch (evt)
        {
            case AccountOpenedEvent e: Apply(e); break;
            case MoneyDepositedEvent e: Apply(e); break;
            case MoneyWithdrawnEvent e: Apply(e); break;
            case AccountClosedEvent e: Apply(e); break;
        }
    }

    public void ClearUncommittedEvents() => _uncommittedEvents.Clear();
}
