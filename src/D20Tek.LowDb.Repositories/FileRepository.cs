using D20Tek.Functional;
using D20Tek.LowDb;
using System.Linq.Expressions;

namespace D20Tek.LowDb.Repositories;

public sealed class FileRepository<TEntity> : IRepository<TEntity>
    where TEntity : class
{
    public static Failure<T> NotFoundError<T>(int id) where T : notnull =>
        Error.NotFound("Entity.NotFound", $"Entity with id={id} not found.");

    public static Failure<int> AlreadyExistsError(int id) =>
        Error.Conflict("Entry.AlreadyExists", $"Entry with id={id} already exists.");

    //private readonly LowDb<TStore> _db;

    //public FileRepository(LowDb<TStore> db) => _db = db;

    public Result<TEntity[]> GetAllAsync(CancellationToken cancellationToken = default) => throw new NotImplementedException();

    public Result<TEntity> GetByIdAsync<TProperty>(Expression<Func<TEntity, TProperty>> idSelector, TProperty id, CancellationToken cancellationToken = default) => throw new NotImplementedException();
    
    public Result<TEntity[]> FindAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default) => throw new NotImplementedException();
    
    public Result<bool> ExistsAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default) => throw new NotImplementedException();
    
    public Result<TEntity> AddAsync(TEntity entity, CancellationToken cancellationToken = default) => throw new NotImplementedException();
    
    public Result<TEntity[]> AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default) => throw new NotImplementedException();
    
    public Result<TEntity> RemoveAsync(TEntity entity, CancellationToken cancellationToken = default) => throw new NotImplementedException();
    
    public Result<TEntity[]> RemoveRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default) => throw new NotImplementedException();
    
    public Result<TEntity> Update(TEntity entity) => throw new NotImplementedException();
}
