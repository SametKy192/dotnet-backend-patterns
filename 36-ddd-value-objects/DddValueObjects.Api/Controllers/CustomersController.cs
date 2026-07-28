using DddValueObjects.Api.Data;
using DddValueObjects.Api.Domain.Entities;
using DddValueObjects.Api.Domain.ValueObjects;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DddValueObjects.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CustomersController : ControllerBase
{
    private readonly AppDbContext _context;

    public CustomersController(AppDbContext context)
    {
        _context = context;
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateCustomerRequest request)
    {
        try
        {
            // Build Value Objects (validation occurs in constructors)
            var email = new Email(request.Email);
            var address = new Address(request.Street, request.City, request.ZipCode, request.Country);
            var initialBalance = new Money(request.BalanceAmount, request.BalanceCurrency);

            // Construct Entity (validation occurs in constructor)
            var customer = new Customer(request.Name, email, address, initialBalance);

            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = customer.Id }, MapToResponse(customer));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { Error = "Domain Validation Failed", Details = ex.Message });
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var customer = await _context.Customers.FindAsync(id);
        if (customer == null)
        {
            return NotFound();
        }

        return Ok(MapToResponse(customer));
    }

    [HttpPost("{id}/deposit")]
    public async Task<IActionResult> Deposit(int id, [FromBody] TransactionRequest request)
    {
        var customer = await _context.Customers.FindAsync(id);
        if (customer == null)
        {
            return NotFound();
        }

        try
        {
            var money = new Money(request.Amount, request.Currency);
            customer.Deposit(money);

            await _context.SaveChangesAsync();
            return Ok(MapToResponse(customer));
        }
        catch (Exception ex) when (ex is ArgumentException or InvalidOperationException)
        {
            return BadRequest(new { Error = "Domain Operation Failed", Details = ex.Message });
        }
    }

    [HttpPost("{id}/withdraw")]
    public async Task<IActionResult> Withdraw(int id, [FromBody] TransactionRequest request)
    {
        var customer = await _context.Customers.FindAsync(id);
        if (customer == null)
        {
            return NotFound();
        }

        try
        {
            var money = new Money(request.Amount, request.Currency);
            customer.Withdraw(money);

            await _context.SaveChangesAsync();
            return Ok(MapToResponse(customer));
        }
        catch (Exception ex) when (ex is ArgumentException or InvalidOperationException)
        {
            return BadRequest(new { Error = "Domain Operation Failed", Details = ex.Message });
        }
    }

    private static CustomerResponse MapToResponse(Customer customer) => new(
        customer.Id,
        customer.Name,
        customer.Email.Value,
        customer.BillingAddress.ToString(),
        customer.Balance.Amount,
        customer.Balance.Currency
    );
}

// Request and Response DTOs
public record CreateCustomerRequest(
    string Name,
    string Email,
    string Street,
    string City,
    string ZipCode,
    string Country,
    decimal BalanceAmount,
    string BalanceCurrency
);

public record TransactionRequest(decimal Amount, string Currency);

public record CustomerResponse(
    int Id,
    string Name,
    string Email,
    string BillingAddress,
    decimal BalanceAmount,
    string BalanceCurrency
);
