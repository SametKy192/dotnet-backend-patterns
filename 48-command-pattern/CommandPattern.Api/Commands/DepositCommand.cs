using CommandPattern.Api.Models;

namespace CommandPattern.Api.Commands;

/// <summary>
/// Deposits money into an account.
/// Undo: debits the same amount back.
/// </summary>
public class DepositCommand(BankAccount account, decimal amount) : IBankCommand
{
    public string Name        => "Deposit";
    public string Description => $"Deposit {amount:C} to account {account.Id} ({account.Owner})";

    public void Execute() => account.Credit(amount);
    public void Undo()    => account.Debit(amount);
}
