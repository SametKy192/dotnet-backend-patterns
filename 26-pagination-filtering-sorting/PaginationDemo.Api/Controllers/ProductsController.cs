using Microsoft.AspNetCore.Mvc;
using PaginationDemo.Application.Common;
using PaginationDemo.Application.Services;

namespace PaginationDemo.Api.Controllers;

/// <summary>
/// Ürün controller'ı — pagination, filtering ve sorting demo
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
    /// Ürünleri filtrele, sırala ve sayfala.
    /// GET /api/products?page=1&pageSize=10&search=laptop&category=electronics&minPrice=100&maxPrice=500&sortBy=price&sortOrder=asc
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] ProductQuery query)
    {
        var result = await _productService.GetProductsAsync(query);
        return Ok(result);
    }
}