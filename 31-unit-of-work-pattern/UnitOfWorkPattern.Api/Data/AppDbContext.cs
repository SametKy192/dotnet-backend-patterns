using Microsoft.EntityFrameworkCore;
using UnitOfWorkPattern.Api.Entities;

namespace UnitOfWorkPattern.Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Product> Products => Set<Product>();
    public DbSet<Order> Orders => Set<Order>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Seed initial products
        modelBuilder.Entity<Product>().HasData(
            new Product { Id = 1, Name = "Sony Headphones", Price = 350.00m, Stock = 5 },
            new Product { Id = 2, Name = "Gaming Mouse", Price = 80.00m, Stock = 10 },
            new Product { Id = 3, Name = "Mechanical Keyboard", Price = 120.00m, Stock = 1 } // Low stock to test rollback!
        );
    }
}
