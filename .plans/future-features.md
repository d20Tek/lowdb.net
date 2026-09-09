# LowDb.Net - Future Feature Candidates

This document captures potential features for the LowDb.Net project, reviewed against the
current functionality. Items are tiered by value-to-effort. Nothing here is committed work;
it is a backlog of candidates for discussion and prioritization.

## Current functionality (baseline)

- **Core:** `LowDb<T>` / `LowDbAsync<T>` with load-on-demand, in-memory model, batched write
  (UnitOfWork), and in-process concurrency guards (`lock` for sync, `SemaphoreSlim` for async).
- **Adapters:** JSON file, text file, and in-memory adapters behind `IStorageAdapter<T>` /
  `IStorageAdapterAsync<T>`. Browser (local/session) storage via the sibling package.
- **Configuration:** builder + factory + dependency injection extensions with lifetime control.
- **Repositories layer:** `IRepository` / `IRepositoryAsync` with `Result<T>` (railway-oriented),
  LINQ predicates, CRUD, range operations, and `SaveChanges`.

## High value, good fit

### 1. Atomic / durable file writes (write-temp-then-rename)
Today `File.WriteAllText(path, ...)` writes in place, so a crash or exception mid-write can leave a
truncated or corrupt file. Writing to `path.tmp` then `File.Move(tmp, path, overwrite: true)`
(atomic rename on the same volume) ensures a reader or restart never sees a half-written file.
Complements the in-process concurrency work: concurrency guards protect within the process, atomic
writes protect against crash / torn files. Small, contained change to the two file adapters.
**Highest priority.**

### 2. Backup / recovery option
Optionally keep a `.bak` of the last-good file before overwrite (builder flag `WithBackup()`).
Cheap insurance for a file-based store; pairs naturally with #1. If deserialization of the main file
fails on load, optionally fall back to the backup.

### 3. Configurable `JsonSerializerOptions`
`JsonFileAdapter` currently hardcodes serializer options (camelCase, trailing commas). Consumers
cannot add converters (enums-as-string, custom date formats), set `WriteIndented`, or supply a
source-generated `JsonSerializerContext` (which also matters for Blazor WASM AOT / trimming, the
same constraint the BrowserStorage package documents). Expose an optional `JsonSerializerOptions`
through the builder, factory, and adapter constructors.

## Medium value

### 4. Change notification / events
A `Changed` event (or callback) fired after a successful `Write`, useful for Blazor UI refresh,
cache invalidation, or audit. The BrowserStorage package already uses this pattern, so it would be
API-consistent across the family.

### 5. `IAsyncDisposable` + `FlushAsync` for `LowDbAsync`
`IDisposable` is already implemented. For the batched-write pattern, an explicit `FlushAsync` alias
for `Write` reads better at call sites, and `IAsyncDisposable` could auto-flush pending unsaved
changes on dispose (opt-in, to avoid surprises).

### 6. Encryption-at-rest adapter (decorator)
An `EncryptedStorageAdapter<T>` decorator wrapping any adapter (AES via DataProtection or a supplied
key). Common ask for local user data. Fits the adapter seam perfectly - pure composition, no core
change.

### 7. Reset / Delete operation
There is `Read` / `Write` / `Get` / `Update` but no first-class "clear the database" / delete-file
operation. Repositories can empty the set, but a `LowDb.Delete()` or adapter `Delete()` would round
out the lifecycle.

## Lower priority / scope-watch

### 8. Optimistic-concurrency version stamp
A `Version` / etag on write to detect changes since load. Only meaningful for the multi-instance /
cross-process scenario that is explicitly out of scope today. Defer unless that boundary moves.

### 9. Additional serialization formats
XML / YAML / MessagePack. The adapter seam supports it, but likely low demand. Build on request.

### 10. Auto-save debounce / throttle
Coalesce rapid `Update` autosaves into a timed flush. Nice for high-frequency writers, but adds
background-timer complexity and lifecycle concerns. Only worth it if a real workload needs it.

## Recommended next increment

A focused, coherent bundle that builds on the concurrency work and stays true to the
"minimal but safe file DB" identity:

1. **Atomic writes (#1)** - correctness, biggest bang for buck.
2. **Configurable `JsonSerializerOptions` (#3)** - unblocks converters and Blazor AOT / trimming.
3. **Optional backup (#2)** - cheap durability, pairs with #1.

These three are small, low-risk, mostly adapter / builder-level, and each has clear real-world
justification. Change events (#4) is a good follow-up for family-consistency with BrowserStorage.
