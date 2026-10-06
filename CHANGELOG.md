# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## Release v1.4.3

### Added

- Configurable `JsonSerializerOptions` for JSON file persistence. `JsonFileAdapter<T>` and `JsonFileAdapterAsync<T>` now accept an optional `JsonSerializerOptions` constructor parameter, defaulting to the existing camel-case, case-insensitive, trailing-comma-tolerant behavior when not supplied.
- `LowDbBuilder` gained a `WithJsonSerializerOptions` method, and `LowDbFactory.CreateJsonLowDb`/`CreateJsonLowDbAsync` and the `AddLowDb`/`AddLowDbAsync` dependency injection extensions gained an optional `serializerOptions` parameter, unblocking custom converters, `WriteIndented`, and source-generated `JsonSerializerContext` scenarios (including Blazor WASM AOT / trimming).
- Optional backup / recovery support for file-backed and browser-backed databases, implemented as reusable `BackupStorageAdapter<T>` and `BackupStorageAdapterAsync<T>` storage adapter decorators. When enabled, the previous value read from the primary adapter is copied to a sibling backup store (a `.bak` file, or a `.bak`-suffixed browser storage key) before each write, and reads fall back to that backup when the primary store is missing or contains invalid JSON.
- `LowDbBuilder` gained a `WithBackup()` method, and `LowDbFactory.CreateJsonLowDb`/`CreateJsonLowDbAsync`, the `AddLowDb`/`AddLowDbAsync` dependency injection extensions, and the `AddLocalLowDbAsync`/`AddSessionLowDbAsync` browser dependency injection extensions gained an optional `enableBackup` parameter that composes the appropriate backup decorator around the primary storage adapter.
- Implmented optional configurable `JsonSerializerOptions` and backup support for the browser storage adapters in `LowDb.Net.Browser`. The `LocalStorageAdapter<T>` and `SessionStorageAdapter<T>` constructors now accept an optional `JsonSerializerOptions` parameter, and the `AddLocalLowDbAsync<T>` and `AddSessionLowDbAsync<T>` dependency injection extensions gained optional `serializerOptions` and `enableBackup` parameters.
- Implemented a full document set for the LowDb library with introduction, getting started, and API reference pages.

### Changed

- Updated package references to the newest versions.
- Updated the Sample.WebApi project to replace Swagger with OpenApi and Scalar.

## Release v1.4.2

### Added
- Converted sln file to new slnx format.
- Updated solution to work with central package and build management.
- Added in-process write concurrency protection to the core `LowDb<T>` and `LowDbAsync<T>` classes. Concurrent `Read`, `Write`, and `Update` calls on a shared instance are now fully serialized (`LowDb<T>` uses a `lock`, `LowDbAsync<T>` uses a `SemaphoreSlim`), preventing backing-store corruption and lost updates. This protection is in-process and per-instance only; it does not coordinate across multiple processes or across separate instances pointing at the same file.
- `LowDbAsync<T>` now implements `IDisposable` to release its internal `SemaphoreSlim`.
- Added XML documentation comments to the public API surface of `LowDb.Net`, `LowDb.Net.Browser`, and `LowDb.Net.Repositories`.
- Migrated nuget-release script to use Nuget Trusted Publishing to publish packages to nuget.org.

### Changed

- `LowDb.Net.Browser` now depends on `D20Tek.Blazor.BrowserStorage` instead of `Blazored.LocalStorage` and `Blazored.SessionStorage`. The browser storage adapters now use the result-based `GetAsync`/`SetAsync` API surface.

### Removed

- Removed the synchronous browser storage support from `LowDb.Net.Browser`. The `AddLocalLowDb<T>` and `AddSessionLowDb<T>` dependency injection extensions and their backing synchronous adapters (`LocalStorageAdapter<T>`, `SessionStorageAdapter<T>`) have been removed because `D20Tek.Blazor.BrowserStorage` provides an async-only API. Use `AddLocalLowDbAsync<T>` and `AddSessionLowDbAsync<T>` with `LowDbAsync<T>` instead.
