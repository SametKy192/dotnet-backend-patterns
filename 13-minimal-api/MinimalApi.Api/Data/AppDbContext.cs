using Microsoft.EntityFrameworkCore;
using MinimalApi.Api.Models;

namespace MinimalApi.Api.Data;

/// <summary>
/// Veritabanı context'i
/// </summary>
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    public DbSet<Product> Products => Set<Product>();
}