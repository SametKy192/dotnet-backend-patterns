using DomainEvents.Domain.Common;
using DomainEvents.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DomainEvents.Infrastructure.Persistence;

public class AppDbContext : DbContext
{
    private readonly IPublisher _publisher;

    public AppDbContext(DbContextOptions<AppDbContext> options, IPublisher publisher)
        : base(options)
    {
        _publisher = publisher;
    }

    public DbSet<Product> Products => Set<Product>();

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // 1. Fetch entities with domain events
        var domainEntities = ChangeTracker
            .Entries<Entity>()
            .Where(x => x.Entity.DomainEvents.Any())
            .ToList();

        var domainEvents = domainEntities
            .SelectMany(x => x.Entity.DomainEvents)
            .ToList();

        // 2. Clear events from entities to avoid duplicate dispatch
        domainEntities.ForEach(entity => entity.Entity.ClearDomainEvents());

        // 3. Dispatch events
        foreach (var domainEvent in domainEvents)
        {
            await _publisher.Publish(domainEvent, cancellationToken);
        }

        // 4. Save modifications
        return await base.SaveChangesAsync(cancellationToken);
    }
}
