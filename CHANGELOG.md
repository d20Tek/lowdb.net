# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## Release v1.4.1

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
