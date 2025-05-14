using D20Tek.Functional;

namespace D20Tek.LowDb.Repositories;

public interface IRepository<TEntity, TId>
    where TEntity : IEntity<TId>
    where TId : notnull, IEquatable<TId>
{
    Result<TEntity> Create(TEntity entity);

    Result<TEntity> Delete(TId id);

    Result<TEntity[]> DeleteMany(TEntity[] entities);

    TEntity[] GetEntities();

    Result<TEntity> GetEntityById(TId id);

    Result<TEntity> Update(TEntity entity);
}
