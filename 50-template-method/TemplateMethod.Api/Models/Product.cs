namespace TemplateMethod.Api.Models;

/// <summary>A single row of product data to be exported.</summary>
public record Product(
    Guid    Id,
    string  Name,
    string  Category,
    decimal Price,
    int     Stock)
{
    /// <summary>Indicates whether the product currently has available stock.</summary>
    public bool InStock => Stock > 0;
}

