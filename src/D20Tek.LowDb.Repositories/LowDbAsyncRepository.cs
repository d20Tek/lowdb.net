using D20Tek.Functional;
using D20Tek.Functional.Async;
using System.Linq.Expressions;

namespace D20Tek.LowDb.Repositories;

public class LowDbAsyncRepository<TEntity, TDocument> : IRepositoryAsync<TEntity>
    where TEntity : class
    where TDocument : DbDocument, new()
{
    private readonly LowDbAsync<TDocument> _db;
    private readonly Func<TDocument, HashSet<TEntity>> GetHashSet;

    public LowDbAsyncRepository(LowDbAsync<TDocument> db, Expression<Func<TDocument, HashSet<TEntity>>> setSelector)
    {
        _db = db;
        GetHashSet = setSelector.Compile();
    }

    public async Task<Result<IEnumerable<TEntity>>> GetAllAsync(CancellationToken token = default) =>
        await TryAsync.RunAsync(async () =>
            Result<IEnumerable<TEntity>>.Success(GetHashSet(await _db.Get(token)).AsEnumerable()));

    public async Task<Result<TEntity>> GetByIdAsync<TProperty>(
        Expression<Func<TEntity, TProperty>> idSelector,
        TProperty id,
        CancellationToken token = default)
        where TProperty : notnull =>
        await TryAsync.RunAsync(async () =>
        {
            var getId = idSelector.Compile();
            var entity = GetHashSet(await _db.Get(token)).FirstOrDefault(x => getId(x)!.Equals(id));
            return entity ?? Errors.NotFoundError<TEntity>(id);
        });

    public async Task<Result<IEnumerable<TEntity>>> FindAsync(
        Expression<Func<TEntity, bool>> predicate,
        CancellationToken token = default) =>
        await TryAsync.RunAsync(async () => Result<IEnumerable<TEntity>>.Success(
            GetHashSet(await _db.Get(token)).AsQueryable().Where(predicate).AsEnumerable()));

    public async Task<Result<bool>> ExistsAsync(
        Expression<Func<TEntity, bool>> predicate,
        CancellationToken token = default) =>
        await TryAsync.RunAsync(async () =>
            Result<bool>.Success(GetHashSet(await _db.Get(token)).AsQueryable().Any(predicate)));

    public async Task<Result<TEntity>> AddAsync(TEntity entity, CancellationToken token = default) =>
        await TryAsync.RunAsync(async () =>
        {
            var doc = await _db.Get(token);
            var added = GetHashSet(doc).Add(entity);
            return added ? entity :  Errors.AddFailedError<TEntity>(entity);
        });
    
    public async Task<Result<IEnumerable<TEntity>>> AddRangeAsync(
        IEnumerable<TEntity> entities,
        CancellationToken token = default) =>
        await TryAsync.RunAsync(async () =>
        {
            var set = GetHashSet(await _db.Get(token));
            foreach (var entity in entities)
            {
                var added = set.Add(entity);
                if (added is false) return Errors.AddFailedError<IEnumerable<TEntity>>(entity);
            }
            return Result<IEnumerable<TEntity>>.Success(entities);
        });
    
    public async Task<Result<TEntity>> RemoveAsync(TEntity entity, CancellationToken token = default) =>
        await TryAsync.RunAsync(async () =>
        {
            var removed = GetHashSet(await _db.Get(token)).Remove(entity);
            return removed ? entity : Errors.RemoveFailedError<TEntity>(entity);
        });
    
    public async Task<Result<IEnumerable<TEntity>>> RemoveRangeAsync(
        IEnumerable<TEntity> entities,
        CancellationToken token = default) =>
        await TryAsync.RunAsync(async () =>
        {
            var set = GetHashSet(await _db.Get(token));
            foreach (var entity in entities)
            {
                var removed = set.Remove(entity);
                if (removed is false) return Errors.RemoveFailedError<IEnumerable<TEntity>>(entity);
            }
            return Result<IEnumerable<TEntity>>.Success(entities);
        });
    
    public Task<Result<TEntity>> UpdateAsync(TEntity entity, CancellationToken token = default) =>
        Task.FromResult(Result<TEntity>.Success(entity));

    public async Task<Result<bool>> SaveChangesAsync(CancellationToken token = default) =>
        await TryAsync.RunAsync(async () =>
        {
            await _db.Write(token);
            return Result<bool>.Success(true);
        });
}
