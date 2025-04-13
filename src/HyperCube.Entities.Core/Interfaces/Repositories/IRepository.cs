using System.Linq.Expressions;
using HyperCube.Entities.Core.Entities.Base;
using Microsoft.EntityFrameworkCore.Query;

namespace HyperCube.Entities.Core.Interfaces.Repositories;

/// <summary>
/// Generic repository interface for database operations.
/// </summary>
/// <typeparam name="TEntity">The entity type.</typeparam>
/// <typeparam name="TKey">The type of the entity's primary key.</typeparam>
public interface IRepository<TEntity, TKey>
    where TEntity : BaseEntity<TKey>
    where TKey : IEquatable<TKey>
{
    /// <summary>
    /// Gets all entities.
    /// </summary>
    /// <param name="include">Navigation properties to include.</param>
    /// <param name="disableTracking">Whether to disable entity tracking.</param>
    /// <returns>All entities.</returns>
    Task<IEnumerable<TEntity>> GetAllAsync(
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null,
        bool disableTracking = true);

    /// <summary>
    /// Gets entities based on a predicate.
    /// </summary>
    /// <param name="predicate">The filter condition.</param>
    /// <param name="include">Navigation properties to include.</param>
    /// <param name="disableTracking">Whether to disable entity tracking.</param>
    /// <returns>Filtered entities.</returns>
    Task<IEnumerable<TEntity>> GetAsync(
        Expression<Func<TEntity, bool>> predicate,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null,
        bool disableTracking = true);

    /// <summary>
    /// Gets a single entity by its ID.
    /// </summary>
    /// <param name="id">The entity ID.</param>
    /// <param name="include">Navigation properties to include.</param>
    /// <param name="disableTracking">Whether to disable entity tracking.</param>
    /// <returns>The entity if found, otherwise null.</returns>
    Task<TEntity?> GetByIdAsync(
        TKey id,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null,
        bool disableTracking = true);

    /// <summary>
    /// Gets a single entity based on a predicate.
    /// </summary>
    /// <param name="predicate">The filter condition.</param>
    /// <param name="include">Navigation properties to include.</param>
    /// <param name="disableTracking">Whether to disable entity tracking.</param>
    /// <returns>The entity if found, otherwise null.</returns>
    Task<TEntity?> GetFirstAsync(
        Expression<Func<TEntity, bool>> predicate,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null,
        bool disableTracking = true);

    /// <summary>
    /// Adds a new entity.
    /// </summary>
    /// <param name="entity">The entity to add.</param>
    /// <returns>The added entity.</returns>
    Task<TEntity> AddAsync(TEntity entity);

    /// <summary>
    /// Adds a range of entities.
    /// </summary>
    /// <param name="entities">The entities to add.</param>
    /// <returns>A task representing the operation.</returns>
    Task AddRangeAsync(IEnumerable<TEntity> entities);

    /// <summary>
    /// Adds a large number of entities in an efficient way using bulk operations.
    /// </summary>
    /// <param name="entities">The collection of entities to add.</param>
    /// <param name="batchSize">The size of each batch (default is 1000).</param>
    /// <returns>A task representing the operation.</returns>
    /// <remarks>
    /// This method is optimized for inserting large numbers of entities.
    /// It breaks the collection into smaller batches to improve performance
    /// and reduce memory usage.
    /// </remarks>
    Task AddBulkAsync(IEnumerable<TEntity> entities, int batchSize = 1000);

    /// <summary>
    /// Updates an existing entity.
    /// </summary>
    /// <param name="entity">The entity to update.</param>
    /// <returns>The updated entity.</returns>
    Task<TEntity> UpdateAsync(TEntity entity);

    /// <summary>
    /// Deletes an entity by its ID.
    /// </summary>
    /// <param name="id">The ID of the entity to delete.</param>
    /// <returns>A task representing the operation.</returns>
    Task DeleteAsync(TKey id);

    /// <summary>
    /// Deletes an entity.
    /// </summary>
    /// <param name="entity">The entity to delete.</param>
    /// <returns>A task representing the operation.</returns>
    Task DeleteAsync(TEntity entity);

    /// <summary>
    /// Deletes a range of entities.
    /// </summary>
    /// <param name="entities">The entities to delete.</param>
    /// <returns>A task representing the operation.</returns>
    Task DeleteRangeAsync(IEnumerable<TEntity> entities);

    /// <summary>
    /// Checks if any entity satisfies the specified condition.
    /// </summary>
    /// <param name="predicate">The condition to check.</param>
    /// <returns>True if any entity satisfies the condition; otherwise, false.</returns>
    Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate);

    /// <summary>
    /// Counts entities that satisfy the specified condition.
    /// </summary>
    /// <param name="predicate">The condition to check.</param>
    /// <returns>The number of entities that satisfy the condition.</returns>
    Task<int> CountAsync(Expression<Func<TEntity, bool>>? predicate = null);

    /// <summary>
    /// Gets a queryable collection of entities.
    /// </summary>
    /// <param name="disableTracking">Whether to disable entity tracking.</param>
    /// <returns>A queryable collection of entities.</returns>
    IQueryable<TEntity> GetQueryable(bool disableTracking = true);
}
