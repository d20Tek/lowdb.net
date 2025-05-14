using D20Tek.Functional;
using System.Linq.Expressions;

namespace D20Tek.LowDb.Repositories;

public interface IRepository<T> where T : class
{
    // Get all entities
    Result<T[]> GetAllAsync(CancellationToken cancellationToken = default);

    // Get a single entity by its primary key
    public Result<T> GetByIdAsync<TProperty>(
        Expression<Func<T, TProperty>> idSelector,
        TProperty id,
        CancellationToken cancellationToken = default);

    // Query with predicate
    Result<T[]> FindAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);

    // Check existence
    Result<bool> ExistsAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);

    // Add an entity
    Result<T> AddAsync(T entity, CancellationToken cancellationToken = default);

    // Add multiple entities
    Result<T[]> AddRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default);

    // Remove an entity
    Result<T> RemoveAsync(T entity, CancellationToken cancellationToken = default);

    // Remove multiple entities
    Result<T[]> RemoveRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default);

    // Update an entity (you might customize this for your use case)
    Result<T> Update(T entity);
}
