namespace TemplateMethod.Api.Models;

/// <summary>A single row of product data to be exported.</summary>
public record Product(
    Guid    Id,
    string  Name,
    string  Category,
    decimal Price,
    int     Stock);
