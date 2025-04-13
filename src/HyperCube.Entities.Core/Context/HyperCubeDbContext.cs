using HyperCube.Entities.Core.Data.Config;
using HyperCube.Entities.Core.Entities.Base;
using HyperCube.Entities.Core.Interceptors;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HyperCube.Entities.Core.Context;

/// <summary>
/// Base database context for HyperCube applications.
/// </summary>
public class HyperCubeDbContext : DbContext
{
    private readonly ILogger<HyperCubeDbContext> _logger;
    private readonly DatabaseConfig _config;

    /// <summary>
    /// Initializes a new instance of the <see cref="HyperCubeDbContext"/> class.
    /// </summary>
    /// <param name="options">The DbContext options.</param>
    /// <param name="logger">The logger.</param>
    /// <param name="config">The database configuration.</param>
    public HyperCubeDbContext(
        DbContextOptions options,
        ILogger<HyperCubeDbContext> logger,
        DatabaseConfig config) : base(options)
    {
        _logger = logger;
        _config = config;
    }

    /// <summary>
    /// Configure the model that was discovered by convention from the entity types.
    /// </summary>
    /// <param name="modelBuilder">The builder being used to construct the model for this context.</param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        _logger.LogDebug("Configuring entity model");

        // Apply configurations from the current assembly
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(HyperCubeDbContext).Assembly);
    }

    /// <summary>
    /// Configure the database to be used for this context.
    /// </summary>
    /// <param name="optionsBuilder">A builder used to create or modify options for this context.</param>
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (_config.EnableSensitiveDataLogging)
        {
            optionsBuilder.EnableSensitiveDataLogging();
        }

        if (_config.EnableDetailedErrors)
        {
            optionsBuilder.EnableDetailedErrors();
        }

        optionsBuilder.AddInterceptors(new UpdateAuditableEntitiesInterceptor());


        base.OnConfiguring(optionsBuilder);
    }

    /// <summary>
    /// Save changes to the database.
    /// </summary>
    /// <param name="acceptAllChangesOnSuccess">Indicates whether AcceptAllChanges() is called after the changes have been sent successfully to the database.</param>
    /// <returns>The number of state entries written to the database.</returns>
    public override int SaveChanges(bool acceptAllChangesOnSuccess)
    {
        UpdateAuditableEntities();
        return base.SaveChanges(acceptAllChangesOnSuccess);
    }

    /// <summary>
    /// Save changes to the database asynchronously.
    /// </summary>
    /// <param name="acceptAllChangesOnSuccess">Indicates whether AcceptAllChanges() is called after the changes have been sent successfully to the database.</param>
    /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
    /// <returns>A task that represents the asynchronous save operation. The task result contains the number of state entries written to the database.</returns>
    public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
    {
        UpdateAuditableEntities();
        return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
    }

    private void UpdateAuditableEntities()
    {
        var now = DateTime.UtcNow;
        var entries = ChangeTracker.Entries()
            .Where(e => e.Entity is BaseEntity<Guid> || e.Entity is BaseEntity<int>)
            .ToList();

        foreach (var entry in entries)
        {
            if (entry.State == EntityState.Added)
            {
                // Set creation timestamp
                if (entry.Entity is BaseEntity<Guid> guidEntity)
                {
                    guidEntity.CreatedAt = now;
                }
                else if (entry.Entity is BaseEntity<int> intEntity)
                {
                    intEntity.CreatedAt = now;
                }
            }
            else if (entry.State == EntityState.Modified)
            {
                // Set update timestamp
                if (entry.Entity is BaseEntity<Guid> guidEntity)
                {
                    guidEntity.UpdatedAt = now;
                }
                else if (entry.Entity is BaseEntity<int> intEntity)
                {
                    intEntity.UpdatedAt = now;
                }
            }
        }
    }
}
