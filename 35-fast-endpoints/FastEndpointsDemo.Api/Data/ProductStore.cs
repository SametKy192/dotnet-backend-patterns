using FastEndpointsDemo.Api.Entities;

namespace FastEndpointsDemo.Api.Data;

public class ProductStore
{
    private readonly List<Product> _products = new()
    {
        new() { Id = 1, Name = "Wireless Mouse", Price = 29.99m },
        new() { Id = 2, Name = "Mechanical Keyboard", Price = 89.99m }
    };

    public List<Product> GetAll() => _products;

    public Product? GetById(int id) => _products.FirstOrDefault(p => p.Id == id);

    public void Add(Product product)
    {
        product.Id = _products.Max(p => p.Id) + 1;
        _products.Add(product);
    }
}
