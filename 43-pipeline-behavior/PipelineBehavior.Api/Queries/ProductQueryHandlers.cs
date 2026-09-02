using MediatR;
using PipelineBehavior.Api.Models;

namespace PipelineBehavior.Api.Queries;

public sealed class GetAllProductsQueryHandler : IRequestHandler<GetAllProductsQuery, IEnumerable<ProductDto>>
{
    private static readonly List<Product> _products = new()
    {
        new Product { Id = Guid.Parse("11111111-1111-1111-1111-111111111111"), Name = "Laptop", Description = "High-performance laptop", Price = 1299.99m, Stock = 50, CreatedAt = DateTime.UtcNow.AddDays(-30) },
        new Product { Id = Guid.Parse("22222222-2222-2222-2222-222222222222"), Name = "Mouse",  Description = "Wireless ergonomic mouse",  Price = 49.99m,   Stock = 200, CreatedAt = DateTime.UtcNow.AddDays(-20) },
        new Product { Id = Guid.Parse("33333333-3333-3333-3333-333333333333"), Name = "Keyboard", Description = "Mechanical keyboard",   Price = 129.99m,  Stock = 100, CreatedAt = DateTime.UtcNow.AddDays(-10) },
    };

    // Expose the in-memory store so command handlers can mutate it.
    internal static List<Product> Products => _products;

    public Task<IEnumerable<ProductDto>> Handle(GetAllProductsQuery request, CancellationToken cancellationToken)
    {
        var result = _products.Select(p => new ProductDto(
            p.Id, p.Name, p.Description, p.Price, p.Stock, p.CreatedAt, p.UpdatedAt));

        return Task.FromResult(result);
    }
}

public sealed class GetProductByIdQueryHandler : IRequestHandler<GetProductByIdQuery, ProductDto?>
{
    public Task<ProductDto?> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
    {
        var product = GetAllProductsQueryHandler.Products.FirstOrDefault(p => p.Id == request.Id);
        if (product is null)
            return Task.FromResult<ProductDto?>(null);

        var dto = new ProductDto(
            product.Id, product.Name, product.Description,
            product.Price, product.Stock, product.CreatedAt, product.UpdatedAt);

        return Task.FromResult<ProductDto?>(dto);
    }
}
