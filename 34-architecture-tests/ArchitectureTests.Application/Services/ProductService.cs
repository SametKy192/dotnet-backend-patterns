using ArchitectureTests.Domain.Entities;
using ArchitectureTests.Domain.Repositories;

namespace ArchitectureTests.Application.Services;

public class ProductService
{
    private readonly IProductRepository _productRepository;

    public ProductService(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<Product?> GetProductAsync(int id)
    {
        return await _productRepository.GetByIdAsync(id);
    }

    public async Task CreateProductAsync(Product product)
    {
        await _productRepository.AddAsync(product);
    }
}
