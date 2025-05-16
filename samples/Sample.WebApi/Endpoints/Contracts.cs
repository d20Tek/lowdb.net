namespace Sample.WebApi.Endpoints;

internal record CreateTaskRequest(string Name);

internal record UpdateTaskRequest(string Name, bool IsCompleted);
