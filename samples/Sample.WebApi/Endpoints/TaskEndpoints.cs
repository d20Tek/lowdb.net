//---------------------------------------------------------------------------------------------------------------------
// Copyright (c) d20Tek.  All rights reserved.
//---------------------------------------------------------------------------------------------------------------------
using D20Tek.Functional.AspNetCore.MinimalApi;
using D20Tek.LowDb;

namespace Sample.WebApi.Endpoints;

public static class TaskEndpoints
{
    public static void MapTaskEntityEndpoints (this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/v1/tasks")
                          .WithTags("Tasks");

        group.MapGet("/", async (ITasksRepository repo) =>
        {
            var tasksResult = await repo.GetAllAsync();
            return tasksResult.ToApiResult();
        })
        .WithName("GetAllTasks")
        .WithOpenApi();

        group.MapGet("/{id}", async (int id, LowDbAsync<TasksDocument> db) =>
        {
            var taskDoc = await db.Get();
            return taskDoc.Tasks.FirstOrDefault(x => x.Id == id);
        })
        .WithName("GetTaskById")
        .WithOpenApi();

        group.MapPut("/{id}", async (int id, UpdateTaskRequest request, LowDbAsync<TasksDocument> db) =>
        {
            TaskEntity? response = null;
            await db.Update(x =>
            {
                var task = x.Tasks.FirstOrDefault(x => x.Id == id);
                if (task is not null)
                {
                    task.Name = request.Name;
                    task.IsCompleted = request.IsCompleted;

                    response = task;
                }
            });

            return TypedResults.Ok(response);
        })
        .WithName("UpdateTask")
        .WithOpenApi();

        group.MapPost("/", async (CreateTaskRequest request, LowDbAsync<TasksDocument> db) =>
        {
            TaskEntity? response = null;
            await db.Update(x =>
            {
                response = new TaskEntity
                {
                    Id = x.GetNextId(),
                    Name = request.Name,
                    IsCompleted = false
                };

                x.Tasks.Add(response);
            });

            return TypedResults.Created($"/api/v1/tasks/{response?.Id}", response);
        })
        .WithName("CreateTask")
        .WithOpenApi();

        group.MapDelete("/{id}", async (int id, LowDbAsync<TasksDocument> db) =>
        {
            await db.Update(x =>
            {
                x.Tasks.RemoveWhere(x => x.Id == id);
            });
            return TypedResults.Ok(new TaskEntity { Id = id });
        })
        .WithName("DeleteTask")
        .WithOpenApi();
    }
}
