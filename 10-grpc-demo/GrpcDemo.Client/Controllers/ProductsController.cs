using Grpc.Net.Client;
using GrpcDemo.Server;
using Microsoft.AspNetCore.Mvc;

namespace GrpcDemo.Client.Controllers;

/// <summary>
/// REST API — arka planda gRPC server'a bağlanır.
/// Dış dünya REST kullanır, servisler arası iletişim gRPC ile olur.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<ProductsController> _logger;

    public ProductsController(IConfiguration configuration, ILogger<ProductsController> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    /// <summary>
    /// gRPC server'dan tek ürün getir — unary call
    /// GET /api/products/{id}
    /// </summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> Get(int id)
    {
        using var channel = GrpcChannel.ForAddress(
            _configuration["GrpcServer"] ?? "http://localhost:5200");

        var client = new ProductService.ProductServiceClient(channel);

        _logger.LogInformation("gRPC GetProduct: {Id}", id);

        var response = await client.GetProductAsync(new GetProductRequest { Id = id });

        return Ok(new { response.Id, response.Name, response.Price });
    }

    /// <summary>
    /// gRPC server'dan tüm ürünleri stream olarak al
    /// GET /api/products
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        using var channel = GrpcChannel.ForAddress(
            _configuration["GrpcServer"] ?? "http://localhost:5200");

        var client = new ProductService.ProductServiceClient(channel);

        // Server streaming call — ürünler teker teker gelir
        var call = client.GetAllProducts(new GetAllProductsRequest());

        var products = new List<object>();

        await foreach (var product in call.ResponseStream.ReadAllAsync())
        {
            products.Add(new { product.Id, product.Name, product.Price });
            _logger.LogInformation("Stream'den alındı: {Name}", product.Name);
        }

        return Ok(products);
    }
}