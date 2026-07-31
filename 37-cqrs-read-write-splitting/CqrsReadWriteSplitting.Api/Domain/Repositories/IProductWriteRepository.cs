using CqrsReadWriteSplitting.Api.Domain.Entities;

namespace CqrsReadWriteSplitting.Api.Domain.Repositories;

public interface IProductWriteRepository
{
    Task AddAsync(Product product, CancellationToken ct);
    Task<Product?> GetByIdAsync(int id, CancellationToken ct);
    Task UpdateAsync(Product product, CancellationToken ct);
}
