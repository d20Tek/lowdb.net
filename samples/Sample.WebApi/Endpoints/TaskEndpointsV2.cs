using D20Tek.Functional.AspNetCore.MinimalApi;

namespace Sample.WebApi.Endpoints;

public static class TaskEndpointsV2
{
    public static void MapTaskV2Endpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/v2/tasks")
                          .WithTags("Tasks V2");

        group.MapGet("/", async (ITasksRepository repo) =>
        {
            var tasksResult = await repo.GetAllAsync();
            return tasksResult.ToApiResult();
        })
        .WithName("GetAllTasks.V2")
        .WithOpenApi();
    }
}
