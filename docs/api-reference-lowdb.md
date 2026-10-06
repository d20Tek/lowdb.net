# API Reference: D20Tek.LowDb

The core `D20Tek.LowDb` package provides the synchronous (`LowDb<T>`) and asynchronous
(`LowDbAsync<T>`) document databases, the storage adapter abstraction, built-in file/memory
adapters, a fluent builder, factory methods, and dependency injection extensions.

## Table of Contents
* [LowDb\<T\>](#lowdbt)
* [LowDbAsync\<T\>](#lowdbasynct)
* [IStorageAdapter\<T\>](#istorageadaptert)
* [IStorageAdapterAsync\<T\>](#istorageadapterasynct)
* [Adapters](#adapters)
  * [JsonFileAdapter\<T\>](#jsonfileadaptert)
  * [JsonFileAdapterAsync\<T\>](#jsonfileadapterasynct)
  * [TextFileAdapter](#textfileadapter)
  * [TextFileAdapterAsync](#textfileadapterasync)
  * [MemoryStorageAdapter\<T\>](#memorystorageadaptert)
  * [MemoryStorageAdapterAsync\<T\>](#memorystorageadapterasynct)
  * [BackupStorageAdapter\<T\>](#backupstorageadaptert)
  * [BackupStorageAdapterAsync\<T\>](#backupstorageadapterasynct)
* [LowDbBuilder](#lowdbbuilder)
* [LowDbFactory](#lowdbfactory)
* [DependencyInjection](#dependencyinjection)

---

## LowDb\<T\>

```csharp
namespace D20Tek.LowDb;

public class LowDb<T>(IStorageAdapter<T> storageAdapter, T? data = null)
	where T : class, new()
```

A lightweight, synchronous, file-backed document database that keeps a single strongly typed
document in memory and persists it through an `IStorageAdapter<T>`. Access to the document is
serialized with an internal lock so that concurrent callers on the same instance cannot
overwrite one another's changes.

### Constructor

| Parameter | Type | Description |
|---|---|---|
| `storageAdapter` | `IStorageAdapter<T>` | The storage adapter used to read and write the document. |
| `data` | `T?` | Optional pre-loaded document. When supplied, the database skips the initial read from storage. |

### Members

| Member | Description |
|---|---|
| `void Read()` | Reads the document from storage and replaces the in-memory state. Falls back to a new empty document when the store is empty. |
| `void Write()` | Persists the current in-memory document to storage. |
| `T Get()` | Gets the current in-memory document, loading it from storage on first access. Mutations to the returned instance are reflected in the database. |
| `void Update(Action<T> updateAction, bool autoSave = true)` | Applies a mutation to the document; by default persists the result as part of the same serialized operation. |

### Example

```csharp
var db = LowDbFactory.CreateJsonLowDb<TasksDocument>("my-tasks.json");

db.Update(x => x.Tasks.Add(new TaskEntity { Id = x.GetNextId(), Name = "Write docs" }));

var document = db.Get();
```

---

## LowDbAsync\<T\>

```csharp
namespace D20Tek.LowDb;

public class LowDbAsync<T>(IStorageAdapterAsync<T> storageAdapter, T? data = null) : IDisposable
	where T : class, new()
```

The asynchronous counterpart to `LowDb<T>`. Keeps a single strongly typed document in memory and
persists it through an `IStorageAdapterAsync<T>`. Access is serialized with an internal
`SemaphoreSlim`, making it suitable for file, in-memory, and browser web-storage backends.

### Constructor

| Parameter | Type | Description |
|---|---|---|
| `storageAdapter` | `IStorageAdapterAsync<T>` | The asynchronous storage adapter used to read and write the document. |
| `data` | `T?` | Optional pre-loaded document. When supplied, the database skips the initial read from storage. |

### Members

| Member | Description |
|---|---|
| `Task Read(CancellationToken token = default)` | Reads the document from storage and replaces the in-memory state. |
| `Task Write(CancellationToken token = default)` | Persists the current in-memory document to storage. |
| `Task<T> Get(CancellationToken token = default)` | Gets the current in-memory document, loading it from storage on first access. |
| `Task Update(Action<T> updateAction, bool autoSave = true, CancellationToken token = default)` | Applies a mutation to the document; by default persists the result as part of the same serialized operation. |
| `void Dispose()` | Releases the internal `SemaphoreSlim` used to guard concurrent access. |

### Example

```csharp
var db = LowDbFactory.CreateJsonLowDbAsync<TasksDocument>("my-tasks.json");

await db.Update(x => x.Tasks.Add(new TaskEntity { Id = x.GetNextId(), Name = "Ship release" }));

var document = await db.Get();
```

---

## IStorageAdapter\<T\>

```csharp
namespace D20Tek.LowDb;

public interface IStorageAdapter<T> where T : class
{
	T? Read();
	void Write(T data);
}
```

Defines a synchronous storage backend that a `LowDb<T>` instance uses to persist and retrieve its
document. `Read` returns `null` when the store does not yet contain data. Implement this
interface to target custom stores (databases, cloud blobs, etc.).

---

## IStorageAdapterAsync\<T\>

```csharp
namespace D20Tek.LowDb;

public interface IStorageAdapterAsync<T> where T : class
{
	Task<T?> Read(CancellationToken token = default);
	Task Write(T data, CancellationToken token = default);
}
```

The asynchronous counterpart to `IStorageAdapter<T>`, used by `LowDbAsync<T>`. Implemented by the
built-in file, memory, and backup adapters, as well as the browser local/session storage adapters
in the `D20Tek.LowDb.Browser` package.

---

## Adapters

All built-in adapters live in the `D20Tek.LowDb.Adapters` namespace.

### JsonFileAdapter\<T\>

```csharp
public class JsonFileAdapter<T>(string filename, JsonSerializerOptions? serializerOptions = null)
	: IStorageAdapter<T> where T : class
```

Persists a document to a JSON file. By default, serializes with camel-case property names and
case-insensitive, trailing-comma-tolerant deserialization; supply a custom
`JsonSerializerOptions` to override this behavior (custom converters, `WriteIndented`, a
source-generated `JsonSerializerContext` for AOT/trimming scenarios, etc.). Internally composes a
`TextFileAdapter` for raw file I/O.

### JsonFileAdapterAsync\<T\>

```csharp
public class JsonFileAdapterAsync<T>(string filename, JsonSerializerOptions? serializerOptions = null)
	: IStorageAdapterAsync<T> where T : class
```

The asynchronous counterpart to `JsonFileAdapter<T>`, built on `TextFileAdapterAsync`.

### TextFileAdapter

```csharp
public class TextFileAdapter(string filename) : IStorageAdapter<string>
```

A low-level synchronous adapter that reads and writes raw text to a file, creating the containing
folder when necessary. Serves as the file-access layer used by `JsonFileAdapter<T>`.

### TextFileAdapterAsync

```csharp
public class TextFileAdapterAsync(string filename) : IStorageAdapterAsync<string>
```

The asynchronous counterpart to `TextFileAdapter`, used by `JsonFileAdapterAsync<T>`.

### MemoryStorageAdapter\<T\>

```csharp
public class MemoryStorageAdapter<T> : IStorageAdapter<T> where T : class
```

Keeps the document in memory only, with no file or external persistence. Data is lost when the
adapter instance is discarded. Useful for unit tests and transient scenarios.

### MemoryStorageAdapterAsync\<T\>

```csharp
public class MemoryStorageAdapterAsync<T> : IStorageAdapterAsync<T> where T : class
```

The asynchronous counterpart to `MemoryStorageAdapter<T>`.

### BackupStorageAdapter\<T\>

```csharp
public class BackupStorageAdapter<T>(IStorageAdapter<T> primary, IStorageAdapter<T> backup)
	: IStorageAdapter<T> where T : class
```

A synchronous storage adapter *decorator* that adds backup/recovery semantics to any
`IStorageAdapter<T>`. Before each write, the previous value read from the wrapped primary adapter
is written to a backup adapter. On read, if the primary adapter returns `null` or throws a
`JsonException`, the backup adapter is consulted as a fallback; if the backup is also unavailable,
the original exception is rethrown. Composed automatically by `LowDbBuilder.WithBackup()`,
`LowDbFactory`, and the dependency injection extensions when `enableBackup: true` is specified -
you generally do not need to construct this type directly.

### BackupStorageAdapterAsync\<T\>

```csharp
public class BackupStorageAdapterAsync<T>(IStorageAdapterAsync<T> primary, IStorageAdapterAsync<T> backup)
	: IStorageAdapterAsync<T> where T : class
```

The asynchronous counterpart to `BackupStorageAdapter<T>`.

---

## LowDbBuilder

```csharp
namespace D20Tek.LowDb;

public class LowDbBuilder
```

A fluent builder for configuring and creating `LowDb<T>` and `LowDbAsync<T>` instances. Select a
storage backend (file or in-memory), an optional folder, JSON serializer options, backup support,
and the dependency injection service lifetime.

### Members

| Member | Description |
|---|---|
| `ServiceLifetime ServiceLifetime { get; }` | The service lifetime used for dependency injection registration. Defaults to `ServiceLifetime.Singleton`. |
| `LowDbBuilder UseFileDatabase(string filename)` | Configures the database to persist to a JSON file with the specified name. |
| `LowDbBuilder UseInMemoryDatabase()` | Configures the database to store its document in memory only, with no file persistence. |
| `LowDbBuilder WithFolder(string folderName)` | Sets the folder in which the database file is stored, combined with the file name from `UseFileDatabase`. |
| `LowDbBuilder WithJsonSerializerOptions(JsonSerializerOptions serializerOptions)` | Sets the `JsonSerializerOptions` used to serialize/deserialize the document. Has no effect for in-memory databases. |
| `LowDbBuilder WithBackup()` | Enables backup support for the JSON file database by composing a `BackupStorageAdapter<T>`/`BackupStorageAdapterAsync<T>` around the primary adapter. Has no effect for in-memory databases. |
| `LowDbBuilder WithLifetime(ServiceLifetime serviceLifetime)` | Sets the service lifetime used when the database is registered with a dependency injection container. |
| `LowDb<T> Build<T>()` | Builds a synchronous `LowDb<T>` instance using the configured storage backend. |
| `LowDbAsync<T> BuildAsync<T>()` | Builds an asynchronous `LowDbAsync<T>` instance using the configured storage backend. |

### Example

```csharp
var db = LowDbFactory.CreateLowDb<TasksDocument>(b =>
{
	b.UseFileDatabase("my-tasks.json")
	 .WithFolder("data")
	 .WithBackup()
	 .WithJsonSerializerOptions(new JsonSerializerOptions { WriteIndented = true });
});
```

---

## LowDbFactory

```csharp
namespace D20Tek.LowDb;

public static class LowDbFactory
```

Provides factory methods for creating `LowDb<T>` and `LowDbAsync<T>` instances, either directly
from a JSON file name or through a fluent `LowDbBuilder` configuration callback.

### Members

| Member | Description |
|---|---|
| `LowDb<T> CreateJsonLowDb<T>(string filename, JsonSerializerOptions? serializerOptions = null, bool enableBackup = false)` | Creates a synchronous JSON file-backed database using the specified file name. |
| `LowDb<T> CreateLowDb<T>(Action<LowDbBuilder> builderAction)` | Creates a synchronous database configured through the supplied builder callback. |
| `LowDbAsync<T> CreateJsonLowDbAsync<T>(string filename, JsonSerializerOptions? serializerOptions = null, bool enableBackup = false)` | Creates an asynchronous JSON file-backed database using the specified file name. |
| `LowDbAsync<T> CreateLowDbAsync<T>(Action<LowDbBuilder> builderAction)` | Creates an asynchronous database configured through the supplied builder callback. |

When `enableBackup` is `true`, these methods compose a `BackupStorageAdapter<T>` /
`BackupStorageAdapterAsync<T>` targeting a sibling `<filename>.bak` file around the primary JSON
file adapter.

---

## DependencyInjection

```csharp
namespace D20Tek.LowDb;

public static class DependencyInjection
```

Extension methods on `IServiceCollection` for registering `LowDb<T>` and `LowDbAsync<T>`.

### Members

| Member | Description |
|---|---|
| `IServiceCollection AddLowDb<T>(this IServiceCollection services, string filename, ServiceLifetime lifetime = ServiceLifetime.Singleton, JsonSerializerOptions? serializerOptions = null, bool enableBackup = false)` | Registers a synchronous JSON file-backed `LowDb<T>`. |
| `IServiceCollection AddLowDb<T>(this IServiceCollection services, Action<LowDbBuilder> builderAction)` | Registers a synchronous `LowDb<T>` configured through a builder callback. The lifetime comes from `LowDbBuilder.ServiceLifetime`. |
| `IServiceCollection AddLowDbAsync<T>(this IServiceCollection services, string filename, ServiceLifetime lifetime = ServiceLifetime.Singleton, JsonSerializerOptions? serializerOptions = null, bool enableBackup = false)` | Registers an asynchronous JSON file-backed `LowDbAsync<T>`. |
| `IServiceCollection AddLowDbAsync<T>(this IServiceCollection services, Action<LowDbBuilder> builderAction)` | Registers an asynchronous `LowDbAsync<T>` configured through a builder callback. |

### Example

```csharp
services.AddLowDb<TasksDocument>("my-tasks.json", enableBackup: true);

services.AddLowDbAsync<TasksDocument>(b =>
{
	b.UseFileDatabase("my-tasks.json").WithLifetime(ServiceLifetime.Scoped);
});
```
