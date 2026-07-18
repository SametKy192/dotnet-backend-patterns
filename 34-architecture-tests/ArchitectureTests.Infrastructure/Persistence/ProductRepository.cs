using ArchitectureTests.Domain.Entities;
using ArchitectureTests.Domain.Repositories;

namespace ArchitectureTests.Infrastructure.Persistence;

public class ProductRepository : IProductRepository
{
    private readonly List<Product> _products = new();

    public Task<Product?> GetByIdAsync(int id)
    {
        var product = _products.FirstOrDefault(p => p.Id == id);
        return Task.FromResult(product);
    }

    public Task AddAsync(Product product)
    {
        _products.Add(product);
        return Task.CompletedTask;
    }
}
