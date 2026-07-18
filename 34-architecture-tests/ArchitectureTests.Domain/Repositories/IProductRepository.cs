using ArchitectureTests.Domain.Entities;

namespace ArchitectureTests.Domain.Repositories;

public interface IProductRepository
{
    Task<Product?> GetByIdAsync(int id);
    Task AddAsync(Product product);
}
