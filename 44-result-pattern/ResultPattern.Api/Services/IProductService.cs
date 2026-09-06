using ResultPattern.Api.Common;
using ResultPattern.Api.Domain;
using ResultPattern.Api.Models;

namespace ResultPattern.Api.Services;

/// <summary>Defines the contract for the product service.</summary>
public interface IProductService
{
    Task<Result<IReadOnlyList<ProductResponse>>> GetAllAsync(CancellationToken ct = default);
    Task<Result<ProductResponse>> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<Result<ProductResponse>> CreateAsync(CreateProductRequest request, CancellationToken ct = default);
    Task<Result<ProductResponse>> UpdateAsync(Guid id, UpdateProductRequest request, CancellationToken ct = default);
    Task<Result> DeleteAsync(Guid id, CancellationToken ct = default);
    Task<Result<ProductResponse>> UpdateStockAsync(Guid id, int quantityDelta, CancellationToken ct = default);
}
