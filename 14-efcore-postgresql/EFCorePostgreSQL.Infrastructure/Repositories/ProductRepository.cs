using EFCorePostgreSQL.Domain.Entities;
using EFCorePostgreSQL.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EFCorePostgreSQL.Infrastructure.Repositories;

/// <summary>
/// Ürün repository — EF Core sorgu örnekleri
/// </summary>
public class ProductRepository
{
    private readonly AppDbContext _dbContext;

    public ProductRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    /// <summary>
    /// Tüm ürünleri kategori ile birlikte getirir — eager loading
    /// Include ile ilişkili entity otomatik yüklenir
    /// </summary>
    public async Task<List<Product>> GetAllWithCategoryAsync()
    {
        return await _dbContext.Products
            .Include(p => p.Category)
            .Include(p => p.Tags)
            .ToListAsync();
    }

    /// <summary>
    /// Kategoriye göre filtrele
    /// </summary>
    public async Task<List<Product>> GetByCategoryAsync(int categoryId)
    {
        return await _dbContext.Products
            .Where(p => p.CategoryId == categoryId)
            .Include(p => p.Category)
            .ToListAsync();
    }

    /// <summary>
    /// Fiyat aralığına göre filtrele
    /// </summary>
    public async Task<List<Product>> GetByPriceRangeAsync(decimal min, decimal max)
    {
        return await _dbContext.Products
            .Where(p => p.Price >= min && p.Price <= max)
            .OrderBy(p => p.Price)
            .ToListAsync();
    }

    /// <summary>
    /// Yeni ürün ekle
    /// </summary>
    public async Task<Product> AddAsync(Product product)
    {
        _dbContext.Products.Add(product);
        await _dbContext.SaveChangesAsync();
        return product;
    }

    /// <summary>
    /// Ürün sil
    /// </summary>
    public async Task DeleteAsync(int id)
    {
        var product = await _dbContext.Products.FindAsync(id);
        if (product != null)
        {
            _dbContext.Products.Remove(product);
            await _dbContext.SaveChangesAsync();
        }
    }
}