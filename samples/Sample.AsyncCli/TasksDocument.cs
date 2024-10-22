//---------------------------------------------------------------------------------------------------------------------
// Copyright (c) d20Tek.  All rights reserved.
//---------------------------------------------------------------------------------------------------------------------
namespace Sample.AsyncCli;

internal class TasksDocument
{
    public int LastId { get; set; } = 0;

    public string Version { get; set; } = "1.0";

    public HashSet<TaskEntity> Tasks { get; init; } = [];

    public int GetNextId() => ++LastId;
}
