using CqrsMediatr.Domain.Entities;
using MediatR;

namespace CqrsMediatr.Application.Products.Queries.GetProducts;

/// <summary>
/// Tüm ürünleri getirme sorgusu.
/// IRequest<List<Product>> → geriye ürün listesi döner.
/// Query olduğu için hiçbir şeyi değiştirmez — sadece okur.
/// </summary>
public class GetProductsQuery : IRequest<List<Product>>
{
    // Bu query'nin parametresi yok — tüm ürünleri getiriyor
    // İleride filtreleme eklenirse buraya property'ler gelir
}