using FluentAssertions;
using CommandPattern.Api.Commands;
using CommandPattern.Api.Models;
using CommandPattern.Api.Services;

namespace CommandPattern.Tests;

/// <summary>
/// 26 unit tests covering BankAccount receiver operations, all three command types,
/// the BankCommandInvoker undo/redo stack, and composite scenarios.
/// </summary>
public class CommandPatternTests
{
    // ── Helpers ───────────────────────────────────────────────────────────────

    private static BankAccount NewAccount(decimal balance = 0m, string owner = "Alice")
    {
        var acc = new BankAccount { Owner = owner };
        if (balance > 0) acc.Credit(balance);
        return acc;
    }

    private static BankCommandInvoker NewInvoker() => new();

    // ══════════════════════════════════════════════════════════════════════════
    // 1. BankAccount — receiver guard clauses
    // ══════════════════════════════════════════════════════════════════════════

    [Fact]
    public void Credit_IncreasesBalance()
    {
        var acc = new BankAccount { Owner = "Alice" };
        acc.Credit(500m);
        acc.Balance.Should().Be(500m);
    }

    [Fact]
    public void Debit_DecreasesBalance()
    {
        var acc = NewAccount(200m);
        acc.Debit(80m);
        acc.Balance.Should().Be(120m);
    }

    [Fact]
    public void Debit_ThrowsOnInsufficientFunds()
    {
        var acc = NewAccount(100m);
        acc.Invoking(a => a.Debit(200m))
            .Should().Throw<InvalidOperationException>()
            .WithMessage("*Insufficient funds*");
    }

    [Fact]
    public void Credit_ThrowsOnClosedAccount()
    {
        var acc = NewAccount(0m);
        acc.Close();
        acc.Invoking(a => a.Credit(100m))
            .Should().Throw<InvalidOperationException>()
            .WithMessage("*closed*");
    }

    [Fact]
    public void Close_ThrowsWhenBalanceIsNotZero()
    {
        var acc = NewAccount(50m);
        acc.Invoking(a => a.Close())
            .Should().Throw<InvalidOperationException>()
            .WithMessage("*zero balance*");
    }

    // ══════════════════════════════════════════════════════════════════════════
    // 2. DepositCommand
    // ══════════════════════════════════════════════════════════════════════════

    [Fact]
    public void DepositCommand_Execute_IncreasesBalance()
    {
        var acc     = NewAccount();
        var invoker = NewInvoker();
        invoker.Execute(new DepositCommand(acc, 300m));
        acc.Balance.Should().Be(300m);
    }

    [Fact]
    public void DepositCommand_Undo_RestoresBalance()
    {
        var acc     = NewAccount();
        var invoker = NewInvoker();
        invoker.Execute(new DepositCommand(acc, 300m));
        invoker.Undo();
        acc.Balance.Should().Be(0m);
    }

    [Fact]
    public void DepositCommand_Name_IsDeposit()
    {
        var acc = NewAccount();
        new DepositCommand(acc, 100m).Name.Should().Be("Deposit");
    }

    // ══════════════════════════════════════════════════════════════════════════
    // 3. WithdrawCommand
    // ══════════════════════════════════════════════════════════════════════════

    [Fact]
    public void WithdrawCommand_Execute_DecreasesBalance()
    {
        var acc     = NewAccount(500m);
        var invoker = NewInvoker();
        invoker.Execute(new WithdrawCommand(acc, 200m));
        acc.Balance.Should().Be(300m);
    }

    [Fact]
    public void WithdrawCommand_Undo_RestoresBalance()
    {
        var acc     = NewAccount(500m);
        var invoker = NewInvoker();
        invoker.Execute(new WithdrawCommand(acc, 200m));
        invoker.Undo();
        acc.Balance.Should().Be(500m);
    }

    [Fact]
    public void WithdrawCommand_ThrowsOnInsufficientFunds_CommandNotRecorded()
    {
        var acc     = NewAccount(100m);
        var invoker = NewInvoker();
        invoker.Invoking(i => i.Execute(new WithdrawCommand(acc, 500m)))
            .Should().Throw<InvalidOperationException>();
        invoker.History.Should().BeEmpty(); // failed command should not be in history
    }

    [Fact]
    public void WithdrawCommand_Name_IsWithdraw()
    {
        var acc = NewAccount(100m);
        new WithdrawCommand(acc, 50m).Name.Should().Be("Withdraw");
    }

    // ══════════════════════════════════════════════════════════════════════════
    // 4. TransferCommand
    // ══════════════════════════════════════════════════════════════════════════

    [Fact]
    public void TransferCommand_Execute_MovesMoneyBetweenAccounts()
    {
        var from    = NewAccount(1000m, "Alice");
        var to      = NewAccount(200m,  "Bob");
        var invoker = NewInvoker();

        invoker.Execute(new TransferCommand(from, to, 400m));

        from.Balance.Should().Be(600m);
        to.Balance.Should().Be(600m);
    }

    [Fact]
    public void TransferCommand_Undo_ReversesBothAccounts()
    {
        var from    = NewAccount(1000m, "Alice");
        var to      = NewAccount(200m,  "Bob");
        var invoker = NewInvoker();

        invoker.Execute(new TransferCommand(from, to, 400m));
        invoker.Undo();

        from.Balance.Should().Be(1000m);
        to.Balance.Should().Be(200m);
    }

    [Fact]
    public void TransferCommand_Name_IsTransfer()
    {
        var a = NewAccount(100m);
        var b = NewAccount(100m);
        new TransferCommand(a, b, 50m).Name.Should().Be("Transfer");
    }

    // ══════════════════════════════════════════════════════════════════════════
    // 5. BankCommandInvoker — undo / redo stack mechanics
    // ══════════════════════════════════════════════════════════════════════════

    [Fact]
    public void Invoker_Undo_ThrowsWhenNothingToUndo()
    {
        var invoker = NewInvoker();
        invoker.Invoking(i => i.Undo())
            .Should().Throw<InvalidOperationException>()
            .WithMessage("Nothing to undo*");
    }

    [Fact]
    public void Invoker_Redo_ThrowsWhenNothingToRedo()
    {
        var invoker = NewInvoker();
        invoker.Invoking(i => i.Redo())
            .Should().Throw<InvalidOperationException>()
            .WithMessage("Nothing to redo*");
    }

    [Fact]
    public void Invoker_Redo_ReappliesUndoneCommand()
    {
        var acc     = NewAccount();
        var invoker = NewInvoker();

        invoker.Execute(new DepositCommand(acc, 500m));
        invoker.Undo();
        invoker.Redo();

        acc.Balance.Should().Be(500m);
    }

    [Fact]
    public void Invoker_NewExecute_ClearsRedoStack()
    {
        var acc     = NewAccount();
        var invoker = NewInvoker();

        invoker.Execute(new DepositCommand(acc, 100m));
        invoker.Undo();

        // New command clears the redo stack
        invoker.Execute(new DepositCommand(acc, 200m));

        invoker.CanRedo.Should().BeFalse();
    }

    [Fact]
    public void Invoker_CanUndo_FalseInitially()
    {
        NewInvoker().CanUndo.Should().BeFalse();
    }

    [Fact]
    public void Invoker_CanRedo_FalseInitially()
    {
        NewInvoker().CanRedo.Should().BeFalse();
    }

    [Fact]
    public void Invoker_History_RecordsAllExecutedCommands()
    {
        var acc     = NewAccount();
        var invoker = NewInvoker();

        invoker.Execute(new DepositCommand(acc, 100m));
        invoker.Execute(new DepositCommand(acc, 200m));

        invoker.History.Should().HaveCount(2);
        invoker.History[0].CommandName.Should().Be("Deposit");
    }

    [Fact]
    public void Invoker_History_MarksUndoneCommandsCorrectly()
    {
        var acc     = NewAccount();
        var invoker = NewInvoker();

        invoker.Execute(new DepositCommand(acc, 100m));
        invoker.Undo();

        invoker.History.Should().ContainSingle(e => e.IsUndone);
    }

    // ══════════════════════════════════════════════════════════════════════════
    // 6. Composite / end-to-end scenarios
    // ══════════════════════════════════════════════════════════════════════════

    [Fact]
    public void MultipleCommands_UndoInReverseOrder()
    {
        var acc     = NewAccount();
        var invoker = NewInvoker();

        invoker.Execute(new DepositCommand(acc,  500m)); // balance: 500
        invoker.Execute(new WithdrawCommand(acc, 200m)); // balance: 300

        invoker.Undo(); // undo withdraw → balance: 500
        acc.Balance.Should().Be(500m);

        invoker.Undo(); // undo deposit → balance: 0
        acc.Balance.Should().Be(0m);
    }

    [Fact]
    public void TransferThenUndoThenRedo_BalancesCorrect()
    {
        var alice   = NewAccount(1000m, "Alice");
        var bob     = NewAccount(0m,    "Bob");
        var invoker = NewInvoker();

        invoker.Execute(new TransferCommand(alice, bob, 300m));
        invoker.Undo();
        invoker.Redo();

        alice.Balance.Should().Be(700m);
        bob.Balance.Should().Be(300m);
    }
}
