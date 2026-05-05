using Microsoft.EntityFrameworkCore;
using MultiTenancy.Domain.Entities;
using MultiTenancy.Infrastructure.Services;

namespace MultiTenancy.Infrastructure.Persistence;

/// <summary>
/// Multi-tenant DbContext — her sorguda otomatik tenant filtresi uygular.
/// Tenant A, Tenant B'nin verisini göremez.
/// </summary>
public class AppDbContext : DbContext
{
    private readonly TenantService _tenantService;

    public AppDbContext(DbContextOptions<AppDbContext> options, TenantService tenantService)
        : base(options)
    {
        _tenantService = tenantService;
    }

    public DbSet<Product> Products => Set<Product>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Global query filter — her Product sorgusuna otomatik tenant filtresi ekler
        // SELECT * FROM Products WHERE TenantId = 'aktif-tenant'
        modelBuilder.Entity<Product>()
            .HasQueryFilter(p => p.TenantId == _tenantService.GetTenantId());
    }

    /// <summary>
    /// SaveChanges override — yeni entity eklenirken TenantId otomatik set edilir
    /// </summary>
    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // Yeni eklenen tenant entity'lerine TenantId yaz
        foreach (var entry in ChangeTracker.Entries<ITenantEntity>()
            .Where(e => e.State == EntityState.Added))
        {
            entry.Entity.TenantId = _tenantService.GetTenantId();
        }

        return base.SaveChangesAsync(cancellationToken);
    }
}