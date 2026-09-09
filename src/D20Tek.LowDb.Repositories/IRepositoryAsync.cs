using D20Tek.Functional;
using System.Linq.Expressions;

namespace D20Tek.LowDb.Repositories;

/// <summary>
/// Defines an asynchronous repository abstraction over a LowDb document that exposes CRUD,
/// range, query, and change-persistence operations for a single entity type. Every operation
/// returns a <see cref="Result{T}"/> to communicate success or a descriptive failure.
/// </summary>
/// <typeparam name="TEntity">The entity type managed by the repository.</typeparam>
public interface IRepositoryAsync<TEntity> where TEntity : class
{
    /// <summary>
    /// Asynchronously gets all entities in the underlying entity set.
    /// </summary>
    /// <param name="token">A token used to observe cancellation requests.</param>
    /// <returns>A task whose result contains the entities, or a failure when the read fails.</returns>
    Task<Result<IEnumerable<TEntity>>> GetAllAsync(CancellationToken token = default);

    /// <summary>
    /// Asynchronously gets a single entity by matching the value of its identifier property.
    /// </summary>
    /// <typeparam name="TProperty">The type of the identifier property.</typeparam>
    /// <param name="idSelector">An expression that selects the identifier property on the entity.</param>
    /// <param name="id">The identifier value to match.</param>
    /// <param name="token">A token used to observe cancellation requests.</param>
    /// <returns>A task whose result contains the matching entity, or a not-found failure.</returns>
    public Task<Result<TEntity>> GetByIdAsync<TProperty>(
        Expression<Func<TEntity, TProperty>> idSelector,
        TProperty id,
        CancellationToken token = default)
        where TProperty : notnull;

    /// <summary>
    /// Asynchronously finds all entities that satisfy the specified predicate.
    /// </summary>
    /// <param name="predicate">The filter predicate applied to the entity set.</param>
    /// <param name="token">A token used to observe cancellation requests.</param>
    /// <returns>A task whose result contains the matching entities, or a failure when the query fails.</returns>
    Task < Result<IEnumerable<TEntity>>> FindAsync(
        Expression<Func<TEntity, bool>> predicate,
        CancellationToken token = default);

    /// <summary>
    /// Asynchronously determines whether any entity satisfies the specified predicate.
    /// </summary>
    /// <param name="predicate">The predicate to evaluate against the entity set.</param>
    /// <param name="token">A token used to observe cancellation requests.</param>
    /// <returns>A task whose result contains <see langword="true"/> when a match exists; otherwise <see langword="false"/>.</returns>
    Task<Result<bool>> ExistsAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken token = default);

    /// <summary>
    /// Asynchronously adds an entity to the entity set.
    /// </summary>
    /// <param name="entity">The entity to add.</param>
    /// <param name="token">A token used to observe cancellation requests.</param>
    /// <returns>A task whose result contains the added entity, or a conflict failure when it already exists.</returns>
    Task<Result<TEntity>> AddAsync(TEntity entity, CancellationToken token = default);

    /// <summary>
    /// Asynchronously adds multiple entities to the entity set.
    /// </summary>
    /// <param name="entities">The entities to add.</param>
    /// <param name="token">A token used to observe cancellation requests.</param>
    /// <returns>A task whose result contains the added entities, or a conflict failure on the first entity that cannot be added.</returns>
    Task<Result<IEnumerable<TEntity>>> AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken token = default);

    /// <summary>
    /// Asynchronously removes an entity from the entity set.
    /// </summary>
    /// <param name="entity">The entity to remove.</param>
    /// <param name="token">A token used to observe cancellation requests.</param>
    /// <returns>A task whose result contains the removed entity, or a failure when it could not be removed.</returns>
    Task<Result<TEntity>> RemoveAsync(TEntity entity, CancellationToken token = default);

    /// <summary>
    /// Asynchronously removes multiple entities from the entity set.
    /// </summary>
    /// <param name="entities">The entities to remove.</param>
    /// <param name="token">A token used to observe cancellation requests.</param>
    /// <returns>A task whose result contains the removed entities, or a failure on the first entity that cannot be removed.</returns>
    Task<Result<IEnumerable<TEntity>>> RemoveRangeAsync(
        IEnumerable<TEntity> entities,
        CancellationToken token = default);

    /// <summary>
    /// Asynchronously marks an entity as updated. Because entities are held by reference, in-place
    /// mutations are already reflected in the entity set; call <see cref="SaveChangesAsync"/> to persist them.
    /// </summary>
    /// <param name="entity">The entity that was updated.</param>
    /// <param name="token">A token used to observe cancellation requests.</param>
    /// <returns>A task whose result contains the updated entity.</returns>
    Task<Result<TEntity>> UpdateAsync(TEntity entity, CancellationToken token = default);

    /// <summary>
    /// Asynchronously persists all pending changes to the underlying LowDb document. Multiple
    /// batched changes can be applied before a single save.
    /// </summary>
    /// <param name="token">A token used to observe cancellation requests.</param>
    /// <returns>A task whose result contains <see langword="true"/> on success, or a failure when the write fails.</returns>
    Task<Result<bool>> SaveChangesAsync(CancellationToken token = default);
}
