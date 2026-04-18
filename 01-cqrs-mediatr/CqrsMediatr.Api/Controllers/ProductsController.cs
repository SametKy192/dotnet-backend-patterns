using CqrsMediatr.Application.Products.Commands.CreateProduct;
using CqrsMediatr.Application.Products.Queries.GetProducts;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CqrsMediatr.Api.Controllers;

/// <summary>
/// Ürün işlemleri için API controller'ı.
/// Controller sadece MediatR'ı tanır — handler'ları, servisleri tanımaz.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    /// <summary>
    /// MediatR — command/query'leri ilgili handler'a yönlendirir
    /// </summary>
    private readonly IMediator _mediator;

    /// <summary>
    /// IMediator dependency injection ile gelir
    /// </summary>
    public ProductsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Tüm ürünleri getirir.
    /// GET /api/products
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        // Query oluştur ve MediatR'a gönder — handler otomatik bulunur
        var result = await _mediator.Send(new GetProductsQuery());
        return Ok(result);
    }

    /// <summary>
    /// Yeni ürün oluşturur.
    /// POST /api/products
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Create(CreateProductCommand command)
    {
        // Command'ı MediatR'a gönder — handler otomatik bulunur, Id döner
        var id = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetAll), new { id }, id);
    }
}