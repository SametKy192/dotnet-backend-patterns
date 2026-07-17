using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using SoftDeleteEfCore.Api.Entities;

namespace SoftDeleteEfCore.Api.Data.Interceptors;

public class SoftDeleteInterceptor : SaveChangesInterceptor
{
    public override InterceptionResult<int> SavingChanges(
        DbContextEventData eventData,
        InterceptionResult<int> result)
    {
        UpdateSoftDeleteStatuses(eventData.Context);
        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        UpdateSoftDeleteStatuses(eventData.Context);
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private void UpdateSoftDeleteStatuses(DbContext? context)
    {
        if (context == null) return;

        var entries = context.ChangeTracker.Entries<ISoftDeletable>();

        foreach (var entry in entries)
        {
            if (entry.State == EntityState.Deleted)
            {
                // Intercept deletion: change state to Modified
                entry.State = EntityState.Modified;
                
                // Set the soft delete flags
                entry.Entity.IsDeleted = true;
                entry.Entity.DeletedAtUtc = DateTime.UtcNow;
            }
        }
    }
}
