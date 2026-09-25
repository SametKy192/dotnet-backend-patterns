using CommandPattern.Api.Models;

namespace CommandPattern.Api.Commands;

/// <summary>
/// Transfers money between two accounts as an atomic operation.
/// Undo: debits the destination and credits the source.
/// </summary>
public class TransferCommand(BankAccount from, BankAccount to, decimal amount) : IBankCommand
{
    public string Name => "Transfer";
    public string Description =>
        $"Transfer {amount:C} from account {from.Id} ({from.Owner}) to account {to.Id} ({to.Owner})";

    public void Execute()
    {
        from.Debit(amount);
        to.Credit(amount);
    }

    public void Undo()
    {
        to.Debit(amount);
        from.Credit(amount);
    }
}
