using HyperCube.Entities.Core.Entities.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace HyperCube.Entities.Core.Interceptors;

/// <summary>
/// Database interceptor that automatically updates creation and modification timestamps.
/// </summary>
public class UpdateAuditableEntitiesInterceptor : SaveChangesInterceptor
{
    /// <summary>
    /// Called just before EF Core sends the INSERT, UPDATE, and DELETE commands to the database.
    /// </summary>
    /// <param name="eventData">The event data.</param>
    /// <param name="result">The result of the save operation.</param>
    /// <returns>The intercepted result.</returns>
    public override InterceptionResult<int> SavingChanges(
        DbContextEventData eventData,
        InterceptionResult<int> result
    )
    {
        if (eventData.Context is not null)
        {
            UpdateAuditableEntities(eventData.Context);
        }

        return base.SavingChanges(eventData, result);
    }

    /// <summary>
    /// Called just before EF Core sends the INSERT, UPDATE, and DELETE commands to the database asynchronously.
    /// </summary>
    /// <param name="eventData">The event data.</param>
    /// <param name="result">The result of the save operation.</param>
    /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
    /// <returns>The intercepted result.</returns>
    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default
    )
    {
        if (eventData.Context is not null)
        {
            UpdateAuditableEntities(eventData.Context);
        }

        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private static void UpdateAuditableEntities(DbContext context)
    {
        var now = DateTime.UtcNow;
        var entries = context.ChangeTracker.Entries()
            .Where(
                e => (e.Entity is BaseEntity<Guid> || e.Entity is BaseEntity<int>) &&
                     (e.State == EntityState.Added || e.State == EntityState.Modified)
            )
            .ToList();

        foreach (var entry in entries)
        {
            if (entry.State == EntityState.Added)
            {
                // Set creation timestamp
                if (entry.Entity is BaseEntity<Guid> guidEntity)
                {
                    guidEntity.CreatedAt = now;
                    guidEntity.UpdatedAt = null;
                }
                else if (entry.Entity is BaseEntity<int> intEntity)
                {
                    intEntity.CreatedAt = now;
                    intEntity.UpdatedAt = null;
                }
            }
            else if (entry.State == EntityState.Modified)
            {
                // Set update timestamp
                if (entry.Entity is BaseEntity<Guid> guidEntity)
                {
                    // Ensure CreatedAt is not modified
                    entry.Property("CreatedAt").IsModified = false;
                    guidEntity.UpdatedAt = now;
                }
                else if (entry.Entity is BaseEntity<int> intEntity)
                {
                    // Ensure CreatedAt is not modified
                    entry.Property("CreatedAt").IsModified = false;
                    intEntity.UpdatedAt = now;
                }
            }
        }
    }
}
