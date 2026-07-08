using Microsoft.AspNetCore.Mvc;

namespace IdempotencyPattern.Api.Controllers;

/// <summary>
/// Sipariş controller'ı — idempotency demo.
/// Aynı Idempotency-Key ile gelen istekler cached response alır.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly ILogger<OrdersController> _logger;
    private static int _orderCounter = 0;

    public OrdersController(ILogger<OrdersController> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Sipariş oluştur — idempotent endpoint.
    /// Aynı Idempotency-Key ile tekrar çağrılırsa yeni sipariş oluşturulmaz.
    /// POST /api/orders
    /// Header: Idempotency-Key: {guid}
    /// </summary>
    [HttpPost]
    public IActionResult Create([FromBody] CreateOrderRequest request)
    {
        // Her çağrıda farklı Id üretilir
        // Idempotency middleware sayesinde aynı key ile ikinci çağrıda buraya gelinmez
        var orderId = Interlocked.Increment(ref _orderCounter);

        _logger.LogInformation("Sipariş oluşturuldu: #{OrderId} — {Customer}",
            orderId, request.CustomerName);

        return CreatedAtAction(nameof(GetById), new { id = orderId }, new
        {
            Id = orderId,
            request.CustomerName,
            request.TotalAmount,
            CreatedAt = DateTime.UtcNow,
            Message = "Sipariş başarıyla oluşturuldu"
        });
    }

    /// <summary>
    /// Sipariş getir
    /// GET /api/orders/{id}
    /// </summary>
    [HttpGet("{id}")]
    public IActionResult GetById(int id)
    {
        return Ok(new { Id = id, CustomerName = "Ahmet", TotalAmount = 299.99 });
    }

    /// <summary>
    /// Ödeme al — idempotent endpoint.
    /// Çift tıklama koruması — aynı ödeme iki kez alınamaz.
    /// POST /api/orders/{id}/payment
    /// </summary>
    [HttpPost("{id}/payment")]
    public IActionResult ProcessPayment(int id, [FromBody] PaymentRequest request)
    {
        _logger.LogInformation("Ödeme işleniyor: OrderId={OrderId}, Amount={Amount}",
            id, request.Amount);

        return Ok(new
        {
            OrderId = id,
            PaymentId = $"PAY-{Guid.NewGuid():N}",
            request.Amount,
            Status = "Completed",
            ProcessedAt = DateTime.UtcNow
        });
    }
}

public record CreateOrderRequest(string CustomerName, decimal TotalAmount);
public record PaymentRequest(decimal Amount, string CardLast4);