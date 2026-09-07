using Microsoft.AspNetCore.Mvc;
using ResultPattern.Api.Common;
using ResultPattern.Api.Models;
using ResultPattern.Api.Services;

namespace ResultPattern.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class ProductsController : ControllerBase
{
    private readonly IProductService _productService;

    public ProductsController(IProductService productService)
    {
        _productService = productService;
    }

    // GET api/products
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<ProductResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        var result = await _productService.GetAllAsync(ct);
        return result.Match<IActionResult>(Ok, HandleError);
    }

    // GET api/products/{id}
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ProductResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var result = await _productService.GetByIdAsync(id, ct);
        return result.Match<IActionResult>(Ok, HandleError);
    }

    // POST api/products
    [HttpPost]
    [ProducesResponseType(typeof(ProductResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Create([FromBody] CreateProductRequest request, CancellationToken ct)
    {
        var result = await _productService.CreateAsync(request, ct);
        return result.Match<IActionResult>(
            product => CreatedAtAction(nameof(GetById), new { id = product.Id }, product),
            HandleError);
    }

    // PUT api/products/{id}
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(ProductResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateProductRequest request, CancellationToken ct)
    {
        var result = await _productService.UpdateAsync(id, request, ct);
        return result.Match<IActionResult>(Ok, HandleError);
    }

    // DELETE api/products/{id}
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        var result = await _productService.DeleteAsync(id, ct);
        return result.Match<IActionResult>(NoContent, HandleError);
    }

    // PATCH api/products/{id}/stock
    [HttpPatch("{id:guid}/stock")]
    [ProducesResponseType(typeof(ProductResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateStock(Guid id, [FromBody] UpdateStockRequest request, CancellationToken ct)
    {
        var result = await _productService.UpdateStockAsync(id, request.Quantity, ct);
        return result.Match<IActionResult>(Ok, HandleError);
    }

    // ──────────────────────────────────────────────
    // Error → HTTP status mapping
    // ──────────────────────────────────────────────
    private IActionResult HandleError(Error error)
    {
        var statusCode = error.Code switch
        {
            var c when c.EndsWith(".NotFound")    => StatusCodes.Status404NotFound,
            var c when c.StartsWith("Validation") => StatusCodes.Status400BadRequest,
            var c when c.EndsWith(".Conflict")    => StatusCodes.Status409Conflict,
            "Auth.Unauthorized"                   => StatusCodes.Status401Unauthorized,
            "Product.InsufficientStock"           => StatusCodes.Status422UnprocessableEntity,
            _                                     => StatusCodes.Status500InternalServerError
        };

        return Problem(
            detail: error.Description,
            statusCode: statusCode,
            title: error.Code);
    }
}
