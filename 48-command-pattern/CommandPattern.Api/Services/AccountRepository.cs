using CommandPattern.Api.Models;
using CommandPattern.Api.Services;

namespace CommandPattern.Api.Services;

/// <summary>
/// In-memory account repository.
/// Keeps all BankAccount instances keyed by ID and exposes
/// a single shared BankCommandInvoker so undo/redo spans all accounts.
/// </summary>
public class AccountRepository
{
    private readonly Dictionary<Guid, BankAccount> _accounts = [];

    /// <summary>The shared command invoker — maintains the full undo/redo history.</summary>
    public BankCommandInvoker Invoker { get; } = new();

    public BankAccount Add(BankAccount account)
    {
        _accounts[account.Id] = account;
        return account;
    }

    public BankAccount? Find(Guid id)
        => _accounts.GetValueOrDefault(id);

    /// <summary>Returns all accounts whose owner name matches (case-insensitive).</summary>
    public IReadOnlyList<BankAccount> FindByOwner(string owner)
        => [.. _accounts.Values
            .Where(a => a.Owner.Equals(owner, StringComparison.OrdinalIgnoreCase))];

    public IReadOnlyList<BankAccount> All()
        => [.. _accounts.Values];
}
