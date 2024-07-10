//---------------------------------------------------------------------------------------------------------------------
// Copyright (c) d20Tek.  All rights reserved.
//---------------------------------------------------------------------------------------------------------------------
using D20Tek.LowDb;
using Sample.AsyncCli;

namespace Sample.Cli;

class Program
{
    private static LowDbAsync<TasksDocument> db = LowDbFactory.CreateJsonLowDbAsync<TasksDocument>("my-async-tasks.json");

    static async Task Main(string[] args)
    {
        Console.WriteLine("Welcome to Task Async Cli!");
        Console.WriteLine("==========================");
        Console.WriteLine();

        ReplaceWithInMemoryDatabase(args);

        while (true)
        {
            Console.Write("Enter command (create, read, update, disable, enable, exit): ");
            var command = Console.ReadLine()?.ToLower();

            switch (command)
            {
                case "create":
                    await CreateTask();
                    break;
                case "read":
                    await ReadTasks();
                    break;
                case "update":
                    await UpdateTask();
                    break;
                case "enable":
                    await EnableTask();
                    break;
                case "disable":
                    await DisableTask();
                    break;
                case "exit":
                    return;
                default:
                    Console.WriteLine("Invalid command. Please try again.");
                    break;
            }
        }
    }

    static void ReplaceWithInMemoryDatabase(string[] args)
    {
        if (args.Any(x => x.Equals("--in-memory", StringComparison.InvariantCultureIgnoreCase)))
        {
            db = LowDbFactory.CreateLowDbAsync<TasksDocument>(b =>
            {
                Console.WriteLine("Running in-memory database mode.");
                b.UseInMemoryDatabase();
            });
        }
    }

    static async Task CreateTask()
    {
        Console.Write("Enter task name: ");
        var name = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(name))
        {
            Console.WriteLine("No task name was entered.");
            return;
        }

        await db.Update(x => x.Tasks.Add(
            new TaskEntity
            {
                Id = x.GetNextId(),
                Name = name,
                IsCompleted = false
            }));

        Console.WriteLine("Task created successfully.");
    }

    static async Task ReadTasks()
    {
        var taskDoc = await db.Get();
        if (taskDoc.Tasks.Count == 0)
        {
            Console.WriteLine("No tasks found.");
            return;
        }

        foreach (var task in taskDoc.Tasks)
        {
            Console.WriteLine(task);
        }
    }

    static async Task UpdateTask()
    {
        Console.Write("Enter task id to update: ");
        if (!int.TryParse(Console.ReadLine(), out int id))
        {
            Console.WriteLine("Invalid id.");
            return;
        }

        await db.Update(x =>
        {
            var task = x.Tasks.FirstOrDefault(t => t.Id == id);
            if (task == null)
            {
                Console.WriteLine("Task not found.");
                return;
            }

            Console.Write("Enter new task name (leave blank to keep current): ");
            var name = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(name))
            {
                task.Name = name;
            }

            Console.Write("Is the task completed? (yes/no): ");
            var isCompleted = Console.ReadLine()?.ToLower();
            if (isCompleted == "yes" || isCompleted == "y")
            {
                task.IsCompleted = true;
            }
            else if (isCompleted == "no" || isCompleted == "n")
            {
                task.IsCompleted = false;
            }
        });

        Console.WriteLine("Task updated successfully.");
    }

    static async Task DisableTask()
    {
        Console.Write("Enter task id to disable: ");
        if (!int.TryParse(Console.ReadLine(), out int id))
        {
            Console.WriteLine("Invalid id.");
            return;
        }

        await db.Update(x =>
        {
            var task = x.Tasks.FirstOrDefault(t => t.Id == id);
            if (task == null)
            {
                Console.WriteLine("Task not found.");
                return;
            }

            task.State = EntityState.Inactive;
        });

        Console.WriteLine("Task disabled successfully.");
    }

    static async Task EnableTask()
    {
        Console.Write("Enter task id to enable: ");
        if (!int.TryParse(Console.ReadLine(), out int id))
        {
            Console.WriteLine("Invalid id.");
            return;
        }

        await db.Update(x =>
        {
            var task = x.Tasks.FirstOrDefault(t => t.Id == id);
            if (task == null)
            {
                Console.WriteLine("Task not found.");
                return;
            }

            task.State = EntityState.Active;
        });

        Console.WriteLine("Task enabled successfully.");
    }
}
