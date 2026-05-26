using EFCorePostgreSQL.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace EFCorePostgreSQL.Infrastructure.Persistence;

/// <summary>
/// EF Core DbContext — ilişkiler ve konfigürasyonlar burada tanımlanır
/// </summary>
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Product> Products => Set<Product>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Tag> Tags => Set<Tag>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // One-to-many: Category → Products
        modelBuilder.Entity<Product>()
            .HasOne(p => p.Category)
            .WithMany(c => c.Products)
            .HasForeignKey(p => p.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        // Many-to-many: Products ↔ Tags
        // EF Core 5+ otomatik junction table oluşturur
        modelBuilder.Entity<Product>()
            .HasMany(p => p.Tags)
            .WithMany(t => t.Products)
            .UsingEntity("ProductTags");

        // Price precision
        modelBuilder.Entity<Product>()
            .Property(p => p.Price)
            .HasPrecision(18, 2);

        // Seed data — uygulama başlangıcında örnek veri
        modelBuilder.Entity<Category>().HasData(
            new Category { Id = 1, Name = "Electronics" },
            new Category { Id = 2, Name = "Accessories" }
        );

        modelBuilder.Entity<Product>().HasData(
            new Product { Id = 1, Name = "Laptop", Price = 999.99m, CategoryId = 1 },
            new Product { Id = 2, Name = "Mouse", Price = 29.99m, CategoryId = 2 },
            new Product { Id = 3, Name = "Keyboard", Price = 79.99m, CategoryId = 2 }
        );

        modelBuilder.Entity<Tag>().HasData(
            new Tag { Id = 1, Name = "Sale" },
            new Tag { Id = 2, Name = "New" }
        );
    }
}