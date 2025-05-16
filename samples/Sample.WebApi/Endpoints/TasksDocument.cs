//---------------------------------------------------------------------------------------------------------------------
// Copyright (c) d20Tek.  All rights reserved.
//---------------------------------------------------------------------------------------------------------------------
using D20Tek.LowDb.Repositories;

namespace Sample.WebApi.Endpoints;

internal class TasksDocument : DbDocument
{
    public int LastId { get; set; } = 0;

    public string Version { get; set; } = "1.0";

    public HashSet<TaskEntity> Tasks { get; init; } = [];

    public int GetNextId() => ++LastId;
}
