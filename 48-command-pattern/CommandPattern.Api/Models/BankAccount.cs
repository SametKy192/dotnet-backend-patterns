namespace CommandPattern.Api.Models;

/// <summary>
/// Represents a bank account — the receiver that commands act upon.
/// All mutations go through the command interface, keeping business rules in one place.
/// </summary>
public class BankAccount
{
    public Guid     Id      { get; init; } = Guid.NewGuid();
    public string   Owner   { get; init; } = string.Empty;
    public decimal  Balance { get; private set; }
    public bool     IsOpen  { get; private set; } = true;

    // ── Primitive operations called by commands ───────────────────────────────

    /// <summary>Increases the balance. Throws if the account is closed.</summary>
    public void Credit(decimal amount)
    {
        EnsureOpen();
        if (amount <= 0) throw new ArgumentOutOfRangeException(nameof(amount), "Credit amount must be positive.");
        Balance += amount;
    }

    /// <summary>Decreases the balance. Throws if insufficient funds or account is closed.</summary>
    public void Debit(decimal amount)
    {
        EnsureOpen();
        if (amount <= 0) throw new ArgumentOutOfRangeException(nameof(amount), "Debit amount must be positive.");
        if (amount > Balance)  throw new InvalidOperationException($"Insufficient funds. Balance: {Balance:C}, Requested: {amount:C}");
        Balance -= amount;
    }

    /// <summary>Closes the account if the balance is zero.</summary>
    public void Close()
    {
        if (Balance != 0) throw new InvalidOperationException("Account must have a zero balance before closing.");
        IsOpen = false;
    }

    /// <summary>Reopens a previously closed account.</summary>
    public void Reopen() => IsOpen = true;

    private void EnsureOpen()
    {
        if (!IsOpen) throw new InvalidOperationException("Cannot operate on a closed account.");
    }
}
