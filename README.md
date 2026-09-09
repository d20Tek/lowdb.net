[![Build Status](https://github.com/d20Tek/lowdb.net/actions/workflows/ci-build.yml/badge.svg)](https://github.com/d20Tek/lowdb.net/actions)
[![NuGet](https://img.shields.io/nuget/v/LowDb.Net)](https://www.nuget.org/packages/LowDb.Net)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE)

# LowDb.Net

A .NET implementation of the LowDB file-based database for use in applications and experiments that only require a minimal database implementation. Supports type-safe JSON-based database file formats.

This library is inspired by the [lowdb Javascript package](https://github.com/typicode/lowdb), but rebuilt for the .NET platform.

## Table of Contents
* [Installation](#installation)
* [Usage](#usage)
* [Repository Pattern](#repository-pattern)
* [Features](#features)
* [Samples](#samples)
* [Limits](#limits)
* [License](#license)
* [Feedback](#feedback)

## Installation
This library is a NuGet package so it is easy to add to your project. To install the package into your solution, you can use the NuGet Package Manager. In PM, please use the following command:
```  
PM > Install-Package LowDb.Net -Version 1.4.1
``` 

There is an optional package that extends the LowDb functionality to work with web storage technologies (session and local storage). If you are working with Blazor client projects you can install this package into your solution using NuGet Package Manager. In PM, please use the following command:
```  
PM > Install-Package LowDb.Net.Browser -Version 1.4.1
``` 

There is also an optional package that adds the repository pattern over a LowDb database. To install it using NuGet Package Manager. In PM, please use the following command:
```  
PM > Install-Package LowDb.Net.Repositories -Version 1.4.1
``` 

To install in the Visual Studio UI, go to the Tools menu > "Manage NuGet Packages". Then search for LowDb.Net, and install any of these packages from there.

## Usage
LowDb is a .NET 9/10 package that works seamlessly with any .NET application type (console, web api, Blazor, and Windows applications).

The following code shows how to consume the LowDb library in a simple console application that just creates and reads tasks:

```csharp
using D20Tek.LowDb;

namespace Sample.Cli;

class Program
{
    private static LowDb<TasksDocument> db = LowDbFactory.CreateLowDb<TasksDocument>("my-tasks.json");

    static void Main(string[] args)
    {
        InitializeDatabase(args);

        while (true)
        {
            Console.Write("Enter command (create, read, exit): ");
            var command = Console.ReadLine()?.ToLower();

            switch (command)
            {
                case "create":
                    CreateTask();
                    break;
                case "read":
                    ReadTasks();
                    break;
                case "exit":
                    return;
                default:
                    Console.WriteLine("Invalid command. Please try again.");
                    break;
            }
        }
    }

    static void InitializeDatabase(string[] args)
    {
        db = LowDbFactory.CreateLowDb<TasksDocument>(b =>
        {
            if (args.Any(x => x.Equals("--in-memory", StringComparison.InvariantCultureIgnoreCase)))
            {
                Console.WriteLine("Running in-memory database mode.");
                b.UseInMemoryDatabase();
            }
            else
            {
                b.UseFileDatabase("my-tasks.json");
            }
        });
    }

    static void CreateTask()
    {
        Console.Write("Enter task name: ");
        var name = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(name))
        {
            Console.WriteLine("No task name was entered.");
            return;
        }

        db.Update(x => x.Tasks.Add(
            new TaskEntity
            {
                Id = x.GetNextId(),
                Name = name,
                IsCompleted = false
            }));

        Console.WriteLine("Task created successfully.");
    }
}
```

For the full implementation of this console application, please see the Sample.Cli project in this repository.

## Repository Pattern
The optional `LowDb.Net.Repositories` package adds a repository abstraction over a LowDb database. It exposes CRUD, range, querying, existence checks, and batched save operations, with every method returning a `Result<T>` so you can handle success and failure without exceptions.

To use it, derive your document from `DbDocument` and expose one or more `HashSet<TEntity>` collections. Then create a repository by inheriting from `LowDbRepository<TEntity, TDocument>` (or `LowDbAsyncRepository<TEntity, TDocument>` for async), selecting the entity set to manage:

```csharp
using D20Tek.LowDb;
using D20Tek.LowDb.Repositories;

// Document that stores the entity set.
public class TasksDocument : DbDocument
{
    public HashSet<TaskEntity> Tasks { get; set; } = [];
}

// Async repository over the Tasks set.
public interface ITasksRepository : IRepositoryAsync<TaskEntity>;

public class TasksRepository(LowDbAsync<TasksDocument> db)
    : LowDbAsyncRepository<TaskEntity, TasksDocument>(db, x => x.Tasks), ITasksRepository
{
}
```

With the repository, you can batch multiple changes and persist them with a single call:

```csharp
await repository.AddAsync(new TaskEntity { Id = 1, Name = "Write docs" });
await repository.AddAsync(new TaskEntity { Id = 2, Name = "Ship release" });
var result = await repository.SaveChangesAsync();
```

For full examples, see the Sample.WebApi and Sample.BlazorWasm projects in this repository.

## Features
* Support for synchronous (LowDb) and async (LowDbAsync) database operations.
* Minimal, lightweight database implementation. Perfect for small databases with low write contention.
* Thread-safe within a process: concurrent Read, Write, and Update calls on a shared instance are serialized so callers cannot corrupt the store or overwrite each other's changes.
* Customizable layer of IStorageAdapter that can be used to target physical files vs in-memory vs web storage api.
* Supports type-safe JSON file database format.
* Supports in-memory adapter that is useful for writing unit tests.
* Ability to build custom storage adapters for additional data formats.
* Factory methods (with builders) to simplify the database creation in your projects.
* Dependency injection extensions to easily add LowDb to applications that support dependency injection.
* Optional repository pattern with Result-based CRUD, querying, and batched SaveChanges via the LowDb.Net.Repositories package.

## Samples
For more detailed examples on how to use LowDb.Net, please review the following samples:

* [Sample.AsyncCli](samples/Sample.AsyncCli) - Console application for managing a task list and persisting a local file. Uses the LowDbAsync class and reads/writes files asynchronously.
* [Sample.BlazorWasm](samples/Sample.BlazorWasm) - Blazor Web Assembly project that implements a Task app with CRUD operations on a storage-based LowDb. Uses local storage per user.
* [Sample.Cli](samples/Sample.Cli) - Console application for managing a task list and persisting a local file. Includes the ability to run the app with an in-memory database instead.
* [Sample.WebApi](samples/Sample.WebApi) - Minimal WebApi project that exposes a TaskList web service with CRUD operations on a shared data file. All users have access to the same database file.

## Limits
This database is intended for local usage for console and Windows applications and local/session storage in Blazor. It can be used on ASP.NET WebApi and Blazor Server projects, but only for projects with low database usage and low write contention.

When a single JSON database file becomes larger than 50-100MB, you may start to see performance degradation. The Write operations serialize the full object tree into JSON and saves it all to the file.

Both `LowDb` and `LowDbAsync` serialize concurrent `Read`, `Write`, and `Update` calls on a shared instance, so multiple callers within the same process will not corrupt the backing store or overwrite each other's changes. This protection is in-process and per-instance only. It does not coordinate across multiple processes or across separate LowDb instances that point at the same file. If you need cross-process or multi-instance safety on a shared file, use a database that provides it, such as SQLite or LiteDB.

The LowDb does expose the Read and Write operations to enable batch multiple updates together to help with performance concerns. With those methods, you could implement the UnitOfWork pattern to make changes in batches.

You can also break up the data into separate files, if they can logically be separated... just as if you were using other Document DBs.

If you require large scale Document DBs, then other tools are more appropriate like Azure CosmosDB and MongoDB.

## License
This project is licensed under the terms of the [MIT License](LICENSE).

## Feedback
If you use this library and have any feedback, bugs, or suggestions, please file them in the Issues section of this repository. I'm still in the process of building it, so any suggestions that would make it more usable are welcome.
