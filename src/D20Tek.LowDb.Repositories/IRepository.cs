using D20Tek.Functional;
using System.Linq.Expressions;

namespace D20Tek.LowDb.Repositories;

public interface IRepository<T> where T : class
{
    // Get all entities
    Result<T[]> GetAll(CancellationToken cancellationToken = default);

    // Get a single entity by its primary key
    public Result<T> GetById<TProperty>(
        Expression<Func<T, TProperty>> idSelector,
        TProperty id,
        CancellationToken cancellationToken = default)
        where TProperty : notnull;

    // Query with predicate
    Result<T[]> Find(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);

    // Check existence
    Result<bool> Exists(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);

    // Add an entity
    Result<T> Add(T entity, CancellationToken cancellationToken = default);

    // Add multiple entities
    Result<T[]> AddRange(IEnumerable<T> entities, CancellationToken cancellationToken = default);

    // Remove an entity
    Result<T> Remove(T entity, CancellationToken cancellationToken = default);

    // Remove multiple entities
    Result<T[]> RemoveRange(IEnumerable<T> entities, CancellationToken cancellationToken = default);

    // Update an entity
    Result<T> Update(T entity);

    // Save changes (allows multiple batched changes per save)
    Result<int> SaveChanges(CancellationToken cancellationToken = default);
}
