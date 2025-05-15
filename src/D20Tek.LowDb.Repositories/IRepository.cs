using D20Tek.Functional;
using System.Linq.Expressions;

namespace D20Tek.LowDb.Repositories;

public interface IRepository<TEntity> where TEntity : class
{
    // Get all entities
    Result<IEnumerable<TEntity>> GetAll();

    // Get a single entity by its primary key
    public Result<TEntity> GetById<TProperty>(Expression<Func<TEntity, TProperty>> idSelector, TProperty id)
        where TProperty : notnull;

    // Query with predicate
    Result<IEnumerable<TEntity>> Find(Expression<Func<TEntity, bool>> predicate);

    // Check existence
    Result<bool> Exists(Expression<Func<TEntity, bool>> predicate);

    // Add an entity
    Result<TEntity> Add(TEntity entity);

    // Add multiple entities
    Result<IEnumerable<TEntity>> AddRange(IEnumerable<TEntity> entities);

    // Remove an entity
    Result<TEntity> Remove(TEntity entity);

    // Remove multiple entities
    Result<IEnumerable<TEntity>> RemoveRange(IEnumerable<TEntity> entities);

    // Update an entity
    Result<TEntity> Update(TEntity entity);

    // Save changes (allows multiple batched changes per save)
    Result<bool> SaveChanges();
}
