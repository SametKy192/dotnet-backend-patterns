using InboxPattern.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;

namespace InboxPattern.Infrastructure.Persistence;

public class InboxDbContext : DbContext
{
    public InboxDbContext(DbContextOptions<InboxDbContext> options) : base(options)
    {
    }

    public DbSet<InboxMessage> InboxMessages => Set<InboxMessage>();
}
