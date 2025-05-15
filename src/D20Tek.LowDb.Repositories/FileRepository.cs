using D20Tek.Functional;
using System.Linq.Expressions;

namespace D20Tek.LowDb.Repositories;

public sealed class FileRepository<TEntity, TDocument> : IRepository<TEntity>
    where TEntity : class
    where TDocument : DbDocument, new()
{
    public static Result<T> NotFoundError<T>(object id) where T : notnull =>
        Result<T>.Failure(Error.NotFound("Entity.NotFound", $"Entity with id={id} not found."));

    private readonly LowDb<TDocument> _db;
    private readonly Func<TDocument, HashSet<TEntity>> GetHashSet;

    public FileRepository(LowDb<TDocument> db, Expression<Func<TDocument, HashSet<TEntity>>> setSelector)
    {
        _db = db;
        GetHashSet = setSelector.Compile();
    }

    public Result<IEnumerable<TEntity>> GetAll() =>
        Try(() => GetHashSet(_db.Get()).AsEnumerable());

    public Result<TEntity> GetById<TProperty>(Expression<Func<TEntity, TProperty>> idSelector, TProperty id)
        where TProperty : notnull =>
        Try(() =>
        {
            var getId = idSelector.Compile();
            var entity = GetHashSet(_db.Get()).FirstOrDefault(x => getId(x)?.Equals(id) ?? false);
            return entity ?? NotFoundError<TEntity>(id);
        }).Flatten();

    public Result<IEnumerable<TEntity>> Find(Expression<Func<TEntity, bool>> predicate) =>
        Try(() => GetHashSet(_db.Get()).AsQueryable().Where(predicate).AsEnumerable());

    public Result<bool> Exists(Expression<Func<TEntity, bool>> predicate) =>
        Try(() => GetHashSet(_db.Get()).AsQueryable().Any(predicate));

    public Result<TEntity> Add(TEntity entity) =>
        Try(() =>
        {
            GetHashSet(_db.Get()).Add(entity);
            return entity;
        });
    
    public Result<IEnumerable<TEntity>> AddRange(IEnumerable<TEntity> entities) =>
        Try(() =>
        {
            var set = GetHashSet(_db.Get());
            foreach (var entity in entities)
            {
                set.Add(entity);
            }
            return entities;
        });
    
    public Result<TEntity> Remove(TEntity entity) => throw new NotImplementedException();
    
    public Result<IEnumerable<TEntity>> RemoveRange(IEnumerable<TEntity> entities) => throw new NotImplementedException();
    
    public Result<TEntity> Update(TEntity entity) => throw new NotImplementedException();

    public Result<bool> SaveChanges() => throw new NotImplementedException();

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
