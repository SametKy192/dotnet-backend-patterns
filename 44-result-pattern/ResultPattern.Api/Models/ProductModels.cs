namespace ResultPattern.Api.Models;

/// <summary>DTO for creating a new product.</summary>
public sealed record CreateProductRequest(
    string Name,
    string Description,
    decimal Price,
    int StockQuantity);

/// <summary>DTO for updating an existing product.</summary>
public sealed record UpdateProductRequest(
    string Name,
    string Description,
    decimal Price,
    int StockQuantity);

/// <summary>DTO for updating the stock quantity of a product.</summary>
public sealed record UpdateStockRequest(int Quantity);

/// <summary>Read-model response for a product.</summary>
public sealed record ProductResponse(
    Guid Id,
    string Name,
    string Description,
    decimal Price,
    int StockQuantity,
    DateTime CreatedAt,
    DateTime? UpdatedAt);
