//---------------------------------------------------------------------------------------------------------------------
// Copyright (c) d20Tek.  All rights reserved.
//---------------------------------------------------------------------------------------------------------------------
using D20Tek.LowDb;

namespace Sample.Cli;

class Program
{
    private static LowDb<TasksDocument> db = LowDbFactory.CreateJsonLowDb<TasksDocument>("my-tasks.json");

    static void Main(string[] args)
    {
        Console.WriteLine("Welcome to Task Cli!");
        Console.WriteLine("====================");
        Console.WriteLine();

        ReplaceWithInMemoryDatabase(args);

        while (true)
        {
            Console.Write("Enter command (create, read, update, delete, exit): ");
            var command = Console.ReadLine()?.ToLower();

            switch (command)
            {
                case "create":
                    CreateTask();
                    break;
                case "read":
                    ReadTasks();
                    break;
                case "update":
                    UpdateTask();
                    break;
                case "delete":
                    DeleteTask();
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
            db = LowDbFactory.CreateLowDb<TasksDocument>(b =>
            {
                Console.WriteLine("Running in-memory database mode.");
                b.UseInMemoryDatabase();
            });
        }
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

    static void ReadTasks()
    {
        var taskDoc = db.Get();
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

    static void UpdateTask()
    {
        Console.Write("Enter task id to update: ");
        if (!int.TryParse(Console.ReadLine(), out int id))
        {
            Console.WriteLine("Invalid id.");
            return;
        }

        db.Update(x =>
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

    static void DeleteTask()
    {
        Console.Write("Enter task id to delete: ");
        if (!int.TryParse(Console.ReadLine(), out int id))
        {
            Console.WriteLine("Invalid id.");
            return;
        }

        db.Update(x =>
        {
            var task = x.Tasks.FirstOrDefault(t => t.Id == id);
            if (task == null)
            {
                Console.WriteLine("Task not found.");
                return;
            }

            x.Tasks.Remove(task);
        });

        Console.WriteLine("Task deleted successfully.");
    }
}
