namespace CqrsReadWriteSplitting.Api.Domain.Entities;

public class Product
{
    public int Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public decimal Price { get; private set; }
    public int Stock { get; private set; }

    // Required by EF Core
    private Product()
    {
    }

    public Product(string name, decimal price, int stock)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Product name is required.");
        if (price < 0) throw new ArgumentException("Price cannot be negative.");
        if (stock < 0) throw new ArgumentException("Stock cannot be negative.");

        Name = name.Trim();
        Price = price;
        Stock = stock;
    }

    public void UpdatePrice(decimal newPrice)
    {
        if (newPrice < 0) throw new ArgumentException("Price cannot be negative.");
        Price = newPrice;
    }

    public void AdjustStock(int amount)
    {
        if (Stock + amount < 0) throw new InvalidOperationException("Insufficient stock.");
        Stock += amount;
    }
}
