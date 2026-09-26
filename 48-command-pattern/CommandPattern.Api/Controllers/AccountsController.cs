using Microsoft.AspNetCore.Mvc;
using CommandPattern.Api.Commands;
using CommandPattern.Api.Models;
using CommandPattern.Api.Services;

namespace CommandPattern.Api.Controllers;

/// <summary>
/// REST façade for the Command Pattern demo.
/// All mutations are encapsulated as IBankCommand objects and routed through
/// the shared BankCommandInvoker — enabling full undo/redo support via HTTP.
/// </summary>
[ApiController]
[Route("api/accounts")]
public class AccountsController(AccountRepository repo) : ControllerBase
{
    // ── Accounts ──────────────────────────────────────────────────────────────

    /// <summary>Creates a new bank account with an optional initial deposit.</summary>
    [HttpPost]
    public IActionResult Create([FromBody] CreateAccountRequest req)
    {
        if (string.IsNullOrWhiteSpace(req.Owner))
            return BadRequest(new { error = "Owner name is required." });
        if (req.InitialDeposit < 0)
            return BadRequest(new { error = "Initial deposit cannot be negative." });

        var account = repo.Add(new BankAccount { Owner = req.Owner });

        if (req.InitialDeposit > 0)
            repo.Invoker.Execute(new DepositCommand(account, req.InitialDeposit));

        return Ok(ToDto(account));
    }

    /// <summary>Lists all accounts.</summary>
    [HttpGet]
    public IActionResult GetAll() => Ok(repo.All().Select(ToDto));

    /// <summary>Returns a single account by ID.</summary>
    [HttpGet("{id:guid}")]
    public IActionResult GetById(Guid id)
    {
        var account = repo.Find(id);
        return account is null ? NotFound() : Ok(ToDto(account));
    }

    // ── Commands ──────────────────────────────────────────────────────────────

    /// <summary>Deposits money into an account.</summary>
    [HttpPost("deposit")]
    public IActionResult Deposit([FromBody] DepositRequest req)
    {
        var account = repo.Find(req.AccountId);
        if (account is null) return NotFound(new { error = $"Account {req.AccountId} not found." });

        try
        {
            repo.Invoker.Execute(new DepositCommand(account, req.Amount));
            return Ok(new { message = $"Deposited {req.Amount:C} successfully.", account = ToDto(account) });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>Withdraws money from an account.</summary>
    [HttpPost("withdraw")]
    public IActionResult Withdraw([FromBody] WithdrawRequest req)
    {
        var account = repo.Find(req.AccountId);
        if (account is null) return NotFound(new { error = $"Account {req.AccountId} not found." });

        try
        {
            repo.Invoker.Execute(new WithdrawCommand(account, req.Amount));
            return Ok(new { message = $"Withdrew {req.Amount:C} successfully.", account = ToDto(account) });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>Transfers money between two accounts.</summary>
    [HttpPost("transfer")]
    public IActionResult Transfer([FromBody] TransferRequest req)
    {
        var from = repo.Find(req.FromAccountId);
        var to   = repo.Find(req.ToAccountId);

        if (from is null) return NotFound(new { error = $"Source account {req.FromAccountId} not found." });
        if (to   is null) return NotFound(new { error = $"Destination account {req.ToAccountId} not found." });

        try
        {
            repo.Invoker.Execute(new TransferCommand(from, to, req.Amount));
            return Ok(new
            {
                message = $"Transferred {req.Amount:C} successfully.",
                from    = ToDto(from),
                to      = ToDto(to)
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    // ── Undo / Redo ───────────────────────────────────────────────────────────

    /// <summary>Undoes the most recently executed command.</summary>
    [HttpPost("undo")]
    public IActionResult Undo()
    {
        try
        {
            repo.Invoker.Undo();
            return Ok(new
            {
                message = "Last command undone.",
                canUndo = repo.Invoker.CanUndo,
                canRedo = repo.Invoker.CanRedo
            });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>Redoes the most recently undone command.</summary>
    [HttpPost("redo")]
    public IActionResult Redo()
    {
        try
        {
            repo.Invoker.Redo();
            return Ok(new
            {
                message = "Last undone command redone.",
                canUndo = repo.Invoker.CanUndo,
                canRedo = repo.Invoker.CanRedo
            });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    // ── History ───────────────────────────────────────────────────────────────

    /// <summary>Returns the full command execution history.</summary>
    [HttpGet("history")]
    public IActionResult GetHistory()
        => Ok(new
        {
            canUndo = repo.Invoker.CanUndo,
            canRedo = repo.Invoker.CanRedo,
            history = repo.Invoker.History
        });

    // ── Projection helper ─────────────────────────────────────────────────────

    private static object ToDto(BankAccount a) => new
    {
        id      = a.Id,
        owner   = a.Owner,
        balance = a.Balance,
        isOpen  = a.IsOpen
    };
}
