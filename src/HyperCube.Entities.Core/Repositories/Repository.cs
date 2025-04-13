using System.Linq.Expressions;
using HyperCube.Entities.Core.Context;
using HyperCube.Entities.Core.Entities.Base;
using HyperCube.Entities.Core.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.Logging;

namespace HyperCube.Entities.Core.Repositories;

/// <summary>
/// Generic repository implementation for database operations.
/// </summary>
/// <typeparam name="TEntity">The entity type.</typeparam>
/// <typeparam name="TKey">The type of the entity's primary key.</typeparam>
public class Repository<TEntity, TKey> : IRepository<TEntity, TKey>
    where TEntity : BaseEntity<TKey>
    where TKey : IEquatable<TKey>
{
    private readonly HyperCubeDbContext _dbContext;
    private readonly DbSet<TEntity> _dbSet;
    private readonly ILogger<Repository<TEntity, TKey>> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="Repository{TEntity, TKey}"/> class.
    /// </summary>
    /// <param name="dbContext">The database context.</param>
    /// <param name="logger">The logger.</param>
    public Repository(
        HyperCubeDbContext dbContext,
        ILogger<Repository<TEntity, TKey>> logger
    )
    {
        _dbContext = dbContext;
        _dbSet = dbContext.Set<TEntity>();
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<IEnumerable<TEntity>> GetAllAsync(
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null,
        bool disableTracking = true
    )
    {
        _logger.LogDebug("Getting all entities of type {EntityType}", typeof(TEntity).Name);

        IQueryable<TEntity> query = _dbSet;

        if (disableTracking)
        {
            query = query.AsNoTracking();
        }

        if (include != null)
        {
            query = include(query);
        }

        return await query.ToListAsync();
    }

    /// <inheritdoc />
    public async Task<IEnumerable<TEntity>> GetAsync(
        Expression<Func<TEntity, bool>> predicate,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null,
        bool disableTracking = true
    )
    {
        _logger.LogDebug("Getting entities of type {EntityType} with predicate", typeof(TEntity).Name);

        IQueryable<TEntity> query = _dbSet;

        if (disableTracking)
        {
            query = query.AsNoTracking();
        }

        if (include != null)
        {
            query = include(query);
        }

        return await query.Where(predicate).ToListAsync();
    }

    /// <inheritdoc />
    public async Task<TEntity?> GetByIdAsync(
        TKey id,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null,
        bool disableTracking = true
    )
    {
        _logger.LogDebug("Getting entity of type {EntityType} with id {Id}", typeof(TEntity).Name, id);

        IQueryable<TEntity> query = _dbSet;

        if (disableTracking)
        {
            query = query.AsNoTracking();
        }

        if (include != null)
        {
            query = include(query);
        }

        return await query.FirstOrDefaultAsync(e => e.Id.Equals(id));
    }

    /// <inheritdoc />
    public async Task<TEntity?> GetFirstAsync(
        Expression<Func<TEntity, bool>> predicate,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null,
        bool disableTracking = true
    )
    {
        _logger.LogDebug("Getting first entity of type {EntityType} with predicate", typeof(TEntity).Name);

        IQueryable<TEntity> query = _dbSet;

        if (disableTracking)
        {
            query = query.AsNoTracking();
        }

        if (include != null)
        {
            query = include(query);
        }

        return await query.FirstOrDefaultAsync(predicate);
    }

    /// <inheritdoc />
    public async Task<TEntity> AddAsync(TEntity entity)
    {
        _logger.LogDebug("Adding entity of type {EntityType}", typeof(TEntity).Name);

        var result = await _dbSet.AddAsync(entity);
        await _dbContext.SaveChangesAsync();

        return result.Entity;
    }

    /// <inheritdoc />
    public async Task AddRangeAsync(IEnumerable<TEntity> entities)
    {
        _logger.LogDebug("Adding multiple entities of type {EntityType}", typeof(TEntity).Name);

        await _dbSet.AddRangeAsync(entities);
        await _dbContext.SaveChangesAsync();
    }

    /// <inheritdoc />
    public async Task AddBulkAsync(IEnumerable<TEntity> entities, int batchSize = 1000)
    {
        _logger.LogDebug("Bulk adding entities of type {EntityType}", typeof(TEntity).Name);

        var entitiesList = entities.ToList();
        var totalCount = entitiesList.Count;

        _logger.LogDebug("Processing {Count} entities in batches of {BatchSize}", totalCount, batchSize);

        for (int i = 0; i < totalCount; i += batchSize)
        {
            // Take a batch of entities
            var batch = entitiesList.Skip(i).Take(batchSize).ToList();

            _logger.LogDebug(
                "Processing batch {BatchNumber}/{TotalBatches} ({BatchSize} entities)",
                (i / batchSize) + 1,
                Math.Ceiling((double)totalCount / batchSize),
                batch.Count
            );

            // Add the batch
            await _dbSet.AddRangeAsync(batch);

            // Save changes for this batch
            await _dbContext.SaveChangesAsync();

            // Detach entities to free up memory
            foreach (var entity in batch)
            {
                _dbContext.Entry(entity).State = EntityState.Detached;
            }
        }

        _logger.LogDebug("Completed bulk insert of {Count} entities", totalCount);
    }

    /// <inheritdoc />
    public async Task<TEntity> UpdateAsync(TEntity entity)
    {
        _logger.LogDebug("Updating entity of type {EntityType} with id {Id}", typeof(TEntity).Name, entity.Id);

        // Check if entity exists
        var exists = await _dbSet.AnyAsync(e => e.Id.Equals(entity.Id));
        if (!exists)
        {
            _logger.LogWarning(
                "Entity of type {EntityType} with id {Id} not found for update",
                typeof(TEntity).Name,
                entity.Id
            );
            throw new InvalidOperationException($"Entity of type {typeof(TEntity).Name} with id {entity.Id} not found");
        }

        _dbContext.Entry(entity).State = EntityState.Modified;
        await _dbContext.SaveChangesAsync();

        return entity;
    }

    /// <inheritdoc />
    public async Task DeleteAsync(TKey id)
    {
        _logger.LogDebug("Deleting entity of type {EntityType} with id {Id}", typeof(TEntity).Name, id);

        var entity = await _dbSet.FindAsync(id);
        if (entity == null)
        {
            _logger.LogWarning("Entity of type {EntityType} with id {Id} not found for deletion", typeof(TEntity).Name, id);
            throw new InvalidOperationException($"Entity of type {typeof(TEntity).Name} with id {id} not found");
        }

        await DeleteAsync(entity);
    }

    /// <inheritdoc />
    public async Task DeleteAsync(TEntity entity)
    {
        _logger.LogDebug("Deleting entity of type {EntityType} with id {Id}", typeof(TEntity).Name, entity.Id);

        _dbSet.Remove(entity);
        await _dbContext.SaveChangesAsync();
    }

    /// <inheritdoc />
    public async Task DeleteRangeAsync(IEnumerable<TEntity> entities)
    {
        _logger.LogDebug("Deleting multiple entities of type {EntityType}", typeof(TEntity).Name);

        _dbSet.RemoveRange(entities);
        await _dbContext.SaveChangesAsync();
    }

    /// <inheritdoc />
    public async Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate)
    {
        _logger.LogDebug("Checking if any entity of type {EntityType} matches predicate", typeof(TEntity).Name);

        return await _dbSet.AnyAsync(predicate);
    }

    /// <inheritdoc />
    public async Task<int> CountAsync(Expression<Func<TEntity, bool>>? predicate = null)
    {
        _logger.LogDebug("Counting entities of type {EntityType}", typeof(TEntity).Name);

        return predicate == null
            ? await _dbSet.CountAsync()
            : await _dbSet.CountAsync(predicate);
    }

    /// <inheritdoc />
    public IQueryable<TEntity> GetQueryable(bool disableTracking = true)
    {
        _logger.LogDebug("Getting queryable for entity type {EntityType}", typeof(TEntity).Name);

        return disableTracking
            ? _dbSet.AsNoTracking()
            : _dbSet;
    }
}
