# Overview

## What is LowDb.Net?

LowDb.Net is a lightweight, file-based document database for .NET. It keeps a single strongly
typed document in memory, exposes simple `Get` / `Update` / `Read` / `Write` operations, and
persists the document through a pluggable storage adapter - a JSON file by default, but also
plain text, in-memory, or browser local/session storage.

It is a .NET port of the idea behind the JavaScript [lowdb](https://github.com/typicode/lowdb)
package: a "low" ceremony database for applications that need durable local state without the
operational overhead of a real database engine.

## The problem it solves

Many applications - console tools, small web APIs, Blazor apps, prototypes, local utilities - need
to persist a modest amount of structured state (a task list, a settings document, a cache, user
preferences) but do not need a relational database, a document database server, or an ORM. Reaching
for SQLite, LiteDB, or a hosted database for this kind of workload adds setup, migrations, and
operational concerns disproportionate to the problem being solved. The alternative - hand-rolling
`File.ReadAllText` / `JsonSerializer.Deserialize` / mutate / `JsonSerializer.Serialize` /
`File.WriteAllText` in every project - is tedious to get right and easy to get wrong: concurrent
callers can race and corrupt the file, a crash mid-write can leave a truncated file, and the
read/deserialize/mutate/serialize/write cycle gets copy-pasted and subtly modified each time.

LowDb.Net packages that pattern once, correctly, and lets you opt into the pieces you need:

* **A single typed document in memory**, so your code works with plain C# objects instead of
  re-parsing JSON on every access.
* **In-process concurrency safety**, so concurrent `Read`, `Write`, and `Update` calls on a shared
  instance cannot corrupt the store or silently drop each other's changes.
* **A storage adapter seam** (`IStorageAdapter<T>` / `IStorageAdapterAsync<T>`), so the same
  programming model works against a JSON file, an in-memory store (ideal for unit tests), or
  browser local/session storage in a Blazor client app - and you can implement your own adapter
  for anything else.
* **Optional backup/recovery**, so a corrupted or missing primary file can fall back to a `.bak`
  copy instead of losing data.
* **Builder, factory, and dependency injection extensions**, so wiring a database into a console
  app, a minimal API, or a DI container is a couple of lines, not boilerplate.
* **An optional repository layer** (`LowDb.Net.Repositories`), so you can work with CRUD, querying,
  and batched saves through a `Result<T>`-returning interface instead of mutating the document
  directly, when that fits your application's architecture better.

## What LowDb.Net is not

LowDb.Net is intentionally scoped to **local, single-machine, low-to-moderate write contention**
scenarios:

* It is **not** a replacement for SQLite, LiteDB, or a hosted database when you need cross-process
  or multi-instance coordination on the same file, large datasets (the whole document is
  (de)serialized on every write), transactions, indexing, or query planning.
* Its concurrency guarantees are **in-process and per-instance only**. Two separate processes (or
  two separate `LowDb`/`LowDbAsync` instances) pointed at the same file can still race.
* It favors **simplicity over scale**. Once a JSON database file grows into the tens of megabytes,
  expect write-time performance to degrade, since each write serializes the entire object graph.

See the [README Limits section](../README.md#limits) for the full list of constraints, and the
[future features backlog](../.plans/future-features.md) for improvements under consideration (for
example, atomic/durable file writes).

## Where LowDb.Net fits

| Package | Use it when... |
|---|---|
| `LowDb.Net` | You need a typed document persisted to a JSON file, plain text, or in-memory store in a console app, service, or ASP.NET Core API with low write contention. |
| `LowDb.Net.Browser` | You're building a Blazor WebAssembly (or Blazor Server, for low-usage scenarios) app and want to persist a document to the browser's local or session storage, per user/session. |
| `LowDb.Net.Repositories` | You want a CRUD/query abstraction (`IRepository<TEntity>` / `IRepositoryAsync<TEntity>`) with `Result<T>`-based error handling layered over a LowDb document, instead of mutating the document directly. |

## Next steps

* New to the library? Follow the [Getting Started guide](getting-started.md) for a step-by-step
  walkthrough covering installation, your first database, async usage, dependency injection,
  the repository pattern, and Blazor browser storage.
* Need the full type-by-type API? See the [API reference index](README.md#packages).
* Already comfortable with the basics? Check the sample projects referenced in the main
  [README](../README.md#samples) for complete, runnable examples.
