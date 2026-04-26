using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;

namespace ApiVersioning.Api.Controllers.V1;

/// <summary>
/// Ürün controller'ı — V1.
/// V1'de sadece temel alanlar var: Id ve Name.
/// Eski istemciler bu versiyonu kullanmaya devam eder.
/// </summary>
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
public class ProductsController : ControllerBase
{
    /// <summary>
    /// V1 ürün listesi — sadece Id ve Name
    /// GET /api/v1/products
    /// </summary>
    [HttpGet]
    public IActionResult GetAll()
    {
        var products = new[]
        {
            new { Id = 1, Name = "Laptop" },
            new { Id = 2, Name = "Mouse" }
        };

        return Ok(products);
    }
}