# LowDb.Net Documentation

LowDb.Net is a .NET implementation of the [lowdb](https://github.com/typicode/lowdb) file-based
database concept: a minimal, type-safe document database for applications and experiments that
do not need a full database engine.

This documentation set covers the public API of each package in the LowDb.Net family. For
installation instructions, quick-start usage, and sample project links, see the main
[README](../README.md). For a history of released changes, see [CHANGELOG.md](../CHANGELOG.md).

## Start here

| Document | Description |
|---|---|
| [overview.md](overview.md) | Introduces LowDb.Net, the problem it solves, what it is (and is not) intended for, and where it fits relative to alternatives like SQLite or LiteDB. |
| [getting-started.md](getting-started.md) | Step-by-step guide: installation, your first database, async usage, builder configuration, JSON options, backup, dependency injection, the repository pattern, Blazor browser storage, and testing. |

## Packages

| Package | Description | API Reference |
|---|---|---|
| `LowDb.Net` | Core synchronous/asynchronous document database, storage adapters (JSON file, text file, in-memory, backup decorator), builder, factory, and dependency injection extensions. | [api-reference-lowdb.md](api-reference-lowdb.md) |
| `LowDb.Net.Browser` | Storage adapters and dependency injection extensions for persisting a LowDb document to browser local/session storage in Blazor client applications. | [api-reference-lowdb-browser.md](api-reference-lowdb-browser.md) |
| `LowDb.Net.Repositories` | Repository pattern abstraction (`IRepository`/`IRepositoryAsync`) over a LowDb document, with `Result<T>`-based CRUD, querying, and batched `SaveChanges`. | [api-reference-lowdb-repositories.md](api-reference-lowdb-repositories.md) |

## Architecture overview

```
LowDb<T> / LowDbAsync<T>              <- in-memory document + serialized read/write
		|
IStorageAdapter<T> / IStorageAdapterAsync<T>   <- storage abstraction
		|
   +----+-------------------+----------------------+
   |                        |                       |
JsonFileAdapter<T>   MemoryStorageAdapter<T>   Browser storage adapters
   |                                            (LocalStorageAdapterAsync<T>,
TextFileAdapter                                 SessionStorageAdapterAsync<T>)
```

- **`LowDb<T>` / `LowDbAsync<T>`** own the in-memory document and guarantee that concurrent
  `Read`, `Write`, `Get`, and `Update` calls on the same instance are serialized.
- **`IStorageAdapter<T>` / `IStorageAdapterAsync<T>`** define the storage seam. Any adapter -
  file, in-memory, browser storage, or a custom implementation - can be plugged in.
- **`BackupStorageAdapter<T>` / `BackupStorageAdapterAsync<T>`** are decorators that add optional
  `.bak`-style backup and recovery semantics around any adapter, composed automatically by
  `LowDbBuilder.WithBackup()`, `LowDbFactory`, and the dependency injection extensions.
- **`LowDbBuilder`** and **`LowDbFactory`** provide fluent and direct construction paths that
  compose the right adapter stack (file/memory, serializer options, backup) for you.
- **Dependency injection extensions** (`AddLowDb`, `AddLowDbAsync`, `AddLocalLowDbAsync`,
  `AddSessionLowDbAsync`) register a fully configured database with an `IServiceCollection`.
- **`LowDb.Net.Repositories`** layers `IRepository<TEntity>` / `IRepositoryAsync<TEntity>` on top
  of a `LowDb<TDocument>` / `LowDbAsync<TDocument>`, projecting a `HashSet<TEntity>` entity set
  out of the document and returning `Result<T>` from every operation.

## Where to start

- New to LowDb.Net? Start with [overview.md](overview.md) for the problem it solves, then follow
  [getting-started.md](getting-started.md) for a hands-on walkthrough.
- Building a custom storage backend? See [IStorageAdapter\<T\> / IStorageAdapterAsync\<T\>](api-reference-lowdb.md#istorageadaptert)
  in the core API reference.
- Using Blazor local/session storage? See [api-reference-lowdb-browser.md](api-reference-lowdb-browser.md).
- Want CRUD/querying instead of raw document access? See [api-reference-lowdb-repositories.md](api-reference-lowdb-repositories.md).
- Curious what's planned next? See [.plans/future-features.md](../.plans/future-features.md).
