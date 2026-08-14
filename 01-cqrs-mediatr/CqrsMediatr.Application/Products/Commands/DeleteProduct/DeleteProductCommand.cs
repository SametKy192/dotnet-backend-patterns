using MediatR;

namespace CqrsMediatr.Application.Products.Commands.DeleteProduct;

/// <summary>
/// Ürün silme komutu.
/// </summary>
public class DeleteProductCommand : IRequest<bool>
{
    public int Id { get; }

    public DeleteProductCommand(int id)
    {
        Id = id;
    }
}
