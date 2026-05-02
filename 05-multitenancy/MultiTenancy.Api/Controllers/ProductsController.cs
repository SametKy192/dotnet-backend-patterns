using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MultiTenancy.Domain.Entities;
using MultiTenancy.Infrastructure.Persistence;

namespace MultiTenancy.Api.Controllers;

/// <summary>
/// Ürün controller'ı — tenant izolasyonu otomatik, controller bilmez.
/// X-Tenant-Id header'ı ile tenant belirlenir.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly AppDbContext _dbContext;

    public ProductsController(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    /// <summary>
    /// Sadece aktif tenant'ın ürünlerini getirir — filtre otomatik
    /// GET /api/products
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        // Global query filter devrede — sadece bu tenant'ın ürünleri gelir
        var products = await _dbContext.Products.ToListAsync();
        return Ok(products);
    }

    /// <summary>
    /// Yeni ürün oluşturur — TenantId otomatik set edilir
    /// POST /api/products
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Create(Product product)
    {
        _dbContext.Products.Add(product);
        // SaveChangesAsync override'da TenantId otomatik yazılır
        await _dbContext.SaveChangesAsync();
        return CreatedAtAction(nameof(GetAll), new { id = product.Id }, product);
    }
}