using ResultPattern.Api.Common;

namespace ResultPattern.Api.Domain;

/// <summary>Domain-specific errors for the Product aggregate.</summary>
public static class ProductErrors
{
    public static Error NotFound(Guid id) =>
        Error.NotFound(nameof(Product), id);

    public static readonly Error NameRequired =
        Error.Validation(nameof(Product.Name), "Product name is required.");

    public static readonly Error NameTooLong =
        Error.Validation(nameof(Product.Name), "Product name must not exceed 100 characters.");

    public static readonly Error PriceNegative =
        Error.Validation(nameof(Product.Price), "Product price must be greater than or equal to zero.");

    public static readonly Error StockNegative =
        Error.Validation(nameof(Product.StockQuantity), "Stock quantity cannot be negative.");

    public static Error NameAlreadyExists(string name) =>
        Error.Conflict(nameof(Product), $"A product with name '{name}' already exists.");

    public static readonly Error InsufficientStock =
        new("Product.InsufficientStock", "Not enough stock available to fulfil the request.");
}
