using System.Data;
using Dapper;
using DapperDemo.Infrastructure.Models;

namespace DapperDemo.Infrastructure.Repositories;

/// <summary>
/// Dapper repository — raw SQL ile veri erişimi.
/// EF Core'dan farklı olarak SQL tamamen kontrolümüzde.
/// Karmaşık sorgular, stored procedure'lar için idealdir.
/// </summary>
public class ProductRepository
{
    private readonly IDbConnection _connection;

    public ProductRepository(IDbConnection connection)
    {
        _connection = connection;
    }

    /// <summary>
    /// Tüm ürünleri kategori adıyla birlikte getirir — JOIN sorgusu
    /// </summary>
    public async Task<IEnumerable<Product>> GetAllAsync()
    {
        // Dapper SQL'i çalıştırır ve sonucu Product nesnesine map eder
        var sql = @"
            SELECT p.Id, p.Name, p.Price, p.CreatedAt,
                   c.Name AS CategoryName
            FROM Products p
            INNER JOIN Categories c ON p.CategoryId = c.Id
            ORDER BY p.Id";

        return await _connection.QueryAsync<Product>(sql);
    }

    /// <summary>
    /// Id ile ürün getirir — parametre kullanımı
    /// </summary>
    public async Task<Product?> GetByIdAsync(int id)
    {
        var sql = @"
            SELECT p.Id, p.Name, p.Price, p.CreatedAt,
                   c.Name AS CategoryName
            FROM Products p
            INNER JOIN Categories c ON p.CategoryId = c.Id
            WHERE p.Id = @Id";

        // @Id — SQL injection'a karşı güvenli parametre
        return await _connection.QueryFirstOrDefaultAsync<Product>(sql, new { Id = id });
    }

    /// <summary>
    /// Fiyat aralığına göre filtrele
    /// </summary>
    public async Task<IEnumerable<Product>> GetByPriceRangeAsync(decimal min, decimal max)
    {
        var sql = @"
            SELECT p.Id, p.Name, p.Price, p.CreatedAt,
                   c.Name AS CategoryName
            FROM Products p
            INNER JOIN Categories c ON p.CategoryId = c.Id
            WHERE p.Price BETWEEN @Min AND @Max
            ORDER BY p.Price";

        return await _connection.QueryAsync<Product>(sql, new { Min = min, Max = max });
    }

    /// <summary>
    /// Yeni ürün ekle — INSERT ve yeni Id'yi döndür
    /// </summary>
    public async Task<int> AddAsync(string name, decimal price, int categoryId)
    {
        var sql = @"
            INSERT INTO Products (Name, Price, CategoryId)
            VALUES (@Name, @Price, @CategoryId);
            SELECT last_insert_rowid()";

        // ExecuteScalarAsync — eklenen kaydın Id'sini döndürür
        return await _connection.ExecuteScalarAsync<int>(sql, new
        {
            Name = name,
            Price = price,
            CategoryId = categoryId
        });
    }

    /// <summary>
    /// Ürün güncelle
    /// </summary>
    public async Task UpdateAsync(int id, string name, decimal price)
    {
        var sql = "UPDATE Products SET Name = @Name, Price = @Price WHERE Id = @Id";
        await _connection.ExecuteAsync(sql, new { Id = id, Name = name, Price = price });
    }

    /// <summary>
    /// Ürün sil
    /// </summary>
    public async Task DeleteAsync(int id)
    {
        await _connection.ExecuteAsync("DELETE FROM Products WHERE Id = @Id", new { Id = id });
    }

    /// <summary>
    /// Kategori bazında ürün sayısı — GROUP BY örneği
    /// </summary>
    public async Task<IEnumerable<dynamic>> GetProductCountByCategoryAsync()
    {
        var sql = @"
            SELECT c.Name AS Category, COUNT(p.Id) AS ProductCount
            FROM Categories c
            LEFT JOIN Products p ON p.CategoryId = c.Id
            GROUP BY c.Id, c.Name";

        return await _connection.QueryAsync(sql);
    }
}