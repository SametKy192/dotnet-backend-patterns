using CqrsMediatr.Domain.Entities;
using MediatR;
using CqrsMediatr.Application.Products.Commands.CreateProduct;

namespace CqrsMediatr.Application.Products.Queries.GetProducts;

/// <summary>
/// GetProductsQuery geldiğinde MediatR bu handler'ı otomatik bulur ve çalıştırır.
/// IRequestHandler<GetProductsQuery, List<Product>> → GetProductsQuery'yi handle et, liste döndür.
/// </summary>
public class GetProductsHandler : IRequestHandler<GetProductsQuery, List<Product>>
{
    // CreateProductHandler'daki aynı liste — static olduğu için paylaşılıyor
    // Gerçek projede DbContext inject edilir, bu liste olmaz
    private static readonly List<Product> _products = CreateProductHandler._products;

    /// <summary>
    /// Tüm ürünleri döndürür — hiçbir şeyi değiştirmez
    /// </summary>
    public Task<List<Product>> Handle(GetProductsQuery request, CancellationToken cancellationToken)
    {
        // Listeyi direkt döndür
        return Task.FromResult(_products);
    }
}