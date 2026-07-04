using GlobalExceptionHandling.Application.Exceptions;
using GlobalExceptionHandling.Application.Models;

namespace GlobalExceptionHandling.Application.Services;

/// <summary>
/// Ürün servisi — custom exception'lar fırlatır.
/// Controller try-catch yazmaz, global handler yakalar.
/// </summary>
public class ProductService
{
    private static readonly List<Product> _products = new()
    {
        new Product { Id = 1, Name = "Laptop", Price = 999.99m },
        new Product { Id = 2, Name = "Mouse", Price = 29.99m }
    };

    public Product GetById(int id)
    {
        var product = _products.FirstOrDefault(p => p.Id == id);

        // NotFoundException — global handler 404 döndürür
        if (product == null)
            throw new NotFoundException("Product", id);

        return product;
    }

    public Product Create(string name, decimal price)
    {
        // ValidationException — global handler 400 döndürür
        var errors = new Dictionary<string, string[]>();

        if (string.IsNullOrWhiteSpace(name))
            errors["Name"] = new[] { "Ürün adı boş olamaz." };

        if (price <= 0)
            errors["Price"] = new[] { "Fiyat 0'dan büyük olmalıdır." };

        if (errors.Any())
            throw new ValidationException(errors);

        // ConflictException — aynı isimde ürün var
        if (_products.Any(p => p.Name.ToLower() == name.ToLower()))
            throw new ConflictException($"'{name}' adında bir ürün zaten mevcut.");

        var product = new Product
        {
            Id = _products.Max(p => p.Id) + 1,
            Name = name,
            Price = price
        };

        _products.Add(product);
        return product;
    }

    public void Delete(int id, string userRole)
    {
        // ForbiddenException — sadece admin silebilir
        if (userRole != "Admin")
            throw new ForbiddenException("Ürün silmek için Admin yetkisi gereklidir.");

        var product = _products.FirstOrDefault(p => p.Id == id)
            ?? throw new NotFoundException("Product", id);

        _products.Remove(product);
    }

    public List<Product> GetAll() => _products;
}