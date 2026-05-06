using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;

namespace ApiVersioning.Api.Controllers.V2;

/// <summary>
/// Ürün controller'ı — V2.
/// V2'de Price alanı eklendi ve yeni endpoint geldi.
/// V1 bozulmadı — eski istemciler etkilenmedi.
/// </summary>
[ApiController]
[ApiVersion("2.0")]
[Route("api/v{version:apiVersion}/[controller]")]
public class ProductsController : ControllerBase
{
    /// <summary>
    /// V2 ürün listesi — Id, Name ve Price
    /// GET /api/v2/products
    /// </summary>
    [HttpGet]
    public IActionResult GetAll()
    {
        var products = new[]
        {
            new { Id = 1, Name = "Laptop", Price = 999.99 },
            new { Id = 2, Name = "Mouse", Price = 29.99 }
        };

        return Ok(products);
    }

    /// <summary>
    /// V2'de eklenen yeni endpoint — V1'de yok
    /// GET /api/v2/products/{id}
    /// </summary>
    [HttpGet("{id}")]
    public IActionResult GetById(int id)
    {
        return Ok(new { Id = id, Name = $"Product {id}", Price = id * 10.0 });
    }
}