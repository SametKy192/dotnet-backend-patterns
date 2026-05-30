using Microsoft.AspNetCore.Mvc;

namespace DockerDemo.Api.Controllers;

/// <summary>
/// Basit ürün controller'ı — Docker demo için
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    /// <summary>
    /// Ürün listesi — ortam değişkenini de döndürür
    /// GET /api/products
    /// </summary>
    [HttpGet]
    public IActionResult GetAll()
    {
        // Ortam değişkeni — Docker'da container içinden okunur
        var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Unknown";

        return Ok(new
        {
            Environment = environment,
            Products = new[]
            {
                new { Id = 1, Name = "Laptop", Price = 999.99 },
                new { Id = 2, Name = "Mouse", Price = 29.99 }
            }
        });
    }

    /// <summary>
    /// Health check endpoint
    /// GET /api/products/health
    /// </summary>
    [HttpGet("health")]
    public IActionResult Health()
    {
        return Ok(new { Status = "Healthy", Time = DateTime.UtcNow });
    }
}