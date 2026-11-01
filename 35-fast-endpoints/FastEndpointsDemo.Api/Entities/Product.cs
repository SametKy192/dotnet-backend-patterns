namespace FastEndpointsDemo.Api.Entities;

public class Product
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }

    /// <summary>Indicates whether the product has a valid non-negative price.</summary>
    public bool HasValidPrice => Price >= 0m;
}
