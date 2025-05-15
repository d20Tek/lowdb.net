using D20Tek.Functional;
using System.Linq.Expressions;

public interface IRepositoryAsync<TEntity> where TEntity : class
{
    // Get all entities
    Task<Result<IEnumerable<TEntity>>> GetAllAsync(CancellationToken token = default);

    // Get a single entity by its primary key
    public Task<Result<TEntity>> GetByIdAsync<TProperty>(
        Expression<Func<TEntity, TProperty>> idSelector,
        TProperty id,
        CancellationToken token = default);

    // Query with predicate
    Task<Result<IEnumerable<TEntity>>> FindAsync(
        Expression<Func<TEntity, bool>> predicate,
        CancellationToken token = default);

    // Check existence
    Task<Result<bool>> ExistsAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken token = default);

    // Add an entity
    Task<Result<TEntity>> AddAsync(TEntity entity, CancellationToken token = default);

    // Add multiple entities
    Task<Result<IEnumerable<TEntity>>> AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken token = default);

    // Remove an entity
    Task<Result<TEntity>> RemoveAsync(TEntity entity, CancellationToken token = default);

    // Remove multiple entities
    Task<Result<IEnumerable<TEntity>>> RemoveRangeAsync(
        IEnumerable<TEntity> entities,
        CancellationToken token = default);

    // Update an entity
    Task<Result<TEntity>> UpdateAsync(TEntity entity);

    // Save changes (allows multiple batched changes per save)
    Task<Result<bool>> SaveChangesAsync(CancellationToken token = default);
}
