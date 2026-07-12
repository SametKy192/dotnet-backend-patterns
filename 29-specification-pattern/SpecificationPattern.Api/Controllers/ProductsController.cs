using Microsoft.AspNetCore.Mvc;
using SpecificationPattern.Application.Interfaces;
using SpecificationPattern.Application.Models;
using SpecificationPattern.Application.Specifications;

namespace SpecificationPattern.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IGenericRepository<Product> _repository;

    public ProductsController(IGenericRepository<Product> repository)
    {
        _repository = repository;
    }

    // GET /api/products
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<Product>>> GetProducts(
        [FromQuery] string? category,
        [FromQuery] decimal? minPrice,
        [FromQuery] decimal? maxPrice,
        [FromQuery] string? sortBy,
        [FromQuery] int? skip,
        [FromQuery] int? take,
        [FromQuery] bool onlyActive = true)
    {
        var spec = new ProductFilterSpecification(category, minPrice, maxPrice, onlyActive);

        if (!string.IsNullOrEmpty(sortBy))
        {
            spec.ApplySorting(sortBy);
        }

        if (skip.HasValue && take.HasValue)
        {
            spec.ApplyPagingOptions(skip.Value, take.Value);
        }

        var products = await _repository.ListAsync(spec);
        return Ok(products);
    }

    // GET /api/products/combined-demo
    // Example of combining specifications: Active AND Furniture AND Price between 100 and 500
    [HttpGet("combined-demo")]
    public async Task<ActionResult<IReadOnlyList<Product>>> GetCombinedDemo()
    {
        var activeSpec = new ActiveProductSpecification();
        var categorySpec = new ProductByCategorySpecification("Furniture");
        var priceSpec = new ProductByPriceRangeSpecification(100m, 500m);

        // Combine using AND logic: Active AND Furniture AND Price [100, 500]
        var combinedSpec = activeSpec.And(categorySpec).And(priceSpec);

        var products = await _repository.ListAsync(combinedSpec);
        return Ok(products);
    }

    // GET /api/products/{id}
    [HttpGet("{id}")]
    public async Task<ActionResult<Product>> GetProduct(int id)
    {
        var product = await _repository.GetByIdAsync(id);
        if (product == null) return NotFound();
        return Ok(product);
    }
}
