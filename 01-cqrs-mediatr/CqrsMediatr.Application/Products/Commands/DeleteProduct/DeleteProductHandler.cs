using MediatR;
using CqrsMediatr.Application.Products.Commands.CreateProduct;

namespace CqrsMediatr.Application.Products.Commands.DeleteProduct;

/// <summary>
/// DeleteProductCommand'ı handle eden sınıf.
/// </summary>
public class DeleteProductHandler : IRequestHandler<DeleteProductCommand, bool>
{
    private static readonly List<CqrsMediatr.Domain.Entities.Product> _products = CreateProductHandler._products;

    public Task<bool> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
    {
        var product = _products.FirstOrDefault(p => p.Id == request.Id);
        if (product == null)
        {
            return Task.FromResult(false);
        }

        _products.Remove(product);
        return Task.FromResult(true);
    }
}
