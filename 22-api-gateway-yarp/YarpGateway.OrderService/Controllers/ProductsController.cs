using Microsoft.AspNetCore.Mvc;

namespace YarpGateway.ProductService.Controllers;

/// <summary>
/// Ürün mikro servisi — port 5002'de çalışır
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    [HttpGet]
    public IActionResult GetAll()
    {
        return Ok(new[]
        {
            new { Id = 1, Name = "Laptop", Price = 999.99, Service = "ProductService:5002" },
            new { Id = 2, Name = "Mouse", Price = 29.99, Service = "ProductService:5002" }
        });
    }
}