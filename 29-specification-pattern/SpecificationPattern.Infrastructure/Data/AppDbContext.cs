using Microsoft.EntityFrameworkCore;
using SpecificationPattern.Application.Models;

namespace SpecificationPattern.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Product> Products => Set<Product>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // Seed initial data
        modelBuilder.Entity<Product>().HasData(
            new Product { Id = 1, Name = "Laptop", Category = "Electronics", Price = 1200.00m, Stock = 10, IsActive = true },
            new Product { Id = 2, Name = "Smartphone", Category = "Electronics", Price = 800.00m, Stock = 25, IsActive = true },
            new Product { Id = 3, Name = "Desk Chair", Category = "Furniture", Price = 150.00m, Stock = 15, IsActive = true },
            new Product { Id = 4, Name = "Coffee Maker", Category = "Appliances", Price = 99.99m, Stock = 0, IsActive = false },
            new Product { Id = 5, Name = "Bluetooth Speaker", Category = "Electronics", Price = 75.00m, Stock = 50, IsActive = true },
            new Product { Id = 6, Name = "Dining Table", Category = "Furniture", Price = 450.00m, Stock = 5, IsActive = true }
        );
    }
}
