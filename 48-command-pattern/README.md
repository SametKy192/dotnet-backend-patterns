# 48 — Command Pattern

Demonstrates the **Command** design pattern in ASP.NET Core — a behavioral pattern that encapsulates a request as an object, enabling parameterisation, queueing, logging, and **undo/redo** support.

## Concept

Instead of calling methods directly on the receiver (a `BankAccount`), every operation is wrapped in a command object that knows both how to `Execute` and how to `Undo`. An **Invoker** (`BankCommandInvoker`) manages the undo stack and redo stack, decoupling the caller from the receiver entirely.

```
Client (Controller)
      │
      ▼
BankCommandInvoker (Invoker)
      │  Execute(command)  →  IBankCommand.Execute()  →  BankAccount (Receiver)
      │  Undo()            →  IBankCommand.Undo()     →  BankAccount (Receiver)
      │
      ├── DepositCommand   — Credit / Debit
      ├── WithdrawCommand  — Debit  / Credit
      └── TransferCommand  — Debit+Credit / Credit+Debit
```

## Project Structure

```
48-command-pattern/
├── CommandPattern.Api/
│   ├── Models/
│   │   ├── BankAccount.cs           # Receiver — Credit, Debit, Close, Reopen
│   │   ├── CommandHistoryEntry.cs   # History record (step, name, isUndone)
│   │   └── Requests.cs             # Inbound DTO records
│   ├── Commands/
│   │   ├── IBankCommand.cs          # Command interface (Execute + Undo)
│   │   ├── DepositCommand.cs        # Credit → Undo: Debit
│   │   ├── WithdrawCommand.cs       # Debit  → Undo: Credit
│   │   └── TransferCommand.cs       # Debit+Credit → Undo: reverse both
│   ├── Services/
│   │   ├── BankCommandInvoker.cs    # Invoker — undo/redo stacks + history
│   │   └── AccountRepository.cs    # In-memory store + shared Invoker
│   ├── Controllers/
│   │   └── AccountsController.cs   # 9 endpoints
│   └── Program.cs
└── CommandPattern.Tests/
    └── CommandPatternTests.cs       # 25 unit tests
```

## Key Abstractions

### `IBankCommand`
```csharp
public interface IBankCommand
{
    string Name        { get; }
    string Description { get; }
    void Execute();
    void Undo();
}
```

### `BankCommandInvoker`
```csharp
public class BankCommandInvoker
{
    private readonly Stack<IBankCommand> _undoStack = new();
    private readonly Stack<IBankCommand> _redoStack = new();

    public void Execute(IBankCommand command) { command.Execute(); _undoStack.Push(command); _redoStack.Clear(); }
    public void Undo()                        { var cmd = _undoStack.Pop(); cmd.Undo(); _redoStack.Push(cmd); }
    public void Redo()                        { var cmd = _redoStack.Pop(); cmd.Execute(); _undoStack.Push(cmd); }
}
```

> [!NOTE] A new `Execute` call clears the redo stack — matching standard text-editor undo/redo behaviour.

## Commands at a Glance

| Command            | Execute                      | Undo                         |
|--------------------|------------------------------|------------------------------|
| **Deposit**        | `account.Credit(amount)`     | `account.Debit(amount)`      |
| **Withdraw**       | `account.Debit(amount)`      | `account.Credit(amount)`     |
| **Transfer**       | `from.Debit` + `to.Credit`   | `to.Debit` + `from.Credit`   |

## API Endpoints

| Method | Route                         | Description                                    |
|--------|-------------------------------|------------------------------------------------|
| POST   | `/api/accounts`               | Create a new account (optional initial deposit)|
| GET    | `/api/accounts`               | List all accounts                              |
| GET    | `/api/accounts/{id}`          | Get account by ID                              |
| POST   | `/api/accounts/deposit`       | Deposit money → executes DepositCommand        |
| POST   | `/api/accounts/withdraw`      | Withdraw money → executes WithdrawCommand      |
| POST   | `/api/accounts/transfer`      | Transfer money → executes TransferCommand      |
| POST   | `/api/accounts/undo`          | Undo the last command                          |
| POST   | `/api/accounts/redo`          | Redo the last undone command                   |
| GET    | `/api/accounts/history`       | Full command execution history                 |

## Running the API

```bash
dotnet run --project CommandPattern.Api
```

## Running Tests

```bash
dotnet test
# Başarılı! - Başarısız: 0, Başarılı: 25, Atlanan: 0
```

## Benefits vs. Direct Method Calls

| Concern              | Direct Calls                              | Command Pattern                               |
|----------------------|-------------------------------------------|-----------------------------------------------|
| Undo support         | Must track state manually                 | Each command knows its own inverse            |
| Audit trail          | Custom logging added per method           | History recorded automatically by the invoker |
| Composability        | Coupling between caller and receiver      | Commands are independent, composable objects  |
| Testing              | Must test through the full service        | Each command unit-tested in isolation         |
| Open/Closed          | Add new op → modify caller or dispatcher  | Add new command class, zero other changes     |
