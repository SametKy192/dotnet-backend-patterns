using CleanArchitecture.Domain.Entities;

namespace CleanArchitecture.Domain.Interfaces;

/// <summary>
/// Repository interface — Domain katmanında tanımlanır.
/// Infrastructure katmanı bu interface'i implement eder.
/// Domain, Infrastructure'ı tanımaz — bağımlılık tersine çevrilir.
/// </summary>
public interface IProductRepository
{
    Task<Product?> GetByIdAsync(int id);
    Task<List<Product>> GetAllAsync();
    Task AddAsync(Product product);
    Task UpdateAsync(Product product);
    Task DeleteAsync(int id);
}