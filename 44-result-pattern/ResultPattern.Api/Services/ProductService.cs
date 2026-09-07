using ResultPattern.Api.Common;
using ResultPattern.Api.Domain;
using ResultPattern.Api.Models;

namespace ResultPattern.Api.Services;

/// <summary>
/// In-memory implementation of <see cref="IProductService"/>.
/// Demonstrates how every operation returns a <see cref="Result{TValue}"/>
/// instead of throwing exceptions for expected failure paths.
/// </summary>
public sealed class ProductService : IProductService
{
    // Simulated in-memory store
    private readonly Dictionary<Guid, Product> _store = new();

    // ──────────────────────────────────────────────
    // Read operations
    // ──────────────────────────────────────────────

    public Task<Result<IReadOnlyList<ProductResponse>>> GetAllAsync(CancellationToken ct = default)
    {
        var products = _store.Values
            .Select(MapToResponse)
            .ToList();

        return Task.FromResult<Result<IReadOnlyList<ProductResponse>>>(
            Result<IReadOnlyList<ProductResponse>>.Success(products));
    }

    public Task<Result<ProductResponse>> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        if (!_store.TryGetValue(id, out var product))
            return Task.FromResult<Result<ProductResponse>>(ProductErrors.NotFound(id));

        return Task.FromResult<Result<ProductResponse>>(MapToResponse(product));
    }

    // ──────────────────────────────────────────────
    // Write operations
    // ──────────────────────────────────────────────

    public Task<Result<ProductResponse>> CreateAsync(CreateProductRequest request, CancellationToken ct = default)
    {
        // Validate
        var validationResult = ValidateProductFields(request.Name, request.Price, request.StockQuantity);
        if (validationResult is not null)
            return Task.FromResult<Result<ProductResponse>>(validationResult);

        // Check for duplicate name (case-insensitive)
        if (_store.Values.Any(p => p.Name.Equals(request.Name, StringComparison.OrdinalIgnoreCase)))
            return Task.FromResult<Result<ProductResponse>>(ProductErrors.NameAlreadyExists(request.Name));

        var product = Product.Create(request.Name, request.Description, request.Price, request.StockQuantity);
        _store[product.Id] = product;

        return Task.FromResult<Result<ProductResponse>>(MapToResponse(product));
    }

    public Task<Result<ProductResponse>> UpdateAsync(Guid id, UpdateProductRequest request, CancellationToken ct = default)
    {
        if (!_store.TryGetValue(id, out var product))
            return Task.FromResult<Result<ProductResponse>>(ProductErrors.NotFound(id));

        var validationResult = ValidateProductFields(request.Name, request.Price, request.StockQuantity);
        if (validationResult is not null)
            return Task.FromResult<Result<ProductResponse>>(validationResult);

        // Duplicate name check — exclude the current product
        if (_store.Values.Any(p => p.Id != id &&
            p.Name.Equals(request.Name, StringComparison.OrdinalIgnoreCase)))
            return Task.FromResult<Result<ProductResponse>>(ProductErrors.NameAlreadyExists(request.Name));

        product.Update(request.Name, request.Description, request.Price, request.StockQuantity);

        return Task.FromResult<Result<ProductResponse>>(MapToResponse(product));
    }

    public Task<Result> DeleteAsync(Guid id, CancellationToken ct = default)
    {
        if (!_store.Remove(id))
            return Task.FromResult(Result.Failure(ProductErrors.NotFound(id)));

        return Task.FromResult(Result.Ok);
    }

    public Task<Result<ProductResponse>> UpdateStockAsync(Guid id, int quantityDelta, CancellationToken ct = default)
    {
        if (!_store.TryGetValue(id, out var product))
            return Task.FromResult<Result<ProductResponse>>(ProductErrors.NotFound(id));

        var newQuantity = product.StockQuantity + quantityDelta;

        if (newQuantity < 0)
            return Task.FromResult<Result<ProductResponse>>(ProductErrors.InsufficientStock);

        product.Update(product.Name, product.Description, product.Price, newQuantity);

        return Task.FromResult<Result<ProductResponse>>(MapToResponse(product));
    }

    // ──────────────────────────────────────────────
    // Helpers
    // ──────────────────────────────────────────────

    private static Error? ValidateProductFields(string name, decimal price, int stock)
    {
        if (string.IsNullOrWhiteSpace(name))
            return ProductErrors.NameRequired;

        if (name.Length > 100)
            return ProductErrors.NameTooLong;

        if (price < 0)
            return ProductErrors.PriceNegative;

        if (stock < 0)
            return ProductErrors.StockNegative;

        return null;
    }

    private static ProductResponse MapToResponse(Product p) =>
        new(p.Id, p.Name, p.Description, p.Price, p.StockQuantity, p.CreatedAt, p.UpdatedAt);
}
