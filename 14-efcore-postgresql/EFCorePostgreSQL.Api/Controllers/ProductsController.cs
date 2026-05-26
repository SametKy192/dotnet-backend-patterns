using EFCorePostgreSQL.Domain.Entities;
using EFCorePostgreSQL.Infrastructure.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace EFCorePostgreSQL.Api.Controllers;

/// <summary>
/// Ürün controller'ı — EF Core sorgu örnekleri
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
    /// Tüm ürünleri kategori ve tag'larıyla getir — eager loading
    /// GET /api/products
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var products = await _repository.GetAllWithCategoryAsync();
        return Ok(products);
    }

    /// <summary>
    /// Kategoriye göre filtrele
    /// GET /api/products/category/{categoryId}
    /// </summary>
    [HttpGet("category/{categoryId}")]
    public async Task<IActionResult> GetByCategory(int categoryId)
    {
        var products = await _repository.GetByCategoryAsync(categoryId);
        return Ok(products);
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
    /// Yeni ürün ekle
    /// POST /api/products
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateProductRequest request)
    {
        var product = new Product
        {
            Name = request.Name,
            Price = request.Price,
            CategoryId = request.CategoryId
        };

        var created = await _repository.AddAsync(product);
        return CreatedAtAction(nameof(GetAll), new { id = created.Id }, created);
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