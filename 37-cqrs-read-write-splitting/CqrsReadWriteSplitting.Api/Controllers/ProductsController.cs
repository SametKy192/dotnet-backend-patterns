using CqrsReadWriteSplitting.Api.Domain.Entities;
using CqrsReadWriteSplitting.Api.Domain.Queries;
using CqrsReadWriteSplitting.Api.Domain.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace CqrsReadWriteSplitting.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IProductWriteRepository _writeRepository;
    private readonly IProductQueries _queries;

    public ProductsController(IProductWriteRepository writeRepository, IProductQueries queries)
    {
        _writeRepository = writeRepository;
        _queries = queries;
    }

    // --- COMMANDS (Writes using EF Core) ---

    [HttpPost]
    public async Task<IActionResult> Create(CreateProductRequest request, CancellationToken ct)
    {
        try
        {
            var product = new Product(request.Name, request.Price, request.Stock);
            await _writeRepository.AddAsync(product, ct);
            return CreatedAtAction(nameof(GetById), new { id = product.Id }, product);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { Error = ex.Message });
        }
    }

    [HttpPut("{id}/price")]
    public async Task<IActionResult> UpdatePrice(int id, [FromBody] UpdatePriceRequest request, CancellationToken ct)
    {
        var product = await _writeRepository.GetByIdAsync(id, ct);
        if (product == null) return NotFound();

        try
        {
            product.UpdatePrice(request.NewPrice);
            await _writeRepository.UpdateAsync(product, ct);
            return NoContent();
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { Error = ex.Message });
        }
    }

    [HttpPut("{id}/stock")]
    public async Task<IActionResult> AdjustStock(int id, [FromBody] AdjustStockRequest request, CancellationToken ct)
    {
        var product = await _writeRepository.GetByIdAsync(id, ct);
        if (product == null) return NotFound();

        try
        {
            product.AdjustStock(request.Amount);
            await _writeRepository.UpdateAsync(product, ct);
            return NoContent();
        }
        catch (Exception ex) when (ex is ArgumentException or InvalidOperationException)
        {
            return BadRequest(new { Error = ex.Message });
        }
    }

    // --- QUERIES (Reads using Dapper) ---

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var products = await _queries.GetAllAsync();
        return Ok(products);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var product = await _queries.GetByIdAsync(id);
        if (product == null) return NotFound();
        return Ok(product);
    }
}

// DTOs
public record CreateProductRequest(string Name, decimal Price, int Stock);
public record UpdatePriceRequest(decimal NewPrice);
public record AdjustStockRequest(int Amount);
