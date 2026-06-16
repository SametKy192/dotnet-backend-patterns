using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using OpenTelemetry.Trace;

namespace Observability.Api.Controllers;

/// <summary>
/// Sipariş controller'ı — tracing ve logging demo
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly ILogger<OrdersController> _logger;

    /// <summary>
    /// ActivitySource — distributed tracing için span oluşturur
    /// </summary>
    private static readonly ActivitySource ActivitySource = new("Observability.Api");

    public OrdersController(ILogger<OrdersController> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Sipariş listesi — structured logging demo
    /// GET /api/orders
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        // Structured logging — key-value çiftleri ile log
        _logger.LogInformation("Siparişler listeleniyor. UserId: {UserId}", User.Identity?.Name ?? "anonymous");

        // Manuel span oluştur — Jaeger'da görünür
        using var activity = ActivitySource.StartActivity("GetAllOrders");
        activity?.SetTag("http.method", "GET");
        activity?.SetTag("order.count", 3);

        await Task.Delay(10); // Simülasyon

        var orders = new[]
        {
            new { Id = 1, CustomerName = "Ahmet", Total = 299.99 },
            new { Id = 2, CustomerName = "Mehmet", Total = 149.99 },
            new { Id = 3, CustomerName = "Ayşe", Total = 499.99 }
        };

        _logger.LogInformation("{OrderCount} sipariş getirildi", orders.Length);
        return Ok(orders);
    }

    /// <summary>
    /// Sipariş detayı — nested span demo
    /// GET /api/orders/{id}
    /// </summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        // Parent span
        using var activity = ActivitySource.StartActivity("GetOrderById");
        activity?.SetTag("order.id", id);

        _logger.LogInformation("Sipariş aranıyor: {OrderId}", id);

        // Child span — DB sorgusu simülasyonu
        using (var dbActivity = ActivitySource.StartActivity("DatabaseQuery"))
        {
            dbActivity?.SetTag("db.type", "postgresql");
            dbActivity?.SetTag("db.statement", "SELECT * FROM Orders WHERE Id = @Id");
            await Task.Delay(20); // DB gecikmesi simülasyonu
        }

        if (id > 100)
        {
            _logger.LogWarning("Sipariş bulunamadı: {OrderId}", id);
            activity?.SetStatus(ActivityStatusCode.Error, "Order not found");
            return NotFound();
        }

        _logger.LogInformation("Sipariş bulundu: {OrderId}", id);
        return Ok(new { Id = id, CustomerName = "Ahmet", Total = 299.99 });
    }

    /// <summary>
    /// Sipariş oluştur — error tracking demo
    /// POST /api/orders
    /// </summary>
    [HttpPost]
    [Obsolete]
    public async Task<IActionResult> Create([FromBody] CreateOrderRequest request)
    {
        using var activity = ActivitySource.StartActivity("CreateOrder");
        activity?.SetTag("customer.name", request.CustomerName);
        activity?.SetTag("order.total", request.TotalAmount);

        try
        {
            // Validation
            if (request.TotalAmount <= 0)
            {
                _logger.LogWarning("Geçersiz sipariş tutarı: {Amount}", request.TotalAmount);
                return BadRequest("Tutar 0'dan büyük olmalıdır.");
            }

            await Task.Delay(30);

            var orderId = new Random().Next(1, 100);
            _logger.LogInformation("Sipariş oluşturuldu: #{OrderId} — {CustomerName}", orderId, request.CustomerName);

            return CreatedAtAction(nameof(GetById), new { id = orderId }, new { Id = orderId });
        }
        catch (Exception ex)
        {
            // Exception'ı span'e ekle — Jaeger'da görünür
            activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
            activity?.RecordException(ex);

            _logger.LogError(ex, "Sipariş oluşturulurken hata: {CustomerName}", request.CustomerName);
            return StatusCode(500, "Sipariş oluşturulamadı.");
        }
    }
}

public record CreateOrderRequest(string CustomerName, decimal TotalAmount);