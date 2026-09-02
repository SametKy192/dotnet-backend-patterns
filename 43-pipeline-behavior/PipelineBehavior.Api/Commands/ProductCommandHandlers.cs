using MediatR;
using PipelineBehavior.Api.Models;
using PipelineBehavior.Api.Queries;

namespace PipelineBehavior.Api.Commands;

public sealed class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, ProductDto>
{
    public Task<ProductDto> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        var product = new Product
        {
            Id          = Guid.NewGuid(),
            Name        = request.Name,
            Description = request.Description,
            Price       = request.Price,
            Stock       = request.Stock,
            CreatedAt   = DateTime.UtcNow
        };

        GetAllProductsQueryHandler.Products.Add(product);

        var dto = new ProductDto(
            product.Id, product.Name, product.Description,
            product.Price, product.Stock, product.CreatedAt, product.UpdatedAt);

        return Task.FromResult(dto);
    }
}

public sealed class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand, ProductDto?>
{
    public Task<ProductDto?> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
    {
        var product = GetAllProductsQueryHandler.Products.FirstOrDefault(p => p.Id == request.Id);
        if (product is null)
            return Task.FromResult<ProductDto?>(null);

        product.Name        = request.Name;
        product.Description = request.Description;
        product.Price       = request.Price;
        product.Stock       = request.Stock;
        product.UpdatedAt   = DateTime.UtcNow;

        var dto = new ProductDto(
            product.Id, product.Name, product.Description,
            product.Price, product.Stock, product.CreatedAt, product.UpdatedAt);

        return Task.FromResult<ProductDto?>(dto);
    }
}

public sealed class DeleteProductCommandHandler : IRequestHandler<DeleteProductCommand, bool>
{
    public Task<bool> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
    {
        var product = GetAllProductsQueryHandler.Products.FirstOrDefault(p => p.Id == request.Id);
        if (product is null)
            return Task.FromResult(false);

        GetAllProductsQueryHandler.Products.Remove(product);
        return Task.FromResult(true);
    }
}
