//---------------------------------------------------------------------------------------------------------------------
// Copyright (c) d20Tek.  All rights reserved.
//---------------------------------------------------------------------------------------------------------------------
namespace Sample.Cli;

internal class TaskEntity
{
    public int Id { get; set; }

    public string Name { get; set; } = "";
    
    public bool IsCompleted { get; set; }

    public override string ToString()
    {
        return $"Id: {Id}, Name: {Name}, IsCompleted: {IsCompleted}";
    }
}
