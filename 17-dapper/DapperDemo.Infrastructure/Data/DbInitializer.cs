using System.Data;
using Dapper;

namespace DapperDemo.Infrastructure.Data;

/// <summary>
/// SQLite veritabanını oluşturur ve seed data ekler.
/// Gerçek projede PostgreSQL veya SQL Server kullanılır.
/// </summary>
public class DbInitializer
{
    private readonly IDbConnection _connection;

    public DbInitializer(IDbConnection connection)
    {
        _connection = connection;
    }

    /// <summary>
    /// Tabloları oluşturur ve örnek veri ekler
    /// </summary>
    public void Initialize()
    {
        // Kategori tablosu
        _connection.Execute(@"
            CREATE TABLE IF NOT EXISTS Categories (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Name TEXT NOT NULL
            )");

        // Ürün tablosu — Category ile foreign key
        _connection.Execute(@"
            CREATE TABLE IF NOT EXISTS Products (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Name TEXT NOT NULL,
                Price DECIMAL(18,2) NOT NULL,
                CategoryId INTEGER NOT NULL,
                CreatedAt DATETIME DEFAULT CURRENT_TIMESTAMP,
                FOREIGN KEY (CategoryId) REFERENCES Categories(Id)
            )");

        // Seed data — sadece boşsa ekle
        var count = _connection.ExecuteScalar<int>("SELECT COUNT(*) FROM Categories");
        if (count == 0)
        {
            _connection.Execute("INSERT INTO Categories (Name) VALUES ('Electronics'), ('Accessories')");
            _connection.Execute(@"
                INSERT INTO Products (Name, Price, CategoryId) VALUES
                ('Laptop', 999.99, 1),
                ('Mouse', 29.99, 2),
                ('Keyboard', 79.99, 2)");
        }
    }
}