using D20Tek.Functional;
using System.Linq.Expressions;

public interface IRepositoryAsync<T> where T : class
{
    // Get all entities
    Task<Result<T[]>> GetAllAsync(CancellationToken cancellationToken = default);

    // Get a single entity by its primary key
    public Task<Result<T>> GetByIdAsync<TProperty>(
        Expression<Func<T, TProperty>> idSelector,
        TProperty id,
        CancellationToken cancellationToken = default);

    // Query with predicate
    Task<Result<T[]>> FindAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);

    // Check existence
    Task<Result<bool>> ExistsAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);

    // Add an entity
    Task<Result<T>> AddAsync(T entity, CancellationToken cancellationToken = default);

    // Add multiple entities
    Task<Result<T[]>> AddRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default);

    // Remove an entity
    Task<Result<T>> RemoveAsync(T entity, CancellationToken cancellationToken = default);

    // Remove multiple entities
    Task<Result<T[]>> RemoveRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default);

    // Update an entity (you might customize this for your use case)
    Task<Result<T>> Update(T entity);
}
