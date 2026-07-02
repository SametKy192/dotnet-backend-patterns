using Microsoft.EntityFrameworkCore;
using PaginationDemo.Application.Models;

namespace PaginationDemo.Application.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Product> Products => Set<Product>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // 50 adet seed data ekle
        var products = new List<Product>();
        var categories = new[] { "Electronics", "Accessories", "Gaming", "Office", "Mobile" };
        var names = new[]
        {
            "Laptop", "Mouse", "Keyboard", "Monitor", "Headset",
            "Webcam", "Microphone", "Speaker", "USB Hub", "Charger"
        };

        for (int i = 1; i <= 50; i++)
        {
            products.Add(new Product
            {
                Id = i,
                Name = $"{names[(i - 1) % names.Length]} {i}",
                Category = categories[(i - 1) % categories.Length],
                Price = Math.Round(10 + (i * 19.99m), 2),
                Stock = i * 3,
                CreatedAt = DateTime.UtcNow.AddDays(-i)
            });
        }

        modelBuilder.Entity<Product>().HasData(products);
    }
}