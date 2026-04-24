using Microsoft.EntityFrameworkCore;
using OutboxPattern.Domain.Entities;

namespace OutboxPattern.Infrastructure.Persistence;

/// <summary>
/// Veritabanı context'i — Order ve OutboxMessage tablolarını yönetir
/// </summary>
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    /// <summary>Siparişler tablosu</summary>
    public DbSet<Order> Orders => Set<Order>();

    /// <summary>Outbox mesajları tablosu</summary>
    public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();
}