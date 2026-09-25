using CommandPattern.Api.Models;

namespace CommandPattern.Api.Commands;

/// <summary>
/// Withdraws money from an account.
/// Undo: deposits the same amount back.
/// </summary>
public class WithdrawCommand(BankAccount account, decimal amount) : IBankCommand
{
    public string Name        => "Withdraw";
    public string Description => $"Withdraw {amount:C} from account {account.Id} ({account.Owner})";

    public void Execute() => account.Debit(amount);
    public void Undo()    => account.Credit(amount);
}
