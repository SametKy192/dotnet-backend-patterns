using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace RateLimiting.Api.Controllers;

/// <summary>
/// Rate limiting demo controller.
/// Her endpoint farklı policy ile korunur.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    /// <summary>
    /// Fixed window policy — 10 saniyede max 5 istek
    /// Limit aşılırsa 429 Too Many Requests döner
    /// GET /api/products
    /// </summary>
    [HttpGet]
    [EnableRateLimiting("fixed")]
    public IActionResult GetAll()
    {
        return Ok(new[] { new { Id = 1, Name = "Laptop" } });
    }

    /// <summary>
    /// Sliding window policy — daha smooth rate limiting
    /// Pencere kayar, ani spike'ları daha iyi yönetir
    /// GET /api/products/search
    /// </summary>
    [HttpGet("search")]
    [EnableRateLimiting("sliding")]
    public IActionResult Search([FromQuery] string q)
    {
        return Ok(new { Query = q, Results = new[] { "Laptop", "Mouse" } });
    }

    /// <summary>
    /// Token bucket policy — burst trafiğe izin verir
    /// Bucket dolu ise birden fazla istek geçebilir
    /// POST /api/products
    /// </summary>
    [HttpPost]
    [EnableRateLimiting("token")]
    public IActionResult Create([FromBody] object product)
    {
        return Ok(new { Message = "Ürün oluşturuldu" });
    }

    /// <summary>
    /// Rate limiting yok — her zaman erişilebilir
    /// GET /api/products/health
    /// </summary>
    [HttpGet("health")]
    [DisableRateLimiting]
    public IActionResult Health()
    {
        return Ok(new { Status = "Healthy" });
    }
}