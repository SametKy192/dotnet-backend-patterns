using Microsoft.AspNetCore.Mvc;

namespace YarpGateway.OrderService.Controllers;

/// <summary>
/// Sipariş mikro servisi — port 5001'de çalışır
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    [HttpGet]
    public IActionResult GetAll()
    {
        return Ok(new[]
        {
            new { Id = 1, CustomerName = "Ahmet", Total = 299.99, Service = "OrderService:5001" },
            new { Id = 2, CustomerName = "Mehmet", Total = 149.99, Service = "OrderService:5001" }
        });
    }

    [HttpGet("{id}")]
    public IActionResult GetById(int id)
    {
        return Ok(new { Id = id, CustomerName = "Ahmet", Total = 299.99, Service = "OrderService:5001" });
    }
}