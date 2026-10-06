# API Reference: D20Tek.LowDb.Browser

The `D20Tek.LowDb.Browser` package extends LowDb with storage adapters and dependency injection
extensions for Blazor client applications, persisting documents to the browser's local or session
storage via the `D20Tek.Blazor.BrowserStorage` package.

## Table of Contents
* [LocalStorageAdapterAsync\<T\>](#localstorageadapterasynct)
* [SessionStorageAdapterAsync\<T\>](#sessionstorageadapterasynct)
* [DependencyInjection](#dependencyinjection)

---

## LocalStorageAdapterAsync\<T\>

```csharp
namespace D20Tek.LowDb.Browser.Adapters;

public class LocalStorageAdapterAsync<T>(string keyname, ILocalStorageService storage)
	: IStorageAdapterAsync<T> where T : class
```

An asynchronous storage adapter that persists a LowDb document to the browser's local storage
under a specified key. Local storage data survives across browser sessions.

### Constructor

| Parameter | Type | Description |
|---|---|---|
| `keyname` | `string` | The local storage key under which the document is stored. |
| `storage` | `ILocalStorageService` | The browser local storage service used to read and write values. |

### Members

| Member | Description |
|---|---|
| `Task<T?> Read(CancellationToken token = default)` | Reads and deserializes the value stored under `keyname`. Returns `null` when the key is missing or the underlying read is unsuccessful. |
| `Task Write(T data, CancellationToken token = default)` | Serializes and writes `data` under `keyname`. Throws when `keyname` is null or empty. |

JSON serialization is performed internally by `ILocalStorageService`, configured through
`BrowserStorageOptions.JsonOptions` (see [DependencyInjection](#dependencyinjection) below) rather
than through adapter constructor parameters.

---

## SessionStorageAdapterAsync\<T\>

```csharp
namespace D20Tek.LowDb.Browser.Adapters;

public class SessionStorageAdapterAsync<T>(string keyname, ISessionStorageService storage)
	: IStorageAdapterAsync<T> where T : class
```

An asynchronous storage adapter that persists a LowDb document to the browser's session storage
under a specified key. Session storage data is cleared when the browser tab or session ends.

### Constructor

| Parameter | Type | Description |
|---|---|---|
| `keyname` | `string` | The session storage key under which the document is stored. |
| `storage` | `ISessionStorageService` | The browser session storage service used to read and write values. |

### Members

| Member | Description |
|---|---|
| `Task<T?> Read(CancellationToken token = default)` | Reads and deserializes the value stored under `keyname`. Returns `null` when the key is missing or the underlying read is unsuccessful. |
| `Task Write(T data, CancellationToken token = default)` | Serializes and writes `data` under `keyname`. Throws when `keyname` is null or empty. |

---

## DependencyInjection

```csharp
namespace D20Tek.LowDb.Browser;

public static class DependencyInjection
```

Extension methods on `IServiceCollection` for registering browser-backed `LowDbAsync<T>`
instances. Each method also registers the required `D20Tek.Blazor.BrowserStorage` services.

### Members

| Member | Description |
|---|---|
| `IServiceCollection AddLocalLowDbAsync<T>(this IServiceCollection services, string keyname, ServiceLifetime lifetime = ServiceLifetime.Scoped, Action<BrowserStorageOptions>? configureStorage = null, bool enableBackup = false)` | Registers an asynchronous `LowDbAsync<T>` backed by the browser's local storage, and registers local storage services via `AddLocalStorage`. |
| `IServiceCollection AddSessionLowDbAsync<T>(this IServiceCollection services, string keyname, ServiceLifetime lifetime = ServiceLifetime.Scoped, Action<BrowserStorageOptions>? configureStorage = null, bool enableBackup = false)` | Registers an asynchronous `LowDbAsync<T>` backed by the browser's session storage, and registers session storage services via `AddSessionStorage`. |

### Parameters

| Parameter | Description |
|---|---|
| `keyname` | The local/session storage key under which the document is stored. |
| `lifetime` | The service lifetime for the `LowDbAsync<T>` registration. Defaults to `ServiceLifetime.Scoped`. |
| `configureStorage` | Optional configuration callback for the underlying storage service, including `BrowserStorageOptions.JsonOptions` used to serialize and deserialize the document. |
| `enableBackup` | When `true`, composes a `BackupStorageAdapter<T>`/`BackupStorageAdapterAsync<T>` (from `D20Tek.LowDb.Adapters`) around the primary adapter, targeting a sibling `<keyname>.bak` storage key. The previous value is copied to the backup key before each write, and reads fall back to that key when the primary key is missing or fails to deserialize. |

### Example

```csharp
services.AddLocalLowDbAsync<TasksDocument>(
	"tasks",
	configureStorage: options => options.JsonOptions.WriteIndented = true,
	enableBackup: true);

services.AddSessionLowDbAsync<TasksDocument>("session-tasks");
```
