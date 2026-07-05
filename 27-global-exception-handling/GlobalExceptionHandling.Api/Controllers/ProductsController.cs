using GlobalExceptionHandling.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace GlobalExceptionHandling.Api.Controllers;

/// <summary>
/// Ürün controller'ı — try-catch YOK.
/// Tüm exception'lar global handler tarafından yakalanır.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly ProductService _productService;

    public ProductsController(ProductService productService)
    {
        _productService = productService;
    }

    /// <summary>
    /// Tüm ürünleri getir
    /// GET /api/products
    /// </summary>
    [HttpGet]
    public IActionResult GetAll()
    {
        return Ok(_productService.GetAll());
    }

    /// <summary>
    /// Id ile ürün getir — bulunamazsa global handler 404 döner
    /// GET /api/products/{id}
    /// </summary>
    [HttpGet("{id}")]
    public IActionResult GetById(int id)
    {
        // NotFoundException fırlatılabilir — try-catch YOK
        var product = _productService.GetById(id);
        return Ok(product);
    }

    /// <summary>
    /// Yeni ürün oluştur — validation hatası olursa global handler 400 döner
    /// POST /api/products
    /// </summary>
    [HttpPost]
    public IActionResult Create([FromBody] CreateProductRequest request)
    {
        // ValidationException veya ConflictException fırlatılabilir
        var product = _productService.Create(request.Name, request.Price);
        return CreatedAtAction(nameof(GetById), new { id = product.Id }, product);
    }

    /// <summary>
    /// Ürün sil — sadece Admin yapabilir
    /// DELETE /api/products/{id}?role=Admin
    /// </summary>
    [HttpDelete("{id}")]
    public IActionResult Delete(int id, [FromQuery] string role = "User")
    {
        // ForbiddenException veya NotFoundException fırlatılabilir
        _productService.Delete(id, role);
        return NoContent();
    }

    /// <summary>
    /// Beklenmedik hata simülasyonu — global handler 500 döner
    /// GET /api/products/error
    /// </summary>
    [HttpGet("error")]
    public IActionResult SimulateError()
    {
        throw new InvalidOperationException("Bu beklenmedik bir hata simülasyonudur.");
    }
}

public record CreateProductRequest(string Name, decimal Price);