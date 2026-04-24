using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using OutboxPattern.Domain.Entities;
using OutboxPattern.Infrastructure.Persistence;

namespace OutboxPattern.Api.Controllers;

/// <summary>
/// Sipariş controller'ı.
/// Sipariş ve outbox mesajı aynı transaction içinde kaydedilir.
/// Servis çökse bile mesaj kaybolmaz.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly AppDbContext _dbContext;

    public OrdersController(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    /// <summary>
    /// Yeni sipariş oluşturur.
    /// Sipariş + OutboxMessage aynı transaction'da kaydedilir.
    /// POST /api/orders
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Create(Order order)
    {
        // Transaction başlat — ikisi birlikte commit ya da ikisi rollback
        await using var transaction = await _dbContext.Database.BeginTransactionAsync();

        // Siparişi kaydet
        _dbContext.Orders.Add(order);

        // Outbox mesajı oluştur — event bilgisini JSON olarak sakla
        var outboxMessage = new OutboxMessage
        {
            EventType = "OrderCreated",
            Payload = JsonConvert.SerializeObject(new
            {
                order.CustomerName,
                order.TotalAmount,
                order.CreatedAt
            })
        };

        _dbContext.OutboxMessages.Add(outboxMessage);

        // İkisini birlikte kaydet
        await _dbContext.SaveChangesAsync();
        await transaction.CommitAsync();

        return CreatedAtAction(nameof(GetAll), new { id = order.Id }, order);
    }

    /// <summary>
    /// Tüm siparişleri getirir
    /// GET /api/orders
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var orders = await _dbContext.Orders.ToListAsync();
        return Ok(orders);
    }

    /// <summary>
    /// Outbox mesajlarını getirir — işlenmiş ve işlenmemiş
    /// GET /api/orders/outbox
    /// </summary>
    [HttpGet("outbox")]
    public async Task<IActionResult> GetOutbox()
    {
        var messages = await _dbContext.OutboxMessages.ToListAsync();
        return Ok(messages);
    }
}