//---------------------------------------------------------------------------------------------------------------------
// Copyright (c) d20Tek.  All rights reserved.
//---------------------------------------------------------------------------------------------------------------------
namespace Sample.WebApi.Endpoints;

internal class TaskEntity
{
    public int Id { get; set; }

    public string Name { get; set; } = "";

    public bool IsCompleted { get; set; }
}
