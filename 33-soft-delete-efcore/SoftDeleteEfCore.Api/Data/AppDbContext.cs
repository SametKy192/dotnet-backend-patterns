using Microsoft.EntityFrameworkCore;
using SoftDeleteEfCore.Api.Entities;

namespace SoftDeleteEfCore.Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // 1. Configure the User entity properties
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(150);
        });

        // 2. Apply EF Core Global Query Filter for Soft Delete
        // This filter will automatically exclude records where IsDeleted is true.
        // It applies to all queries (e.g., Users.ToList(), FindAsync, etc.) unless bypassed using .IgnoreQueryFilters()
        modelBuilder.Entity<User>().HasQueryFilter(u => !u.IsDeleted);
    }
}
