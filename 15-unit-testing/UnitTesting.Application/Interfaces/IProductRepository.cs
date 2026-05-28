using UnitTesting.Application.Models;

namespace UnitTesting.Application.Interfaces;

/// <summary>
/// Repository interface — test'te mock'lanacak
/// </summary>
public interface IProductRepository
{
    Task<Product?> GetByIdAsync(int id);
    Task<List<Product>> GetAllAsync();
    Task<Product> AddAsync(Product product);
    Task DeleteAsync(int id);
}