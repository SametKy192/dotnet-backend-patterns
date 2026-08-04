namespace StronglyTypedIds.Api.Domain.Entities;

public class Product
{
    public ProductId Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public decimal Price { get; private set; }

    private Product() { } // EF Core

    public Product(ProductId id, string name, decimal price)
    {
        if (id.Value == Guid.Empty) throw new ArgumentException("Product ID cannot be empty.");
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Product name is required.");
        if (price < 0) throw new ArgumentException("Price cannot be negative.");

        Id = id;
        Name = name.Trim();
        Price = price;
    }
}
