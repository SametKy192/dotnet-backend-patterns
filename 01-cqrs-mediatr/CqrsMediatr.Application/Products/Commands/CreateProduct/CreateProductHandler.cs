using CqrsMediatr.Domain.Entities;
using MediatR;

namespace CqrsMediatr.Application.Products.Commands.CreateProduct;

/// <summary>
/// CreateProductCommand geldiğinde MediatR bu handler'ı otomatik bulur ve çalıştırır.
/// IRequestHandler<CreateProductCommand, int> → CreateProductCommand'ı handle et, int döndür.
/// </summary>
public class CreateProductHandler : IRequestHandler<CreateProductCommand, int>
{
    // Şimdilik veritabanı yok, in-memory liste kullanıyoruz
    // İleride DbContext buraya inject edilecek
    public static readonly List<Product> _products = new();

    /// <summary>
    /// MediatR bu metodu otomatik çağırır.
    /// request → controller'dan gelen CreateProductCommand
    /// cancellationToken → işlemi iptal etmek için kullanılır
    /// </summary>
    public Task<int> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        // Yeni ürün nesnesi oluşturuluyor
        var product = new Product
        {
            // Basit ID üretimi — gerçek projede veritabanı halleder
            Id = _products.Count + 1,
            Name = request.Name,
            Price = request.Price,
            CreatedAt = DateTime.UtcNow
        };

        // Listeye ekleniyor
        _products.Add(product);

        // Oluşturulan ürünün Id'si döndürülüyor
        return Task.FromResult(product.Id);
    }
}