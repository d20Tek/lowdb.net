using D20Tek.Functional;
using System.Linq.Expressions;

namespace D20Tek.LowDb.Repositories;

/// <summary>
/// Defines a synchronous repository abstraction over a LowDb document that exposes CRUD,
/// range, query, and change-persistence operations for a single entity type. Every operation
/// returns a <see cref="Result{T}"/> to communicate success or a descriptive failure.
/// </summary>
/// <typeparam name="TEntity">The entity type managed by the repository.</typeparam>
public interface IRepository<TEntity> where TEntity : class
{
    /// <summary>
    /// Gets all entities in the underlying entity set.
    /// </summary>
    /// <returns>A result containing the entities, or a failure when the read fails.</returns>
    Result<IEnumerable<TEntity>> GetAll();

    /// <summary>
    /// Gets a single entity by matching the value of its identifier property.
    /// </summary>
    /// <typeparam name="TProperty">The type of the identifier property.</typeparam>
    /// <param name="idSelector">An expression that selects the identifier property on the entity.</param>
    /// <param name="id">The identifier value to match.</param>
    /// <returns>A result containing the matching entity, or a not-found failure.</returns>
    public Result<TEntity> GetById<TProperty>(Expression<Func<TEntity, TProperty>> idSelector, TProperty id)
        where TProperty : notnull;

    /// <summary>
    /// Finds all entities that satisfy the specified predicate.
    /// </summary>
    /// <param name="predicate">The filter predicate applied to the entity set.</param>
    /// <returns>A result containing the matching entities, or a failure when the query fails.</returns>
    Result<IEnumerable<TEntity>> Find(Expression<Func<TEntity, bool>> predicate);

    /// <summary>
    /// Determines whether any entity satisfies the specified predicate.
    /// </summary>
    /// <param name="predicate">The predicate to evaluate against the entity set.</param>
    /// <returns>A result containing <see langword="true"/> when a match exists; otherwise <see langword="false"/>.</returns>
    Result<bool> Exists(Expression<Func<TEntity, bool>> predicate);

    /// <summary>
    /// Adds an entity to the entity set.
    /// </summary>
    /// <param name="entity">The entity to add.</param>
    /// <returns>A result containing the added entity, or a conflict failure when it already exists.</returns>
    Result<TEntity> Add(TEntity entity);

    /// <summary>
    /// Adds multiple entities to the entity set.
    /// </summary>
    /// <param name="entities">The entities to add.</param>
    /// <returns>A result containing the added entities, or a conflict failure on the first entity that cannot be added.</returns>
    Result<IEnumerable<TEntity>> AddRange(IEnumerable<TEntity> entities);

    /// <summary>
    /// Removes an entity from the entity set.
    /// </summary>
    /// <param name="entity">The entity to remove.</param>
    /// <returns>A result containing the removed entity, or a failure when it could not be removed.</returns>
    Result<TEntity> Remove(TEntity entity);

    /// <summary>
    /// Removes multiple entities from the entity set.
    /// </summary>
    /// <param name="entities">The entities to remove.</param>
    /// <returns>A result containing the removed entities, or a failure on the first entity that cannot be removed.</returns>
    Result<IEnumerable<TEntity>> RemoveRange(IEnumerable<TEntity> entities);

    /// <summary>
    /// Marks an entity as updated. Because entities are held by reference, in-place mutations
    /// are already reflected in the entity set; call <see cref="SaveChanges"/> to persist them.
    /// </summary>
    /// <param name="entity">The entity that was updated.</param>
    /// <returns>A result containing the updated entity.</returns>
    Result<TEntity> Update(TEntity entity);

    /// <summary>
    /// Persists all pending changes to the underlying LowDb document. Multiple batched changes
    /// can be applied before a single save.
    /// </summary>
    /// <returns>A result containing <see langword="true"/> on success, or a failure when the write fails.</returns>
    Result<bool> SaveChanges();
}
