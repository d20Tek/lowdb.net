using D20Tek.Functional;
using System.Linq.Expressions;

namespace D20Tek.LowDb.Repositories;

public sealed class FileRepository<TEntity, TDocument> : IRepository<TEntity>
    where TEntity : class
    where TDocument : DbDocument, new()
{
    private readonly LowDb<TDocument> _db;
    private readonly Func<TDocument, HashSet<TEntity>> GetHashSet;

    public FileRepository(LowDb<TDocument> db, Expression<Func<TDocument, HashSet<TEntity>>> setSelector)
    {
        _db = db;
        GetHashSet = setSelector.Compile();
    }

    public Result<IEnumerable<TEntity>> GetAll() =>
        Try(() => Result<IEnumerable<TEntity>>.Success(GetHashSet(_db.Get()).AsEnumerable()));

    public Result<TEntity> GetById<TProperty>(Expression<Func<TEntity, TProperty>> idSelector, TProperty id)
        where TProperty : notnull =>
        Try(() =>
        {
            var getId = idSelector.Compile();
            var entity = GetHashSet(_db.Get()).FirstOrDefault(x => getId(x)?.Equals(id) ?? false);
            return entity ?? Errors.NotFoundError<TEntity>(id);
        });

    public Result<IEnumerable<TEntity>> Find(Expression<Func<TEntity, bool>> predicate) =>
        Try(() => Result<IEnumerable<TEntity>>.Success(
            GetHashSet(_db.Get()).AsQueryable().Where(predicate).AsEnumerable()));

    public Result<bool> Exists(Expression<Func<TEntity, bool>> predicate) =>
        Try(() => Result<bool>.Success(GetHashSet(_db.Get()).AsQueryable().Any(predicate)));

    public Result<TEntity> Add(TEntity entity) =>
        Try(() =>
        {
            var added = GetHashSet(_db.Get()).Add(entity);
            return added ? entity :  Errors.AddFailedError<TEntity>(entity);
        });
    
    public Result<IEnumerable<TEntity>> AddRange(IEnumerable<TEntity> entities) =>
        Try(() =>
        {
            var set = GetHashSet(_db.Get());
            foreach (var entity in entities)
            {
                var added = set.Add(entity);
                if (added is false) return Errors.AddFailedError<IEnumerable<TEntity>>(entity);
            }
            return Result<IEnumerable<TEntity>>.Success(entities);
        });
    
    public Result<TEntity> Remove(TEntity entity) =>
        Try(() =>
        {
            var removed = GetHashSet(_db.Get()).Remove(entity);
            return removed ? entity : Errors.RemoveFailedError<TEntity>(entity);
        });
    
    public Result<IEnumerable<TEntity>> RemoveRange(IEnumerable<TEntity> entities) =>
        Try(() =>
        {
            var set = GetHashSet(_db.Get());
            foreach (var entity in entities)
            {
                var removed = set.Remove(entity);
                if (removed is false) return Errors.RemoveFailedError<IEnumerable<TEntity>>(entity);
            }
            return Result<IEnumerable<TEntity>>.Success(entities);
        });
    
    public Result<TEntity> Update(TEntity entity) => Result<TEntity>.Success(entity);

    public Result<bool> SaveChanges() => 
        Try(() =>
        {
            _db.Write();
            return Result<bool>.Success(true);
        });

    private static Result<T> Try<T>(Func<Result<T>> operation) where T : notnull
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
