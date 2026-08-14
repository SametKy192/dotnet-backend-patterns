using CqrsMediatr.Domain.Entities;
using MediatR;
using CqrsMediatr.Application.Products.Commands.CreateProduct;

namespace CqrsMediatr.Application.Products.Queries.GetProductById;

/// <summary>
/// GetProductByIdQuery'yi handle eden sınıf.
/// </summary>
public class GetProductByIdHandler : IRequestHandler<GetProductByIdQuery, Product?>
{
    private static readonly List<Product> _products = CreateProductHandler._products;

    public Task<Product?> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
    {
        var product = _products.FirstOrDefault(p => p.Id == request.Id);
        return Task.FromResult(product);
    }
}
