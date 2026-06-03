using DapperDemo.Infrastructure.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace DapperDemo.Api.Controllers;

/// <summary>
/// Ürün controller'ı — Dapper repository kullanır
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly ProductRepository _repository;

    public ProductsController(ProductRepository repository)
    {
        _repository = repository;
    }

    /// <summary>
    /// Tüm ürünleri kategori adıyla getir
    /// GET /api/products
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var products = await _repository.GetAllAsync();
        return Ok(products);
    }

    /// <summary>
    /// Id ile ürün getir
    /// GET /api/products/{id}
    /// </summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var product = await _repository.GetByIdAsync(id);
        if (product == null) return NotFound();
        return Ok(product);
    }

    /// <summary>
    /// Fiyat aralığına göre filtrele
    /// GET /api/products/price?min=10&max=100
    /// </summary>
    [HttpGet("price")]
    public async Task<IActionResult> GetByPrice([FromQuery] decimal min, [FromQuery] decimal max)
    {
        var products = await _repository.GetByPriceRangeAsync(min, max);
        return Ok(products);
    }

    /// <summary>
    /// Kategori bazında ürün sayısı
    /// GET /api/products/stats
    /// </summary>
    [HttpGet("stats")]
    public async Task<IActionResult> GetStats()
    {
        var stats = await _repository.GetProductCountByCategoryAsync();
        return Ok(stats);
    }

    /// <summary>
    /// Yeni ürün ekle
    /// POST /api/products
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateProductRequest request)
    {
        var id = await _repository.AddAsync(request.Name, request.Price, request.CategoryId);
        return CreatedAtAction(nameof(GetById), new { id }, new { id });
    }

    /// <summary>
    /// Ürün güncelle
    /// PUT /api/products/{id}
    /// </summary>
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateProductRequest request)
    {
        await _repository.UpdateAsync(id, request.Name, request.Price);
        return NoContent();
    }

    /// <summary>
    /// Ürün sil
    /// DELETE /api/products/{id}
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _repository.DeleteAsync(id);
        return NoContent();
    }
}

public record CreateProductRequest(string Name, decimal Price, int CategoryId);
public record UpdateProductRequest(string Name, decimal Price);