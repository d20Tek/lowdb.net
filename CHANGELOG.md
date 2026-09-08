# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## Release v1.4.1

### Changed

- `LowDb.Net.Browser` now depends on `D20Tek.Blazor.BrowserStorage` instead of `Blazored.LocalStorage` and `Blazored.SessionStorage`. The browser storage adapters now use the result-based `GetAsync`/`SetAsync` API surface.

### Removed

- Removed the synchronous browser storage support from `LowDb.Net.Browser`. The `AddLocalLowDb<T>` and `AddSessionLowDb<T>` dependency injection extensions and their backing synchronous adapters (`LocalStorageAdapter<T>`, `SessionStorageAdapter<T>`) have been removed because `D20Tek.Blazor.BrowserStorage` provides an async-only API. Use `AddLocalLowDbAsync<T>` and `AddSessionLowDbAsync<T>` with `LowDbAsync<T>` instead.
