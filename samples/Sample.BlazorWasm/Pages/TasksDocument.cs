namespace Sample.BlazorWasm.Pages;

internal class TasksDocument
{
    public int LastId { get; set; } = 0;

    public string Version { get; set; } = "1.0";

    public List<TaskEntity> Tasks { get; init; } = [];

    public int GetNextId() => ++LastId;
}
