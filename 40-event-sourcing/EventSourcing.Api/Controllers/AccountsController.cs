using EventSourcing.Api.Domain;
using EventSourcing.Api.Infrastructure;
using EventSourcing.Api.Projections;
using Microsoft.AspNetCore.Mvc;

namespace EventSourcing.Api.Controllers;

public record OpenAccountRequest(string OwnerName, decimal InitialBalance);
public record DepositRequest(decimal Amount);
public record WithdrawRequest(decimal Amount);

[ApiController]
[Route("api/accounts")]
public class AccountsController : ControllerBase
{
    private readonly InMemoryEventStore _eventStore;
    private readonly AccountProjection _projection;

    public AccountsController(InMemoryEventStore eventStore, AccountProjection projection)
    {
        _eventStore = eventStore;
        _projection = projection;
    }

    /// <summary>Opens a new bank account and persists the AccountOpenedEvent.</summary>
    [HttpPost]
    public IActionResult Open(OpenAccountRequest request)
    {
        var accountId = Guid.NewGuid().ToString("N")[..8];
        var account = BankAccount.Open(accountId, request.OwnerName, request.InitialBalance);
        _eventStore.Append(accountId, account.UncommittedEvents);
        account.ClearUncommittedEvents();
        return CreatedAtAction(nameof(GetSummary), new { id = accountId },
            new { accountId, message = "Account opened" });
    }

    /// <summary>Deposits money: raises MoneyDepositedEvent.</summary>
    [HttpPost("{id}/deposit")]
    public IActionResult Deposit(string id, DepositRequest request)
    {
        var events = _eventStore.LoadEvents(id);
        if (!events.Any()) return NotFound($"Account '{id}' not found.");
        var account = BankAccount.Rehydrate(events);
        account.Deposit(request.Amount);
        _eventStore.Append(id, account.UncommittedEvents);
        account.ClearUncommittedEvents();
        return Ok(new { message = $"Deposited {request.Amount:C}", newBalance = account.Balance });
    }

    /// <summary>Withdraws money: raises MoneyWithdrawnEvent.</summary>
    [HttpPost("{id}/withdraw")]
    public IActionResult Withdraw(string id, WithdrawRequest request)
    {
        var events = _eventStore.LoadEvents(id);
        if (!events.Any()) return NotFound($"Account '{id}' not found.");
        var account = BankAccount.Rehydrate(events);
        try { account.Withdraw(request.Amount); }
        catch (InvalidOperationException ex) { return BadRequest(ex.Message); }
        _eventStore.Append(id, account.UncommittedEvents);
        account.ClearUncommittedEvents();
        return Ok(new { message = $"Withdrew {request.Amount:C}", newBalance = account.Balance });
    }

    /// <summary>Closes the account: raises AccountClosedEvent.</summary>
    [HttpPost("{id}/close")]
    public IActionResult Close(string id)
    {
        var events = _eventStore.LoadEvents(id);
        if (!events.Any()) return NotFound($"Account '{id}' not found.");
        var account = BankAccount.Rehydrate(events);
        try { account.Close(); }
        catch (InvalidOperationException ex) { return BadRequest(ex.Message); }
        _eventStore.Append(id, account.UncommittedEvents);
        account.ClearUncommittedEvents();
        return Ok(new { message = "Account closed." });
    }

    /// <summary>Returns the projected account summary (current state from events).</summary>
    [HttpGet("{id}")]
    public IActionResult GetSummary(string id)
    {
        var summary = _projection.Project(id);
        return summary is null ? NotFound() : Ok(summary);
    }

    /// <summary>Returns the full raw event log for an account.</summary>
    [HttpGet("{id}/events")]
    public IActionResult GetEvents(string id)
    {
        var events = _eventStore.LoadEvents(id);
        return Ok(events);
    }

    /// <summary>Returns all events in the entire event store.</summary>
    [HttpGet("store/all")]
    public IActionResult GetAllEvents() => Ok(_eventStore.GetAll());
}
