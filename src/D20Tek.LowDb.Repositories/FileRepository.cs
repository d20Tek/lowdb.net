using D20Tek.Functional;
using System.Linq.Expressions;

namespace D20Tek.LowDb.Repositories;

public sealed class FileRepository<TEntity, TDocument> : IRepository<TEntity>
    where TEntity : class
    where TDocument : DbDocument, new()
{
    public static Result<T> NotFoundError<T>(object id) where T : notnull =>
        Result<T>.Failure(Error.NotFound("Entity.NotFound", $"Entity with id={id} not found."));

    public static Failure<int> AlreadyExistsError(int id) =>
        Error.Conflict("Entry.AlreadyExists", $"Entry with id={id} already exists.");

    private readonly LowDb<TDocument> _db;
    private readonly Func<TDocument, HashSet<TEntity>> GetHashSet;

    public FileRepository(LowDb<TDocument> db, Expression<Func<TDocument, HashSet<TEntity>>> setSelector)
    {
        _db = db;
        GetHashSet = setSelector.Compile();
    }

    public Result<TEntity[]> GetAllAsync(CancellationToken cancellationToken = default) =>
        Try(() => GetHashSet(_db.Get()).ToArray());

    public Result<TEntity> GetByIdAsync<TProperty>(
        Expression<Func<TEntity, TProperty>> idSelector,
        TProperty id,
        CancellationToken cancellationToken = default)
        where TProperty : notnull =>
        Try(() =>
        {
            var getId = idSelector.Compile();
            var entity = GetHashSet(_db.Get()).FirstOrDefault(x => getId(x)?.Equals(id) ?? false);
            return entity ?? NotFoundError<TEntity>(id);
        }).Flatten();
    
    public Result<TEntity[]> FindAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default) => throw new NotImplementedException();
    
    public Result<bool> ExistsAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default) => throw new NotImplementedException();
    
    public Result<TEntity> AddAsync(TEntity entity, CancellationToken cancellationToken = default) => throw new NotImplementedException();
    
    public Result<TEntity[]> AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default) => throw new NotImplementedException();
    
    public Result<TEntity> RemoveAsync(TEntity entity, CancellationToken cancellationToken = default) => throw new NotImplementedException();
    
    public Result<TEntity[]> RemoveRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default) => throw new NotImplementedException();
    
    public Result<TEntity> Update(TEntity entity) => throw new NotImplementedException();

    private static Result<T> Try<T>(Func<T> operation) where T : notnull
    {
        try
        {
            return operation();
        }
        catch (Exception ex)
        {
            return Result<T>.Failure(Error.Exception(ex));
        }
    }
}
