namespace CommandPattern.Api.Models;

// ── Inbound request DTOs ──────────────────────────────────────────────────────

/// <summary>Creates a new bank account.</summary>
public record CreateAccountRequest(string Owner, decimal InitialDeposit);

/// <summary>Deposits an amount into an account.</summary>
public record DepositRequest(Guid AccountId, decimal Amount);

/// <summary>Withdraws an amount from an account.</summary>
public record WithdrawRequest(Guid AccountId, decimal Amount);

/// <summary>Transfers an amount between two accounts.</summary>
public record TransferRequest(Guid FromAccountId, Guid ToAccountId, decimal Amount);
