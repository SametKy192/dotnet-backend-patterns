namespace ResultPattern.Api.Domain;

/// <summary>Product entity used throughout the Result Pattern demo.</summary>
public sealed class Product
{
    public Guid Id { get; private set; }
    public string Name { get; private set; }
    public string Description { get; private set; }
    public decimal Price { get; private set; }
    public int StockQuantity { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    private Product() { } // EF / serializer ctor

    public static Product Create(string name, string description, decimal price, int stockQuantity)
    {
        return new Product
        {
            Id = Guid.NewGuid(),
            Name = name,
            Description = description,
            Price = price,
            StockQuantity = stockQuantity,
            CreatedAt = DateTime.UtcNow
        };
    }

    public void Update(string name, string description, decimal price, int stockQuantity)
    {
        Name = name;
        Description = description;
        Price = price;
        StockQuantity = stockQuantity;
        UpdatedAt = DateTime.UtcNow;
    }
}
