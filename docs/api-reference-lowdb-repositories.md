# API Reference: D20Tek.LowDb.Repositories

The `D20Tek.LowDb.Repositories` package adds a repository abstraction over a LowDb database. It
exposes CRUD, range, querying, existence checks, and batched save operations, with every method
returning a `Result<T>` (from `D20Tek.Functional`) so callers can handle success and failure
without exceptions.

## Table of Contents
* [DbDocument](#dbdocument)
* [IRepository\<TEntity\>](#irepositorytentity)
* [IRepositoryAsync\<TEntity\>](#irepositoryasynctentity)
* [LowDbRepository\<TEntity, TDocument\>](#lowdbrepositorytentity-tdocument)
* [LowDbAsyncRepository\<TEntity, TDocument\>](#lowdbasyncrepositorytentity-tdocument)
* [Errors](#errors)

---

## DbDocument

```csharp
namespace D20Tek.LowDb.Repositories;

public abstract class DbDocument
```

The base type for LowDb database documents used with the repository pattern. A document
typically exposes one or more `HashSet<T>` collections that a repository projects to and manages
as entity sets.

### Example

```csharp
public class TasksDocument : DbDocument
{
	public HashSet<TaskEntity> Tasks { get; set; } = [];
}
```

---

## IRepository\<TEntity\>

```csharp
namespace D20Tek.LowDb.Repositories;

public interface IRepository<TEntity> where TEntity : class
```

Defines a synchronous repository abstraction over a LowDb document that exposes CRUD, range,
query, and change-persistence operations for a single entity type.

### Members

| Member | Description |
|---|---|
| `Result<IEnumerable<TEntity>> GetAll()` | Gets all entities in the underlying entity set. |
| `Result<TEntity> GetById<TProperty>(Expression<Func<TEntity, TProperty>> idSelector, TProperty id)` | Gets a single entity by matching the value of its identifier property. Returns a not-found failure when no match exists. |
| `Result<IEnumerable<TEntity>> Find(Expression<Func<TEntity, bool>> predicate)` | Finds all entities that satisfy the specified predicate. |
| `Result<bool> Exists(Expression<Func<TEntity, bool>> predicate)` | Determines whether any entity satisfies the specified predicate. |
| `Result<TEntity> Add(TEntity entity)` | Adds an entity to the entity set. Returns a conflict failure when it already exists. |
| `Result<IEnumerable<TEntity>> AddRange(IEnumerable<TEntity> entities)` | Adds multiple entities. Returns a conflict failure on the first entity that cannot be added. |
| `Result<TEntity> Remove(TEntity entity)` | Removes an entity from the entity set. Returns a failure when it could not be removed. |
| `Result<IEnumerable<TEntity>> RemoveRange(IEnumerable<TEntity> entities)` | Removes multiple entities. Returns a failure on the first entity that cannot be removed. |
| `Result<TEntity> Update(TEntity entity)` | Marks an entity as updated. Entities are held by reference, so in-place mutations are already reflected in the entity set; call `SaveChanges` to persist them. |
| `Result<bool> SaveChanges()` | Persists all pending changes to the underlying LowDb document. Multiple batched changes can be applied before a single save. |

---

## IRepositoryAsync\<TEntity\>

```csharp
namespace D20Tek.LowDb.Repositories;

public interface IRepositoryAsync<TEntity> where TEntity : class
```

The asynchronous counterpart to `IRepository<TEntity>`. Every member mirrors its synchronous
equivalent, accepts a `CancellationToken token = default`, and returns a `Task<Result<T>>`.

### Members

| Member | Description |
|---|---|
| `Task<Result<IEnumerable<TEntity>>> GetAllAsync(CancellationToken token = default)` | Gets all entities in the underlying entity set. |
| `Task<Result<TEntity>> GetByIdAsync<TProperty>(Expression<Func<TEntity, TProperty>> idSelector, TProperty id, CancellationToken token = default)` | Gets a single entity by matching the value of its identifier property. |
| `Task<Result<IEnumerable<TEntity>>> FindAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken token = default)` | Finds all entities that satisfy the specified predicate. |
| `Task<Result<bool>> ExistsAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken token = default)` | Determines whether any entity satisfies the specified predicate. |
| `Task<Result<TEntity>> AddAsync(TEntity entity, CancellationToken token = default)` | Adds an entity to the entity set. |
| `Task<Result<IEnumerable<TEntity>>> AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken token = default)` | Adds multiple entities to the entity set. |
| `Task<Result<TEntity>> RemoveAsync(TEntity entity, CancellationToken token = default)` | Removes an entity from the entity set. |
| `Task<Result<IEnumerable<TEntity>>> RemoveRangeAsync(IEnumerable<TEntity> entities, CancellationToken token = default)` | Removes multiple entities from the entity set. |
| `Task<Result<TEntity>> UpdateAsync(TEntity entity, CancellationToken token = default)` | Marks an entity as updated; call `SaveChangesAsync` to persist in-place mutations. |
| `Task<Result<bool>> SaveChangesAsync(CancellationToken token = default)` | Persists all pending changes to the underlying LowDb document. |

---

## LowDbRepository\<TEntity, TDocument\>

```csharp
namespace D20Tek.LowDb.Repositories;

public class LowDbRepository<TEntity, TDocument>(
	LowDb<TDocument> db,
	Expression<Func<TDocument, HashSet<TEntity>>> setSelector) : IRepository<TEntity>
	where TEntity : class
	where TDocument : DbDocument, new()
```

A synchronous `IRepository<TEntity>` implementation that manages a `HashSet<TEntity>` stored
within a LowDb document. Read operations query the in-memory document (via `LowDb<TDocument>.Get()`)
while `SaveChanges` persists the current state through the underlying database
(`LowDb<TDocument>.Write()`).

### Constructor

| Parameter | Type | Description |
|---|---|---|
| `db` | `LowDb<TDocument>` | The LowDb database that stores the document. |
| `setSelector` | `Expression<Func<TDocument, HashSet<TEntity>>>` | An expression that selects the entity set within the document. |

### Example

```csharp
public interface ITasksRepository : IRepository<TaskEntity>;

public class TasksRepository(LowDb<TasksDocument> db)
	: LowDbRepository<TaskEntity, TasksDocument>(db, x => x.Tasks), ITasksRepository
{
}

var result = repository.Add(new TaskEntity { Id = 1, Name = "Write docs" });
repository.SaveChanges();
```

---

## LowDbAsyncRepository\<TEntity, TDocument\>

```csharp
namespace D20Tek.LowDb.Repositories;

public class LowDbAsyncRepository<TEntity, TDocument>(
	LowDbAsync<TDocument> db,
	Expression<Func<TDocument, HashSet<TEntity>>> setSelector) : IRepositoryAsync<TEntity>
	where TEntity : class
	where TDocument : DbDocument, new()
```

The asynchronous counterpart to `LowDbRepository<TEntity, TDocument>`, backed by
`LowDbAsync<TDocument>`.

### Constructor

| Parameter | Type | Description |
|---|---|---|
| `db` | `LowDbAsync<TDocument>` | The asynchronous LowDb database that stores the document. |
| `setSelector` | `Expression<Func<TDocument, HashSet<TEntity>>>` | An expression that selects the entity set within the document. |

### Example

```csharp
public interface ITasksRepository : IRepositoryAsync<TaskEntity>;

public class TasksRepository(LowDbAsync<TasksDocument> db)
	: LowDbAsyncRepository<TaskEntity, TasksDocument>(db, x => x.Tasks), ITasksRepository
{
}

await repository.AddAsync(new TaskEntity { Id = 1, Name = "Write docs" });
await repository.AddAsync(new TaskEntity { Id = 2, Name = "Ship release" });
var result = await repository.SaveChangesAsync();
```

---

## Errors

```csharp
namespace D20Tek.LowDb.Repositories;

internal static class Errors
```

Internal helper methods that build standardized `Result<T>` failures used throughout the
repository implementations:

| Member | Description |
|---|---|
| `Result<T> NotFoundError<T>(object id)` | Builds a not-found failure for a missing entity identifier. |
| `Result<T> AddFailedError<T>(object entity)` | Builds a conflict failure when an entity cannot be added (for example, a duplicate in the `HashSet<T>`). |
| `Result<T> RemoveFailedError<T>(object entity)` | Builds a failure when an entity cannot be removed. |

This type is internal and not part of the package's public API; it is documented here for
completeness when reading the repository source.
