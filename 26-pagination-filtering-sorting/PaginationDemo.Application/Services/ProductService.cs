using Microsoft.EntityFrameworkCore;
using PaginationDemo.Application.Common;
using PaginationDemo.Application.Data;

namespace PaginationDemo.Application.Services;

/// <summary>
/// Ürün servisi — filtering, sorting ve pagination uygular.
/// Tüm işlemler veritabanı seviyesinde yapılır — performanslı.
/// </summary>
public class ProductService
{
    private readonly AppDbContext _dbContext;

    public ProductService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    /// <summary>
    /// Filtreleme, sıralama ve sayfalama uygular.
    /// IQueryable kullanılır — SQL tek seferde çalışır.
    /// </summary>
    public async Task<PagedResult<object>> GetProductsAsync(ProductQuery query)
    {
        // IQueryable — henüz SQL çalışmadı
        var queryable = _dbContext.Products.AsQueryable();

        // ===== FILTERING =====

        // İsme göre arama — LIKE '%search%'
        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            queryable = queryable.Where(p =>
                p.Name.ToLower().Contains(query.Search.ToLower()));
        }

        // Kategoriye göre filtrele
        if (!string.IsNullOrWhiteSpace(query.Category))
        {
            queryable = queryable.Where(p =>
                p.Category.ToLower() == query.Category.ToLower());
        }

        // Fiyat aralığı
        if (query.MinPrice.HasValue)
            queryable = queryable.Where(p => p.Price >= query.MinPrice.Value);

        if (query.MaxPrice.HasValue)
            queryable = queryable.Where(p => p.Price <= query.MaxPrice.Value);

        // ===== SORTING =====

        queryable = query.SortBy.ToLower() switch
        {
            "name" => query.SortOrder == "desc"
                ? queryable.OrderByDescending(p => p.Name)
                : queryable.OrderBy(p => p.Name),

            "price" => query.SortOrder == "desc"
                ? queryable.OrderByDescending(p => p.Price)
                : queryable.OrderBy(p => p.Price),

            "stock" => query.SortOrder == "desc"
                ? queryable.OrderByDescending(p => p.Stock)
                : queryable.OrderBy(p => p.Stock),

            "createdat" => query.SortOrder == "desc"
                ? queryable.OrderByDescending(p => p.CreatedAt)
                : queryable.OrderBy(p => p.CreatedAt),

            _ => queryable.OrderBy(p => p.Id)
        };

        // ===== PAGINATION =====

        // Toplam kayıt sayısı — COUNT SQL
        var totalCount = await queryable.CountAsync();

        // Sayfa verisi — OFFSET + FETCH SQL
        var items = await queryable
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .Select(p => new
            {
                p.Id,
                p.Name,
                p.Category,
                p.Price,
                p.Stock,
                p.CreatedAt
            })
            .ToListAsync<object>();

        return new PagedResult<object>
        {
            Items = items,
            TotalCount = totalCount,
            Page = query.Page,
            PageSize = query.PageSize
        };
    }
}