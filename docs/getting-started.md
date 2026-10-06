# Getting Started

This guide walks through installing LowDb.Net and progressively adding capabilities: a basic
synchronous database, the async database, dependency injection, backup support, the repository
pattern, and browser storage for Blazor apps. For the conceptual background, see
[Overview](overview.md). For exhaustive type-by-type details, see the
[API reference index](README.md#packages).

## Table of Contents
* [Prerequisites](#prerequisites)
* [Installation](#installation)
* [1. Your first database (synchronous)](#1-your-first-database-synchronous)
* [2. Defining your document and entities](#2-defining-your-document-and-entities)
* [3. Reading, updating, and batching changes](#3-reading-updating-and-batching-changes)
* [4. Switching to the async database](#4-switching-to-the-async-database)
* [5. Choosing a storage backend with the builder](#5-choosing-a-storage-backend-with-the-builder)
* [6. Configuring JSON serialization](#6-configuring-json-serialization)
* [7. Enabling backup / recovery](#7-enabling-backup--recovery)
* [8. Using dependency injection](#8-using-dependency-injection)
* [9. Adding the repository pattern](#9-adding-the-repository-pattern)
* [10. Using browser storage in Blazor](#10-using-browser-storage-in-blazor)
* [11. Testing with the in-memory adapter](#11-testing-with-the-in-memory-adapter)
* [Next steps](#next-steps)

## Prerequisites

* .NET 9 or .NET 10 SDK.
* A console, web, or Blazor project to add the package to.

## Installation

Install the core package from NuGet:

```powershell
Install-Package LowDb.Net
```

Add the optional packages only if you need them:

```powershell
# Repository pattern (CRUD/querying/Result<T> over a LowDb document)
Install-Package LowDb.Net.Repositories

# Blazor browser local/session storage support
Install-Package LowDb.Net.Browser
```

## 1. Your first database (synchronous)

The fastest way to get a working database is `LowDbFactory.CreateJsonLowDb<T>`, which persists a
document of type `T` to a JSON file:

```csharp
using D20Tek.LowDb;

var db = LowDbFactory.CreateJsonLowDb<TasksDocument>("my-tasks.json");
```

If `my-tasks.json` does not exist yet, it is created automatically (along with any missing parent
folder) the first time you write to it.

## 2. Defining your document and entities

A LowDb document is a plain class with a public parameterless constructor. Expose one or more
collections (typically `HashSet<T>`) for the entities you want to store:

```csharp
public class TasksDocument
{
	public int LastId { get; set; } = 0;

	public HashSet<TaskEntity> Tasks { get; init; } = [];

	public int GetNextId() => ++LastId;
}

public class TaskEntity
{
	public int Id { get; set; }

	public string Name { get; set; } = string.Empty;

	public bool IsCompleted { get; set; }
}
```

> If you plan to use the repository pattern (see [step 9](#9-adding-the-repository-pattern)),
> your document should instead derive from `DbDocument` in the `LowDb.Net.Repositories` package.

## 3. Reading, updating, and batching changes

`LowDb<T>` keeps the document in memory and loads it from storage on first access. Use `Get()` to
read the live document, and `Update()` to mutate it:

```csharp
// Get() loads the document from the JSON file on first call, then returns the cached instance.
var document = db.Get();
Console.WriteLine($"Current task count: {document.Tasks.Count}");

// Update() mutates the document and, by default, immediately persists the change.
db.Update(x => x.Tasks.Add(new TaskEntity
{
	Id = x.GetNextId(),
	Name = "Write docs",
	IsCompleted = false
}));
```

To batch multiple changes into a single file write, pass `autoSave: false` and call `Write()`
explicitly once you are done:

```csharp
db.Update(x => x.Tasks.Add(new TaskEntity { Id = document.GetNextId(), Name = "Task A" }), autoSave: false);
db.Update(x => x.Tasks.Add(new TaskEntity { Id = document.GetNextId(), Name = "Task B" }), autoSave: false);
db.Write(); // persists both changes in one write
```

`LowDb<T>` serializes concurrent `Read`, `Write`, `Get`, and `Update` calls on the same instance
with an internal lock, so multiple callers in the same process cannot corrupt the file or
overwrite each other's changes.

## 4. Switching to the async database

For async-first applications (ASP.NET Core, Blazor, I/O-bound console apps), use
`LowDbAsync<T>` via `CreateJsonLowDbAsync`. The API mirrors the synchronous version, with
`CancellationToken` support throughout:

```csharp
using D20Tek.LowDb;

var db = LowDbFactory.CreateJsonLowDbAsync<TasksDocument>("my-tasks.json");

await db.Update(x => x.Tasks.Add(new TaskEntity
{
	Id = x.GetNextId(),
	Name = "Ship release"
}), token: cancellationToken);

var document = await db.Get(cancellationToken);
```

`LowDbAsync<T>` implements `IDisposable` to release its internal `SemaphoreSlim`; dispose it (or
let your DI container do so) when the database is no longer needed.

## 5. Choosing a storage backend with the builder

`LowDbFactory.CreateLowDb` / `CreateLowDbAsync` accept a builder callback for more control over the
storage backend - for example, placing the file in a specific folder, or swapping to an
in-memory store for local development or testing:

```csharp
var db = LowDbFactory.CreateLowDb<TasksDocument>(b =>
{
	if (useInMemory)
	{
		b.UseInMemoryDatabase();
	}
	else
	{
		b.UseFileDatabase("my-tasks.json").WithFolder("data");
	}
});
```

## 6. Configuring JSON serialization

By default, JSON file adapters serialize with camel-case property names and case-insensitive,
trailing-comma-tolerant deserialization. Supply your own `JsonSerializerOptions` when you need
custom converters, `WriteIndented` output, or a source-generated `JsonSerializerContext` (useful
for Blazor WebAssembly AOT/trimming scenarios):

```csharp
using System.Text.Json;

var options = new JsonSerializerOptions { WriteIndented = true };

var db = LowDbFactory.CreateJsonLowDb<TasksDocument>("my-tasks.json", serializerOptions: options);

// ...or through the builder:
var db2 = LowDbFactory.CreateLowDb<TasksDocument>(b =>
	b.UseFileDatabase("my-tasks.json").WithJsonSerializerOptions(options));
```

## 7. Enabling backup / recovery

Pass `enableBackup: true` (or call `WithBackup()` on the builder) to keep a sibling `.bak` file
with the previous good state. Internally, this composes a `BackupStorageAdapter<T>` around the
primary JSON file adapter: before each write, the previous value is copied to the backup file, and
if the primary file is missing or contains invalid JSON on read, the backup is used as a fallback.

```csharp
var db = LowDbFactory.CreateJsonLowDb<TasksDocument>("my-tasks.json", enableBackup: true);

// ...or through the builder:
var db2 = LowDbFactory.CreateLowDb<TasksDocument>(b =>
	b.UseFileDatabase("my-tasks.json").WithBackup());
```

See [BackupStorageAdapter\<T\>](api-reference-lowdb.md#backupstorageadaptert) for the exact
fallback and rethrow semantics.

## 8. Using dependency injection

In an ASP.NET Core or other DI-based application, register the database with
`IServiceCollection` instead of constructing it directly:

```csharp
using D20Tek.LowDb;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddLowDbAsync<TasksDocument>(b =>
	b.UseFileDatabase("tasks.json")
	 .WithFolder("data")
	 .WithLifetime(ServiceLifetime.Scoped));

var app = builder.Build();
```

Inject `LowDbAsync<TasksDocument>` (or `LowDb<TasksDocument>` for the synchronous database) into
your endpoints, controllers, or services as usual. A `string filename` overload is also available
for simple cases that do not need the builder:

```csharp
builder.Services.AddLowDb<TasksDocument>("my-tasks.json", enableBackup: true);
```

## 9. Adding the repository pattern

The optional `LowDb.Net.Repositories` package adds a `Result<T>`-based CRUD abstraction over a
LowDb document. Derive your document from `DbDocument`, then derive a repository from
`LowDbRepository<TEntity, TDocument>` (or `LowDbAsyncRepository<TEntity, TDocument>`), selecting
the entity set it manages:

```csharp
using D20Tek.LowDb;
using D20Tek.LowDb.Repositories;

public class TasksDocument : DbDocument
{
	public HashSet<TaskEntity> Tasks { get; set; } = [];
}

public interface ITasksRepository : IRepositoryAsync<TaskEntity>;

public class TasksRepository(LowDbAsync<TasksDocument> db)
	: LowDbAsyncRepository<TaskEntity, TasksDocument>(db, x => x.Tasks), ITasksRepository
{
}
```

Register the repository alongside the database and consume it through its interface:

```csharp
builder.Services.AddLowDbAsync<TasksDocument>(b => b.UseFileDatabase("tasks.json"));
builder.Services.AddScoped<ITasksRepository, TasksRepository>();
```

```csharp
var addResult = await repository.AddAsync(new TaskEntity { Id = 1, Name = "Write docs" });
await repository.AddAsync(new TaskEntity { Id = 2, Name = "Ship release" });

var saveResult = await repository.SaveChangesAsync();
if (saveResult.IsFailure)
{
	// inspect saveResult.Error
}
```

Every repository method returns a `Result<T>` (success or a descriptive failure) instead of
throwing, so callers can branch on outcome without try/catch. See the
[Repositories API reference](api-reference-lowdb-repositories.md) for the full member list.

## 10. Using browser storage in Blazor

The optional `LowDb.Net.Browser` package persists a document to the browser's local or session
storage instead of a server-side file, which is the right fit for Blazor WebAssembly apps where
each user's data should stay in their own browser.

Register the storage-backed database in `Program.cs`:

```csharp
using D20Tek.LowDb.Browser;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

// Persist to local storage (survives across browser sessions).
builder.Services.AddLocalLowDbAsync<TasksDocument>("d20tek-tasks");

// ...or session storage (cleared when the tab/session ends):
// builder.Services.AddSessionLowDbAsync<TasksDocument>("d20tek-tasks-session");

await builder.Build().RunAsync();
```

Inject `LowDbAsync<TasksDocument>` into your components or a repository exactly as you would for
the file-based database - the browser storage adapter implements the same
`IStorageAdapterAsync<T>` seam. Backup support and custom serializer options are available through
the same `enableBackup` and `configureStorage` parameters documented in the
[Browser API reference](api-reference-lowdb-browser.md#dependencyinjection).

## 11. Testing with the in-memory adapter

For unit tests, avoid touching the file system by using the in-memory adapter via the builder:

```csharp
var db = LowDbFactory.CreateLowDb<TasksDocument>(b => b.UseInMemoryDatabase());

// Exercise your code against `db` exactly as you would in production,
// with no files created or cleaned up.
```

This is the same mechanism the `Sample.Cli` and `Sample.AsyncCli` projects use to support an
`--in-memory` command-line switch for local development.

## Next steps

* Browse the full [API reference](README.md#packages) for every type, member, and parameter.
* Review the sample projects in the main [README](../README.md#samples) for complete, runnable
  applications (console, async console, minimal API, and Blazor WebAssembly).
* Read the [Overview](overview.md) for the problem LowDb.Net solves and where it fits relative to
  other options like SQLite or LiteDB.
* Check the [future features backlog](../.plans/future-features.md) for what's being considered
  next.
