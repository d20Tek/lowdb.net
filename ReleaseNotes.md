# LowDb.Net Release Notes
These are the release notes for each major release of the LowDb.Net package:

## Release 1.1.0
WebStorage based storage adapters (for local and session storage)

- Added sync and async LocalStorageAdapters to persist document into browser local storage.
- Added sync and async SessionStorageAdapters to persist documents into browser session storage.
- Factories and DI methods for setting up these new LowDb database flavors.
- Added Sample.BlazorWasm to show usage of LocalLowDb.

## Release 1.0.4
Bug fix release

- Fixed bug in LowDb/LowDbAsync that didn't respect data initialization via the constructor.

## Release 1.0.3
Minor updates to the initial LowDb.Net release

- Added Release Notes document to repository and link to nuget package definition.
- Added unit tests to verify using list of entities in addition to root document.
- Added optional autoSave parameter to Update method to control whether changes are persisted with every update.
- Added Sample.AsyncCli to show usage of LowDbAsync.

## Release 1.0.1
Initial lowdb file-based database

- Initial library for type-safe file-based database targeting the .NET platform.
- Factories and DI methods for setting up the LowDb databases.
- Synchronous and async versions of  LowDb.
- Unit tests for initial feature set.
- Samples for console and WebApi based .NET applications.
