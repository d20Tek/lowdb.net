using D20Tek.Functional;
using System.Linq.Expressions;

namespace D20Tek.LowDb.Repositories;

public class LowDbRepository<TEntity, TDocument> : IRepository<TEntity>
    where TEntity : class
    where TDocument : DbDocument, new()
{
    private readonly LowDb<TDocument> _db;
    private readonly Func<TDocument, HashSet<TEntity>> GetHashSet;

    public LowDbRepository(LowDb<TDocument> db, Expression<Func<TDocument, HashSet<TEntity>>> setSelector)
    {
        _db = db;
        GetHashSet = setSelector.Compile();
    }

    public Result<IEnumerable<TEntity>> GetAll() =>
        Try.Run(() => Result<IEnumerable<TEntity>>.Success(GetHashSet(_db.Get()).AsEnumerable()));

    public Result<TEntity> GetById<TProperty>(Expression<Func<TEntity, TProperty>> idSelector, TProperty id)
        where TProperty : notnull =>
        Try.Run(() =>
        {
            var getId = idSelector.Compile();
            var entity = GetHashSet(_db.Get()).FirstOrDefault(x => getId(x)!.Equals(id));
            return entity ?? Errors.NotFoundError<TEntity>(id);
        });

    public Result<IEnumerable<TEntity>> Find(Expression<Func<TEntity, bool>> predicate) =>
        Try.Run(() => Result<IEnumerable<TEntity>>.Success(
            GetHashSet(_db.Get()).AsQueryable().Where(predicate).AsEnumerable()));

    public Result<bool> Exists(Expression<Func<TEntity, bool>> predicate) =>
        Try.Run(() => Result<bool>.Success(GetHashSet(_db.Get()).AsQueryable().Any(predicate)));

    public Result<TEntity> Add(TEntity entity) =>
        Try.Run(() =>
        {
            var added = GetHashSet(_db.Get()).Add(entity);
            return added ? entity :  Errors.AddFailedError<TEntity>(entity);
        });
    
    public Result<IEnumerable<TEntity>> AddRange(IEnumerable<TEntity> entities) =>
        Try.Run(() =>
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
        Try.Run(() =>
        {
            var removed = GetHashSet(_db.Get()).Remove(entity);
            return removed ? entity : Errors.RemoveFailedError<TEntity>(entity);
        });
    
    public Result<IEnumerable<TEntity>> RemoveRange(IEnumerable<TEntity> entities) =>
        Try.Run(() =>
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
        Try.Run(() =>
        {
            _db.Write();
            return Result<bool>.Success(true);
        });
}
