using MassTransit;
using MassTransitDemo.Contracts.Events;
using Microsoft.AspNetCore.Mvc;

namespace MassTransitDemo.Api.Controllers;

/// <summary>
/// Sipariş controller'ı — event publish eder.
/// Consumer'ları tanımaz, sadece event gönderir.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    /// <summary>
    /// IBus — MassTransit message bus, event göndermek için kullanılır
    /// </summary>
    private readonly IBus _bus;

    public OrdersController(IBus bus)
    {
        _bus = bus;
    }

    /// <summary>
    /// Sipariş oluşturur ve OrderCreatedEvent publish eder.
    /// Consumer'lar bu event'i alır ve işler.
    /// POST /api/orders
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateOrderRequest request)
    {
        // Simülasyon: sipariş kaydedildi
        var orderId = new Random().Next(1000, 9999);

        // Event publish et — consumer'lar otomatik alır
        await _bus.Publish(new OrderCreatedEvent
        {
            OrderId = orderId,
            CustomerName = request.CustomerName,
            TotalAmount = request.TotalAmount
        });

        return Ok(new { OrderId = orderId, Message = "Sipariş oluşturuldu, event gönderildi" });
    }

    /// <summary>
    /// Kargo eventi publish eder
    /// POST /api/orders/{id}/ship
    /// </summary>
    [HttpPost("{id}/ship")]
    public async Task<IActionResult> Ship(int id)
    {
        await _bus.Publish(new OrderShippedEvent
        {
            OrderId = id,
            TrackingNumber = $"TRK{new Random().Next(100000, 999999)}"
        });

        return Ok(new { OrderId = id, Message = "Kargo eventi gönderildi" });
    }
}

public record CreateOrderRequest(string CustomerName, decimal TotalAmount);