using MediatR;

namespace CqrsMediatr.Application.Products.Commands.CreateProduct;

/// <summary>
/// Yeni ürün oluşturma komutu.
/// IRequest<int> → bu command çalışınca geriye int (oluşturulan ürünün Id'si) döner.
/// Command olduğu için veri değiştirir — bu yüzden Query değil Command klasöründe.
/// </summary>
public class CreateProductCommand : IRequest<int>
{
    /// <summary>
    /// Oluşturulacak ürünün adı — controller'dan gelir
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Oluşturulacak ürünün fiyatı — controller'dan gelir
    /// </summary>
    public decimal Price { get; set; }
}