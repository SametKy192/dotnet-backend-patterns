using CqrsMediatr.Domain.Entities;
using MediatR;

namespace CqrsMediatr.Application.Products.Queries.GetProductById;

/// <summary>
/// Belirli bir ürünü Id'sine göre getirme sorgusu.
/// </summary>
public class GetProductByIdQuery : IRequest<Product?>
{
    public int Id { get; }

    public GetProductByIdQuery(int id)
    {
        Id = id;
    }
}
