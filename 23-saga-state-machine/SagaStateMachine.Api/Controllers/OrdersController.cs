using MassTransit;
using Microsoft.AspNetCore.Mvc;
using SagaStateMachine.Contracts.Events;

namespace SagaStateMachine.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly IBus _bus;

    public OrdersController(IBus bus)
    {
        _bus = bus;
    }

    /// <summary>
    /// Sipariş başlat — saga tetiklenir
    /// POST /api/orders
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Submit([FromBody] SubmitOrderRequest request)
    {
        var orderId = Guid.NewGuid();

        await _bus.Publish(new OrderSubmitted
        {
            OrderId = orderId,
            CustomerName = request.CustomerName,
            TotalAmount = request.TotalAmount,
            ProductId = request.ProductId,
            Quantity = request.Quantity
        });

        return Accepted(new
        {
            OrderId = orderId,
            Message = "Sipariş alındı, işleniyor",
            Note = "ProductId=999 → stok yetersiz, Amount>1000 → ödeme başarısız"
        });
    }
}

public record SubmitOrderRequest(
    string CustomerName,
    decimal TotalAmount,
    int ProductId,
    int Quantity);