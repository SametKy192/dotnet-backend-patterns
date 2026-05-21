using JwtAuth.Application.Models;
using Microsoft.EntityFrameworkCore;

namespace JwtAuth.Infrastructure.Persistence;

/// <summary>
/// Auth veritabanı context'i
/// </summary>
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
}